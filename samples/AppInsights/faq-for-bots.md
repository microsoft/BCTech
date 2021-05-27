# What is Business Central telemetry?
Business Central continuously emits telemetry about events that happen in the service.


Read more about telemetry here: https://aka.ms/bctelemetry

# What can I use telemetry for?
Telemetry can be useful for partners, e.g., when troubleshooting an issue or to determine how often a feature is used.



# What kind of telemetry exists?
As a developer of an app (typically referred to as an **ISV**), which gets installed in a Business Central environment, or as the partner on record for a customer (typically referred to as a **VAR**), you can obtain some of this telemetry.


Read more about telemetry here: https://aka.ms/bctelemetry

# What resources can I find in aka.ms/bctelemetrysamples?
The sample GitHub repository https://aka.ms/bctelemetrysamples contains instructions for how you can obtain the telemetry.


It also contains resources that help you get immediate value from the telemetry.


Please visit the FAQ page https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md for any questions on how to get started, pricing, privacy, and more.


## Want to Learn Business Central telemetry in a week?
Haven't got the time to learn Business Central telemetry? Good news: we have recorded a series of short videos on the topic. Watch one every morning for a week and you should be up and running. 


Find the video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


### How do I get started with Application Insights?
To get started with Application Insights, watch this 4 min video to learn how to connect an environment or an extension to Azure Application Insights: https://www.youtube.com/watch?v=f4rxr20QG04


Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


### How can I analyze performance using the Power BI performance report? 
To analyze performance using the Power BI performance report, watch this 8 min video. You will learn how to use the standard Power BI performance report to triage performance issues happening in an environment or an extension: https://www.youtube.com/watch?v=Kbq6YB8VU-8


Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


### How can I triage errors using the Power BI error report? (6 min)
To triage errors using the Power BI error report, watch this 6 min video. You will learn how to use the standard Power BI error report to triage errors happening in an environment or an extension: https://www.youtube.com/watch?v=ByealbDQqIU


Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


### How can I write custom Kusto queries in Dynamics 365 Business Central? 
Watch this 9 min video to learn how to write Kusto queries against Business Central telemetry: https://www.youtube.com/watch?v=A1e9ZMo5xcY


Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


Learn more about KQL here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/KQL/README.md


### How can I troubleshoot issues using the Jupyter Notebook troubleshooting guides? 
To learn how to troubleshoot issues using the Jupyter Notebook troubleshooting guides, watch this 8 min video. You will learn how to troubleshoot issues using the Jupyter Notebook troubleshooting guides: https://www.youtube.com/watch?v=B3EL0xdvaUY


Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


Learn more about Business Central troubleshooting guides here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/TroubleShootingGuides/README.md


### How can I emit telemetry to Azure Application Insights from AL code? 
To learn how to emit telemetry to Azure Application Insights from AL code, watch this 1 min video. You will learn how custom telemetry works: https://www.youtube.com/watch?v=gFG5E9Xd5bA


If you develop extensions for Business Central, then this is a must-see. 


Learn more about Business Central custom telemetry here: https://aka.ms/bctelemetry

### How can I add Azure Alerts as push notifications on my phone?
Watch this 4 min video to learn how to add Azure Alerts as push notifications on your phone: https://www.youtube.com/watch?v=nqqVEISjSGE

Find sample queries that you can use as a starting point here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts 



## How do I get started with telemetry?
Business Central can send telemetry to one or more **Azure Application Insights** (Application Insights) accounts.
The first step thus is for you to create an Application Insights account.


See https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tenant-admin-center-telemetry for instructions on how to do that.


Once you have created the Application Insights account, make a note of the instrumentation key.


The next step depends on whether you are an ISV or a VAR:


If you are an ISV partner, you must specify the instrumentation key in your app.json file. Once the app is installed in a Business Central environment, telemetry relating to your app will start to flow into your Application Insights account.


If you are a VAR partner, you must enter the instrumentation key in the Business Central Admin Center of your customer(s). Once you have done that, telemetry relating to your customers will start to flow into your Application Insights account. You can also set the instrumentation key using the Business Central Administration Center API.


