# This file contains examples of API calls for viewing and cancelling sessions.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Get active sessions for an environment
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/sessions" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Cancel a session
$sessionId = "<FILL OUT FROM THE RESPONSE OF THE PREVIOUS CALL>"
$response = Invoke-WebRequest `
    -Method Delete `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.11/applications/businesscentral/environments/$environmentName/sessions/$sessionId" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host "Responded with: $($response.StatusCode) $($response.StatusDescription)"
