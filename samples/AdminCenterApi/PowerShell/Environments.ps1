# This file contains examples of API calls that can be used to manage environments for a customer.
# WARNING: the sample will modify some environments; execute this code at your own risk.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Get list of environments
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Create new environment
$newEnvironmentName = "MyNewSandbox"
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$newEnvironmentName" `
    -Body   (@{
                 EnvironmentType = "Sandbox"
                 CountryCode     = "DK"
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"


# Copy production environment to a sandbox environment
$environmentName = "MyProd"
$newEnvironmentName = "MyNewSandboxAsACopy"
$response = Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/copy" `
    -Body   (@{
                 EnvironmentName = $newEnvironmentName
                 Type            = "Sandbox"
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"


# Rename an environment
$currentEnvironmentName = "MyProd"
$newEnvironmentName = "MyNewProd"
$response = Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/rename" `
    -Body   (@{
                 NewEnvironmentName = $newEnvironmentName
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"


# Restart an environment
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/restart" `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"


# Set AppInsights key
$environmentName = "MyProd"
$newAppInsightsKey = "00000000-1111-2222-3333-444444444444"
$response = Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/settings/appinsightskey" `
    -Body   (@{
                 key = $newAppInsightsKey
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
Write-Host "Responded with: $($response.StatusCode) $($response.StatusDescription)"


# Get support settings
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/support/applications/businesscentral/environments/$environmentName/supportcontact" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get storage (database and file) used by an environment in KB
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/usedstorage" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get storage (database and file) across all environments
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/environments/usedstorage" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content)) 


# Get all tenant quotas
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/environments/quotas" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get operations for an environment
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/environments/$environmentName/operations" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get operations for all environments
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/environments/operations" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get an operation for an environment
$environmentName = "MyProd"
$operationId = "00000000-1111-2222-3333-444444444444"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/environments/$environmentName/operations/$operationId" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


