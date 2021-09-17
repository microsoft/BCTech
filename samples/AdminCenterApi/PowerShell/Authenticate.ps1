# This sample authenticates to Azure Active Directory (AAD) an obtains an access token.
# The access token can be used for authenticating to Business Central APIs.


# Parameters
$aadAppId = "a19cb26a-2e4c-408b-82e1-6311742ecc50"        # partner's AAD app id
$aadAppRedirectUri = "http://localhost"                   # partner's AAD app redirect URI
$aadTenantId = "8c8dbccd-c171-4937-a134-e3c5a5dd0470"     # customer's tenant id


# Load Microsoft.Identity.Client.dll
# Install-Package -Name Microsoft.Identity.Client -Source nuget.org -ProviderName nuget -SkipDependencies -Destination .\lib # run this line once to download the DLL
Add-Type -Path ".\lib\Microsoft.Identity.Client.4.36.0\lib\net461\Microsoft.Identity.Client.dll"
#Add-Type -Path ".\lib\Microsoft.Identity.Client.4.36.0\lib\netcoreapp2.1\Microsoft.Identity.Client.dll" # enable this line instead if you use PowerShell Core (pwsh)

# Get access token
$clientApplication = [Microsoft.Identity.Client.PublicClientApplicationBuilder]::Create($aadAppId).WithAuthority("https://login.microsoftonline.com/$aadTenantId").WithRedirectUri($aadAppRedirectUri).Build()
$authResult = $clientApplication.AcquireTokenInteractive([string[]]"https://api.businesscentral.dynamics.com/.default").ExecuteAsync().GetAwaiter().GetResult()
$accessToken = $authResult.AccessToken
Write-Host -ForegroundColor Cyan 'Authentication complete - we have an access token for Business Central, and it is stored in the $accessToken variable.'

# Peek inside the access token (this is just for education purposes; in actual API calls we'll just pass it as one long string)
$middlePart = $accessToken.Split('.')[1]
$middlePartPadded = "$middlePart$(''.PadLeft((4-$middlePart.Length%4)%4,'='))"
$middlePartDecoded = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($middlePartPadded))
$middlePartDecodedPretty = (ConvertTo-Json (ConvertFrom-Json $middlePartDecoded))
Write-Host "Contents of the access token:"
Write-Host $middlePartDecodedPretty
