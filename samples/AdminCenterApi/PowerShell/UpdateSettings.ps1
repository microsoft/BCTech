# This file contains examples of API calls that can be used to update settings for environments.
# WARNING: the sample will modify some environments; execute this code at your own risk.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Get update window
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Update window: $(ConvertTo-Json (ConvertFrom-Json $response.Content) )"


# Set update window
$environmentName = "MyProd"
$preferredStartTimeUtc = "2020-06-01T02:00:00Z"
$preferredEndTimeUtc   = "2020-06-01T09:00:00Z"
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/settings/upgrade" `
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
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/upgrade" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Scheduled update: $(ConvertTo-Json (ConvertFrom-Json $response.Content) )"


# Get security group access
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/settings/securitygroupaccess" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Security group access: $(ConvertTo-Json (ConvertFrom-Json $response.Content) )"


# Set security group access
$environmentName = "MyProd"
$aadGroupAccessValue = "00000000-1111-2222-3333-444444444444"
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/settings/securitygroupaccess" `
    -Body   (@{
                 Value = $aadGroupAccessValue
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"


# Delete security group access
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Delete `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/settings/securitygroupaccess" `
    -Headers @{Authorization=("Bearer $accessToken")}



