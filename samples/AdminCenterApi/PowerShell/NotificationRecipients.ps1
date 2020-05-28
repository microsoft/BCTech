# This file contains examples of API calls that can be used to manage the notification recipients for a customer.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Get notification recipients
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/settings/notification/recipients" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Add a notification recipient
$environmentName = "MyProd"
$email = "partnernotifications@partnerdomain.com"
$name = "Partner Notifications Mail Group"
$response = Invoke-WebRequest `
    -Method PUT `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/settings/notification/recipients" `
    -Body   (@{
                 email = $email
                 name = $name
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
Write-Host "Responded with: $($response.StatusCode) $($response.StatusDescription)"

