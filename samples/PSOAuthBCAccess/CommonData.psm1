
Import-Module MSAL.PS


$ClientId = "<Azure AD Client Id>"
$RedirectUri = "<Desktop redirect Uri>"
$RedirectUriWeb = "http://localhost:8080/login"

$BaseAuthorityUri = "https://login.microsoftonline.com"
$AadTenantId = "<Azure AD Tenant Id>"
$AuthorityUri = "$BaseAuthorityUri/$AadTenantId"


$BcAppIdUri = "https://api.businesscentral.dynamics.com"
$BcScopes = @( "$BcAppIdUri/user_impersonation", "$BcAppIdUri/Financials.ReadWrite.All" )
$BcAutomationScopes = @( "$BcAppIdUri/.default" )

$BcBaseUri = "https://api.businesscentral.dynamics.com"
$BcEnvironmentName = "Production"
$BcWebServiceName = "Chart_of_Accounts"
$BcAutomationServiceName = "automationCompanies"

$SampleBCODataUrl = "$BcBaseUri/v2.0/$AadTenantId/$BcEnvironmentName/api/v2.0/companies"
$SampleBCAutomationUrl = "$BcBaseUri/v2.0/$AadTenantId/$BcEnvironmentName/api/microsoft/automation/v2.0/companies"

function Invoke-BCWebService
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [string] $RequestUrl,

        [Parameter(Mandatory=$true)]
        [string] $AccessToken
    )

    return Invoke-RestMethod -Uri $RequestUrl -Headers @{ Authorization = "Bearer $AccessToken" }
}

function Write-BCWebServiceResponse
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory=$true)]
        [PSObject] $Response
    )

    $Response.value | Format-Table -Property ID, Name
}

Export-ModuleMember -Variable ClientId,RedirectUri,RedirectUriWeb,AadTenantId,AuthorityUri,BcScopes,BcAutomationScopes,SampleBCODataUrl,SampleBCAutomationUrl
Export-ModuleMember -Function Invoke-BCWebService,Write-BCWebServiceResponse,Write-BCAutomationResponse
