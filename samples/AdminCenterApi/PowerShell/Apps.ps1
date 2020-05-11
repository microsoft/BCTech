# This file contains examples of API calls that can be used to manage apps for an environment.


# Shared Parameters
$environmentName = "MyProd"
#$accessToken = "" # get this from the Authenticate sample


# List installed apps
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps" `
    -Headers @{Authorization=("Bearer $accessToken")}
$installedApps = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host $installedApps


# Get available updates
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps/availableUpdates" `
    -Headers @{Authorization=("Bearer $accessToken")}
$availableUpdates = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host $availableUpdates


# Update one of the apps
$appIdToUpdate = "63ca2fa4-4f03-4f2b-a480-172fef340d3f"
$appTargetVersion = "16.0.11240.12736"
$response= Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps/$appIdToUpdate/update" `
    -Body   (@{
                 targetVersion = $appTargetVersion
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
$operationStatus = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host $operationStatus


# Check update status
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/apps/$appIdToUpdate/operations" `
    -Headers @{Authorization=("Bearer $accessToken")}
$operations = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host $operations