Please visit the documentation for more details 
Business Central Developer and IT-pro documentation - Monitoring and Analyzing Telemetry: https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview


Business Central Administration Center API - How to set the telemetry key: https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/administration-center-api#put-appinsights-key

Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.

## What does telemetry cost?
Application Insights is billed based on the volume of telemetry data that your application sends. Currently, the first 5 GB of data per month is free. Regarding data retention, every GB of data ingested can be retained at no charge for up to first 90 days.


Please check the documentation <https://azure.microsoft.com/en-us/pricing/details/monitor/> for up-to-date information on pricing.


Azure monitor alerts are billed separately.


Please visit the FAQ(https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.

## How can I reduce telemetry cost?
To reduce ingestion cost, you can


1) set limits on daily data ingestion, or


2) reduce data ingestion by sampling to only ingest a percentage of the inbound data (see https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#ingestion-sampling), or


3) set alerts on cost thresholds being exceeded to get notified if this happens


Use the KQL helper query MonthlyIngestion.kql to see the data distribution of different event ids in your telemetry database.


See all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


Please visit the [FAQ](FAQ.md) for more frequently asked questions.


## Where can I learn more about Kusto Query Language (KQL) and Azure Data Studio?
Please visit the [KQL README page](KQL/README.md) for learning resources on KQL and the [Trouble Shooting Guides README page](TroubleShootingGuides/README.md) for learning resources on Azure Data Studio.


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.

## How can I see what data is available in my Application Insights subscription
Use this KQL query [AvailableSignal.kql](KQL/Queries/HelperQueries/AvailableSignal.kql) to see if you have any data in your telemetry database, and also what kind of signal is present.


See all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.


## I deployed Azure dashboards, but they show no data
If you have data present in Application Insights, please check the setting in the *Time range* selector on the dashboard.


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.


## What is the data retention policy in Application Insights?
The default retention for Application Insights resources is 90 days. Different retention periods can be selected for each Application Insights resource. The full set of available retention periods is 30, 60, 90, 120, 180, 270, 365, 550 or 730 days.


See https://docs.microsoft.com/en-us/azure/azure-monitor/app/pricing#change-the-data-retention-period


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.


## How do I delete data from Application Insights?
You can purge data in an Application Insights component by a set of user-defined filters.


See https://docs.microsoft.com/en-us/rest/api/application-insights/components/purge#examples


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.


## Can I grant read-only access to Application Insights?
To grant a person read-only access to Application Insights, go to the Access control (IAM) page in the Application Insights portal, and then add the role assignment "Reader" to the person. 


You might also need to add the role assignment "Reader" to the person on the Resource Group for the Application Insights subscription.


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.


## What about Privacy regulations such as GDPR?
The Business Central service does not emit any End User Identifiable Information (EUII) to Application Insights. So the telemetry is born GDPR compliant. The service only emits data that is classified as either System Metadata or Organization Identifiable Information (OII). The meaning of these classifications are described here: [DataClassification Option Type](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/methods-auto/dataclassification/dataclassification-option)


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.



## Will you backport the Application Insights instrumentation to versions prior to 15.0?
It took a lot of refactoring in the server and client to make this happen. So it is unlikely that we will backport the Application Insights instrumentation to versions prior to 15.0.


For each new signal type we add, we try to backport to the current major release (16.x right now) if possible.


For on-premises installations (private or public cloud), you can create an application/service that listens on the ETW (Event Tracing for Windows) events that we use for internal telemetry and then send them to Application Insights. Note that this approach is depending on internal telemetry events that might change and that are not documented by Microsoft.


This is documented here: https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tools-monitor-performance-counters-and-events and here https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/monitor-server-events


See the Application Insights documentation for an introduction on how to emit telemetry from a .NET console application:
[The Application Insights for .NET console applications](https://docs.microsoft.com/en-us/azure/azure-monitor/app/console)


Please visit the FAQ (https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md) for more frequently asked questions.

## How do I create alerts in Azure Application Insights?
To create alerts in Azure Application Insights, here is an example to get you started


1. Open the Azure portal and locate your Application Insights resource


2. Click "Alerts" in the navigation pane on the left


3. Use one of the KQL samples from this section in the condition for a custom log search 


Please visit the Alerting FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts) for more frequently asked questions about alerts.

