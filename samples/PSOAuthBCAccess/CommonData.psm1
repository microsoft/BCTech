
Import-Module ./Microsoft.Identity.Client.dll

$ClientId = "<Entra Client Id>"
$RedirectUri = "http://localhost"
$RedirectUriWeb = "http://localhost:8080/login"

$BaseAuthorityUri = "https://login.microsoftonline.com"
$EntraTenantId = "<Entra Tenant Id>"
$AuthorityUri = "$BaseAuthorityUri/$EntraTenantId"

$BcAppIdUri = "https://api.businesscentral.dynamics.com"
[string[]]$BcScopes = @( "$BcAppIdUri/user_impersonation", "$BcAppIdUri/Financials.ReadWrite.All" )
[string[]]$BcApplicationScopes = @( "$BcAppIdUri/.default" )

$BcBaseUri = "https://api.businesscentral.dynamics.com"
$BcEnvironmentName = "Production"
$BcWebServiceName = "Chart_of_Accounts"
$BcAutomationServiceName = "automationCompanies"

$SampleBCODataUrl = "$BcBaseUri/v2.0/$EntraTenantId/$BcEnvironmentName/api/v2.0/companies"
$SampleBCAutomationUrl = "$BcBaseUri/v2.0/$EntraTenantId/$BcEnvironmentName/api/microsoft/automation/v2.0/companies"

$SampleBCS2SUrl = "$BcBaseUri/v2.0/$EntraTenantId/$BcEnvironmentName/api/v2.0/companies"


function Invoke-BCWebService {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [string] $RequestUrl,

        [Parameter(Mandatory = $true)]
        [string] $AccessToken
    )

    return Invoke-RestMethod -Uri $RequestUrl -Headers @{ Authorization = "Bearer $AccessToken" }
}

function Write-BCWebServiceResponse {
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [PSObject] $Response
    )

    $Response.value | Format-Table -Property ID, Name
}

Export-ModuleMember -Variable ClientId, RedirectUri, RedirectUriWeb, AadTenantId, AuthorityUri, BcScopes, BcAutomationScopes, SampleBCODataUrl, SampleBCS2SUrl, SampleBCAutomationUrl
Export-ModuleMember -Function Invoke-BCWebService, Write-BCWebServiceResponse, Write-BCAutomationResponse
