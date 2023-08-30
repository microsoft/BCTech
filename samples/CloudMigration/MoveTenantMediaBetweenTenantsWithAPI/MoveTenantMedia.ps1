Import-Module "MSAL.PS"

# These value can be found on the Business Central Admin Center
$script:SourceEnvironmentUlr = 'PROVIDEVALUE' # E.g. "https://api.businesscentral.dynamics.com/v2.0/a5876f8b-c580-4861-823a-ecfda5e99d94/MySandbox/api/MSFT/moveData/v1.0/companies"
$script:TargetEnvironmentUlr = 'PROVIDEVALUE' # E.g."https://api.businesscentral.dynamics.com/v2.0/a5876f8b-c580-4861-823a-ecfda5e99d94/Production/api/MSFT/moveData/v1.0/companies"
$script:AADTenantID = "PROVIDEVALUE" # e.g. a5876f8b-c580-4861-823a-ecfda5e99d94"

# OAuth2 App parameters - Register your app and update to match - Values can be found on portal.azure.com
$script:ClientId = "PROVIDEVALUE" # e.g. 3f65a4a6-af4c-4b7a-92a9-5b2fcde8f428
$script:RedirectUri = "PROVIDEVALUE" # e.g, "https://login.microsoftonline.com/common/oauth2/nativeclient"

# End of the values to provide
$script:BaseAuthorityUri = "https://login.microsoftonline.com"
$script:BcAppIdUri = "https://api.businesscentral.dynamics.com"
$script:BcScopes = @("$BcAppIdUri/user_impersonation", "$BcAppIdUri/Financials.ReadWrite.All" )
$script:AuthorityUri = "$BaseAuthorityUri/$AadTenantId"

$script:DefaultAuthrization = 'Bearer'
$script:TokenExpirationTime = (Get-Date)

function Copy-TenantMedia
(
    [int] $startIndex = 0, # Use this parameters if you want to run the script in parallel. Start the first scripts by specifying e.g. $maxCount 10.000 and second script with $startIndex 10.000 and $maxCount 10.000
    [int] $maxCount = 0
)
{
    # Setup URLs
    Write-Host "Connecting to the environments"    
    $sourceCompanyId = Get-SourceCompanyURL -EnvironmentUlr $script:SourceEnvironmentUlr
    $sourceMediaAPIUrl = $script:SourceEnvironmentUlr + '(' + $sourceCompanyId + ')/tenantMedia' 
    $sourceMediaIDsAPIUrl = $script:SourceEnvironmentUlr + '(' + $sourceCompanyId + ')/tenantMediaIds' 
    Write-Host "Using company with ID $sourceCompanyId as source"    

    $targetCompanyId = Get-SourceCompanyURL -EnvironmentUlr $script:TargetEnvironmentUlr
    $targetMediaAPIUrl = $script:TargetEnvironmentUlr + '(' + $targetCompanyId + ')/tenantMedia' 
    $targetMediaIDsAPIUrl = $script:TargetEnvironmentUlr + '(' + $targetCompanyId + ')/tenantMediaIds' 
    Write-Host "Using company with ID $targetCompanyId as target"    

    # Get Tenant Media Records to copy
    Write-Host "Discovering tenant media records to move"
    $sourceTenantMediaIDs = Get-TenantMediaIDs -MediaIDsAPIUrl $sourceMediaIDsAPIUrl
    [System.Collections.ArrayList] $itemsToMove =  $sourceTenantMediaIDs | Select-Object -Property id | Sort-Object -Property id

    $targetTenantMediaIDs = Get-TenantMediaIDs -MediaIDsAPIUrl $targetMediaIDsAPIUrl
    $ExistingTenantMedia = New-Object System.Collections.Specialized.OrderedDictionary

    if($targetTenantMediaIDs)
    {
        if($targetTenantMediaIDs.value.Count -gt 0)
        {
            [System.Collections.ArrayList] $targetTenantMediaIDs = $targetTenantMediaIDs | Select-Object -Property id  | Sort-Object -Property id

            for ($i = 0; $i -lt $targetTenantMediaIDs.Count; $i++)
            {
                $ExistingTenantMedia.Add($targetTenantMediaIDs[$i].id,'')
            }
        }
    }

    $endIndexOfItemsToMove = $itemsToMove.Count

    if($maxCount) 
    {
        $endIndexOfItemsToMove =  $startIndex + $maxCount
    }

    # Move selected records
    for($i = $startIndex; $i -le $endIndexOfItemsToMove; $i++)
    {
        $mediaIDToMove = $itemsToMove[$i].id
        if (-not $ExistingTenantMedia.Contains($itemsToMove[$i].id))
        {
            $sourceURl = $sourceMediaAPIUrl + '(' + $mediaIDToMove + ')'
            $Token = Get-AADToken
            $sourceResponse = Invoke-GetMethod -Uri $sourceURl -Token $Token
        
            $tenantMediaBody = @{
                id="$($sourceResponse.id)";
                companyName=$($sourceResponse.companyName);
                creatingUser=$($sourceResponse.creatingUser);
                description=$($sourceResponse.description);
                expirationDate=$($sourceResponse.expirationDate);
                fileName=$($sourceResponse.fileName);
                height=$($sourceResponse.height);
                base64ContentTxt=$($sourceResponse.base64ContentTxt);
                mimeType=$($sourceResponse.mimeType);
                prohibitCache=$($sourceResponse.prohibitCache);
                securityToken=$($sourceResponse.securityToken);
                width=$($sourceResponse.width);
            }

            Write-Host "Moving Media with ID " $tenantMediaBody.id " File Name " $tenantMediaBody.fileName
            $response = Invoke-PostMethod -Uri $targetMediaAPIUrl -Token $Token -Body $tenantMediaBody
        }
    }
}

