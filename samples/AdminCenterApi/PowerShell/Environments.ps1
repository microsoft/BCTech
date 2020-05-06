# This sample shows how to manage environments for a customer.
# Warning: the sample will modify some environments; execute this code at your own risk.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Read from the access token which tenant we are authenticated to
$aadTenantId = (ConvertTo-Json (ConvertFrom-Json ([System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($accessToken.Split('.')[1])))).tid)


# Get list of environments
Write-Host -ForegroundColor Cyan "Listing environments for customer $aadTenantId..."
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments" `
    -Headers @{Authorization=("Bearer $accessToken")}
$environments = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host $environments


# Set AppInsights key
$environmentName = "MyProd"
$newAppInsightsKey = [guid]::NewGuid()
Write-Host -ForegroundColor Cyan "Updating the AppInsights key to $newAppInsightsKey for environment $environmentName for customer $aadTenantId..."
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
Write-Host -ForegroundColor Cyan "Getting the update window for environment $environmentName for customer $aadTenantId..."
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Headers @{Authorization=("Bearer $accessToken")}
$updateWindow = ConvertTo-Json (ConvertFrom-Json $response.Content) # prettify json
Write-Host "Update window: $updateWindow"



# set update window
$environmentName = "MyProd"
$updateWindowStart = "2000-01-01T02:00:00Z"
$updateWindowEnd   = "2000-01-01T09:00:00Z"
Write-Host -ForegroundColor Cyan "Setting the update window for environment $environmentName for customer $aadTenantId..."
$response = Invoke-WebRequest `
    -Method Put `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/applications/businesscentral/environments/$environmentName/settings/upgrade" `
    -Body   (@{
                 preferredStartTimeUtc = $updateWindowStart
                 preferredEndTimeUtc   = $updateWindowEnd
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"




# Create new environment




# Copy production environment to a sandbox environment



