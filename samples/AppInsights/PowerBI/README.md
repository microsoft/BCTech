In this folder, you will find samples that illustrate how you can use Application Insights data in PowerBI reports.

# Convert a KQL query into a M query
If you have a nice KQL query that you want to use in Power BI, then do as follows
1) run the query in the Application Insights portal (under Logs)
2) Click *Export* and choose *Export to Power BI (M query)*. This downloads a M query as a txt file.
3) Follow the instructions in the txt file to use the M query as a datasource in PowerBI 

Read more about this topic here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/export-power-bi

# Use PowerBI parameters to set properties such as AAD tenant id and environment name
If your Application Insights resource contain data from multiple environments, then you might want to use parameters in your Power BI reports. In the sample report *PartnerTelemetryTemplate.pbix*, you can see how to use parameters to achieve this.

Read more about Power BI parameters topic here: https://powerbi.microsoft.com/en-us/blog/deep-dive-into-query-parameters-and-power-bi-templates/