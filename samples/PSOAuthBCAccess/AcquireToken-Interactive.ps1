
<#
   Interactive Token acquisition

   Application registered in Azure AD should have at least one 'Redirect URI' value set for 'Mobile and desktop applications'
#>

Import-Module ./CommonData.psm1 -Force

# acquire access token interactively
$AuthenticationResult = Get-MsalToken -ClientId $ClientId -RedirectUri $RedirectUri -TenantId $AadTenantId -Authority $AuthorityUri -Prompt SelectAccount -Scopes $BcScopes

# use access token to get data from BC
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCOdataUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCWebServiceResponse -Response $BcResponse
