# What is Business Central Data Analysis?
In the 2023 release wave 1 of Business Central, we are introducing a new powerful feature that allows you to analyze data on lists directly in the Business Central.

Read more about Data Analysis here: release plan entry TBA

# How can I use Data Analysis?
Data Analysis is meant for quick fact checking and ad-hoc analysis when you don't want to run a report, if a report for your specific needs does exist, or if you want to quickly iterate to get a good overview on part of your business.

In the following table, you'll find examples of usage scenarios for each main area in the Business Central application.

| Area    | Scenario | How Data Analysis can help | Data Foundation | Fields to get you started | Built-in report | 
| ------- | ---------| -------------------- |
| Finance (Accounts Receivables) | You want to see what your customers owe you, maybe broken down into time intervals for when amounts are due. | Open the Customer Ledger Entries list and switch on Analyze. Go to the Columns menu and remove all columns (click the box next to the _Search_ field). Turn on _Pivot_ mode (located directly above the _Search_ field). Now, drag the _Customer Name_ field to the _Row Groups_ area and drag _Remaining Amount_ to the _Values_ area. Finally, find the _Due Date Month_ field and drag it to the _Column Labels_ area. If you want to restrict the analysis to a given year/quarter, apply a filter in the _Additional Filters_ menu (to the right, just below the _Columns_ menu.) Rename your analysis tab to "Aged Accounts by Month" or something that describes this analysis for you. | (Customer Ledger Entries)[https://businesscentral.dynamics.com/?page=25]| _Customer Name_, _Due Date_, and _Remaining Amount_. | [Aged Accounts Receivables](https://businesscentral.dynamics.com/?report=120) |




# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
