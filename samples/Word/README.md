# What is the Word add-in for document layouts in Business Central?
Business Central 2024 release wave 2 (version 25) introduced a new Word add-in as a way to layout document reports with conditional visibility.

# Layout controls for document reports

The new Word add-in for Business Central introduces *layout controls* for the layout creator to encode a layout file with conditional visibility of field, tables, table rows, and table columns based on data. It also includes a way for layout creators to include comments in the layout file. These comments will then be removed from the document when the report is rendered.

## The *comment* control

As a layout creator, you might want to include comments in the layout file to help you or the next person who need to maintain the layout. These comments will then be removed from the document when the report is rendered. You can include text or tables in a comment control. 

Use comments for things such as describing difficult parts of the layout, or maybe add a change log table in the end of the file to track different versions of the layout. This could be useful when troubleshooting a report issue (you will need to get both a copy of the rendered report and the layout as the comment will have been removed from the former at runtime).

### Exercise: Add a versioning table

Download any Word layout from Business Central from the Report Layouts page (filter to type *Word* and then use the *Export Layout* action). Navigate to the end of the Word file. Add a table with three columns and two rows like this:

| Layout description | Version | Date of change |
| ------------------ | ------- | -------------- | 
| Layout using a comment | 1.0 | <todays date> |

Then mark the table and choose the *comment* control from the *Layout Controls* menu in the Business Central add-in.

When you place your cursor somewhere in the table, you will notice that the control displays a "Hidden Comment" text. 

Contratulations, you have added your first comment. 

Now, try importing the layout back to Business Central: from the Report Layouts page, make sure you have focus on the report, and then use the *New* action to import the layout. Then use the *Run* action to test the layout. Hopefully, the comment is now gone (from the generated report.)

## Hide if empty: the *Hide Field if Zero* control

In some reports, you might want to mimic the BlankZero or BlankNumbers properties that exist on table and page fields. You can achieve this in the dataset, but what if you don't have control over the AL code? Or if some layouts should show zeros and others blank them out? 

Here, the *Hide Field if Zero* control comes to the rescue: simply apply it to a field (standalone or as part of a repeater). At runtime, the Business Central server will then convert any zero values to a blank string.


## Hide if empty: the *Hide Empty Table* control

If you have a data item in the dataset that might have data and might not, you can enclose the repeater in a table with the *Hide Empty Table* control. If no rows exists when rendering the report at runtime, the Business Central server will then simply cut the enclosing table from the document. 

**Note!** The *Hide Empty table* you have to place on the table itself, not on the repeater.

## Hide if empty: the *Hide Empty Table Row* control

If you have a data item in the dataset, where one field should determine if the row is shown, you can enclose that fields in the repeater with the *Hide Empty Table Row* control. For rows, where that field has no value, when rendering the report at runtime, the Business Central server will then simply cut the row from the table. 


## Hide if empty: the *Hide Empty Table Column* control

If you have a data item in the dataset, where no table header and table column should be visible in the absense of data in the field (across all rows in the dataitem), you can enclose that fields in the table header with the *Hide Empty Table Column* control. For datasets, where that no values exist for that field, when rendering the report at runtime, the Business Central server will then simply cut the column from the table. 

One use case for this layout control is discounts, where you want to remove the discount column from the invoice if no discount has been applied. 

**Note!** You can combine *Hide Empty Table Column* with *Hide Field if Zero* to hide columns with zero values. Just add the *Hide Field if Zero* to the field in the repeater and use *Hide Empty Table Column* on the corresponding field in the table header.

### Exercise: Make a version of the purchase invoice report where discount column is removed

The RDLC layout for report 406, *Purchase - Invoice* has a table with a *Discount %* column. The demo data for Cronus should have plenty of purchase invoices without any discount. 

In this exercise, try to create an empty Word layout for report 406 and then see if you can re-create the table from the RDLC layout, but now in a version where the *Discount %* column is removed if no discount is given.


# Disclaimer
Microsoft Corporation (“Microsoft”) grants you a nonexclusive, perpetual, royalty-free right to use and modify the software code provided by us for the purposes of illustration  ("Sample Code") and to reproduce and distribute the object code form of the Sample Code, provided that you agree: (i) to not use our name, logo, or trademarks to market your software product in which the Sample Code is embedded; (ii) to include a valid copyright notice on your software product in which the Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend us and our suppliers from and against any claims or lawsuits, whether in an action of contract, tort or otherwise, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code or the use or other dealings in the Sample Code. Unless applicable law gives you more rights, Microsoft reserves all other rights not expressly granted herein, whether by implication, estoppel or otherwise. 

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL MICROSOFT OR ITS LICENSORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THE SAMPLE CODE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DA MAGE.
