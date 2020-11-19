
<#
   Application Token acquisition
#>

Import-Module ./CommonData.psm1 -Force

Write-Host "Enter client secret:"
$ClientSecret = Read-Host -AsSecureString

# acquire access token as application with client id and secret
$AuthenticationResult = Get-MsalToken -ClientId $ClientId -ClientSecret $ClientSecret -TenantId $AadTenantId -Authority $AuthorityUri -Scopes $BcAutomationScopes

# use access token to call BC automation web service
$BcResponse = Invoke-BCWebService -RequestUrl $SampleBCAutomationUrl -AccessToken $AuthenticationResult.AccessToken
Write-BCAutomationResponse -Response $BcResponse