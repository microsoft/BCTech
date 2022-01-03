Telemetry in Azure Application Insights is a very powerful tool to investigate issues after they have occurred (where you cannot attach a debugger to troubleshoot the issue). Having a good arsenal of Kusto query language (KQL) scripts can be very handy, but sometimes executing KQL statements one by one in the Azure Application Insights portal can be tedious. This is where notebooks come in handy, and in this part of the samples repository we show you how to get started using Jupyter notebooks in Azure Data Studio. With the Kqlmagic module, you can now combine KQL with Python to create powerful interactive trouble shooting guides (TSGs).

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
* Investigating results for the technical validation of AppSource submissions

| TSG | Description |
| ----------- | ----------- |
| Login-issues-TSG.ipynb | Use this TSG for cases related to login issues, such as if one or more users cannot login (check failure reasons in authorization checks before/after onCompanyOpen trigger, to troubleshoot if a login issue is related to firewall/network issues at the customer site (check if web service requests get in but client requests are not present), or to check if logins are successful again after an incident |
| Performance-overview-TSG.ipynb | Use this TSG to analyze performance problems. The TSG runs through most of common root causes for perf issues and has links to the performance tuning guide so that partners can fix their code to be more performant
| Web-services-TSG-version.ipynb | Use this TSG to analyze issues with web services: Is the environments being throttled? Analyze endpoint performance. Analyze failure reasons (HTTP codes 401, 404, 408, 426) |
| Microsoft-Connectors-TSG-version.ipynb | Use this TSG to analyze issues with Microsoft connectors (PowerBI, PowerApps, LogicApps, or Flow): Is the environments being throttled? Analyze endpoint performance. Analyze failure reasons (HTTP codes 400, 401, 404, 408, 426) |
| Performance-partner-code-TSG.ipynb | Use this TSG to analyze performance problems in partner code. The TSG filters telemetry on object ids outside the ranges used for the base app and localizations (the code written by Microsoft) | 
| Data-related-TSG.ipynb | Use this TSG to analyze data-related issues: Long running queries, database locks, reports that runs many sql statements | 
| Extensions-TSG.ipynb | Use this TSG to analyze issues with the extension lifecycle (compile, synchronize, publish, install, update, un-install, un-publish). You can set filters on AAD tenant id, environment name, and extension id to troubleshoot all environments in a AAD tenant id, a single environment, single extensions, or any other combination of the three |
| AppSource-Submission-TSG.ipynb | Use this TSG to analyze the results of the technical validation of AppSource submissions. You can see whether your submission passed the technical validation, which errors were reported, which countries and releases were validated, but also which baselines were used for breaking change validation |

# What is Kqlmagic?
Kqlmagic is a command that extends the capabilities of the Python kernel in Azure Data Studio notebooks. You can combine Python and Kusto query language (KQL) to query and visualize data using rich Plot.ly library integrated with render commands. Kqlmagic brings you the benefit of notebooks, data analysis, and rich Python capabilities all in the same location. Supported data sources with Kqlmagic include Azure Data Explorer, Application Insights, and Azure Monitor logs.

# How can I run Jupyter notebooks?
You can either run Jupyter notebooks in a hosted environment such as Github Codespaces or run them locally using Azure Data Studio. Below, you can find detailed installation and configuration steps for both approaches.

# I have installed the tools. Now what?
You are now ready to run some notebooks. Either write your own or download some from this repository.

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


# Running Jupyter notebooks using GitHub Codespaces

## What is GitHub Codespaces?
A GitHub Codespace is a development environment that's hosted in the cloud. Codespaces run on a variety of VM-based compute options hosted by GitHub.com, which you can configure from 2 core machines up to 32 core machines. You can connect to your codespaces from the browser or locally using Visual Studio Code.

## Where is GitHub Codespaces available?
GitHub Codespaces is available for organizations using GitHub Team or GitHub Enterprise Cloud. For more information, see [GitHub's products](https://docs.github.com/en/get-started/learning-about-github/githubs-products).

## How do I learn more about GitHub Codespaces?
Here are some resources for you to get started with GitHub Codespaces. Use CTRL+click to open them in a new browser tab/window.
* [About GitHub Codespaces](https://docs.github.com/en/codespaces)
* [Codespaces pricing](https://docs.github.com/en/billing/managing-billing-for-github-codespaces/about-billing-for-codespaces)

## How do I configure a GitHub Codespace?
- First, check if your GitHub account is an organizational account of type GitHub Team or GitHub Enterprise Cloud.
- (Optional) If you use the notebooks from this repository, then clone the the repository to your own account. 
- On the page [https://github.com/codespaces](https://github.com/codespaces), click on the "New codespace" button and choose the repository and branch where your notebooks are located. Click "Create codespace" and choose the size of VM that you want to use. The supplied notebooks in this repository do not use a lot of resources, so maybe just start with the smallest VM size available. Click "Create codespace" to confirm.
- After a little waiting time, your code space is ready with Visual Studio Code opened in your browser.
- On the Explorer menu to the left, find the folder(s) containing your notebooks. The notebooks supplied with this repository are located in _BCTECH [CODESPACES]_ under _samples, AppInsights, TroubleShootingGuides, D365BC Troubleshooting Guides (TSG), content_ .

## How do I install Python in my codespace?
Open a notebook. If this is the first time you open a notebook that uses KQLMagic, you will be asked to install Python. Follow the instructions (Choose _Open a terminal_, copy the two _sudo apt-get_ commands and run them in the terminal).

You only need to do this once.

## How do I install KQLMagic in my codespace?
In any Python code cell, run the following code:
```python
import sys
!{sys.executable} -m pip install Kqlmagic --no-cache-dir --upgrade
```

You only need to do this once.

# Running Jupyter notebooks locally using Azure Data Studio

## What is Azure Data Studio?
Azure Data Studio is a cross-platform database tool for data professionals using the Microsoft family of on-premises and cloud data platforms on Windows, MacOS, and Linux. Azure Data Studio offers a modern editor experience with IntelliSense, code snippets, source control integration, and an integrated terminal. It's engineered with the data platform user in mind, with built-in charting of query result sets and customizable dashboards. 

## How do I learn more about Azure Data Studio?
Here are some resources for you to get started on Azure Data Studio. Use CTRL+click to open them in a new browser tab/window.
* [How to Start with Microsoft Azure Data Explorer (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-explorer-starting/table-of-contents)
* [Exploring Data in Microsoft Azure Using Kusto Query Language and Azure Data Explorer (Pluralsight course, requires subscription)](https://app.pluralsight.com/library/courses/microsoft-azure-data-exploring/table-of-contents)

## How do I install and configure Azure Data Studio?
1. First, you need to download and install Azure Data Studio. See how to here: https://docs.microsoft.com/en-us/sql/azure-data-studio/download-azure-data-studio
2. Now create a new notebook (needed for steps 3+4 below) and choose the Python3 kernel, see https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#create-a-notebook
3. Install a Python kernel for Azure Data Studio (let Azure Data Studio do it for you or reuse an existing Python installation). See how to here: https://docs.microsoft.com/en-us/sql/azure-data-studio/notebooks-tutorial-python-kernel#change-the-python-kernel

## How do I install and set up Kqlmagic?
To install and set up Kqlmagic, open a python notebook. Click the "Manage Packages" icon on the right of the notebook:

![Manage Python packages](../images/install-kqlmagic-1.png)

Under the "Add new" tab, type "kqlmagic" and click Install (we generally recommend people not to install “dev” version of Kqlmagic):

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


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
