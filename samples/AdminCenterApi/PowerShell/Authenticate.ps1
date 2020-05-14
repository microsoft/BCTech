# This sample authenticates to Azure Active Directory (AAD) an obtains an access token.
# The access token can be used for authenticating to Business Central APIs.


# Parameters
$aadAppId = "a19cb26a-2e4c-408b-82e1-6311742ecc50"        # partner's AAD app id
$aadAppRedirectUri = "nativeBusinessCentralClient://auth" # partner's AAD app redirect URI

$aadTenantId = "14ae87bb-40fe-434a-b185-8eea9645e857"     # customer's tenant id


# Load Microsoft.IdentityModel.Clients.ActiveDirectory.dll
Add-Type -Path "C:\Program Files\WindowsPowerShell\Modules\AzureAD\2.0.2.76\Microsoft.IdentityModel.Clients.ActiveDirectory.dll" # Install-Module AzureAD to get this


# Get access token
$ctx = [Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext]::new("https://login.microsoftonline.com/$aadTenantId")
$redirectUri = New-Object -TypeName System.Uri -ArgumentList $aadAppRedirectUri
$platformParameters = New-Object -TypeName Microsoft.IdentityModel.Clients.ActiveDirectory.PlatformParameters -ArgumentList ([Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior]::Always)
$accessToken = $ctx.AcquireTokenAsync("https://api.businesscentral.dynamics.com", $aadAppId, $redirectUri, $platformParameters).GetAwaiter().GetResult().AccessToken


# Print access token
Write-Host (ConvertTo-Json (ConvertFrom-Json ([System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($accessToken.Split('.')[1])))) -Depth 99)
Write-Host -ForegroundColor Cyan 'Authentication complete - we have an access token for Business Central, and it is stored in the $accessToken variable.'


# Extract the AAD tenant id from the access token. If you ever have an access token, and you want to know in which customer AAD tenant it is valid, you can execute this line:
$aadTenantId = (ConvertTo-Json (ConvertFrom-Json ([System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($accessToken.Split('.')[1])))).tid)
