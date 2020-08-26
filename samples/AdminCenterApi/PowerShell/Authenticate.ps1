# This sample authenticates to Azure Active Directory (AAD) an obtains an access token.
# The access token can be used for authenticating to Business Central APIs.


# Parameters
$aadAppId = "a19cb26a-2e4c-408b-82e1-6311742ecc50"        # partner's AAD app id
$aadAppRedirectUri = "http://localhost"                   # partner's AAD app redirect URI
$aadTenantId = "8c8dbccd-c171-4937-a134-e3c5a5dd0470"     # customer's tenant id


# Load Microsoft.IdentityModel.Clients.ActiveDirectory.dll
Add-Type -Path "C:\Program Files\WindowsPowerShell\Modules\AzureAD\2.0.2.106\Microsoft.IdentityModel.Clients.ActiveDirectory.dll" # Install-Module AzureAD to get this


# Get access token
$ctx = [Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext]::new("https://login.microsoftonline.com/$aadTenantId")
$redirectUri = New-Object -TypeName System.Uri -ArgumentList $aadAppRedirectUri
$platformParameters = New-Object -TypeName Microsoft.IdentityModel.Clients.ActiveDirectory.PlatformParameters -ArgumentList ([Microsoft.IdentityModel.Clients.ActiveDirectory.PromptBehavior]::Always)
$accessToken = $ctx.AcquireTokenAsync("https://api.businesscentral.dynamics.com", $aadAppId, $redirectUri, $platformParameters).GetAwaiter().GetResult().AccessToken
Write-Host -ForegroundColor Cyan 'Authentication complete - we have an access token for Business Central, and it is stored in the $accessToken variable.'

# Peek inside the access token (this is just for education purposes; in actual API calls we'll just pass it as one long string)
$middlePart = $accessToken.Split('.')[1]
$middlePartPadded = "$middlePart$(''.PadLeft((4-$middlePart.Length%4)%4,'='))"
$middlePartDecoded = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($middlePartPadded))
$middlePartDecodedPretty = (ConvertTo-Json (ConvertFrom-Json $middlePartDecoded))
Write-Host "Contents of the access token:"
Write-Host $middlePartDecodedPretty
