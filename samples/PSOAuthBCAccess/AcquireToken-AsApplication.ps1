
<#
   Application Token acquisition
#>

Import-Module ./CommonData.psm1 -Force

Write-Host "Enter the secret for client $ClientId :"
$ClientSecret = Read-Host -AsSecureString

# acquire access token as application with client id and secret
$AuthenticationResult = Get-MsalToken -ClientId $ClientId -ClientSecret $ClientSecret -TenantId $AadTenantId -Authority $AuthorityUri -Scopes $BcApplicationScopes


# use access token to call BC web service
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCS2SUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCWebServiceResponse -Response $BcResponse


# use access token to call BC *automation* web service
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCAutomationUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCWebServiceResponse -Response $BcResponse
