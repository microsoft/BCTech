# Extracting data from Business Central
A common problem that organizations face is how to gather data from multiple sources, in multiple formats. Then you'd need to move it to one or more data stores. The destination might not be the same type of data store as the source. Often the format is different, or the data needs to be shaped or cleaned before loading it into its final destination.

Various tools, services, and processes have been developed over the years to help address these challenges. No matter the process used, there's a common need to coordinate the work and apply some level of data transformation within the data pipeline. The following sections highlight the common methods used to perform these tasks.

# Extract, transform, and load (ETL) process
Extract, transform, and load (ETL) is a data pipeline used to collect data from various sources. It then transforms the data according to business rules, and it loads the data into a destination data store. The transformation work in ETL takes place in a specialized engine, and it often involves using staging tables to temporarily hold data as it is being transformed and ultimately loaded to its destination.

When establishing a data lake or a data warehouse, you typically need to do two types of data extraction:

1. A historical load (all data from a given point-in-time)
2. Delta loads (what's changed since the historical load)

## Getting a historical load
The fastest (and least disruptive) way to get a historical load from Business Central online is to get a database export as a BACPAC file (using the Business Central admin center) and restore it in Azure SQL Database or on a SQL Server. For on-premises installations, you can just take a backup of the tenant database.

## Getting delta loads
The fastest (and least disruptive) way to get delta loads from Business Central online is to set up API queries configured with read-scaleout and use the data audit field LastModifiedOn (introduced in version 17.0) to filter only the data that was changed/added since the last time you read data.


# Tools for reading data
For Business Central on-premises, you can just read directly from the environment database. Note that this option is not available for Business Central online, so establishing this type of integration could block you from migrating from on-premises to the online version. 

For Business Central online, the only supported option for reading data is using APIs (either the standard APIs that are shipped with Business Central or custom APIs that you build in vscode and ship as an extension). 

There exists a code sample that you may use to be more productive with APIs: API generator. 
For more information, see https://github.com/microsoft/BCTech/tree/master/samples/APIQueryGenerator

# Throughput of data reads with APIs
Measurements have shown that it is not unusual to be able to read 2 MB/sec per API call. This means that it is possible to transfer up to 120 MB/min or 7200 MB/hour for pipelines running sequentially. 

In Business Central online, the current parallelism for API calls is 5. This means that you can read up to 35 GB/hour if no other processes call web services on Business Central. For an ETL setup that update the staging area nightly with yesterdays changes, this should fit most maintenance windows (unless the environment has several GB of new/updated data per day).

Even if the ETL setup reads the historical dataset before switching to incremental loads, with a 2 MB/sec throughput, it will take up to one day to load a 100 GB database and this operation is something you only do once. 

# Stability of ETL pipelines
No matter which tool you choose, you must make your data pipelines robust towards timeouts and design them so that they are re-runnable. All Business Central tables have system fields _SystemRowversion_ and _SystemLastModifiedOn_ (read more about system fields here: https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-table-system-fields). If your ETL setup track which watermark (either a date or a rowversion) that was read last time, then data pipelines can utilize this to read changes since the watermark.


# Extract, transform, and load (ETL) tools
You can use your ETL tool of choice to read and tranform data. Below, you can read about some popular options.

## SQL Server Integration Services (SSIS)
For Business Central on-premises, one popular tool of choice is SQL Server Integration Services (SSIS) that is shipped with SQL Server. It is possible to auto-generate SSIS packages from a metadata store using script tasks or a tool such as BIML (see https://www.varigence.com/biml for more information). For more information about SQL Server Integration Services (SSIS), see
https://learn.microsoft.com/en-us/sql/integration-services/sql-server-integration-services?view=sql-server-ver16

## Azure Data Factory
For Business Central online, you can use a tool such as Azure Data Factory to read and transform data. Azure Data Factory is a managed cloud service that is built for complex hybrid extract-transform-load (ETL), extract-load-transform (ELT), and data integration projects. For more information about Azure Data Factory, see https://learn.microsoft.com/en-us/azure/data-factory/introduction

You can use the Azure Data Factory OData connector with service principal authentication to extract data from Business Central. For more information, see
* https://learn.microsoft.com/azure/data-factory/connector-odata?tabs=data-factory
* https://learn.microsoft.com/answers/questions/751705/how-to-connect-business-central-to-azure-data-fact

## Power BI dataflows
It is also possible to use Power BI dataflows for your extract pipelines. With Power BI dataflows, you can connect to Business Central APIs and utilize incremental refresh to only load data that was changed since last refresh (see https://learn.microsoft.com/en-us/power-query/dataflows/incremental-refresh for more information). For more information about Power BI dataflows, see https://learn.microsoft.com/en-us/power-bi/transform-model/dataflows/dataflows-introduction-self-service

Microsoft MVP Steven Renders has written a very nice blog post on how to use Power BI dataflows with Business Central:
https://thinkaboutit.be/2023/02/how-do-i-create-a-power-bi-dataflow-with-business-central-data/

## bc2adls code sample (unsupported)
Another (unsupported) option is to use the _bc2adls_ code sample to transfer data from the Business Central server (NST) directly to a Azure Data Lake Storage (ADLS) data lake. For more information about bc2adls, see https://github.com/microsoft/bc2adls



# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