## How do I get alerts via email?
If you want alerts via email, you can just create a new action group in your Application Insights resource, and in your alerts add an action to send an email.


Please visit the Alerting FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts) for more frequently asked questions about alerts.


## How do I get alerts via Microsoft Teams?
If you want to send alerts to a channel in Microsoft Teams, then see an example of how to set that up here: https://dailydotnettips.com/sending-your-azure-application-insights-alerts-to-team-sites-using-azure-logic-app/


Please visit the Alerting FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts) for more frequently asked questions about alerts.


## How do I get alerts on my phone?
To get alerts on your phone, take a look at this video: https://www.youtube.com/watch?v=nqqVEISjSGE


Please visit the Alerting FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts) for more frequently asked questions about alerts.

## Want to learn more about Azure Monitor alerts?
If you want to learn about Azure Monitor alerts, please read more in the documentation for Azure Application Insights: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/alerts-unified-log


Please visit the Alerting FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts) for more frequently asked questions about alerts.

# How do I get started with Azure dashboards?
To get started with Azure dashboards, we have prepared a set of dashboards that you can deploy to your Azure portal:


1. Open Company Performance


2. Session overview


3. Long Running SQL Queries


4. Failed Authorization


5. Web Service Calls


Please visit the Dashboard FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Dashboard) for more frequently asked questions about Azure Dashboards.

# How can I make my own Azure dashboards?
If you want to make my own Azure dashboards, we recommend that you clone the github samples repository and make your adjustments there, before importing the dashboard in the Azure portal.

Please visit the Dashboard FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Dashboard) for more frequently asked questions about Azure Dashboards.


# Where can I use Kusto Queries?
You can use Kusto queries as the data source in a number of places. E.g.


the Logs part of Application Insights in the Azure portal


PowerBI reports


Azure Monitor Alerts


Azure Dashboards


Jupyter Notebooks (with the Kqlmagic extension)


Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.

# Where can I learn more about KQL (and SQL)?
Here are some resources for you to get started on Kusto Query Language (KQL):


