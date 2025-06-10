reportextension 50952 SalesInvoiceWithPdf extends "Standard Sales - Invoice"
{
    dataset
    {
        modify(Header)
        {
            trigger OnAfterAfterGetRecord()
            begin

            end;
        }
    }

    requestpage
    {
        layout
        {
            addafter(Options)
            {
                field("Number of Attachments"; NumberOfAttachments)
                {
                    ApplicationArea = All;
                }
                field("Number of Files to Append"; NumberOfFilesToAppend)
                {
                    ApplicationArea = All;
                }
                field("User Code"; UserCode)
                {
                    ApplicationArea = All;
                }
                field("Admin Code"; AdminCode)
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    trigger OnPreReport()
    var
        i: Integer;
        FileName: Text;
        Name: Text;
        MimeType: Text;
        Description: Text;
        FileObject: File;
        DataType: enum PdfAttachmentDataRelationShip;
    begin
        // Initialize the E-invoicing xml document if needed.        
        if CurrReport.TargetFormat = ReportFormat::PDF then begin
            // Add the legal documents to append to the report
            clear(ReportRenderingCompleteHandlerInstance);
            // HINT: The following code is for demonstration purposes only. In a real-world scenario, the user and admin codes should be stored securely,
            // and the pdf to append or attach have to be generated bassed on information captured in the OnAfterAfterGetRecord trigger on the iterator data item.

            for i := 1 to NumberOfFilesToAppend do begin
                ReportRenderingCompleteHandlerInstance.AddFilesToAppend(CreatePdfFile('LegalDocumentToAppend' + format(i) + '.pdf', 'This is legal document ' + format(i)));
            end;

            for i := 1 to NumberOfAttachments do begin
                DataType := PdfAttachmentDataRelationShip::Data;
                Name := 'Attachment' + format(i) + '.pdf';
                FileName := CreatePdfFile(Name, 'Attachment Content index' + format(i));
                MimeType := 'application/pdf';
                Description := 'This is attachment ' + format(i);

                ReportRenderingCompleteHandlerInstance.AddAttachment(Name, DataType, MimeType, FileName, Description, false);
            end;

            ReportRenderingCompleteHandlerInstance.ProtectDocument(UserCode, AdminCode);
        end;
    end;

    trigger OnPreRendering(var RenderingPayload: JsonObject)
    var
        FileName: Text;
        Name: Text;
        MimeType: Text;
        Description: Text;
        IsPrimaryDocument: Boolean;
        DataType: enum PdfAttachmentDataRelationShip;
    begin
        if CurrReport.TargetFormat <> ReportFormat::PDF then
            exit;

        // HINT: The following code is for demonstration purposes only. In a real-world scenario the xml file has to be generated bassed on 
        // information captured in the OnAfterAfterGetRecord trigger on the iterator data item.

        // Configure the rendering complete handler
        Name := 'factur-x.xml';
        FileName := CreateXmlFile(Name, 'Invoice content');
        DataType := PdfAttachmentDataRelationShip::Alternative;
        MimeType := 'text/xml';
        Description := 'This is the e-invoicing xml document';
        IsPrimaryDocument := true;
        ReportRenderingCompleteHandlerInstance.AddAttachment(Name, DataType, MimeType, FileName, Description, IsPrimaryDocument);

        RenderingPayload := ReportRenderingCompleteHandlerInstance.ToJson(RenderingPayload);
    end;

    local procedure CreatePdfFile(Filename: Text; Content: Text) FilePath: Text
    var
        DocPro: DotNet WordHandler;
        File: File;
        OutStream: OutStream;
    begin
        FilePath := System.TemporaryPath + Filename;
        FilePath := DocPro.CreateSamplePdfDocument(FilePath, Content)
    end;

    local procedure CreateXmlFile(Filename: Text; Content: Text) FilePath: Text
    var
        FileObject: File;
        OutStream: OutStream;
    begin
        FilePath := System.TemporaryPath + Filename;
        FileObject.TextMode := true;
        FileObject.Create(FilePath);
        FileObject.CreateOutStream(OutStream);
        OutStream.WriteText('<?xml version="1.0" encoding="utf-8" standalone="yes"?>');
        OutStream.WriteText('<root><test>');
        OutStream.WriteText(Content);
        OutStream.WriteText('</test></root>');
        FileObject.Close();
    end;

    var
        ReportRenderingCompleteHandlerInstance: Codeunit ReportRenderingCompleteHandler;
        NumberOfAttachments: Integer;
        NumberOfFilesToAppend: Integer;
        UserCode: Text;
        AdminCode: Text;
}