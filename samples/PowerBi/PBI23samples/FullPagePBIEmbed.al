page 50110 PBIHost

{
    AboutTitle = 'Sales Insights (overview)';
    AboutText = 'The sales insights overview page shows you all the possible ways you can analyze your sales, e.g. Sales summary, Sales pipeline, Sales leaderboard, Sales activity';

    layout
    {
        area(Content)
        {
            part("partname"; "Power BI Embedded Report Part")
            {
                Caption = 'Sales overview';
                SubPageView = where(Context = const('LockedCustomerReport'));
            }
        }
    }


    trigger OnOpenPage()
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
        // More information on how to set these parameters in file 'AddActionToOpenReportExpanded.PageExt.al'.
        if not PowerBIDisplayedElement.Get(UserSecurityId(), 'LockedCustomerReport', PowerBIDisplayedElement.MakeReportVisualKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565'), PowerBIDisplayedElement.ElementType::"Report Visual") then begin
            PowerBIDisplayedElement.Init();
            PowerBIDisplayedElement.ElementType := PowerBIDisplayedElement.ElementType::"Report Visual";
            PowerBIDisplayedElement.ElementId := PowerBIDisplayedElement.MakeReportVisualKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565');
            PowerBIDisplayedElement.ElementEmbedUrl := 'https://app.powerbi.com/reportEmbed?reportId=061ce0f5-3918-44ee-b820-a8d0d384fb2e&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMzLUEtUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOnRydWUsInVzYWdlTWV0cmljc1ZOZXh0Ijp0cnVlfX0%3d';
            PowerBIDisplayedElement.Context := 'LockedCustomerReport'; // Use here the same context that you specified in the SubPageView of the part;
            PowerBIDisplayedElement.UserSID := UserSecurityId();
            PowerBIDisplayedElement.ShowPanesInExpandedMode := false;
            PowerBIDisplayedElement.ShowPanesInNormalMode := false;
            PowerBIDisplayedElement.Insert();
        end;

        // In PowerBIContextSettings, you can ensure that the controls to change report are disabled for this context.
        PowerBIContextSettings.CreateOrReadForCurrentUser('LockedCustomerReport'); // Use here the same context that you specified in the SubPageView of the part
        if not PowerBIContextSettings.LockToSelectedElement then begin
            PowerBIContextSettings.LockToSelectedElement := true;
            PowerBIContextSettings.Modify();
        end;

        // Now, every part using the context 'LockedCustomerReport' will be locked to whichever element was selected for it, 
        // including the one that we added on top of this file.
    end;
}
