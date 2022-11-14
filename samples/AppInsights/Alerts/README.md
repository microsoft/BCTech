# Get alerts when something happens
If something happens in your environment or for one of your customers that you need to take action on, it is better that the system sends you an alert. Azure Application Insights makes it easy to define such alerts.

Read more here in docs:
https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-overview#setting-up-alerts-on-telemetry-events

# Alerting condition KQL samples

When defining an alert based on telemetry, you need to define two things:
1. A Kusto (KQL) query that defines the alerting condition. 
2. How often you want to run the alerting query. 

This part of the samples repo contains examples of KQL queries that you can use for alerting conditions.

| Condition | Area | Relevant for | Description | Event Id(s) | KQL sample code (*CTRL+click* to open in new page) |
| --------- | -----| ------------ | ----------- | --------------- | ------------ |
| Appsource validation failures | Errors | ISV | As an ISV, you can submit an app to be validated against specific countries/regions and versions of Business Central. Setup notifications if a validation fails. | LC0035 | [AppsourceAdmissionFailures.kql](./AlertingKQLSamples/AppsourceAdmissionFailures.kql) |
| Azure Keyvault lookup failures | Errors | ISV | As an ISV, you should not store secrets in the app code but use secure storage such as Azure Keyvault. If lookups of secrets fail, your app might not work for the customers that have installed it. | RT0015, RT0017 | [AppKeyvaultFailures.kql](./AlertingKQLSamples/AppKeyvaultFailures.kql) |
| Database performance | Performance | ISV/VAR | Spot database regressions: Look back 60 days and count number of long running SQL queries for 30 days. Compare with the count for the previous 30 days. | RT0005 | [DatabaseRegressions.kql](./AlertingKQLSamples/DatabaseRegressions.kql) |
| Database performance | Performance | ISV/VAR | Spot if you have seen a give long running query before by getting a md5 hash on the SQL query and the AL stack trace | RT0005 | [LongRunningQueriesForBugskql.kql](./AlertingKQLSamples/LongRunningQueriesForBugskql.kql) |
| Extensions(s) failed to install  | Errors | VAR/ISV | Alert if one or more extensions fail to install. | LC0011 | [ExtensionInstallFailures.kql](./AlertingKQLSamples/ExtensionInstallFailures.kql) |
| Extensions(s) failed to upgrade  | Errors | VAR/ISV | Alert if one or more extensions fail to upgrade. | RT0010 | [ExtensionUpgradeFailures.kql](./AlertingKQLSamples/ExtensionUpgradeFailures.kql) |
| Extensions(s) installed  | Lifecycle | VAR/ISV | Alert if one or more extensions was installed. | LC0010 | [ExtensionInstalled.kql](./AlertingKQLSamples/ExtensionInstalled.kql) |
| Extensions(s) uninstalled  | Lifecycle | VAR/ISV | Alert if one or more extensions was uninstalled. | LC0016 | [ExtensionUninstalled.kql](./AlertingKQLSamples/ExtensionUninstalled.kql) |
| Environment update(s) available | Updates | VAR | Alert when new updates are available for environment(s). | LC0100 | [EnvironmentUpdateAvailable.kql](./AlertingKQLSamples/EnvironmentUpdateAvailable.kql) |
| Environment(s) failed to update  | Errors | VAR | Alert if environment(s) fail to update. | LC0107 | [EnvironmentUpdateFailures.kql](./AlertingKQLSamples/EnvironmentUpdateFailures.kql) |
| Permission errors | Errors | VAR | Setup notifications if users get permission errors. | RT0031 | [Permissions.kql](../KQL/Queries/ExampleQueriesForEachArea/Permissions.kql) |
| Job Queue errors | Errors | VAR | Get alerted on job queue entries fail. | AL0000E26 | [JobQueueFailures.kql](./AlertingKQLSamples/JobQueueFailures.kql) |
| Job Queue errors | Errors | VAR | Get alerted if no job queue entries have been started in a given time period. | AL0000E26 | [NoJobQueueRuns.kql](./AlertingKQLSamples/NoJobQueueRuns.kql) |
| Sensitive field monitoring | Security/Auditing | VAR | Alert if sensitive fields are added/removed to the monitoring list and if their values change. | AL0000DD3, AL0000EMW, AL0000CTE | [SensitiveFieldMonitoring.kql](./AlertingKQLSamples/SensitiveFieldMonitoring.kql) |
| Login errors | Errors | VAR | Alert if sessions fail to get created. | RT0001, RT0002 | [LoginFailures.kql](./AlertingKQLSamples/LoginFailures.kql) |
| Login performance | Performance | VAR | Alert if sessions take long to create. | RT0004 | [LoginPerformance.kql](./AlertingKQLSamples/LoginPerformance.kql) |


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
