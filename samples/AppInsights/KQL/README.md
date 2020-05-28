In this folder, you will find samples of Kusto Query Language (KQL) for each type of signal that is sent to Application Insights.

# Usage of Kusto Queries
You can use Kusto queries as the data source in a number of places. E.g.
* the Logs part of Application Insights in the Azure portal
* PowerBI reports
* Azure Monitor Alerts
* Azure Dashboards
* Jupyter Notebooks (with the Kqlmagic extension)


# Resources 
Here are some resources for you to get started on Kusto Query Language (KQL)
* [Kusto Query Language Overview](https://docs.microsoft.com/en-us/azure/kusto/query/)
* [Kusto Query Language Tutorial](https://docs.microsoft.com/en-us/azure/kusto/query/tutorial)
* [Kusto Query Language (KQL) from Scratch (Pluralsight course, requires subscription)](https://www.pluralsight.com/courses/kusto-query-language-kql-from-scratch)


# Signal overview
Signal is added incrementally to Business Central. In this table you can see in which version or update a class of signal was added/modified:

|Signal | Emited from version, update | Documentation (use CTRL+click to open in a new tab) |
| ------ | ------ | ------ |
| Long running operation (SQL query) | 2019 release wave 2, RC | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-long-running-sql-query-trace |
| Authorization | 2019 release wave 2, Update 2| https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace |
| Web Service Request | 2020 release wave 1, RC| https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-webservices-trace |
| Report Execution | 2020 release wave 1, RC| https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-reports-trace | 
| Open Company timing | 2020 release wave 1, RC | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-authorization-trace |
| Company lifecycle (create/copy/delete) | 2020 release wave 1, Update 1 | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-company-lifecycle-trace (not live yet) |
| Update errors due to exceptions in upgrade code | 2020 release wave 1, Update 2 | https://docs.microsoft.com/en-us/dynamics365-release-plan/2020wave1/dynamics365-business-central/update-error-telemetry-application-insights-partners (pending docs link) |
| Database lock timeouts | 2020 release wave 1, Update 2 | https://docs.microsoft.com/en-us/dynamics365/business-central/dev-itpro/administration/telemetry-database-locks-trace (not live yet)|
| Extension lifecycle | 2020 release wave 1, Update 2/3 | https://docs.microsoft.com/en-us/dynamics365-release-plan/2020wave1/dynamics365-business-central/extension-lifecycle-telemetry-application-insights-partners (pending docs link)  |


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
