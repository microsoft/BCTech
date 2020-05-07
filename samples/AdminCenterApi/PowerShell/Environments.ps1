# This file contains examples of API calls that can be used to manage environments for a customer.
# WARNING: the sample will modify some environments; execute this code at your own risk.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Get list of environments
Write-Host -ForegroundColor Cyan "Listing environments for a customer..."
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments" `
    -Headers @{Authorization=("Bearer $accessToken")}
$environments = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host $environments


# Set AppInsights key
$environmentName = "MyProd"
$newAppInsightsKey = "00000000-1111-2222-3333-444444444444"
$response = Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/appinsightskey" `
    -Body   (@{
                 key = $newAppInsightsKey
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
Write-Host "Responded with: $($response.StatusCode) $($response.StatusDescription)"


# Get update window
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Update window: $(ConvertTo-Json (ConvertFrom-Json $response.Content) )"


# set update window
$environmentName = "MyProd"
$preferredStartTimeUtc = "2000-01-01T02:00:00Z"
$preferredEndTimeUtc   = "2000-01-01T09:00:00Z"
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Body   (@{
                 preferredStartTimeUtc = $preferredStartTimeUtc
                 preferredEndTimeUtc   = $preferredEndTimeUtc
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"


# Create new environment
$newEnvironmentName = "MyNewSandbox"
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$newEnvironmentName" `
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
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$newEnvironmentName" `
    -Body   (@{
                 EnvironmentType         = "Sandbox"
                 CountryCode             = "DK"
                 copyFromEnvironmentName = $environmentName
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"

