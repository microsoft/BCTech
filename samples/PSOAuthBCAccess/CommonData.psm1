
Import-Module MSAL.PS


$ClientId = "<Client ID>"
$RedirectUri = "<desktop redirect Uri>"
$RedirectUriWeb = "http://localhost:8080/login"

$BaseAuthorityUri = "https://login.microsoftonline.com" # for pre-production environment, use "https://login.windows-ppe.net"
$TenantId = "<Azure AD Tenant Id>"
$AuthorityUri = "$BaseAuthorityUri/$TenantId"


$BcAppIdUri = "https://projectmadeira.com" # for pre-production environment, use "https://projectmadeira-ppe.com"
$BcScopes = @( "$BcAppIdUri/user_impersonation", "$BcAppIdUri/Financials.ReadWrite.All" )
$BcAutomationScopes = @( "$BcAppIdUri/Automation.ReadWrite.All" )

$BcBaseUri = "https://api.businesscentral.dynamics.com" # for pre-production environment, use "https://api.businesscentral.dynamics-tie.com"
$BcEnvironment = "Production"
$BcCompanyUrlEncoded = "<BC company urlencoded>"
$BcWebServiceName = "Chart_of_Accounts"
$BcAutomationServiceName = "automationCompanies"

$SampleBCODataUrl = "$BcBaseUri/v2.0/$TenantId/$BcEnvironment/ODataV4/Company('$BcCompanyUrlEncoded')/$BcWebServiceName"
$SampleBCAutomationUrl = "$BcBaseUri/v2.0/$TenantId/$BcEnvironment/api/microsoft.automation/v1.0/companies('$BcCompanyUrlEncoded')/$BcAutomationServiceName"

function Invoke-BCWebService
{
    [CmdletBinding()]
    param (
        [Parameter()]
        [string]
        $RequestUrl,

        [Parameter()]
        [string]
        $AccessToken
    )

    return Invoke-RestMethod -Uri $RequestUrl -Headers @{ Authorization = "Bearer $AccessToken" }
}

function Write-BCWebServiceResponse
{
    [CmdletBinding()]
    param (
        [Parameter()]
        $Response
    )

    $Response.value | Format-Table -Property No, Name
}

function Write-BCAutomationResponse
{
    [CmdletBinding()]
    param (
        [Parameter()]
        $Response
    )

    $Response.value | Format-Table -Property ID, Name
}

Export-ModuleMember -Variable ClientId,RedirectUri,RedirectUriWeb,TenantId,AuthorityUri,BcScopes,BcAutomationScopes,SampleBCODataUrl,SampleBCAutomationUrl
Export-ModuleMember -Function Invoke-BCWebService,Write-BCWebServiceResponse,Write-BCAutomationResponse
