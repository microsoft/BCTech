# Guidelines for designing Excel layout data

## Dataset form
Most Excel layouts work best with flat datasets (think denormalized star schema) where datatypes are not converted to strings and where data is not too aggregated. This type of data fits great into a pivot table.

## Dataset size
Excel has a limit on the total number of rows in a worksheet. Current limit is 1 mio. rows. If you design reports with Excel layouts on large datasets, you will need likely to give the user an option to aggregate the data (or educate them to apply filters on the report request page).

For more information on Excel limits, see
https://support.microsoft.com/en-us/office/excel-specifications-and-limits-1672b34d-7043-467e-8e27-269d656771c3

## Using Power Query
When a user runs a report with an Excel layout, the Business Central server will merge the report layout workbook with data and send the merged Excel workbook to the user. No other processing is done on the server. You can utilize this to do post-processing on the users workbook using VBA or Power Query. Especially with Power Query, you can apply almost any transformation you can think of to the dataset, such as adding computed columns, aggregate or explode data, define new entities such as hierachies, or even load data from external data sources (files, databases, or web services). 

For more information on Excel and Power Query, see [Power Query in Excel](https://powerquery.microsoft.com/en-us/excel/)

# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DA MAGE.
