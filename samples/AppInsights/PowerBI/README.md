In this folder, you will find samples that illustrate how you can use Application Insights data in PowerBI reports.

# Performance Report
To use the Performance Report, do as follows
1) Download and install Power BI Desktop: https://powerbi.microsoft.com/en-us/downloads/
2) Download the Performance Report.pbit template file
3) Open the Performance Report.pbit template file in Power BI Desktop
4) Fill in parameters (Azure Application Insights App id is required. Get it from the *API Access* menu in the Azure Application Insights portal). 
5) (Optional) Save the report as a normal .pbix file

# Authentication support
Currently, Power BI only supports AAD authentication to Application Insights. This means that the approaches we use in Jupyter notebooks (using App id and API key) and Azure Dashboards (using Application Insights subscription id, Application Insights name, and Application Insights resource group) are not applicable in Power BI. To use Power BI with data from Application Insights, the user of the report must be in the same AAD tenant as the Application Insights resource and need to have read access to Application Insights resource.

# Convert a KQL query into a M query
If you have a nice KQL query that you want to use in Power BI, then do as follows
1) run the query in the Application Insights portal (under Logs)
2) Click *Export* and choose *Export to Power BI (M query)*. This downloads a M query as a txt file.
3) Follow the instructions in the txt file to use the M query as a datasource in PowerBI 

Read more about this topic here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/export-power-bi

# Use PowerBI parameters to set properties such as AAD tenant id and environment name
If your Application Insights resource contain data from multiple environments, then you might want to use parameters in your Power BI reports. In the sample report *PartnerTelemetryTemplate.pbix*, you can see how to use parameters to achieve this.

Read more about Power BI parameters topic here: https://powerbi.microsoft.com/en-us/blog/deep-dive-into-query-parameters-and-power-bi-templates/