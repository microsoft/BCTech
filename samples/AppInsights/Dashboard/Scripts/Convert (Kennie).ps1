. (Join-Path $PSScriptRoot "ConvertExportedDashboardToDashboardTemplate.ps1") `
    -ExportedDashboardFile "C:\Users\freddyk\Downloads\KennieTestWS.json" `
    -TemplateFile "C:\Users\freddyk\Downloads\Kennie Test Dashboard.json" `
    -myAppInsightsSubscription "0eac3279-98e8-4f24-bc7e-217785e4dfdd" `
    -myAppInsightsResourceGroup "moagar-test-PartnerTelemetry" `
    -myAppinsightsName "AppInsightsClient1" `
    -myDashboardSubscription "" `
    -myDashboardResourceGroup "" `
    -myDashboardResourceId ""
