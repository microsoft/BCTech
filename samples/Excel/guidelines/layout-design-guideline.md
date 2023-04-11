# Guidelines for designing Excel layouts

## Worksheet naming and location
Locate worksheets in the order you think will be most useful for users so that they do not have to scroll when using the report. 

Good worksheet names help users quickly get an overview of the information they can obtain by navigating to the worksheet. 

An Excel worksheet can be up to 31 characters long.
For more information, see https://stackoverflow.com/questions/3681868/is-there-a-limit-on-an-excel-worksheets-name-length

## One or multiple worksheets
In contrast to a report designed for print or pdf, an Excel report typically consists of multiple worksheets, each of which is designed for a different purpose. Some common types of worksheets are
1. Overview dashboard
2. Pivot table
3. Table
4. Print-friendly
5. About the report

There is no technical limit to the number of worksheets in a report, but users probably prefer a number they can overskue. Note that you can develop multiple Excel layouts for the same report, so maybe design for a specific persona.

### Overview dashboard worksheet
Excel has visuals such as charts and maps, sparklines, and slicers, that can be used for creating dashboards known from Business Intelligence products such as Power BI. You can use these to create dashboard worksheets that lets a user interact with the data and get visually appealing graphs showing trends or top 10 numbers.

### Pivot table worksheet
Use a pivot table worksheet to give users a way to interact with the dataset. Typically there is only the need of one pivot worksheet in the report (unless you want to offer multiple pre-baked analysis). If you add multiple pre-baked pivot table worksheets, consider naming them _X by Y_, such as _Customers by Item_ or _Sales by Region_. 

### Table worksheet
A table worksheet can be used to display a specific view on the dataset, maybe only showing a subset of the columns and maybe even with some added computed columns. 

### Print-friendly worksheet
Some users might want to print the report, but this should not force you to design all worksheets to be print-friendly. Instead, consider having one or more worksheets that has been opmitized for print, maybe one for horizontal and one for landscape paper orientation.

### About the report worksheet
Users are not always 100% sure how your report can be used and for whom it was designed. Consider always having a _About the report_ worksheet that explains
1. What the report is about and maybe also for which persona.
2. A description for each worksheet that explains what the users can do here.
3. Maybe also add _See also_ links to documentation in case the user wants to learn more.


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DA MAGE.
