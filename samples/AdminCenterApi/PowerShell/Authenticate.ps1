# This sample authenticates to Azure Active Directory (AAD) an obtains an access token.
# The access token can be used for authenticating to Business Central APIs.


# Parameters
$aadAppId = "0999645b-84e4-4980-bd52-e3e6cccf15b3"    # partner's AAD app id
$aadAppRedirectUri = "http://localhost"               # partner's AAD app redirect URI
$aadTenantId = "f5b6b245-5dd2-4bf5-94d4-35ef04d73c6d" # customer's tenant id


# Only needs to be done once: Install the MSAL PowerShell module (see https://github.com/AzureAD/MSAL.PS)
#Install-Module MSAL.PS

# Get access token
$msalToken = Get-MsalToken `
    -Authority "https://login.microsoftonline.com/$aadTenantId" `
    -ClientId $aadAppId `
    -RedirectUri $aadAppRedirectUri `
    -Scope "https://api.businesscentral.dynamics.com/.default"
$accessToken = $msalToken.AccessToken
Write-Host -ForegroundColor Cyan 'Authentication complete - we have an access token for Business Central, and it is stored in the $accessToken variable.'

# Peek inside the access token (this is just for education purposes; in actual API calls we'll just pass it as one long string)
$middlePart = $accessToken.Split('.')[1]
$middlePartPadded = "$middlePart$(''.PadLeft((4-$middlePart.Length%4)%4,'='))"
$middlePartDecoded = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($middlePartPadded))
$middlePartDecodedPretty = (ConvertTo-Json (ConvertFrom-Json $middlePartDecoded))
Write-Host "Contents of the access token:"
Write-Host $middlePartDecodedPretty
