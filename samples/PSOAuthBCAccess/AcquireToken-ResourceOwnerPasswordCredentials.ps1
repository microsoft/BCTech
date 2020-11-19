
<#
   Token acquisition with user credentials

   Application registered in Azure AD should have at least one 'Redirect URI' value set for 'Mobile and desktop applications' platform
   Application registered in Azure AD should have 'Allow public client flows' set to 'Yes'
#>

Import-Module ./CommonData.psm1 -Force

# get user credentials
$UserCredential = Get-Credential

# acquire access token with user credentials
$AuthenticationResult = Get-MsalToken -ClientId $ClientId -RedirectUri $RedirectUri -TenantId $AadTenantId -Authority $AuthorityUri -UserCredential $UserCredential -Scopes $BcScopes

# use access token to get data from BC
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCOdataUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCWebServiceResponse -Response $BcResponse
