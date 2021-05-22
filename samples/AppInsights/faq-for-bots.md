# What is Business Central telemetry?
Business Central continuously emits telemetry about events that happen in the service.

Read more about telemetry here: https://aka.ms/bctelemetry

# What can I use telemetry for?
This telemetry can be useful for partners, e.g., when troubleshooting an issue or to determine how often a feature is used.

# What kind of telemetry exists?
As a developer of an app (typically referred to as an **ISV**), which gets installed in a Business Central environment, or as the partner on record for a customer (typically referred to as a **VAR**), you can obtain some of this telemetry.

Read more about telemetry here: https://aka.ms/bctelemetry

# What resources can I find in aka.ms/bctelemetrysamples?
This repo contains instructions for how you can obtain the telemetry.

It also contains resources that help you get immediate value from the telemetry.

Please visit the FAQ page https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/FAQ.md for any questions on how to get started, pricing, privacy, and more.

## Want to Learn Business Central telemetry in a week?
Haven't got the time to learn Business Central telemetry? Good news: we have recorded a series of short videos on the topic. Watch one every morning for a week and you should be up and running. 


### How do I get started with Application Insights?
Watch this 4 min video to learn how to connect an environment or an extension to Azure Application Insights:
https://www.youtube.com/watch?v=f4rxr20QG04

Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md


### How can I analyze performance using the Power BI performance report? 
Watch this 8 min video to learn how to use the standard Power BI performance report to triage performance issues happening in an environment or an extension: https://www.youtube.com/watch?v=Kbq6YB8VU-8

Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


### How can I triage errors using the Power BI error report? (6 min)
Watch this 6 min video to learn how to use the standard Power BI error report to triage errors happening in an environment or an extension: https://www.youtube.com/watch?v=ByealbDQqIU

Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


### How can I write custom Kusto queries in Dynamics 365 Business Central? 
Watch this 9 min video to learn how to write Kusto queries against Business Central telemetry: https://www.youtube.com/watch?v=A1e9ZMo5xcY

Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md

Learn more about KQL here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/KQL/README.md

### How can I troubleshoot issues using the Jupyter Notebook troubleshooting guides? 
Watch this 8 min video to learn how to troubleshoot issues using the Jupyter Notebook troubleshooting guides: https://www.youtube.com/watch?v=B3EL0xdvaUY

Find other video resources here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/VIDEOS.md

Learn more about Business Central troubleshooting guides here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/TroubleShootingGuides/README.md


### How can I emit telemetry to Azure Application Insights from AL code? 
Watch this 1 min video to learn how custom telemetry works: https://www.youtube.com/watch?v=gFG5E9Xd5bA

If you develop extensions for Business Central, then this is a must-see. 

Learn more about Business Central custom telemetry here: https://aka.ms/bctelemetry

### How can I add Azure Alerts as push notifications on my phone?
Watch this 4 min video to learn how to add Azure Alerts as push notifications on your phone: https://www.youtube.com/watch?v=nqqVEISjSGE

Find sample queries that you can use as a starting point here: https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/Alerts 


## How do I get started with telemetry?
Business Central can send telemetry to one or more **Azure Application Insights** (Application Insights) accounts.
The first step thus is for you to create an Application Insights account.
See [HERE](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tenant-admin-center-telemetry) for instructions on how to do that.

Once you have created the Application Insights account, make a note of the *instrumentation key*.

The next step depends on whether you are an ISV or a VAR:
* If you are an **ISV**, you must specify the instrumentation key in your app.json file. Once the app is installed in a Business Central environment, telemetry relating to your app will start to flow into your Application Insights account.

* If you are a **VAR**, you must enter the instrumentation key in the Business Central Admin Center of your customer(s). Once you have done that, telemetry relating to your customers will start to flow into your Application Insights account. You can also set the instrumentation key using the Business Central Administration Center API.

