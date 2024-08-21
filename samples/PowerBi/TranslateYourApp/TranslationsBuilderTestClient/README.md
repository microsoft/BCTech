# App-Owns-Data Hello World Sample
This is a minimal .NET 5 sample application to embed either a standard Power BI report or a paginated report. 
You must install the .NET 5 SDK before you can run this sample. 
You should be able run and test the sample in either Visual Studio Code or Visual Studio 2019.
The only file you need to update is **appSettings.json** which initially looks like this. 

```javascript
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "YOUR_TENANT_NAME",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "ClientSecret": "YOUR_CLIENT_SECRET",

  },
  "PowerBi": {
    "PowerBiServiceApiRoot": "https://api.powerbi.com/",
    "PowerBiServiceApiResourceId": "https://analysis.windows.net/powerbi/api",
    "WorkspaceId": "YOUR_WORKSPACE_ID",
    "ReportId": "YOUR_REPORT_ID"
},
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

The default settings for **PowerBiServiceApiRoot** and **PowerBiServiceApiResourceId** are configured to use the Power BI Public Cloud.
If you are using Power BI embedding in a different cloud, the settings for *PowerBiServiceApiRoot* and *PowerBiServiceApiResourceId* 
must be updated according to the cloud-specific values shown below.

### Power BI Public Cloud
```Javascript
"PowerBiServiceApiRoot": "https://api.powerbi.com/",
"PowerBiServiceApiResourceId": "https://analysis.windows.net/powerbi/api",
```

## Power BI GCC Cloud
```Javascript
"PowerBiServiceApiRoot": "https://api.powerbigov.us/",
"PowerBiServiceApiResourceId": "https://analysis.usgovcloudapi.net/powerbi/api",
```

### Power BI DoDCON Cloud
```Javascript
"PowerBiServiceApiRoot": "https://api.high.powerbigov.us/",
"PowerBiServiceApiResourceId": "https://high.analysis.usgovcloudapi.net/powerbi/api",
```

### Power BI DoD Cloud
```Javascript
"PowerBiServiceApiRoot": "https://api.mil.powerbigov.us/",
"PowerBiServiceApiResourceId": "https://mil.analysis.usgovcloudapi.net/powerbi/api",
```

### Power BI German Cloud
```Javascript
"PowerBiServiceApiRoot": "https://api.powerbi.de/",
"PowerBiServiceApiResourceId": "https://analysis.cloudapi.de/powerbi/api",
```

### Power BI Chinese Cloud
```Javascript
"PowerBiServiceApiRoot": "https://api.powerbi.cn/",
"PowerBiServiceApiResourceId": "https://analysis.chinacloudapi.cn/powerbi/api",
```