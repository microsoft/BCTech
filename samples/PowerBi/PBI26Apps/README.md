# Power BI apps for Business Central?
Since version 25.1, Business Central has had support for Power BI apps for different functional areas.

Learn more in the official documentation here: 
https://learn.microsoft.com/en-gb/dynamics365/business-central/across-powerbi-apps-by-functional-area

Starting in Spring 2025, the source code for these apps will be released as open source. 

# What resources can I find in this directory?
This directory contains early access code samples for the Power BI apps for Finance, Manufacturing, Inventory, and Inventory Valuation.

Note that this location is not the final one for the source code of the open sourced apps.

The file `Finance-DuplicateGLAccounts` contains an alternative version of the Finance app that addresses an issue when sourcing G/L Accounts that blocked data refresh. If your Finance app can't refresh with an error like "Column G/L Account No. contains a duplicate value" this PBIX will help you to load your data while we fix the main report. Be aware that this fix is not final and it changes the report tabs "Balance Sheet" and "Income Statement" to use the G/L Account Category hierarchy instead of the calculated G/L Account hierarchy, therefore you need to configure in Business Central your G/L Accounts to set a G/L Account Subcategory and the corresponding mappings in the "Power BI Account Categories" page.

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.