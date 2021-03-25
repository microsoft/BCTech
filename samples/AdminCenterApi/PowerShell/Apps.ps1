# This file contains examples of API calls that can be used to manage apps for an environment.


# Shared Parameters
$environmentName = "MyProd"
#$accessToken = "" # get this from the Authenticate sample


# List installed apps
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/businesscentral/environments/$environmentName/apps" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Get available updates
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/businesscentral/environments/$environmentName/apps/availableUpdates" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Update one of the apps
$appIdToUpdate = "334ef79e-547e-4631-8ba1-7a7f18e14de6"
$appTargetVersion = "16.0.11240.12188"
$response= Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/businesscentral/environments/$environmentName/apps/$appIdToUpdate/update" `
    -Body   (@{
                 targetVersion = $appTargetVersion
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Check update status
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/businesscentral/environments/$environmentName/apps/$appIdToUpdate/operations" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# Install AppSource app 
# Remember to read the the app provider's terms of use and privacy policy on AppSource. 
# Understand that the rights to use this app do not come from Microsoft, unless Microsoft is the provider. 
# To acknowledge set "AcceptIsvEula" to $true.

$appIdToInstall = "638dd7bc-cb27-4c70-9e1d-963fdd46da19"
$appTargetVersion = "17.0.40514.0"

try {
    $response= Invoke-WebRequest `
        -Method Post `
        -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/BusinessCentral/environments/$environmentName/apps/$appIdToInstall/install" `
        -Body   (@{
                    "AcceptIsvEula" = $false #set to $true once you've read the the app provider's terms of use and privacy policy
                    "targetVersion" = $appTargetVersion
                    "languageId" = "1033"    
                    "installOrUpdateNeededDependencies" = $false
                    "useEnvironmentUpdateWindow" = $false
                } | ConvertTo-Json) `
        -Headers @{Authorization=("Bearer $accessToken")} `
        -ContentType "application/json"
} 
catch [System.Net.WebException]
{
    $Exception = $_.Exception
    $respStream = $Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($respStream)
    $respBody = $reader.ReadToEnd() | ConvertFrom-Json | ConvertTo-Json -Depth 100
    $reader.Close();
    Write-Error $Exception.Message
    Write-Error $respBody -ErrorAction Stop
} 

# Check install status
$response= Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/BusinessCentral/environments/$environmentName/apps/$appIdToInstall/operations" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))

# Uninstall AppSource app
$appIdToUninstall = "6992416f-3f39-4d3c-8242-3fff61350bea"
try {
    $response= Invoke-WebRequest `
        -Method Post `
        -Uri    "https://api.businesscentral.dynamics.com/admin/v2.6/applications/BusinessCentral/environments/$environmentName/apps/$appIdToUninstall/uninstall" `
        -Body   (@{
                    "uninstallDependents" = $true 
                    "useEnvironmentUpdateWindow" = $false
                    "deleteData" = $false
                } | ConvertTo-Json) `
        -Headers @{Authorization=("Bearer $accessToken")} `
        -ContentType "application/json"
} 
catch [System.Net.WebException]
{
    $Exception = $_.Exception
    $respStream = $Exception.Response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($respStream)
    $respBody = $reader.ReadToEnd() | ConvertFrom-Json | ConvertTo-Json -Depth 100
    $reader.Close();
    Write-Error $Exception.Message
    Write-Error $respBody -ErrorAction Stop
} 
