In this folder, you will find samples of Kusto Query Language (KQL) for each type of signal that is send to Application Insights.

You can use Kusto queries as the data source in a number of places. E.g.
* the Logs part of Application Insights in the Azure portal
* PowerBI reports
* Azure Monitor Alerts
* Azure Dashboards
* Jupyter Notebooks (with the Kqlmagic extension)

Signal is added incrementally to Business Central. In this table you can see in which version or update a class of signal was added/modified:

|Signal | Emited from version, update | Documentation |
| ------ | ------ | ------ |
| Long running operation (SQL query) | 2019 release wave 2, RC | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-long-running-sql-query-trace |
| Authorization | 2019 release wave 2, Update 2| https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace |
| Web Service Request | 2020 release wave 1, RC| https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace (not live yet) |
| Report Execution | 2020 release wave 1, RC| https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-reports-trace (not live yet) | 
| Open Company timing | 2020 release wave 1, RC | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace (not live yet) |


# Disclaimer
Sample code included in this repository is made available AS IS.