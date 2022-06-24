# Script to be used to get AAD mapping json for the PowerBI app https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/Reports/AppSource/README.md#power-bi-prerequisites

# Instructions
# 1) download CSV with the customers from your Microsoft Partner Center
# 2) Use the csv file as input into this script: Get-AADMapping.ps1 -CsvFile filename.csv
# Output is the minified json you can use in the settings of the Power BI App

param(
    [Parameter(Mandatory=$true)]
    $CsvFile
)

$Customers = Import-Csv -Path $CsvFile -Delimiter ','

$map = @()
foreach($Customer in $Customers) {
    $NewMap = New-Object -TypeName PSObject
    $NewMap | Add-Member -MemberType NoteProperty -Name 'AAD tenant id' -Value $Customer.'Microsoft ID'
    $NewMap | Add-Member -MemberType NoteProperty -Name 'Domain' -Value $Customer.'Primary domain name'
    $map += $NewMap
}
$Mapping=@{"map"=$map}

$Mapping | ConvertTo-Json -Depth 3 -Compress


#
# Kudos to Microsoft MVP Kamil Sacek for providing the script
#