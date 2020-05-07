# This sample authenticates to Azure Active Directory (AAD) an obtains an access token.
# The access token can be used for authenticating to Business Central APIs.


# Parameters
$identityClientPath = "C:\Program Files\WindowsPowerShell\Modules\AzureAD\2.0.2.76\Microsoft.IdentityModel.Clients.ActiveDirectory.dll" # Install-Module AzureAD to get this
$aadTenantId = "14ae87bb-40fe-434a-b185-8eea9645e857"     # customer's tenant id
$aadAppId = "a19cb26a-2e4c-408b-82e1-6311742ecc50"        # partner's AAD app id
$aadAppRedirectUri = "nativeBusinessCentralClient://auth" # partner's AAD app redirect URI


# Load Microsoft.IdentityModel.Clients.ActiveDirectory.dll
if (!(Test-Path $identityClientPath))
{
    Write-Error "There is no file at $identityClientPath"
}
Add-Type -Path $identityClientPath


# Get access token
Write-Host -ForegroundColor Cyan "Authenticating to Azure Active Directory in tenant $aadTenantId to get an access token for Business Central..."
$ctx = [Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext]::new("https://login.microsoftonline.com/$aadTenantId")
$redirectUri = New-Object -TypeName System.Uri -ArgumentList $aadAppRedirectUri
$platformParameters = New-Object -TypeName Microsoft.IdentityModel.Clients.ActiveDirectory.PlatformParameters -ArgumentList ([Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior]::Always)
$accessToken = $ctx.AcquireTokenAsync("https://api.businesscentral.dynamics.com", $aadAppId, $redirectUri, $platformParameters).GetAwaiter().GetResult().AccessToken


# Extract the AAD tenant id from the access token. If you ever have an access token, and you want to know in which customer AAD tenant it is valid, you can execute this line:
$aadTenantId = (ConvertTo-Json (ConvertFrom-Json ([System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($accessToken.Split('.')[1])))).tid)


# Print access token
$middlePart = $accessToken.Split('.')[1]
$middlePartBytes = [Convert]::FromBase64String($middlePart)
$middlePartJson = [System.Text.Encoding]::UTF8.GetString($middlePartBytes)
$middlePartJson = ConvertTo-Json (ConvertFrom-Json $middlePartJson) -Depth 99 # prettify json
Write-Host $middlePartJson
Write-Host -ForegroundColor Cyan 'Authentication complete - we have an access token for Business Central, and it is stored in the $accessToken variable.'