[Kusto Query Language Overview](https://docs.microsoft.com/en-us/azure/kusto/query/)


[Kusto Query Language Tutorial](https://docs.microsoft.com/en-us/azure/kusto/query/tutorial)


[I know SQL. How do I do that in KQL?](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/sqlcheatsheet)


[Kusto Query Language (KQL) from Scratch (Pluralsight course, requires subscription)](https://www.pluralsight.com/courses/kusto-query-language-kql-from-scratch)


[Microsoft Azure Data Explorer - Advanced KQL (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-explorer-advanced-query-capabilities/table-of-contents)


Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.


# Which tools can I use (KQL editors and clients)?
You can write and execute KQL in various tools. E.g.


[Kusto Explorer (desktop application)](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/tools/kusto-explorer). Here is 


[How to connect to Application Insights in Kusto Explorer](https://docs.microsoft.com/en-us/azure/data-explorer/query-monitor-data)


In a Jupyter notebook hosted in [Azure Data Studio](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides#what-is-azure-data-studio)


In a Jupyter notebook hosted in Visual Studio Code (with the Python and Jupyter Notebooks extensions installed)


Application Insights portal (Under *Logs* in the *Monitoring* menu)


PowerShell (using the REST api). See an example here: https://demiliani.com/2020/12/16/using-powershell-to-retrieve-your-dynamics-365-business-central-telemetry/


Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.


# What signal/telemetry is available in which version?
Signal is added incrementally to Business Central. 

Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.


# Do you have Kusto (KQL) queries for performance investigations?
The PerformanceTuning folder in https://aka.ms/bctelemetrysamples have queries that can help you investigate a performance issue.

Note that there also exist predefined troubleshooting guides for performance investigations both in the form ofJupyter notebooks and as a Power BI report.


Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.


Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


Learn more about Business Central troubleshooting guides here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/TroubleShootingGuides/README.md


# Where can I find example KQL queries for each of the event ids in telemetry?
The folder ExampleQueriesForEachArea in https://aka.ms/bctelemetrysamples have KQL scripts with KQL queries for each type of event id present in telemetry.


Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.


Read more about telemetry here: https://aka.ms/bctelemetry


# Which helper KQL queries are useful for what?
The folder HelperQueries in https://aka.ms/bctelemetrysamples have KQL scripts with KQL queries that can be used to quickly answer common questions, such as 


which environments log telemetry to my resource?


which signal is present in my resource?


How much data is ingested in my resource?


Please visit the KQL FAQ (https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL) for more frequently asked questions about Kusto queries.




# Which environments log telemetry to my resource?
To analyze which environments log telemetry to your Application Insights resource, run the helper query AvailableEnvironments.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# Which signal is present in my resource?
To analyze which signal is present in your Application Insights resource, run the helper query AvailableSignal.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# How much data is ingested in my resource?
To analyze how much data is ingested into your Application Insights resource, run the query MonthlyIngestion.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# Which browsers are users using?
To analyze which browsers users are using, run the query BrowserUsage.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries

# When were upgrades happening?
To analyze when upgrades were happening, run the query ComponentUpdates.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries

# Are our environments using deprecated protocols or features?
To analyze if your environments are using deprecated protocols or features, run the query DeprecatedWebServiceProtocols.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# How do I troubleshoot web service issues?
To troubleshoot web service issues, run the query WebServiceCallStatus.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


You can also run the TSG for web service issues. Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I troubleshoot Microsoft connectors?
To troubleshoot Microsoft connector issues, run the query MicrosoftConnectorUsage.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# How do I troubleshoot traffic from Microsoft Power BI?
To troubleshoot traffic from Microsoft Power BI, run the query PowerBIConnectorUsage.kql


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# How can I get a timeline of what happens in a session?
To get a timeline of what happens in a session, run the query *Timeline.kql*


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries

# How can I distinguish data from a per-tenant extension, an appsource extension, or from Microsoft base app?
To distinguish data from a per-tenant extension, an appsource extension, or from Microsoft base app in a KQL query, use the code snippet partnerCodePredicate.kql in your queries.


Get all helper queries here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/KQL/Queries/HelperQueries


# Which Power BI reports are available?
The sample GitHub repository https://aka.ms/bctelemetrysamples contains Power BI reports for 


Investigating performance issues 


Investigating errors


Learn more about Business Central Power BI here: [Power BI FAQ](https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md)


# How do I use Power BI reports for troubleshooting
To use the Performance or Error report, do as follows


1) Download and install Power BI Desktop: [Download Power BI Desktop](https://powerbi.microsoft.com/en-us/downloads/)


2) Download the .pbit template files from the *Reports* directory


3) Open a .pbit template file in Power BI Desktop


4) Fill in parameters (Azure Application Insights App id is required. Get it from the *API Access* menu in the Azure Application Insights portal). 


5) (Optional) Save the report as a normal .pbix file


Learn more about Business Central Power BI here: [Power BI FAQ](https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md)


# How does authentication work between Power BI and Azure Application Insights?
Currently, Power BI only supports AAD authentication to Application Insights. This means that the approaches we use in Jupyter notebooks (using App id and API key) and Azure Dashboards (using Application Insights subscription id, Application Insights name, and Application Insights resource group) are not applicable in Power BI. To use Power BI with data from Application Insights, the user of the report must be in the same AAD tenant as the Application Insights resource and need to have read access to Application Insights resource.


Learn more about Business Central Power BI here: [Power BI FAQ](https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md)


# How do I convert a KQL query into a M query (Power Query)?
If you have a nice KQL query that you want to use in Power BI, then do as follows


1) run the query in the Application Insights portal (under Logs)


2) Click *Export* and choose *Export to Power BI (M query)*. This downloads a M query as a txt file.


3) Follow the instructions in the txt file to use the M query as a datasource in PowerBI 


Learn more about Business Central Power BI here: [Power BI FAQ](https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md)


