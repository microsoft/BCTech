In this folder, you will find samples of Kusto Query Language (KQL) for each type of signal that is sent to Application Insights.

# Where can I use Kusto Queries?
You can use Kusto queries as the data source in a number of places. E.g.
* the Logs part of Application Insights in the Azure portal
* PowerBI reports
* Azure Monitor Alerts
* Azure Dashboards
* Jupyter Notebooks (with the Kqlmagic extension)

# Where can I learn more about KQL?
Here are some resources for you to get started on Kusto Query Language (KQL). Use CTRL+click to open them in a new browser tab/window.
* [Kusto Query Language Overview](https://docs.microsoft.com/en-us/azure/kusto/query/)
* [Kusto Query Language Tutorial](https://docs.microsoft.com/en-us/azure/kusto/query/tutorial)
* [I know SQL. How do I do that in KQL?](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/query/sqlcheatsheet)
* [Kusto Query Language (KQL) from Scratch (Pluralsight course, requires subscription)](https://www.pluralsight.com/courses/kusto-query-language-kql-from-scratch)
* [Microsoft Azure Data Explorer - Advanced KQL (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-explorer-advanced-query-capabilities/table-of-contents)

# Which tools can I use (KQL editors and clients)?
You can write and execute KQL in various tools. E.g.
* [Kusto Explorer (desktop application)](https://docs.microsoft.com/en-us/azure/data-explorer/kusto/tools/kusto-explorer). Here is [How to connect to Application Insights in Kusto Explorer](https://docs.microsoft.com/en-us/azure/data-explorer/query-monitor-data)
* [Azure Data Explorer](https://dataexplorer.azure.com). Here is [How to connect to Application Insights in Azure Data Explorer](https://docs.microsoft.com/en-us/azure/data-explorer/query-monitor-data)
* In a Jupyter notebook hosted in [Azure Data Studio](https://github.com/microsoft/BCTech/tree/master/samples/AppInsights/TroubleShootingGuides#what-is-azure-data-studio)
* In a Jupyter notebook hosted in Visual Studio Code (with the Python and Jupyter Notebooks extensions installed)
* Application Insights portal (Under *Logs* in the *Monitoring* menu)
* PowerShell (using the REST api). See an example here: [Powershell samples](../Powershell)


# What signal is available in which version?
Signal is added incrementally to Business Central. In this table you can see in which version or update a class of signal was added/modified:

|Signal | Emitted from version, update | Documentation (use CTRL+click to open in a new tab) |
| ------ | ------ | ------ |
| Long running operation (SQL query) | 2019 release wave 2 (15.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-long-running-sql-query-trace |
| Authorization | 2019 release wave 2 (15.2) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace |
| Web Service Request | 2020 release wave 1 (16.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace |
| Report Execution | 2020 release wave 1 (16.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-reports-trace | 
| Open Company timing | 2020 release wave 1 (16.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace |
| Company lifecycle (create/copy/delete) | 2020 release wave 1 (16.1) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-company-lifecycle-trace |
| Upgrade errors due to exceptions in upgrade code | 2020 release wave 1 (16.2) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-update-trace |
| Database lock timeouts | 2020 release wave 1 (16.2, later backported to 16.0 and 16.1 in SaaS) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-database-locks-trace |
| Extension lifecycle | 2020 release wave 1 (16.3) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-lifecycle-trace |
| Client page views | 2020 release wave 1 (16.3) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-page-view-trace |
| HTTP status and HTTP headers added to Web Service Request signal | 2020 release wave 1 (16.3) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace |
| AL HttpClient signal (outgoing web service calls) | 2020 release wave 2 (17.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-outgoing-trace |
| App Key Vault Secret Acquisitions | 2020 release wave 2 (17.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-key-vault-trace  |
| Long Running AL execution | 2020 release wave 2 (17.1) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-al-method-trace |
| Extension lifecycle for app telemetry | 2020 release wave 2 (17.1) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-lifecycle-trace |
| Sensitive Field Monitoring | 2020 release wave 2 (17.1) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-field-monitoring-trace |
| Retention Policy | 2020 release wave 2 (17.1) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-retention-policy-trace |
| Performance Toolkit | 2020 release wave 2 (17.1) | (pending link to docs)) |
| Job Queue | 2020 release wave 2 (17.2) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-job-queue-lifecycle-trace |
| Permissions | 2020 release wave 2 (17.2) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-permission-changes-trace |
| Configuration Package | 2020 release wave 2 (17.2) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-configuration-package-trace |
| Email | 2020 release wave 2 (17.2) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-email-trace |
| Posting | 2020 release wave 2 (17.2) | pending link to docs, sample KQL code updated |
| Web Service Key Authentication | 2020 release wave 2 (17.3) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-access-key-trace |
| Reporting engine in reporting signal | 2020 release wave 2 (17.3) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-reports-trace |
| HTTP authorization | 2020 release wave 2 (17.3) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace |
| Client page views (for ISVs) | 2021 release wave 1 (18.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-page-view-trace |
| Index/Key lifecycle | 2021 release wave 1 (18.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-table-index-trace#enabled |
| Sensitive Field Monitoring (field added/removed) | 2021 release wave 1 (18.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-field-monitoring-trace |
| Upgrade tag signal | 2021 release wave 1 (18.0) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-extension-update-trace |
| Extension lifecycle (dependent extensions) | 2021 release wave 1 (18.1) | (pending docs), sample KQL code updated |
| Appsource validation | 2021 release wave 1 (18.4) | (pending docs), sample KQL code updated |
| Use of read scale-out in reporting | 2021 release wave 1 (19.1) | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-reports-trace |
| Feature Telemetry | 2021 release wave 2 (19.1) | (pending docs) |
| Database deadlocks | 2022 release wave 1 (20.0) | (pending link to release note) |
| Database wait statistics | 2022 release wave 1 (20.0) | (pending link to release note) |
| Verbose telemetry enabled | 2022 release wave 1 (20.0) | (pending link to release note) |
| General error dialogs | 2022 release wave 1 (20.0) | (pending link to release note) |
| Permission error dialogs | 2022 release wave 1 (20.0) | (pending link to release note) |
| Use of read scale-out in web service calls | 2022 release wave 1 (20.0) | (pending link to release note) |


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
