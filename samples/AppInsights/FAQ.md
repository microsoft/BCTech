# Business Central Telemetry FAQ (Frequently Asked Questions)

## How do I get started?
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

## What does it cost?
Application Insights is billed based on the volume of telemetry data that your application sends. Currently, the first 5 GB of data per month is free. Regarding data retention, every GB of data ingested can be retained at no charge for up to first 90 days.

Please check the documentation <https://azure.microsoft.com/en-us/pricing/details/monitor/> for up-to-date information on pricing.

Azure monitor alerts are billed separately.

Here is a quote from a partner using telemetry:
_We have been using telemetry for some months now and have enabled 20+ apps as well as environment data from dev systems and build pipelines. Last month we ingested 800+ traces that corresponded to 2.3GB of data. Eventually we might hit some of those thresholds, but then we can decide if we want to spend money on telemetry (probably will) and how much. With our current setup, we will probably limit ingestion and once that no longer suffices, we will add sampling to the mix._

## How can I reduce cost?
To reduce ingestion cost, you can
* set limits on daily data ingestion
* reduce data ingestion by sampling to only ingest a percentage of the inbound data (see https://docs.microsoft.com/en-us/azure/azure-monitor/app/sampling#ingestion-sampling)
* set a daily limit of how much data that can be ingested
* purge data from your Application Insights resource (see _How do I delete data from Application Insights?_ below)
* set alerts on cost thresholds being exceeded to get notified if this happens

Use this KQL query [MonthlyIngestion.kql](KQL/Queries/HelperQueries/MonthlyIngestion.kql) to see the data distribution of different event ids in your telemetry database.

See all helper queries here: [HelperQueries](KQL/Queries/HelperQueries/)

## Should each customer/app have their own Application Insights resource, rather than one insight for multiple customers?
Partitioning of Application Insights resources across multiple customers or apps depends on what you use telemetry for. The benefit of having a 1-1 relationship between customers/apps and Application Insights resources is that you can also use the Usage features in the Application Insights portal to monitor how a particular customer is using BC. It also makes it easy to separate the cost of telemetry per customer/app. Downside of a 1-1 relationship between customers/apps and Application Insights resources is that you have more Azure resources to manage, including any cross-customer alerting/monitoring that you might want to setup.

Also, it is recommended to use per-environment telemetry from per-app telemetry into separate Application Insights resources.

## Where can I learn more about Kusto Query Language (KQL) and Azure Data Studio?
Please visit the [KQL README page](KQL/README.md) for learning resources on KQL and the [Trouble Shooting Guides README page](TroubleShootingGuides/README.md) for learning resources on Azure Data Studio.

## How can I see what data is available in my Application Insights subscription
Use this KQL query [AvailableSignal.kql](KQL/Queries/HelperQueries/AvailableSignal.kql) to see if you have any data in your telemetry database, and also what kind of signal is present.

See all helper queries here: [HelperQueries](KQL/Queries/HelperQueries/)

## I deployed Azure dashboards, but they show no data
If you have data present in Application Insights, please check the setting in the *Time range* selector on the  dashboard:
![Time range selector in Azure Dashboard](images/dashboard.png)

## What is the data retention policy in Application Insights?
The default retention for Application Insights resources is 90 days. Different retention periods can be selected for each Application Insights resource. The full set of available retention periods is 30, 60, 90, 120, 180, 270, 365, 550 or 730 days.

See <https://docs.microsoft.com/en-us/azure/azure-monitor/app/pricing#change-the-data-retention-period> 

## How do I delete data from Application Insights?
Purge data in an Application Insights component by a set of user-defined filters.

See <https://docs.microsoft.com/en-us/rest/api/application-insights/components/purge#examples> 

You can use Powershell to setup a purge process, see an example here: [How do I use Powershell to delete telemetry data?](Powershell/README.md)

## Can I grant read-only access to Application Insights?
To grant a person read-only access to Application Insights, go to the Access control (IAM) page in the Application Insights portal, and then add the role assignment "Reader" to the person. 

You might also need to add the role assignment "Reader" to the person on the Resource Group for the Application Insights subscription.

## What about Privacy regulations such as GDPR?
The Business Central service does not emit any End User Identifiable Information (EUII) to Application Insights. So the telemetry is born GDPR compliant. The service only emits data that is classified as either System Metadata or Organization Identifiable Information (OII). The meaning of these classifications are described here: [DataClassification Option Type](https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/methods-auto/dataclassification/dataclassification-option)

## Will you backport the Application Insights instrumentation to versions prior to 15.0?
It took a lot of refactoring in the server and client to make this happen. So it is unlikely that we will backport the Application Insights instrumentation to versions prior to 15.0.

For each new signal type we add, we try to backport to the current major release (16.x right now) if possible.

For on-premises installations (private or public cloud), you can create an application/service that listens on the ETW (Event Tracing for Windows) events that we use for internal telemetry and then send them to Application Insights. Note that this approach is depending on internal telemetry events that might change and that are not documented by Microsoft.

This is documented here: https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/tools-monitor-performance-counters-and-events and here https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/monitor-server-events

See the Application Insights documentation for an introduction on how to emit telemetry from a .NET console application:
[The Application Insights for .NET console applications](https://docs.microsoft.com/en-us/azure/azure-monitor/app/console)


Another option is to emit ETW events to Azure Log Analytics, see Marije Brummels blog post [Using Azure Log Analytics on older Dynamics NAV versions (blog post)](https://marijebrummel.blog/2021/11/28/using-azure-log-analytics-on-older-dynamics-nav-versions/) or her Github sample repo [Using Azure Log Analytics with Dynamics NAV (Github repo)](https://github.com/marijebrummel/Azure.LogAnalytics.NAV) for examples.

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
