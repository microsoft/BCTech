Telemetry in Azure Application Insights is a very powerful tool to investigate issues after they have occured (where you cannot attach a debugger to troubleshoot the issue). Having a good arsenal of Kusto query language (KQL) scripts can be very handy, but sometimes executing KQL statements one by one in the Azure Application Insights portal can be tedious. This is where notebooks come in handy, and in this part of the samples repository we show you how to get started using Jupyter notebooks in Azure Data Studio. With the Kqlmagic module, you can now combine KQL with Python to create powerful interactive trouble shooting guides (TSGs).

# Trouble Shooting Guides (TSGs)
This repository contains TSGs for 
* Investigating performance issues (overview analysis)
* Investigating performance issues in your code (analysis outside Microsoft code base)
* Investigating web service issues (throttling, performance, bad endpoints)
* Investigating Microsoft connector (PowerBI, PowerApps, LogicApps, or Flow) issues (throttling, performance, bad endpoints)
* Investigating data-related issues (long running queries, database locks, report performance)
* Investigating login issues (authentication and authorization flows)
* Investigating if environments are using deprecated web service protocols
* Investigating lifecycle issues with extensions (compile, synchronize, publish, install, update, un-install, un-publish)

| TSG | Description |
| ----------- | ----------- |
| Login-issues-TSG.ipynb | Use this TSG for cases related to login issues, such as if one or more users cannot login (check failure reasons in authorization checks before/after onCompanyOpen trigger, to troubleshoot if a login issue is related to firewall/network issues at the customer site (check if web service requests get in but client requests are not present), or to check if logins are successful again after an incident |
| Performance-overview-TSG.ipynb | Use this TSG to analyze performance problems. The TSG runs through most of common root causes for perf issues and has links to the performance tuning guide so that partners can fix their code to be more performant
| Web-services-TSG-version.ipynb | Use this TSG to analyze issues with web services: Is the environments being throttled? Analyze endpoint performance. Analyze failure reasons (HTTP codes 401, 404, 408, 426) |
| Microsoft-Connectors-TSG-version.ipynb | Use this TSG to analyze issues with Microsoft connectors (PowerBI, PowerApps, LogicApps, or Flow): Is the environments being throttled? Analyze endpoint performance. Analyze failure reasons (HTTP codes 401, 404, 408, 426) |
| Performance-partner-code-TSG.ipynb | Use this TSG to analyze performance problems in partner code. The TSG filters telemetry on object ids outside the ranges used for the base app and localizations (the code written by Microsoft) | 
| Data-related-TSG.ipynb | Use this TSG to analyze data-related issues: Long running queries, database locks, reports that runs many sql statements | 
| Extensions-TSG.ipynb | Use this TSG to analyze issues with the extension lifecycle (compile, synchronize, publish, install, update, un-install, un-publish). You can set filters on AAD tenant id, environment name, and extension id to troubleshoot all environments in a AAD tenant id, a single environment, single extensions, or any other combination of the three |



# What is Azure Data Studio?
Azure Data Studio is a cross-platform database tool for data professionals using the Microsoft family of on-premises and cloud data platforms on Windows, MacOS, and Linux. Azure Data Studio offers a modern editor experience with IntelliSense, code snippets, source control integration, and an integrated terminal. It's engineered with the data platform user in mind, with built-in charting of query result sets and customizable dashboards. 

# What is Kqlmagic?
Kqlmagic is a command that extends the capabilities of the Python kernel in Azure Data Studio notebooks. You can combine Python and Kusto query language (KQL) to query and visualize data using rich Plot.ly library integrated with render commands. Kqlmagic brings you the benefit of notebooks, data analysis, and rich Python capabilities all in the same location. Supported data sources with Kqlmagic include Azure Data Explorer, Application Insights, and Azure Monitor logs.

# Get up and running (install and configure Azure Data Studio)
1. First, you need to download and install Azure Data Studio. See how to here: https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
2. Now create a new notebook (needed for steps 3+4 below) and choose the Python3 kernel, see https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#create-a-notebook
3. Install a Python kernel for Azure Data Studio (let Azure Data Studio do it for you or reuse an existing Python installation). See how to here: https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#change-the-python-kernel

# Install and set up Kqlmagic
To install and set up Kqlmagic, open a python notebook. Click the "Manage Packages" icon on the right of the notebook:

![Manage Python packages](../images/install-kqlmagic-1.png)

Under the "Add new" tab, type "kqlmagic" and click Install:

![Install Kqlmagic](../images/install-kqlmagic-2.png)

# How to connect Kqlmagic to an Azure Application Insights resource
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



