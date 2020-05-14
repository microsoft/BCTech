# This file contains examples of API calls that can be used to manage apps for an environment.


# Shared Parameters
$environmentName = "MyProd"
#$accessToken = "" # get this from the Authenticate sample


# List installed apps
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get available updates
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps/availableUpdates" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Update one of the apps
$appIdToUpdate = "334ef79e-547e-4631-8ba1-7a7f18e14de6"
$appTargetVersion = "16.0.11240.12188"
$response= Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps/$appIdToUpdate/update" `
    -Body   (@{
                 targetVersion = $appTargetVersion
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Check update status
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps/$appIdToUpdate/operations" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))

