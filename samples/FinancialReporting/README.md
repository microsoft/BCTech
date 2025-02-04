# What is Financial Reporting templates for Business Central?
The Financial Reporting feature gives you insights into the financial data shown on your chart of accounts (COA). A financial report consists of three elements: a row definition, a column definition, and a report definition. These definitions are also called templates. 

To learn more about the Finacial Reporting feature, go to
https://learn.microsoft.com/en-us/dynamics365/business-central/bi-how-work-account-schedule

# What resources can I find here?
This repo contains configuration packages for Financial Reporting templates that was releases in 2025 release wave 1 (version 26)

# How do I import the templates?

It is recommended to import templates into a test company before importing them in a production company.

You need to import the configuration packages in a specific order:
1. import row and column definitions first 
1. then import report definitions (as they depend on the row/column definitions)

To learn how to import column definitions, go to https://learn.microsoft.com/en-us/dynamics365/business-central/bi-column-definitions#import-or-export-financial-report-column-definitions

To learn how to import row definitions, go to https://learn.microsoft.com/en-us/dynamics365/business-central/bi-row-definitions#import-or-export-financial-reporting-row-definitions

To learn how to import report definitions, go to https://learn.microsoft.com/en-us/dynamics365/business-central/bi-how-work-account-schedule#import-or-export-financial-report-definitions

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DA MAGE.
