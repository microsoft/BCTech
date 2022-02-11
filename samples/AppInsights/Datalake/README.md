In this folder, you will find samples that illustrate how you can use Azure Application Insights data in PowerBI reports.

# How can I get a copy of my telemetry data in a Datalake?
Using Azure Data Factory, you can export (extracts of) telemetry to a data lake (ADLSgen2). 


# Why export data to a datalake?
Your Azure Application Insights resource only holds data for the duration that you have set in your rentention policy. Once data is older than this, Azure Application Insights will delete it. 

You might want to export (extracts of) telemetry to a datalake to be able to query the data for longer time
- Build reports in Power BI (adding a Power Bi dataflow on each “table” in the lake)
- Use Azure Synapse Serverless to do ad-hoc queries with normal SQL

# Should I store all details or aggregated data in the lake?
It is totally up to you. Maybe you do not need all details years back in time, but you want to track trends. In the latter case, aggregations such as sum/count of events and/or durations make sense to store. Other descriptive statistics such as averages or standard deviations can be harder to use in reports once part of the dataset is pre-aggregated because these statistics are harder to combine (you can always sum over counts to further aggregate counts, or sum over sums to further aggregate sums).

# How do I connect Azure Data Factory to Azure Application Insights?
<!-- To read data from Azure Application Insights in Azure Data Factory, you need to 
- Open Azure Data Factory Studio from your Azure Data Factory and go to the Author menu
- Create a new pipeline and add a Copy Data activity
-  -->

Read more in the documentation here: https://docs.microsoft.com/en-us/azure/data-factory/connector-azure-data-explorer?tabs=data-factory

See also this blog post by Bert Verbeek: https://www.bertverbeek.nl/blog/2022/02/10/two-ways-of-exporting-bc-telemetry/


# How do I make daily extracts?
```kql
// start your query with defining a date_to_query variable that is populated when the pipeline runs
let date_to_query = startofday(datetime('@{formatDateTime(pipeline().parameters.windowStart,'yyyy-MM-dd')}'), -1)
;
// now query away and use the variable for filtering
traces
| where startofday(timestamp) == date_to_query
```

Now, set up a trigger in Azure Data Factory and use it to schedule the pipeline daily.

# How do I format my KQL queries to make data useful in CSV files
If you use CSV files as the file format in the data lake, it is important to follow a few best practices to make the data useful/easy to consume
- always end your kusto query with an explicit 'project' operator and do not use whitespace or special characters in column names. A common practice is to only use lowercase alphanumeric characters (a-z0-9) and underscore, e.g. company_name.
- always sanitize data fields in the KQL query to exclude separator and newline characters

# How do I use data in the datalake in Power BI? 
Once you have data in the datalake, you can setup Power BI dataflows to consume it easily in Power BI.

Read more here: https://docs.microsoft.com/en-us/power-bi/transform-model/dataflows/dataflows-introduction-self-service

# Must I use Azure Data Factory and Azure Datalake gen2?
No, you can use any tool you like to read data from Azure Application Insights and any data store to store the results. 

Maybe a Powershell script that reads data and dumps it to csv files on file storage is more like you? Then check out the Powershell sample in this repository.

Any ETL tool that can read from a REST service API can read data from Azure Application Insights. The Powershell sample in this repository use this method and maybe you can reuse code snippets from that code to achieve what you need.

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
