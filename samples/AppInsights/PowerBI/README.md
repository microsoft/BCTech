In this folder, you will find samples that illustrate how you can use Azure Application Insights data in PowerBI reports.

# Which Power BI reports are available?
This repository contains Power BI reports for 
* Investigating performance issues 
* Investigating errors

# How do I use Power BI reports for troubleshooting
To use the Performance or Error report, do as follows
1) Download and install Power BI Desktop: https://powerbi.microsoft.com/en-us/downloads/
2) Download the .pbit template files from the *Reports* directory
3) Open a .pbit template file in Power BI Desktop
4) Fill in parameters (Azure Application Insights App id is required. Get it from the *API Access* menu in the Azure Application Insights portal). 
5) (Optional) Save the report as a normal .pbix file

# How does authentication work between Power BI and Azure Application Insights?
Currently, Power BI only supports AAD authentication to Application Insights. This means that the approaches we use in Jupyter notebooks (using App id and API key) and Azure Dashboards (using Application Insights subscription id, Application Insights name, and Application Insights resource group) are not applicable in Power BI. To use Power BI with data from Application Insights, the user of the report must be in the same AAD tenant as the Application Insights resource and need to have read access to Application Insights resource.

# How do I convert a KQL query into a M query (Power Query)?
If you have a nice KQL query that you want to use in Power BI, then do as follows
1) run the query in the Application Insights portal (under Logs)
2) Click *Export* and choose *Export to Power BI (M query)*. This downloads a M query as a txt file.
3) Follow the instructions in the txt file to use the M query as a datasource in PowerBI 

Read more about this topic here: https://docs.microsoft.com/en-us/azure/azure-monitor/app/export-power-bi

# How do I use PowerBI parameters to set properties such as AAD tenant id and environment name?
If your Application Insights resource contain data from multiple environments, then you might want to use parameters in your Power BI reports. In the sample report *PartnerTelemetryTemplate.pbix*, you can see how to use parameters to achieve this.

Read more about Power BI parameters topic here: https://powerbi.microsoft.com/en-us/blog/deep-dive-into-query-parameters-and-power-bi-templates/

# How can I modify the standard reports?
All M queries used in the standard reports are available in the *M queries* directory. You can use them if you want to create your own reports based on the data sources defined in the standard reports.


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