# How do I use PowerBI parameters to set properties such as AAD tenant id and environment name?
If your Application Insights resource contain data from multiple environments, then you might want to use parameters in your Power BI reports. In the sample report PartnerTelemetryTemplate.pbix, you can see how to use parameters to achieve this.


Learn more about Business Central Power BI here: [Power BI FAQ](https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md)


# How can I modify the standard reports?
All M queries used in the standard reports are available in the "M queries" directory. You can use them if you want to create your own reports based on the data sources defined in the standard reports.


Learn more about Business Central Power BI here: <a href="https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md">Power BI FAQ</a>


# Which Powershell scripts are available?
The sample GitHub repository https://aka.ms/bctelemetrysamples contains Powershell for querying Azure Application Insights.


Please visit the [Powershell FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Powershell) for more frequently asked questions about using Powershell with Kusto queries.


# How do I use Powershell scripts to query telemetry?
To use the script, do as follows


1) Go the Azure Application Insights portal, go to the *API Access* menu, and copy the application id and generate an API key 


2) Open Powershell and 


a) run GetTelemetryData.ps1 -appid <app id> -apikey <api key> -kqlquery <kql query you want to run>


or


b) pipe the content of a text file with a KQL query into the script


Please visit the [Powershell FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Powershell) for more frequently asked questions about using Powershell with Kusto queries.


# How can I use telemetry to troubleshoot problems?
Telemetry in Azure Application Insights is a very powerful tool to investigate issues after they have occurred (where you cannot attach a debugger to troubleshoot the issue). Having a good arsenal of Kusto query language (KQL) scripts can be very handy, but sometimes executing KQL statements one by one in the Azure Application Insights portal can be tedious. This is where notebooks come in handy, and in this part of the samples repository we show you how to get started using Jupyter notebooks in Azure Data Studio. With the Kqlmagic module, you can now combine KQL with Python to create powerful interactive trouble shooting guides 
(TSGs).


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# Which Jupyter Notebook Trouble Shooting Guides (TSGs) are available?
The sample GitHub repository https://aka.ms/bctelemetrysamples contains Jupyter Notebook TSGs for 


Investigating performance issues (overview analysis)


Investigating performance issues in your code (analysis outside Microsoft code base)


Investigating web service issues (throttling, performance, bad endpoints)


