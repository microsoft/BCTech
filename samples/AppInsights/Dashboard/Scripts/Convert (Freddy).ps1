. (Join-Path $PSScriptRoot "ConvertExportedDashboardToDashboardTemplate.ps1") `
    -ExportedDashboardFile "C:\Users\freddyk\Downloads\Business Central VAR Dashboard.json" `
    -TemplateFile "C:\Users\freddyk\Downloads\Business Central VAR Dashboard2.json" `
    -myAppInsightsSubscription "97d6b765-89fc-40e9-b253-baee2b19d6db" `
    -myAppInsightsResourceGroup "myinsights" `
    -myAppinsightsName "fkappinsights" `
    -myDashboardSubscription "97d6b765-89fc-40e9-b253-baee2b19d6db" `
    -myDashboardResourceGroup "myinsights" `
    -myDashboardResourceId "34c6ce6f-94bc-4730-975e-679bfe3f0612"