function Get-TenantMediaIDs
(
    [string] $MediaIDsAPIUrl
)
{
    $Token = Get-AADToken
    $response = Invoke-GetMethod -Uri $MediaIDsAPIUrl -Token $Token
    [System.Collections.ArrayList] $TenantMediaIDs = $response.value
    while($response.'@odata.nextLink')
    {
        $Token = Get-AADToken
        $response = Invoke-GetMethod -Uri ($response.'@odata.nextLink') -Token $Token
        $TenantMediaIDs.AddRange($response.value)
    }

    return $TenantMediaIDs
}


function Get-SourceCompanyURL
(
    [string] $EnvironmentUlr
)
{
    $Token = Get-AADToken
    $response = Invoke-GetMethod -Uri $EnvironmentUlr -Token $Token 
    if (@($Response.value).Count -le 1)
    {
        $companyId = $Response.value.id
    }
    else
    {
        $companyId = $Response.value[0].id
    }

    return $companyId
}

function Invoke-GetMethod
(
[string] $Uri,
[string] $Token,
[string] $Authorization = $script:DefaultAuthrization
)
{
    $response = Invoke-RestMethod -Method GET -Uri $Uri -Headers (Create-AuthorizationHeader -Token $Token -Authorization $Authorization)
    return $response
}

function Invoke-PostMethod
(
[string] $Uri,
[string] $Token,
[string] $Authorization = $script:DefaultAuthrization,
$Body
)
{
    $headers = Create-AuthorizationHeader -Token $Token -Authorization $Authorization;
    try
    {
        $response = Invoke-RestMethod -Method POST -Uri $Uri -Headers $headers -Body (convertto-json $Body) -ContentType "application/json"
    }
    catch
    {
        Write-Host "Failed:" $Uri -ForegroundColor Red
        Write-Host "StatusCode:" $_.Exception.Response.StatusCode.value__  -ForegroundColor Red
        $responseStream = $_.Exception.Response.GetResponseStream()
        $responseStream.Position = 0;
        $streamReader = New-Object System.IO.StreamReader($responseStream)
        $errorMessage = $streamReader.ReadToEnd()
        $streamReader.Close()
        $responseStream.Close()

        Write-Host "Message: $errorMessage" -ForegroundColor Red
    }

    return $response
}

function Get-AADToken 
(
[string] $UserName = $script:CurrentUserName,
[string] $Password = $script:CurrentPassword,
[string] $AADTenantID = $script:AADTenantID,
[string] $Version = "v1.0"
)
{
    if($script:TokenExpirationTime)
    {
        if ($script:TokenExpirationTime -gt (Get-Date))
        {
            return $script:CurrentToken
        }
    }

    try
    {
        $securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
        $UserCredential = New-Object System.Management.Automation.PSCredential($UserName, $securePassword)
        $AuthenticationResult = Get-MsalToken -ClientId $script:ClientId -RedirectUri $script:RedirectUri -TenantId $AADTenantID -Authority $script:AuthorityUri -UserCredential $UserCredential -Scopes $script:BcScopes
    }
    catch {
       $AuthenticationResult =  Get-MsalToken -ClientId $script:ClientId -RedirectUri $script:RedirectUri -TenantId $AADTenantID -Authority $script:AuthorityUri -Prompt SelectAccount -Scopes $script:BcScopes
    }

    $script:CurrentToken =  $AuthenticationResult.AccessToken;
    
    $script:TokenExpirationTime = ($AuthenticationResult.ExpiresOn - (New-TimeSpan -Minutes 3))
    return $AuthenticationResult.AccessToken;
}

function Create-AuthorizationHeader
(
[string] $Token,
[string] $Authorization = $script:DefaultAuthrization
)
{
    return @{"Authorization"=$Authorization + " " + $Token;}
}