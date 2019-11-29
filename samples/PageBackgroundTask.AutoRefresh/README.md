# Purpose
This AL extension is showing how to implement automatic refresh of a sub part by using Page Background Tasks in Business Central.

# Requirement
Business Central 2019 wave 2 release CU2 (Also kown as version 15.2)
Understanding of how Page Background Tasks are working.

# How to run
Clone the repository, build the extension and publish it to your sandbox.

**On the Customer List, a factbox has been added on the right:**
* AutoRefresh with Page Background Task

**A new page (card) has been added:**
* Auto Refresh Repeater Sample

# Design

The design of auto refresh with PBTs is to enqueue a new PBT from a completion trigger (success or error). Doing so chains all the completion trigger, and enables to have an "infinite" auto-refresh.

The code unit "AutoRefresh GetRecords" is running in a child session of the parent session, it fetches the records to be displayed, and then they are being passed onto the completion trigger and finally to the subform.

It is required to have a parent form where the record doesn't change, or if there is no record at all, like a card or a role center. This is due to the design of PBTs that are automatically cancelled when the record changes on the form.

Every time the repeater source table is updated, this may introduce a little flicker. This is normal because the client must re-calculate/render the entire list when this happens. Therefor it is recommended to update as minimum as possible the list in an auto refresh scenario.

# Remark

Auto refresh consumes child sessions (one per auto refresh) and this put stresses on the system. Therefor it is recommended to use it only where it is absolutely required, and try to keep the quantity of data to be updated all the time as small as possible.
