# Sample dashboards

To reduce the time-to-value for you, we have prepared a set of dashboards that you can deploy to your Azure portal:
1. Open Company Performance
2. Session overview
3. Long Running SQL Queries
4. Failed Authorization
5. Web Service Calls

Clicking the *Deploy To Azure* button below will launch the Azure Portal with an ARM template, where you need to specify the subscription, resource group and name of 
your Application Insights Resource. All requested dashboards will be installed and you can now remove the ones you do not need.

<a href="https://freddyk.azurewebsites.net/api/AzureDeploy" target="_blank"><img src="https://raw.githubusercontent.com/Azure/azure-quickstart-templates/master/1-CONTRIBUTION-GUIDE/images/deploytoazure.png"/></a>

Each dashboard is a JSON file, that describes which *widgets* the dashboard should contain.



# Clone the repo

We know that the dashboards we have provided might not match your needs exactly, and if you want to customize them, we recommend that you clone this repo and make your adjustments there, before importing the dashboard in the Azure portal.

Adding dashboards is done by exporting a dashboard from the Azure Portal, running the ConvertExportedDashboardToDashboardTemplate.ps1 and then adding the new template to the resources section in azuredeploy.json.

As we improve our dashboards, you can merge the changes into your cloned repo and in this way stay up-to-date.



# Disclaimer
Sample code included in this repository is made available AS IS.