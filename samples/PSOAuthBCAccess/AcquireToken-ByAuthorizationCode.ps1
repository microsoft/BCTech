
<#
   Token acquisition by authorization code

   Application registered in Azure AD should have 'Redirect URI' value set to 'http://localhost:8080/login' for 'Web' platform
#>

Import-Module ./CommonData.psm1 -Force

Write-Host "Enter the secret for client $ClientId :"
$ClientSecret = Read-Host -AsSecureString
$ClientApplication = New-MsalClientApplication -ClientId $ClientId -ClientSecret $ClientSecret -TenantId $AadTenantId -Authority $AuthorityUri -RedirectUri $RedirectUriWeb

$BcScopesList = New-Object 'System.Collections.Generic.List[string]'
foreach ($BcScope in $BcScopes)
{
    $BcScopesList.Add($BcScope)
}

# get url to acquire authorization code
$AuthorizationRequestUrl = $ClientApplication.GetAuthorizationRequestUrl($BcScopesList).ExecuteAsync().GetAwaiter().GetResult()

# Web browser opens to get authorization code
Start-Process $AuthorizationRequestUrl

# Local web server starts to handle authorization code callback
$AuthorizationCode = ./Start-TestWebServer.ps1

# redeem authorization code to acquire access token
$AuthenticationResult = Get-MsalToken -ClientId $ClientId -ClientSecret $ClientSecret -AuthorizationCode $AuthorizationCode -TenantId $AadTenantId -Authority $AuthorityUri -RedirectUri $RedirectUriWeb -Scopes $BcScopes

# use access token to get data from BC
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCOdataUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCWebServiceResponse -Response $BcResponse