Please visit the documentation for more details (use CTRL + click to open in a new browser tab/page):
* [Business Central Developer and IT-pro documentation - Monitoring and Analyzing Telemetry](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview)
* [Business Central Administration Center API - How to set the telemetry key](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/administration-center-api#put-appinsights-key)

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## What does telemetry cost?
Application Insights is billed based on the volume of telemetry data that your application sends. Currently, the first 5 GB of data per month is free. Regarding data retention, every GB of data ingested can be retained at no charge for up to first 90 days.

Please check the documentation <https://azure.microsoft.com/en-us/pricing/details/monitor/> for up-to-date information on pricing.

Azure monitor alerts are billed separately.

Here is a quote from a partner using telemetry:
_We have been using telemetry for some months now and have enabled 20+ apps as well as environment data from dev systems and build pipelines. Last month we ingested 800+ traces that corresponded to 2.3GB of data. Eventually we might hit some of those thresholds, but then we can decide if we want to spend money on telemetry (probably will) and how much. With our current setup, we will probably limit ingestion and once that no longer suffices, we will add sampling to the mix._

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## How can I reduce telemetry cost?
To reduce ingestion cost, you can
* set limits on daily data ingestion
* reduce data ingestion by sampling to only ingest a percentage of the inbound data (see https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#ingestion-sampling)
* set alerts on cost thresholds being exceeded to get notified if this happens

Use this KQL query [MonthlyIngestion.kql](KQL/Queries/HelperQueries/MonthlyIngestion.kql) to see the data distribution of different event ids in your telemetry database.

See all helper queries here: [HelperQueries](KQL/Queries/HelperQueries/)

Please visit the [FAQ](FAQ.md) for more frequently asked questions.


## Where can I learn more about Kusto Query Language (KQL) and Azure Data Studio?
Please visit the [KQL README page](KQL/README.md) for learning resources on KQL and the [Trouble Shooting Guides README page](TroubleShootingGuides/README.md) for learning resources on Azure Data Studio.

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## How can I see what data is available in my Application Insights subscription
Use this KQL query [AvailableSignal.kql](KQL/Queries/HelperQueries/AvailableSignal.kql) to see if you have any data in your telemetry database, and also what kind of signal is present.

See all helper queries here: [HelperQueries](KQL/Queries/HelperQueries/)

Please visit the [FAQ](FAQ.md) for more frequently asked questions.


## I deployed Azure dashboards, but they show no data
If you have data present in Application Insights, please check the setting in the *Time range* selector on the  dashboard:
![Time range selector in Azure Dashboard](images/dashboard.png)

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## What is the data retention policy in Application Insights?
The default retention for Application Insights resources is 90 days. Different retention periods can be selected for each Application Insights resource. The full set of available retention periods is 30, 60, 90, 120, 180, 270, 365, 550 or 730 days.

See <https://docs.microsoft.com/en-us/azure/azure-monitor/app/pricing#change-the-data-retention-period> 

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## How do I delete data from Application Insights?
Purge data in an Application Insights component by a set of user-defined filters.

See <https://docs.microsoft.com/en-us/rest/api/application-insights/components/purge#examples> 

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## Can I grant read-only access to Application Insights?
To grant a person read-only access to Application Insights, go to the Access control (IAM) page in the Application Insights portal, and then add the role assignment "Reader" to the person. 

You might also need to add the role assignment "Reader" to the person on the Resource Group for the Application Insights subscription.

Please visit the [FAQ](FAQ.md) for more frequently asked questions.


## What about Privacy regulations such as GDPR?
The Business Central service does not emit any End User Identifiable Information (EUII) to Application Insights. So the telemetry is born GDPR compliant. The service only emits data that is classified as either System Metadata or Organization Identifiable Information (OII). The meaning of these classifications are described here: [DataClassification Option Type](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/methods-auto/dataclassification/dataclassification-option)

Please visit the [FAQ](FAQ.md) for more frequently asked questions.


## Will you backport the Application Insights instrumentation to versions prior to 15.0?
It took a lot of refactoring in the server and client to make this happen. So it is unlikely that we will backport the Application Insights instrumentation to versions prior to 15.0.

For each new signal type we add, we try to backport to the current major release (16.x right now) if possible.

For on-premises installations (private or public cloud), you can create an application/service that listens on the ETW (Event Tracing for Windows) events that we use for internal telemetry and then send them to Application Insights. Note that this approach is depending on internal telemetry events that might change and that are not documented by Microsoft.

This is documented here: https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tools-monitor-performance-counters-and-events and here https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/monitor-server-events

See the Application Insights documentation for an introduction on how to emit telemetry from a .NET console application:
[The Application Insights for .NET console applications](https://docs.microsoft.com/en-us/azure/azure-monitor/app/console)

Please visit the [FAQ](FAQ.md) for more frequently asked questions.

## How do I create alerts in Azure Application Insights?
Here is an example to get you started:
 1. Open the Azure portal and locate your Application Insights resource
 2. Click "Alerts" in the navigation pane on the left
 3. Use one of the KQL samples from this section in the condition for a custom log search 

Please visit the [Alerting FAQ](Alerts) for more frequently asked questions about alerts.

## How do I get alerts via email?
If you want alerts via email, you can just create a new action group in your Application Insights resource, and in your alerts add an action to send an email.

Please visit the [Alerting FAQ](Alerts) for more frequently asked questions about alerts.


## How do I get alerts via Microsoft Teams?
You can also send alerts to a channel in Microsoft Teams. See an example of how to set that up here: https://dailydotnettips.com/sending-your-azure-application-insights-alerts-to-team-sites-using-azure-logic-app/

Please visit the [Alerting FAQ](Alerts) for more frequently asked questions about alerts.


## How do I get alerts on my phone?
This video shows how you can get alerts as push notifications on your phone: https://www.youtube.com/watch?v=nqqVEISjSGE

Please visit the [Alerting FAQ](Alerts) for more frequently asked questions about alerts.

## Want to learn more about Azure Monitor alerts?
Please read more in the documentation for Azure Application Insights: https://docs.microsoft.com/en-us/azure/azure-monitor/platform/alerts-unified-log

Please visit the [Alerting FAQ](Alerts) for more frequently asked questions about alerts.

# How do I get started with Azure dashboards?

To reduce the time-to-value for you, we have prepared a set of dashboards that you can deploy to your Azure portal:
1. Open Company Performance
2. Session overview
3. Long Running SQL Queries
4. Failed Authorization
5. Web Service Calls

Please visit the [Dashboard FAQ](Dashboard) for more frequently asked questions about Azure Dashboards.

# How can I make my own Azure dashboards?

We know that the dashboards we have provided might not match your needs exactly, and if you want to customize them, we recommend that you clone this repo and make your adjustments there, before importing the dashboard in the Azure portal.

Adding dashboards is done by exporting a dashboard from the Azure Portal, running the ConvertExportedDashboardToDashboardTemplate.ps1 and then adding the new template to the resources section in azuredeploy.json.

As we improve our dashboards, you can merge the changes into your cloned repo and in this way stay up-to-date.

Please visit the [Dashboard FAQ](Dashboard) for more frequently asked questions about Azure Dashboards.


# Where can I use Kusto Queries?
You can use Kusto queries as the data source in a number of places. E.g.
* the Logs part of Application Insights in the Azure portal
* PowerBI reports
* Azure Monitor Alerts
* Azure Dashboards
* Jupyter Notebooks (with the Kqlmagic extension)

Please visit the [KQL FAQ](KQL) for more frequently asked questions about Kusto queries.


# Where can I learn more about KQL?
Here are some resources for you to get started on Kusto Query Language (KQL). Use CTRL+click to open them in a new browser tab/window.
* [Kusto Query Language Overview](https://docs.microsoft.com/en-us/azure/kusto/query/)
* [Kusto Query Language Tutorial](https://docs.microsoft.com/en-us/azure/kusto/query/tutorial)
* [I know SQL. How do I do that in KQL?](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/sqlcheatsheet)
* [Kusto Query Language (KQL) from Scratch (Pluralsight course, requires subscription)](https://www.pluralsight.com/courses/kusto-query-language-kql-from-scratch)
* [Microsoft Azure Data Explorer - Advanced KQL (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-explorer-advanced-query-capabilities/table-of-contents)

Please visit the [KQL FAQ](KQL) for more frequently asked questions about Kusto queries.


# Which tools can I use (KQL editors and clients)?
You can write and execute KQL in various tools. E.g.
* [Kusto Explorer (desktop application)](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/tools/kusto-explorer). Here is [How to connect to Application Insights in Kusto Explorer](https://docs.microsoft.com/en-us/azure/data-explorer/query-monitor-data)
* In a Jupyter notebook hosted in [Azure Data Studio](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides#what-is-azure-data-studio)
* In a Jupyter notebook hosted in Visual Studio Code (with the Python and Jupyter Notebooks extensions installed)
* Application Insights portal (Under *Logs* in the *Monitoring* menu)
* PowerShell (using the REST api). See an example here: https://demiliani.com/2020/12/16/using-powershell-to-retrieve-your-dynamics-365-business-central-telemetry/

Please visit the [KQL FAQ](KQL) for more frequently asked questions about Kusto queries.


# What signal is available in which version?
Signal is added incrementally to Business Central. 

Please visit the [KQL FAQ](KQL) to see a table you can see in which version or update a class of signal was added/modified.


# Do you have Kusto (KQL) queries for performance investigations?
Yes, the *PerformanceTuning* folder have queries that can help you investigate a performance issue.

Note that there also exist predefined troubleshooting guides for performance investigations both in the form ofJupyter notebooks and as a Power BI report.

Please visit the [KQL FAQ](KQL) for more frequently asked questions about Kusto queries.

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md

Learn more about Business Central troubleshooting guides here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/TroubleShootingGuides/README.md


# Where can I find example KQL queries for each of the event ids in telemetry?
The folder *ExampleQueriesForEachArea* have KQL scripts with KQL queries for each type of event id present in telemetry.

Please visit the [KQL FAQ](KQL) for more frequently asked questions about Kusto queries.

Read more about telemetry here: https://aka.ms/bctelemetry


# Which helper KQL queries are useful for what?
The folder *HelperQueries* have KQL scripts with KQL queries that can be used to quickly answer common questions, such as 
* which environments log telemetry to my resource?
* which signal is present in my resource?
* How much data is ingested in my resource?

Please visit the [KQL FAQ](KQL) for more frequently asked questions about Kusto queries.




# Which environments log telemetry to my resource?
Run the query *AvailableEnvironments.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.


# Which signal is present in my resource?
Run the query *AvailableSignal.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.


# How much data is ingested in my resource?
Run the query *MonthlyIngestion.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.


# Which browsers are users using?
Run the query *BrowserUsage.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# When was upgrades happening?
Run the query *ComponentUpdates.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# Are we using deprecated protocols or features?
Run the query *DeprecatedWebServiceProtocols.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# How do I troubleshoot web service issues?
Run the query *WebServiceCallStatus.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# How do I troubleshoot Microsoft connectors?
Run the query *MicrosoftConnectorUsage.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# How do I troubleshoot traffic from Microsoft Power BI?
Run the query *PowerBIConnectorUsage.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# How can I get a timeline of what happens in a session?
Run the query *Timeline.kql*

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.

# How can I distinguish data from a per-tenant extension, an appsource extension, or from Microsoft base app?
Use the code snippet *partnerCodePredicate.kql* in your queries

Please visit the [KQL Helper queries FAQ](KQL/Queries/HelperQueries) for more frequently asked questions about helper queries.


In this folder, you will find samples that illustrate how you can use Application Insights data in PowerBI reports.

# Which Power BI reports are available?
This repository contains Power BI reports for 
* Investigating performance issues 
* Investigating errors

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


# How do I use Power BI reports for troubleshooting
To use the Performance or Error report, do as follows
1) Download and install Power BI Desktop: https://powerbi.microsoft.com/en-us/downloads/
2) Download the .pbit template files from the *Reports* directory
3) Open a .pbit template file in Power BI Desktop
4) Fill in parameters (Azure Application Insights App id is required. Get it from the *API Access* menu in the Azure Application Insights portal). 
5) (Optional) Save the report as a normal .pbix file

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


# How does authentication work between Power BI and Azure Application Insights?
Currently, Power BI only supports AAD authentication to Application Insights. This means that the approaches we use in Jupyter notebooks (using App id and API key) and Azure Dashboards (using Application Insights subscription id, Application Insights name, and Application Insights resource group) are not applicable in Power BI. To use Power BI with data from Application Insights, the user of the report must be in the same AAD tenant as the Application Insights resource and need to have read access to Application Insights resource.

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


# How do I convert a KQL query into a M query (Power Query)?
If you have a nice KQL query that you want to use in Power BI, then do as follows
1) run the query in the Application Insights portal (under Logs)
2) Click *Export* and choose *Export to Power BI (M query)*. This downloads a M query as a txt file.
3) Follow the instructions in the txt file to use the M query as a datasource in PowerBI 

Read more about this topic here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/export-power-bi

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


# How do I use PowerBI parameters to set properties such as AAD tenant id and environment name?
If your Application Insights resource contain data from multiple environments, then you might want to use parameters in your Power BI reports. In the sample report *PartnerTelemetryTemplate.pbix*, you can see how to use parameters to achieve this.

Read more about Power BI parameters topic here: https://powerbi.microsoft.com/en-us/blog/deep-dive-into-query-parameters-and-power-bi-templates/

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


# How can I modify the standard reports?
All M queries used in the standard reports are available in the *M queries* directory. You can use them if you want to create your own reports based on the data sources defined in the standard reports.

Learn more about Business Central Power BI here: https://github.com/microsoft/BCTech/blob/master/samples/AppInsights/PowerBI/README.md


# Which Powershell scripts are available?
This repository contains Powershell for 
* querying Azure Application Insights

Please visit the [Powershell FAQ](Powershell) for more frequently asked questions about using Powershell with Kusto queries.


# How do I use Powershell scripts to query telemetry?
To use the script, do as follows
1) Go the Azure Application Insights portal, go to the *API Access* menu, and copy the application id and generate an API key 
2) Open Powershell and 
a) run GetTelemetryData.ps1 -appid <app id> -apikey <api key> -kqlquery <kql query you want to run>
or
b) pipe the content of a text file with a KQL query into the script: Get-Content <file> | .\GetTelemetryData.ps1 -appid <app id> -apikey <api key> 

