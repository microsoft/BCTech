[CmdletBinding()]
param (
    [Parameter(Mandatory=$true)]
    [string] $ExportedDashboardFile,

    [Parameter(Mandatory=$true)]
    [string] $TemplateFile,

    [Parameter(Mandatory=$true)]
    [string] $myAppInsightsSubscription,

    [Parameter(Mandatory=$true)]
    [string] $myAppInsightsResourceGroup,

    [Parameter(Mandatory=$true)]
    [string] $myAppinsightsName,

    [Parameter(Mandatory=$true)]
    $myDashboardSubscription,

    [Parameter(Mandatory=$true)]
    $myDashboardResourceGroup,

    [Parameter(Mandatory=$true)]
    $myDashboardResourceId
)

Write-Host "Reading"
$dashboardStr = Get-Content -Path $ExportedDashboardFile -Raw
$dashboardStr = @"
{
 "`$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
 "contentVersion": "1.0.0.0",
 "parameters": {
  "subscriptionid": { "type": "string" },
  "resourcegroupname": { "type": "string" },
  "appinsightname": { "type": "string" },
  "resourcelocation": { "type": "string" },
  "appinsightresourceid": { "type": "string" },
  "dashboardresourceid": { "type": "string" }
 },
 "resources": [
$dashboardStr
 ],
 "outputs": {}
}
"@

Write-Host "Converting"
# Fix references to AppInsight, Dashboard and the "INSERT LOCATION" made by dashboard export
$replacements = [ordered]@{
    "/subscriptions/$myAppInsightsSubscription/resourceGroups/$myAppInsightsResourceGroup/providers/microsoft.insights/components/$myAppinsightsName" = "[parameters('appinsightresourceid')]"
    "/subscriptions/$myDashboardSubscription/resourceGroups/$myDashboardResourceGroup/providers/Microsoft.Portal/dashboards/$myDashboardResourceId" = "[parameters('dashboardresourceid')]"
    """$myAppInsightsSubscription""" = """[parameters('subscriptionid')]"""
    """$myAppInsightsResourceGroup""" = """[parameters('resourcegroupname')]"""
    """$myAppinsightsName""" = """[parameters('appinsightname')]"""
    """INSERT LOCATION""" = """[parameters('resourcelocation')]"""
}

# Remove special characters from resource names
$dashboardJson = $dashboardStr | ConvertFrom-Json
$dashboardJson.resources | % {
    $replacements += [ordered]@{
        """name"": ""$($_.name)""" = """name"": ""$($_.name -replace '[\W]','')"""
    }
}

$replacements.GetEnumerator() | % {
    $dashboardStr = $dashboardStr -replace $_.Name, $_.Value
}

Write-Host "Writing"
Set-Content -Path $TemplateFile -Value $dashboardStr
