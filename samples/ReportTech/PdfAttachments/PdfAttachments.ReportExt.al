//using System;

reportextension 50953 PdfAttachmentsExt extends PdfAttachments
{
    rendering
    {
    }

    trigger OnPreReport()
    var
        UserCode: SecretText;
        AdminCode: SecretText;
        i: Integer;
        FileName: Text;
        Name: Text;
        MimeType: Text;
        Description: Text;
        FileObject: File;
        OutStream: OutStream;
        DataType: enum PdfAttachmentDataRelationShip;
    begin
        // Add your code here
        // Initialize the E-invoicing xml document if needed.        
        // Add the legal documents to append to the report
        clear(ReportRenderingHandlerUTExtension);
        ReportRenderingHandlerUTExtension.AddFilesToAppend(CreatePdfFile('ExtDocumentToAppend1.pdf', 'This is legal document 1 from extetension'));
        ReportRenderingHandlerUTExtension.AddFilesToAppend(CreatePdfFile('ExtDocumentToAppend2.pdf', 'This is legal document 2 from extetension'));

        for i := 1 to 3 do begin
            DataType := PdfAttachmentDataRelationShip::Data;
            Name := 'AttachmentExt' + format(i) + '.pdf';
            FileName := CreatePdfFile(Name, 'Attachment Extension Content index' + format(i));
            MimeType := 'application/pdf';
            Description := 'This is ext attachment ' + format(i);

            ReportRenderingHandlerUTExtension.AddAttachment(Name, DataType, MimeType, FileName, Description, false);
        end;
    end;

    trigger OnPreRendering(var RenderingPayload: JsonObject)
    begin
        this.OnRenderingCompleteJsonProc1(RenderingPayload);
    end;

    local procedure CreatePdfFile(Filename: Text; Content: Text) FilePath: Text
    var
        DocPro: DotNet WordTransformation;
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
        OutStream.Write('<?xml version="1.0" encoding="utf-8" standalone="yes"?>');
        OutStream.Write('<root><test>');
        OutStream.Write(Content);
        OutStream.Write('</test></root>');
        FileObject.Close();
    end;

    //[NonDebuggable]
    local procedure OnRenderingCompleteJsonProc1(var RenderingPayload: JsonObject)
    var
        UserCode: SecretText;
        AdminCode: SecretText;
        i: Integer;
        FileName: Text;
        Name: Text;
        MimeType: Text;
        Description: Text;
        DataType: enum PdfAttachmentDataRelationShip;
    begin
        // Configure the rendering complete handler

        if EnableXmlAttachment then begin
            Name := 'factur-x.xml';
            FileName := CreateXmlFile(Name, 'This is my xml file');
            DataType := PdfAttachmentDataRelationShip::Alternative;
            MimeType := 'text/xml';
            Description := 'This is the e-invoicing xml document';

            ReportRenderingHandlerUTExtension.AddAttachment(Name, DataType, MimeType, FileName, Description, true);
        end;

        RenderingPayload := ReportRenderingHandlerUTExtension.ToJson(RenderingPayload);
    end;

    var
        ReportRenderingHandlerUTExtension: Codeunit ReportRenderingCompleteHandler;
        EnableXmlAttachment: Boolean;

}