Please visit the [Powershell FAQ](Powershell) for more frequently asked questions about using Powershell with Kusto queries.


# How can I use telemetry to troubleshoot problems?
Telemetry in Azure Application Insights is a very powerful tool to investigate issues after they have occurred (where you cannot attach a debugger to troubleshoot the issue). Having a good arsenal of Kusto query language (KQL) scripts can be very handy, but sometimes executing KQL statements one by one in the Azure Application Insights portal can be tedious. This is where notebooks come in handy, and in this part of the samples repository we show you how to get started using Jupyter notebooks in Azure Data Studio. With the Kqlmagic module, you can now combine KQL with Python to create powerful interactive trouble shooting guides 
(TSGs).

Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# Which Jupyter Notebook Trouble Shooting Guides (TSGs) are available?
This repository contains Jupyter Notebook TSGs for 
* Investigating performance issues (overview analysis)
* Investigating performance issues in your code (analysis outside Microsoft code base)
* Investigating web service issues (throttling, performance, bad endpoints)
* Investigating Microsoft connector (PowerBI, PowerApps, LogicApps, or Flow) issues (throttling, performance, bad endpoints)
* Investigating data-related issues (long running queries, database locks, report performance)
* Investigating login issues (authentication and authorization flows)
* Investigating if environments are using deprecated web service protocols
* Investigating lifecycle issues with extensions (compile, synchronize, publish, install, update, un-install, un-publish)

Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# What is Azure Data Studio?
Azure Data Studio is a cross-platform database tool for data professionals using the Microsoft family of on-premises and cloud data platforms on Windows, MacOS, and Linux. Azure Data Studio offers a modern editor experience with IntelliSense, code snippets, source control integration, and an integrated terminal. It's engineered with the data platform user in mind, with built-in charting of query result sets and customizable dashboards. 

Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# What is Kqlmagic?
Kqlmagic is a command that extends the capabilities of the Python kernel in Azure Data Studio notebooks. You can combine Python and Kusto query language (KQL) to query and visualize data using rich Plot.ly library integrated with render commands. Kqlmagic brings you the benefit of notebooks, data analysis, and rich Python capabilities all in the same location. Supported data sources with Kqlmagic include Azure Data Explorer, Application Insights, and Azure Monitor logs.

Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I learni more about Azure Data Studio?
Here are some resources for you to get started on Azure Data Studio. Use CTRL+click to open them in a new browser tab/window.
* [How to Start with Microsoft Azure Data Explorer (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-explorer-starting/table-of-contents)
* [Exploring Data in Microsoft Azure Using Kusto Query Language and Azure Data Explorer (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-exploring/table-of-contents)

Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I install and configure Azure Data Studio?
1. First, you need to download and install Azure Data Studio. See how to here: https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
2. Now create a new notebook (needed for steps 3+4 below) and choose the Python3 kernel, see https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#create-a-notebook
3. Install a Python kernel for Azure Data Studio (let Azure Data Studio do it for you or reuse an existing Python installation). See how to here: https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#change-the-python-kernel


Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.

# How do I install and set up Kqlmagic?
To install and set up Kqlmagic, open a python notebook. Click the "Manage Packages" icon on the right of the notebook:

![Manage Python packages](../images/install-kqlmagic-1.png)

Under the "Add new" tab, type "kqlmagic" and click Install (we generally recommend people to install non “dev” version of Kqlmagic):

![Install Kqlmagic](../images/install-kqlmagic-2.png)
For some users, the KQL Magic installation fails with an error. Please try these two steps 

Step 1:
```python
import sys
!{sys.executable} -m pip install --upgrade pip
```

Step 2:
```python
import sys
!{sys.executable} -m pip install Kqlmagic --no-cache-dir --upgrade
```
in a Python code cell (just create a new notebook and change to the Python Kernel).

Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.


# How do I connect a notebook to an Azure Application Insights resource?
You need two GUIDs to be able to read data from your Application Insights resource: 
1. Application ID
2. an API key

Get the Application ID from the *API Access* page in the Application Insights portal:
![Get Application ID](../images/api-access-1.png)

Then generate an API key 
![Get Application ID](../images/api-access-2.png)

![Get Application ID](../images/api-access-3.png)

![Get Application ID](../images/api-access-4.png)

You can now use the Application ID and API key to connect to and read from the Application Insights resource from the notebook. 

See more here: https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-kqlmagic#kqlmagic-with-application-insights


Please visit the [Troubleshooting FAQ](TroubleShootingGuides) for more frequently asked questions about using jupyter notebook troubleshooting guides.

