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
    var
        PowerBIServiceMgt: Codeunit "Power BI Service Mgt.";
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

        PowerBIServiceMgt.AddReportVisualForContext('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565', 'ContosoFactbox');
    end;
}