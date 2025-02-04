# Sales Order Word Layout - Microsoft Directions EMEA 

This is AL extension for Dynamics 365 Business Central that generates demo data to showcase the new Word layout capabilities introduced in version 25. This extension has been used in Directions EMEA to simulate a real life case of a Sales Order report. You can find more details on how to use the extension in the instructions below.

## Sales Order Word Layout Demo
The extension provides a set of tables, reports, pages, and codeunit to generate demo data that simulates a real life scenario for sales order data. Precisely:
- Sales Order table: a table containing a sales order records, each record has an Id and a description.
- Sales Order Lines table: a table containing all different lines related to a sales order. Each line has an Id, a description, a quantity, an amount, a parent Id (the sales order it belongs to), a Line No., a Field to Hide used to showcase the Hide Field If Zero layout control, a Row To Hide used to showcase the Hide Empty Table Row layout control, and a Column To Hide used to showcase the Hide Empty Table Column layout controls.
- Sales Order Additional Info table: a table containing additional info for sales orders. Each records has an Id, a description, a parent Id (the sales order it belongs to), and a miscellaneouos.
- Sales Order report: a WordMergeDataItem report to print a distinct report for each Sales Order.
- Sales Order - List report: a report to print the list of sales orders.
- Sales Order Data page: a page providing actions to generate demo data and interact with the reports.
- Sales Order codeunit: a codeunit providing utility functions to generate demo data.

### How to generate demo data
To generate demo data:
- Install the extension on your Business Central environment.
- Navigate to page _Sales Order Data (Id 52800)_.
- Expand the _Actions_ tab.
- Click on:
  - _Init Data_ to generate demo standard data.
  - _Init Conditional Data_ to generate demo data that are crafted to showcase the new visibility layout controls introduced with the new Business Central Word Add-in.

  Note, both init actions will be reset all the data in the 3 tables. As such, any modifications to those tables (e.g. new records) will be deleted.

### How to run the reports
To run the reports you can follow two approaches.
1. Navigate to the _Report Layouts_ and search for reports _Sales Order (Id 52800)_ and _Sales Order - List (Id 52801)_. From here, follow the standard way of working with report. You can find more at https://aka.ms/bcword.
2. Navigate to page _Sales Order Data (Id 52800)_. Here, under the _Reports_ tab you can run and save both reports as PDF or Word.

### Notes
This extension is purely used to showcase the new visibiltiy controls introduced with the new Business Central Word Add-in in version 25. As such, data have been crafted to provide an ideal scenario for such demo.

> [!NOTE]
> Please note that the demo data presented here are randomly generated and do not refer to any real-life person or scenario. Any resemblance to actual persons, living or dead, or actual events is purely coincidental.
