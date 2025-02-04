page 52800 "Sales Order Data"
{
    PageType = List;
    SourceTable = "Sales Orders";

    layout
    {
        area(content)
        {
            repeater(Group)
            {
                field(Id; Rec.Id)
                {
                    ApplicationArea = Basic;
                }
                field(Description; Rec.Description)
                {
                    ApplicationArea = Basic;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action("Init Data")
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    SalesOrderCU: Codeunit "Sales Order";
                begin
                    SalesOrderCU.InitData(3, 4, 4);
                end;
            }
            action("Init Conditional Data")
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    SalesOrderCU: Codeunit "Sales Order";
                begin
                    SalesOrderCU.InitConditionalData(3, 4, 4);
                end;
            }
            action("Reset Selection")
            {
                ApplicationArea = All;
                trigger OnAction()
                begin
                    ClearSelection(Report::"Sales Order");
                end;
            }
        }
        area(Reporting)
        {
            action("Run Save As - Sales Order - PDF")
            {
                ApplicationArea = All;
                trigger OnAction()
                begin
                    SetSelection(Report::"Sales Order", ReportLayoutType::Word, '');
                    RunReportWithFormatStream(Report::"Sales Order", ReportFormat::Pdf, 'SalesOrder.pdf');
                end;
            }
            action("Run Save As - Sales Order List - PDF")
            {
                ApplicationArea = All;
                trigger OnAction()
                begin
                    SetSelection(Report::"Sales Order - List", ReportLayoutType::Word, '');
                    RunReportWithFormatStream(Report::"Sales Order - List", ReportFormat::Pdf, 'SalesOrderList.pdf');
                end;
            }
            action("Run Save As - Sales Order - Word (WordLayout)")
            {
                ApplicationArea = All;
                trigger OnAction()
                begin
                    SetSelection(Report::"Sales Order", ReportLayoutType::Word, '');
                    RunReportWithFormatStream(Report::"Sales Order", ReportFormat::Word, 'SalesOrderWord.docx');
                end;
            }
            action("Run Save As - Sales Order List - Word (WordLayout)")
            {
                ApplicationArea = All;
                trigger OnAction()
                begin
                    SetSelection(Report::"Sales Order - List", ReportLayoutType::Word, '');
                    RunReportWithFormatStream(Report::"Sales Order - List", ReportFormat::Word, 'SalesOrderListWord.docx');
                end;
            }
        }
    }

    trigger OnOpenPage()
    begin
    end;

    procedure SetSelection(ReportId: Integer; layoutFormat: ReportLayoutType; Name: Text)
    var
        ReportLayoutSelection: Record "Tenant Report Layout Selection";
        ReportLayout: Record "Report Layout List";

    begin
        ReportLayout.SetFilter(ReportLayout."Report ID", format(ReportId));
        ReportLayout.SetFilter(ReportLayout."Layout Format", format(layoutFormat));
        if (Name <> '') then
            ReportLayout.SetFilter(ReportLayout.Name, Name);

        if not ReportLayout.FindSet() then
            error('No layout of type %1', layoutFormat);

        ReportLayoutSelection.Init();
        ReportLayoutSelection."Report ID" := ReportLayout."Report ID";
        ReportLayoutSelection."App ID" := ReportLayout."Application ID";
        ReportLayoutSelection."Layout Name" := ReportLayout.Name;

        if not ReportLayoutSelection.Insert(false) then
            ReportLayoutSelection.Modify(false);
    end;

    procedure ClearSelection(ReportId: Integer)
    var
        ReportLayoutSelection: Record "Tenant Report Layout Selection";
    begin
        ReportLayoutSelection.SetFilter(ReportLayoutSelection."Report ID", format(ReportId));
        ReportLayoutSelection.DeleteAll();
    end;

    procedure RunReportRunAction(UseRequestPage: Boolean)
    var
        reportObject: Report "Sales Order";
    begin
        reportObject.UseRequestPage := UseRequestPage;
        reportObject.Run();
    end;

    procedure RunReportWithFormatStream(ReportId: Integer; TargetFormat: ReportFormat; StreamName: text)
    var
        ReportTargetStream: OutStream;
        ReportInTargetStream: InStream;
        TempBlob: Codeunit "Temp Blob";
    begin
        TempBlob.CreateInStream(ReportInTargetStream);
        TempBlob.CreateOutStream(ReportTargetStream);
        Report.SaveAs(ReportId, '', TargetFormat, ReportTargetStream);
        SaveStream(ReportInTargetStream, StreamName);
    end;

    procedure SaveStream(DocumentStream: InStream; Path: Text)
    begin
        DownloadFromStream(DocumentStream, 'Report target', '', '', Path);
    end;
}