Investigating Microsoft connector (PowerBI, PowerApps, LogicApps, or Flow) issues (throttling, performance, bad endpoints


Investigating data-related issues (long running queries, database locks, report performance)


Investigating login issues (authentication and authorization flows)


Investigating if environments are using deprecated web service protocols


Investigating lifecycle issues with extensions (compile, synchronize, publish, install, update, un-install, un-publish)


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# What is Azure Data Studio?
Azure Data Studio is a cross-platform database tool for data professionals using the Microsoft family of on-premises and cloud data platforms on Windows, MacOS, and Linux. Azure Data Studio offers a modern editor experience with IntelliSense, code snippets, source control integration, and an integrated terminal. It's engineered with the data platform user in mind, with built-in charting of query result sets and customizable dashboards. 


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# What is Kqlmagic?
Kqlmagic is a command that extends the capabilities of the Python kernel in Azure Data Studio notebooks. You can combine Python and Kusto query language (KQL) to query and visualize data using rich Plot.ly library integrated with render commands. Kqlmagic brings you the benefit of notebooks, data analysis, and rich Python capabilities all in the same location. Supported data sources with Kqlmagic include Azure Data Explorer, Application Insights, and Azure Monitor logs.


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I learn more about Azure Data Studio?
Here are some resources for you to get started on Azure Data Studio. 


[How to Start with Microsoft Azure Data Explorer (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-explorer-starting/table-of-contents)


[Exploring Data in Microsoft Azure Using Kusto Query Language and Azure Data Explorer (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-exploring/table-of-contents)


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I install and configure Azure Data Studio?
To install and configure Azure Data Studio, you need to 


1. First, you need to download and install Azure Data Studio. See how to here: [Azure Data Studio documentation](https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio)


2. Now create a new notebook (needed for steps 3+4 below) and choose the Python3 kernel, see [Azure Data Studio documentation - Notebooks](https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#create-a-notebook)


3. Install a Python kernel for Azure Data Studio (let Azure Data Studio do it for you or reuse an existing Python installation). See how to here: [Azure Data Studio documentation - Python kernel](https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#change-the-python-kernel)


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.

# How do I install and set up Kqlmagic?
To install and set up Kqlmagic, open a python notebook. Click the "Manage Packages" icon on the right of the notebook.


Under the "Add new" tab, type "kqlmagic" and click Install (we generally recommend people to install non “dev” version of Kqlmagic)


For some users, the KQL Magic installation fails with an error. The Troubleshooting FAQ have solutions for this (see link below).


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I connect a notebook to an Azure Application Insights resource?
To connect a notebook to an Azure Application Insights resource, you need two GUIDs to be able to read data from your Application Insights resource: 


1. Application ID


2. an API key


See more here: [Azure Data Studio documentation - KQL Magic](https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-kqlmagic#kqlmagic-with-application-insights)


Please visit the [Troubleshooting FAQ](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# What is Tenant-level (tenant admin center) and extension-level (app.json) telemetry?
Application Insights can be enabled on two different levels: tenant and extension. When enabled on the tenant, either for a Business Central online tenant or on-premises Business Central Server instance, telemetry is emitted to a single Application Insights resource for gathering data on tenant-wide operations.


With Business Central 2020 release wave 2 and later, Application Insights can also be enabled on a per-extension basis. An Application Insights key is set in the extension's manifest (app.json file). At runtime, certain events related to the extension are emitted to the Application Insights resource. This feature targets publishers of per-tenant extensions to give them insight into issues in their extension before partners and customers report them.


Read more here: [Telemetry documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview#tenant-level-and-extension-level-telemetry)


# Where can I get an overview of telemetry?
Currently, Business Central offers telemetry on numerous operations such as upgrade failures, long running code execution, and user page views.


Get the full overview of telemetry areas here: [Telemetry Signal Overview documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview#available-telemetry)


Get the full list of telemetry event ids here: 
[Event Id Overview documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-event-ids)


# How do I enable telemetry?
Sending telemetry data to Application Insights requires you have an Application Insights resource in Azure. Once you have the Application Insights resource, you can start to configure your tenants and extensions to send telemetry data to your Application Insights resource. The configuration is different for Online and On-premises.


Read more here: [Telemetry documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview#enable)


# How do I read telemetry data in the in Application Insights portal?
Telemetry from Business Central is stored in Azure Monitor Logs in the traces table. You can view collected data by writing log queries. Log queries are written in the Kusto query language (KQL).


Read more here: [Telemetry documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview#viewing)


# What is custom dimension in telemetry?
Each trace includes a customDimensions column. The customDimensions column, in turn, includes a set dimensions that contain metrics specific to the trace.


Read more here: [Custom dimensions documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview#customdimensions)



# Can I enable telemetry / Application Insights for On-Prem?
Telemetry also works for on-premises installations. 


Read more here on how to set it up: [On-premises telemetry documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-enable-application-insights)


# How can I analyze App Key Vault Secrets?
App key vault telemetry gathers information about the acquisition of secrets in Azure Key Vaults by extensions at runtime


Read more here: [App Key Vault Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-key-vault-trace)


# How can I analyze company lifecycle (create company, delete company, copy company)?
Company lifecycle telemetry gathers data about the success or failure of the following company-related operations:
creating/copying/deleting a company.


Failed operations result in a trace log entry that includes a reason for the failure.


Read more here: [Company Lifecycle Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-company-lifecycle-trace)


# How can I analyze Configuration Package RapidStart Lifecycle (import package, export package, apply package, delete package)?
Configuration package telemetry gathers data about the following operations on configuration packages: Export, Import, Apply, and Delete.


Read more here: [Configuration Package Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-configuration-package-trace)


# How can I analyze (SQL) Database Lock Timeout?
Database lock timeout telemetry gathers information about database locks that have timed out. The telemetry data allows you to troubleshoot what caused these locks.


Two types of trace events are emitted to Application Insights:


    The first is a Database lock timed out event. This event includes general information about the lock request. This event includes information like the AL object and code that is impacted by the lock, the extension involved, and more.


    The Database lock timed out event then triggers one or more Database lock snapshot events. Database lock snapshot events provide details about SQL sessions that hold database locks at the time of lock timeout, including the session that caused the lock timeout. These events include specific details about the SQL lock request on the database, like the type, status, mode, and the table.


Read more here: [Database Lock Timeout Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-database-locks-trace)


# How can I analyze emails?
Email telemetry gathers data about the following operations: 


An email was sent successfully


An attempt to send an email failed.


Read more here: [Email Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-email-trace)


# How can I analyze Extension Lifecycle (Compile an extension, Synchronize an extension, Publish an extension, Install an extension, Update an extension, Uninstall an extension, Unpublish an extension)?
Extension lifecycle telemetry gathers data about the success or failure of the following extension-related operations:


Compiling an extension


Synchronizing an extension


Publishing an extension


Installing an extension


Updating an extension


Uninstalling an extension


Unpublishing an extension


Read more here: [Extension Lifecycle Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-lifecycle-trace)


# How can I analyze Extension Updates?
Extension upgrade telemetry gathers information about failures that occur during extension upgrades. Specifically, it provides information about exceptions that are thrown by code run by upgrade codeunits. 


This data can help you identify, troubleshoot, and resolve issues with per-tenant and AppSource extensions that are blocking data upgrade.


Read more here: [Extension Update Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-update-trace)


# How can I analyze sensitive field monitoring?
Keeping sensitive data secure and private is a core concern for most businesses. To add a layer of security, you can monitor important fields when someone changes a value. For example, you might want to know if someone changes your company's IBAN number.


To gather this data, you'll have to start field monitoring and specify the fields that you want to monitor. For more information, see Auditing Changes in Business Central - Monitoring Sensitive Fields in the Application help.


Telemetry is then logged for the following operations:


When field monitoring is stopped or started


When a field is added or removed for monitoring (only in version 18.0 and later)


When a field value is changed


Read more here: [Field Monitoring Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-field-monitoring-trace)


# How can I analyze Long Running AL Methods?
The Business Central Server server will emit telemetry about the execution time of long running AL methods, including the time spent in the database. The signal also includes a breakdown of how much time each event subscriber added to the total time. As a partner, this data gives you insight into bad performing code and enables you to troubleshoot performance issues caused by extensions.


Read more here: [Long Running AL Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-al-method-trace)


# How can I analyze Long Running Operation (SQL) queries?
A SQL query that takes longer than 1000 milliseconds to execute will be sent to your Application Insights resource.


There are multiple reasons that affect the time it takes SQL queries to run. For example, the database could be waiting for a lock to be released. Or, the database is executing an operation that does badly because of missing indexes. 


Read more here: [Long Running SQL Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-long-running-sql-query-trace)


# How can I analyze Report Generation?
Report generation telemetry gathers data about reports that are run on the service. It provides information about whether the report dataset generation succeeded, failed, or was canceled. For each report, it tells you how long it ran, how many SQL statements it executed, and how many rows it consumed.


You use this data to gather statistics to help identify slow-running reports.


Read more here: [Reporting Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-reports-trace)


# How can I analyze retention policies?
Administrators define retention policies to specify how frequently they want Business Central to delete outdated data in tables that contain log entries and archived records. For example, cleaning up log entries makes it easier to work with the data that's relevant. Policies can include all data in the tables that is past the expiration date. Or you can add filter criteria to include only certain expired data in the policy.


Telemetry is logged for the following operations:


A table is added to the list of allowed tables


An error occurs when adding a new retention policy


A retention policy is enabled


A retention policy is applied, either manually or automatically


Data is deleted because of a retention policy


The first retention policy is enabled on a table in a company


The last retention policy is removed in a company


Read more here: [Retention Policy Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-retention-policy-trace)


# How can I analyze Incoming Web Services (SOAP, OData, API) Requests?
Web services telemetry gathers data about SOAP, OData, and API requests through the service. It provides information like the request's endpoint, time to complete, the SQL statements run, and more.


As a developer, you use the data to learn about conditions that you can change to improve performance and stability. 


Read more here: [(Incoming) Web Service Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace)


# How can I analyze Outgoing Web Service Request (HttpClient AL)?
Outgoing web service request telemetry gathers data about outgoing web service requests sent using the AL HTTPClient module. As a partner, the data gives you insight into the execution time and failures that happen in external services that your environment and extensions depend on. Use the data to monitor environments for performance issues caused by external services, and be more proactive in preventing issues from occurring.


Read more here: [Outgoing Web Services Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-outgoing-trace)


# How can I analyze (SQL) indexes?
The table index trace gathers data when a index is added to, modified, or removed from a base table by a table extension.


In AL, an index is defined by a key, which can include one or more table fields. A key in a table extension object can include either fields from the base table or fields from the table extension object itself. A key that includes base tables fields is 
added as an index on the base table in the SQL database. For more information, see Table Keys.


This signal is emitted when a table extension object is installed or upgraded on a tenant, and it does one of the following operations:


Adds an index to the base table


Changes an existing index on the base table, for example, it adds or removes fields


Removes an existing index from the base table


Read more here: [Index/Key Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-table-index-trace)


# How can I analyze Web Service Access Key?
The Business Central emits telemetry data about the success or failure of authenticating web service access keys on web service requests.


In a future release, web service access key feature will be deprecated. As a partner or customer, this data lets you monitor the use of web service access keys on your environments in preparation for this change.


Read more here: [Web Service key Authentication Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-access-key-trace)


# How can I analyze basic auth?
In version 17.3, a truncated version of the Authorization header was introduced in the httpHeaders dimension to enable querying for the use of basic or token authorization. 


Read more here: [(Incoming) Web Service Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace)


# How can I analyze permissions?
Permission changes telemetry gathers data about the following operations on permission sets:


A user-defined permission set was added or removed


A link between a user-defined permission set and system permission set was added or removed


A permission set was assigned to or removed from a user or user group


Read more here: [Permissions Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-permission-changes-trace)


# How can I analyze which pages (pageviews) that users visit click on?
Page view telemetry gathers data about the pages that users open in the Business Central client. Each page view tells you how long it took to open the page, information about the user's environment, and more.


Use the data to gather statistics about system usage and also troubleshoot performance issues caused by the users' environments.


Read more here: [PageView Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-page-view-trace)


# How can I analyze Job Queue Lifecycle (job queue entry was enqueued, job queue entry was started, job queue entry finished)?
Job queue lifecycle telemetry gathers data about the following operations: a job queue entry was enqueued, started, or finished.


Read more here: [Job Queue Signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-job-queue-lifecycle-trace)


# How can I analyze when users cannot login?
Authorization telemetry provides information about the authorization of users when they try to sign in to Business Central. This telemetry data can help you identify problems a user might experience when signing in.


Authorization signals are emitted in two stages of sign-in. The first stage is the initial authorization, before the CompanyOpen trigger is run. In this stage, the system verifies that the user account is enabled in the tenant and has the correct entitlements. 


The next stage occurs after a successful authorization attempt, when trying to open the company (that is, when the CompanyOpen trigger run). The telemetry data indicates whether the company opened successfully or failed (for some reason).


Read more here: [Authorization signal documentation](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace)


Also, try out the login issues TSG. Learn more about Business Central troubleshooting guides here: [Troubleshooting guide FAQ](https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/TroubleShootingGuides)


# How can I analyze performance (it is slow)?
The following performance telemetry is available in Azure Application Insights (if that has been configured for the environment or app):


Database locks


Long Running AL operations


Long Running SQL Queries


Page views


Reports


Sessions started


Web Service Requests


Read more here: [Performance tuning guide - performance telemetry](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/performance/performance-developer#performance-telemetry)


# Can I see errors in telemetry?
Currently, errors are not submitted to telemetry. It might be added in a future release of Business Central.
