pageextension 50104 ContosoFactboxSample extends "Item Card"
{
    layout
    {
        addfirst(factboxes)
        {
            part(PowerBILockedReport; "Power BI Embedded Report Part")
            {
                ApplicationArea = All;
                Caption = 'Power BI report';
                SubPageView = where(Context = const('ContosoFactbox'));
            }
        }
    }

    trigger OnOpenPage()
    begin
        // The easiest way to get the necessary IDs for report visuals is to:
        //   1. Open the Power BI report in the browser
        //   2. Hover over the visual you want to embed, and click on the three dots menu
        //   3. Choose to "Share" the visual, and choose "Link to this Visual"
        //   4. Use the "Copy" button to copy the URL
        //   5. From the URL, you can find:
        //      a. The Report ID after the /reports/ segment 
        //      b. The Report Page right after the Report ID
        //      c. The visual ID in a URL query parameter called "visual"
        // 
        // Example URL:
        // https://app.powerbi.com/groups/me/reports/<REPORT ID>/<PAGE ID>?[...]&visual=<VISUAL ID>

        AddPowerBIVisualToThisPage('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565');
    end;

    local procedure AddPowerBIVisualToThisPage(ReportId: Text; ReportPage: Text; ReportVisualId: Text)
    var
        PowerBIContextSettings: Record "Power BI Context Settings";
        PowerBIDisplayedElement: Record "Power BI Displayed Element";
    begin
        PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
        if PowerBIContextSettings.IsEmpty() then
            exit; // User has not set up the Power BI integration

        // Every page of type "Power BI Embedded Report Part" is associated with what we call a Context. A context identifies the set of elements 
        // (e.g. reports and dashboards) and settings that are used for that part for the current user. That means, you can have two "Power BI Embedded Report Part"
        // in the same page, which show different elements, as long as their Context properties are different. The same way, you can have two "Power BI Embedded Report Part"
        // in two completely different parts of Business Central that show the same set of reports, as long as they are referring to the same Context.

        // For example, let's add a selected report for the current user and for this Context.
        // More information on how to set these parameters in file 'AddActionToOpenReportExpanded.PageExt.al' and 'AddCustomerCardLockedPart.PageExt.al'.
        if not PowerBIDisplayedElement.Get(UserSecurityId(), 'ContosoFactbox', PowerBIDisplayedElement.MakeReportVisualKey(ReportId, ReportPage, ReportVisualId), PowerBIDisplayedElement.ElementType::"Report Visual") then begin
            PowerBIDisplayedElement.Init();
            PowerBIDisplayedElement.ElementType := PowerBIDisplayedElement.ElementType::"Report Visual";
            PowerBIDisplayedElement.ElementId := PowerBIDisplayedElement.MakeReportVisualKey(ReportId, ReportPage, ReportVisualId);
            // NOTE: The Power BI team recommends to get the embed URL from the Power BI REST APIs, as the URL format might change in the future. 
            // However, currently the approach below is also supported.
            PowerBIDisplayedElement.ElementEmbedUrl := StrSubstNo('https://app.powerbi.com/reportEmbed?reportId=%1', ReportId);
            PowerBIDisplayedElement.Context := 'ContosoFactbox'; // Use here the same context that you specified in the SubPageView of the part;
            PowerBIDisplayedElement.UserSID := UserSecurityId();
            PowerBIDisplayedElement.ShowPanesInExpandedMode := false;
            PowerBIDisplayedElement.ShowPanesInNormalMode := false;
            PowerBIDisplayedElement.Insert();
        end;

        // In PowerBIContextSettings, you can ensure that the controls to change report are disabled for this context.
        PowerBIContextSettings.CreateOrReadForCurrentUser('ContosoFactbox'); // Use here the same context that you specified in the SubPageView of the part
        if not PowerBIContextSettings.LockToSelectedElement then begin
            PowerBIContextSettings.LockToSelectedElement := true;
            PowerBIContextSettings.Modify();
        end;

        // Now, every part using the context 'ContosoFactbox' will be locked to whichever element was selected for it, 
        // including the one that we added on top of this file.
    end;
}