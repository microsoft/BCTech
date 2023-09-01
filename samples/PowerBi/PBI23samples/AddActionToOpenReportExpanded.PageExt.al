pageextension 50100 AddActionToOpenReportExpanded extends "Customer List"
{
    actions
    {
        addfirst(processing)
        {
            action(OpenPBIReportVisualExpanded)
            {
                ApplicationArea = All;
                Caption = 'Open Power BI report visual (expanded)';
                Image = BarChart;

                trigger OnAction()
                var
                    TempPowerBIDisplayedElement: Record "Power BI Displayed Element" temporary;
                    PowerBIContextSettings: Record "Power BI Context Settings";
                    PowerBIServiceMgt: Codeunit "Power BI Service Mgt.";
                    PowerBIEmbedSetupWizard: Page "Power BI Embed Setup Wizard";
                    PowerBIElementCard: Page "Power BI Element Card";
                begin
                    // If you integrate with the Power BI embed framework, you as a partner are responsible for checking that the user has agreed to use the 
                    // Power BI integration. You can do it through this code:
                    PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
                    if PowerBIContextSettings.IsEmpty() then begin
                        PowerBIEmbedSetupWizard.SetContext('PartnerCreatedPage'); // This string is to identify where the user ran the wizard from. Use a unique Text with 1 to 30 characters.
                        if PowerBIEmbedSetupWizard.RunModal() in [Action::Cancel, Action::LookupCancel] then
                            Error(UserDidNotAcceptPowerBITermsErr);
                    end;

                    // After that, you can pass a record to Page "Power BI Element Card" to display it in an expanded context. The record can be temporary or not.
                    // For this example, the values are hardcoded, but you can implement your own logic to generate or retrieve these values.
                    TempPowerBIDisplayedElement.Init();
                    // For all element types, you need to specify the ElementType and the ElementId. Use the helper functions to make a key for your element type.
                    // The easiest way to find the IDs you need (and the ElementEmbedUrl) is to use the Power BI REST APIs. You can try them out from the 
                    // documentation page for the respective element type, for example for reports you can use the "Try it" button in this page: 
                    // https://learn.microsoft.com/en-us/rest/api/power-bi/reports/get-reports
                    // Report visuals are not available from the Power BI REST APIs at the moment, but you can find the report visual ID by opening the report
                    // in Power BI online, then clicking the three-dots menu on the top right corner of your chosen visual, and choose the action Share > Link to visual.
                    // The generated link will contain the visual ID in the URL parameters. 
                    // Also notice: unfortunately, the Power BI Javascript library does not throw an error if the visual ID does not exist, but just displays an empty page.
                    TempPowerBIDisplayedElement.ElementType := TempPowerBIDisplayedElement.ElementType::"Report Visual";
                    TempPowerBIDisplayedElement.ElementId := TempPowerBIDisplayedElement.MakeReportVisualKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565');
                    // You also need to specify the ElementEmbedUrl (Note: for report visuals, provide the embed URL of the report that contains the visual).
                    TempPowerBIDisplayedElement.ElementEmbedUrl := 'https://app.powerbi.com/reportEmbed?reportId=061ce0f5-3918-44ee-b820-a8d0d384fb2e&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMzLUEtUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOnRydWUsInVzYWdlTWV0cmljc1ZOZXh0Ijp0cnVlfX0%3d';
                    // Optionally, you can set the value of ShowPanesInExpandedMode, to control whether the report page selection and filter controls are shown for this element.
                    // If the ElementType is Report, you can also specify a ReportPage, and the report will open showing that specific page. This parameter is ignored for elements that are not reports.
                    TempPowerBIDisplayedElement.ReportPage := '';
                    TempPowerBIDisplayedElement.ShowPanesInExpandedMode := true;
                    TempPowerBIDisplayedElement.Insert();

                    PowerBIElementCard.SetDisplayedElement(TempPowerBIDisplayedElement);
                    PowerBIElementCard.Run();
                end;
            }
            action(OpenPBIReportExpanded)
            {
                ApplicationArea = All;
                Caption = 'Open Power BI report (expanded)';
                Image = "Report";

                trigger OnAction()
                var
                    TempPowerBIDisplayedElement: Record "Power BI Displayed Element" temporary;
                    PowerBIContextSettings: Record "Power BI Context Settings";
                    PowerBIServiceMgt: Codeunit "Power BI Service Mgt.";
                    PowerBIEmbedSetupWizard: Page "Power BI Embed Setup Wizard";
                    PowerBIElementCard: Page "Power BI Element Card";
                begin
                    PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
                    if PowerBIContextSettings.IsEmpty() then begin
                        PowerBIEmbedSetupWizard.SetContext('PartnerCreatedPage'); // This string is to identify where the user ran the wizard from. Use a unique Text with 1 to 30 characters.
                        if PowerBIEmbedSetupWizard.RunModal() in [Action::Cancel, Action::LookupCancel] then
                            Error(UserDidNotAcceptPowerBITermsErr);
                    end;

                    TempPowerBIDisplayedElement.Init();
                    TempPowerBIDisplayedElement.ElementType := TempPowerBIDisplayedElement.ElementType::"Report";
                    TempPowerBIDisplayedElement.ElementId := TempPowerBIDisplayedElement.MakeReportKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e');
                    TempPowerBIDisplayedElement.ElementEmbedUrl := 'https://app.powerbi.com/reportEmbed?reportId=061ce0f5-3918-44ee-b820-a8d0d384fb2e&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMzLUEtUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOnRydWUsInVzYWdlTWV0cmljc1ZOZXh0Ijp0cnVlfX0%3d';
                    TempPowerBIDisplayedElement.ReportPage := 'ReportSection1';
                    TempPowerBIDisplayedElement.ShowPanesInExpandedMode := true;
                    TempPowerBIDisplayedElement.Insert();

                    PowerBIElementCard.SetDisplayedElement(TempPowerBIDisplayedElement);
                    PowerBIElementCard.Run();
                end;
            }

            action(OpenPBIDashboardExpanded)
            {
                ApplicationArea = All;
                Caption = 'Open Power BI dashboard (expanded)';
                Image = Worksheet;

                trigger OnAction()
                var
                    TempPowerBIDisplayedElement: Record "Power BI Displayed Element" temporary;
                    PowerBIContextSettings: Record "Power BI Context Settings";
                    PowerBIServiceMgt: Codeunit "Power BI Service Mgt.";
                    PowerBIEmbedSetupWizard: Page "Power BI Embed Setup Wizard";
                    PowerBIElementCard: Page "Power BI Element Card";
                begin
                    PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
                    if PowerBIContextSettings.IsEmpty() then begin
                        PowerBIEmbedSetupWizard.SetContext('PartnerCreatedPage'); // This string is to identify where the user ran the wizard from. Use a unique Text with 1 to 30 characters.
                        if PowerBIEmbedSetupWizard.RunModal() in [Action::Cancel, Action::LookupCancel] then
                            Error(UserDidNotAcceptPowerBITermsErr);
                    end;

                    TempPowerBIDisplayedElement.Init();
                    TempPowerBIDisplayedElement.ElementType := TempPowerBIDisplayedElement.ElementType::Dashboard;
                    TempPowerBIDisplayedElement.ElementId := TempPowerBIDisplayedElement.MakeDashboardKey('4cdaab93-3758-4781-b5eb-95b0b4862529');
                    TempPowerBIDisplayedElement.ElementEmbedUrl := 'https://app.powerbi.com/dashboardEmbed?dashboardId=4cdaab93-3758-4781-b5eb-95b0b4862529&appId=0315d8b4-8b02-4a3f-9f57-04e592a4b204&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMzLUEtUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOmZhbHNlfX0%3d';
                    TempPowerBIDisplayedElement.ReportPage := '';
                    TempPowerBIDisplayedElement.ShowPanesInExpandedMode := true;
                    TempPowerBIDisplayedElement.Insert();

                    PowerBIElementCard.SetDisplayedElement(TempPowerBIDisplayedElement);
                    PowerBIElementCard.Run();
                end;
            }

            action(OpenPBIDashboardTileExpanded)
            {
                ApplicationArea = All;
                Caption = 'Open Power BI dashboard tile (expanded)';
                Image = NumberGroup;

                trigger OnAction()
                var
                    TempPowerBIDisplayedElement: Record "Power BI Displayed Element" temporary;
                    PowerBIContextSettings: Record "Power BI Context Settings";
                    PowerBIServiceMgt: Codeunit "Power BI Service Mgt.";
                    PowerBIEmbedSetupWizard: Page "Power BI Embed Setup Wizard";
                    PowerBIElementCard: Page "Power BI Element Card";
                begin
                    PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
                    if PowerBIContextSettings.IsEmpty() then begin
                        PowerBIEmbedSetupWizard.SetContext('PartnerCreatedPage'); // This string is to identify where the user ran the wizard from. Use a unique Text with 1 to 30 characters.
                        if PowerBIEmbedSetupWizard.RunModal() in [Action::Cancel, Action::LookupCancel] then
                            Error(UserDidNotAcceptPowerBITermsErr);
                    end;

                    TempPowerBIDisplayedElement.Init();
                    TempPowerBIDisplayedElement.ElementType := TempPowerBIDisplayedElement.ElementType::"Dashboard Tile";
                    TempPowerBIDisplayedElement.ElementId := TempPowerBIDisplayedElement.MakeDashboardTileKey('4cdaab93-3758-4781-b5eb-95b0b4862529', '8627a5c5-3b28-4873-9ab0-c7294d9cab5b');
                    TempPowerBIDisplayedElement.ElementEmbedUrl := 'https://app.powerbi.com/embed?dashboardId=4cdaab93-3758-4781-b5eb-95b0b4862529&tileId=8627a5c5-3b28-4873-9ab0-c7294d9cab5b&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMzLUEtUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOmZhbHNlfX0%3d';
                    TempPowerBIDisplayedElement.ReportPage := '';
                    TempPowerBIDisplayedElement.ShowPanesInExpandedMode := true;
                    TempPowerBIDisplayedElement.Insert();

                    PowerBIElementCard.SetDisplayedElement(TempPowerBIDisplayedElement);
                    PowerBIElementCard.Run();
                end;
            }
        }


        addfirst(Promoted)
        {
            group(PowerBIDemoActions)
            {
                Caption = 'Power BI demo actions';
                actionref(OpenPBIReportExpandedPromoted; OpenPBIReportExpanded)
                {
                }

                actionref(OpenPBIReportVisualExpandedPromoted; OpenPBIReportVisualExpanded)
                {
                }

                actionref(OpenPBIReportDashboardPromoted; OpenPBIDashboardExpanded)
                {
                }

                actionref(OpenPBIDashboardTileExpandedPromoted; OpenPBIDashboardTileExpanded)
                {
                }
            }
        }
    }

    var
        UserDidNotAcceptPowerBITermsErr: Label 'We cannot perform this action, because you havent''t set up the Power BI integration.';
}