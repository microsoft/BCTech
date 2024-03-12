
<#
   Interactive Token acquisition

   Application registered in Azure AD should have at least one 'Redirect URI' value set for 'Mobile and desktop applications'
#>

Import-Module ./CommonData.psm1 -Force

# acquire access token interactively
$app = [Microsoft.Identity.Client.PublicClientApplicationBuilder]::Create($ClientId).WithTenantId($EntraTenantId).WithAuthority($AuthorityUri).WithRedirectUri($RedirectUri).Build()
$AuthenticationResult = $app.AcquireTokenInteractive($BcScopes).WithPrompt([Microsoft.Identity.Client.Prompt]::SelectAccount).ExecuteAsync().GetAwaiter().GetResult()

# use access token to get data from BC
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCOdataUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCWebServiceResponse -Response $BcResponse
