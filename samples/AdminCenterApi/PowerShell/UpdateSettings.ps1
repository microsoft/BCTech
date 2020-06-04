# This file contains examples of API calls that can be used to update settings for environments.
# WARNING: the sample will modify some environments; execute this code at your own risk.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Get update window
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Update window: $(ConvertTo-Json (ConvertFrom-Json $response.Content) )"


# Set update window
$environmentName = "MyProd"
$preferredStartTimeUtc = "2020-06-01T02:00:00Z"
$preferredEndTimeUtc   = "2020-06-01T09:00:00Z"
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Body   (@{
                 preferredStartTimeUtc = $preferredStartTimeUtc
                 preferredEndTimeUtc   = $preferredEndTimeUtc
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"

# Get scheduled updates
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/upgrade" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Scheduled update: $(ConvertTo-Json (ConvertFrom-Json $response.Content) )"



