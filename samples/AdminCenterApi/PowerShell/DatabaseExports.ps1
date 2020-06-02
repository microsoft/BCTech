# This file contains examples of API calls for starting and monitoring database exports.


# Shared Parameters
#$accessToken = "" # get this from the Authenticate sample


# Getting export metrics, specifically how many times can you export
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/exports/applications/businesscentral/environments/$environmentName/metrics" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))


# The next section covers exporting a database to an Azure storage account.
# It is assumed that you have an Azure subscription, and that you have created a storage account in it.
$azureSubscriptionName = "<FILL OUT>"
$resourceGroupName = "<FILL OUT>"
$storageAccountName = "<FILL OUT>"
Login-AzAccount -Subscription $azureSubscriptionName
$storageAccount = Get-AzStorageAccount -ResourceGroupName $resourceGroupName -Name $storageAccountName


# Create a storage container in the storage account
$containerName = "mycontainer2"
Get-AzStorageContainer -Context $storageAccount.Context -Name $containerName -ErrorVariable containerError -ErrorAction Ignore
if (!$containerError)
{
    New-AzStorageContainer -Context $storageAccount.Context -Name $containerName
}


# Create a SAS URL for the blob that should hold our exported database
$blobName = "myblob2"
$sasToken = New-AzStorageAccountSASToken -Context $storageAccount.Context -Service Blob -ResourceType Container,Object -Permission cdrw -ExpiryTime (Get-Date).AddHours(25) -Protocol HttpsOnly
$sasUrl = "$($storageAccount.PrimaryEndpoints.Blob)$containerName/$blobName$sasToken"
Write-Host "SAS URL: $sasUrl"


# Export environment to a .bacpac file in a storage account
$environmentName = "MyProd"
$response = Invoke-WebRequest `
    -Method Post `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/exports/applications/businesscentral/environments/$environmentName" `
    -Body   (@{
                 storageAccountSasUri = $sasUrl
                 container = $containerName
                 blob = $blobName
              } | ConvertTo-Json) `
    -Headers @{Authorization=("Bearer $accessToken")} `
    -ContentType "application/json"
Write-Host "Responded with: $($response.StatusCode) $($response.StatusDescription)"



# Check export history
$startTime = (Get-Date).AddDays(-1)
$endTime = (Get-Date).AddDays(1)
$response = Invoke-WebRequest `
    -Method Get `
    -Uri    "https://api.businesscentral.dynamics.com/admin/v2.1/exports/history?start=$startTime&end=$endTime" `
    -Headers @{Authorization=("Bearer $accessToken")}
Write-Host (ConvertTo-Json (ConvertFrom-Json $response.Content))

