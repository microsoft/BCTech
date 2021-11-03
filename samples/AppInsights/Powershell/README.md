In this folder, you will find samples that illustrate how you can query Azure Application Insights data using Powershell.

# Which Powershell scripts are available?
This repository contains Powershell for 
* querying Azure Application Insights

# How do I use Powershell scripts to query telemetry?
To use the script, do as follows
1) Go the Azure Application Insights portal, go to the *API Access* menu, and copy the application id and generate an API key 
2) Open Powershell and 
a) run GetTelemetryData.ps1 -appid <app id> -apikey <api key> -kqlquery <kql query you want to run>
or
b) pipe the content of a text file with a KQL query into the script: Get-Content <file> | .\GetTelemetryData.ps1 -appid <app id> -apikey <api key> 

Limitations: The current version of the script fails if either the KQL query only returns one column or one row. See comments in the code to learn more (and do reach out if you have a solution for this).

# How do I use Powershell to delete telemetry data?
See an example here: https://demiliani.com/2021/03/30/deleting-application-insights-telemetry-data-on-demand-with-powershell/

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
