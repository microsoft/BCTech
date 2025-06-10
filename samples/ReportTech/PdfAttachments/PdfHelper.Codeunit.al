codeunit 50950 PdfHelper
{
    trigger OnRun()
    begin
        Init();
    end;

    var
        TempFolderPath: Text;
        Initialized: Boolean;
        ValidNamesTxt: TextConst ENU = 'factur-x.xml, xrechnung.xml, zugferd-invoice.xml';

    procedure Init()
    begin
        if Initialized then exit;

        TempFolderPath := GetOutputPath();

        Initialized := true;
    end;

    procedure GetOutputPath(): text
    begin
        //exit('c:\temp\PdfTest\'); // Use this path for local fast examination of the caputured files.)
        exit(System.TemporaryPath);
    end;

    procedure SanitizeFilename(FileName: Text): Text
    begin
        FileName := FileName.Replace('/', '_');
        FileName := FileName.Replace('\', '_');
        exit(FileName);
    end;

    procedure GetInvoiceAttachmentStream(pdfStream: InStream; TempBlob: Codeunit "Temp Blob"): Boolean
    var
        PdfAttachmentManager: DotNet PdfAttachmentManager;
        MemoryStream: DotNet MemoryStream;
        name: Text;
        PdfAttachmentOutStream: OutStream;
        PdfAttachmentInStream: InStream;
    begin
        TempBlob.CreateOutStream(PdfAttachmentOutStream);
        TempBlob.CreateInStream(PdfAttachmentInStream);

        PdfAttachmentManager := PdfAttachmentManager.PdfAttachmentManager(pdfStream);

        // Try to get the invoice attachment stored in the pdf xml metedata
        MemoryStream := PdfAttachmentManager.GetInvoiceAttachment('');

        if IsNull(MemoryStream) then begin
            // xmp did not register the attachment name, try to get the attachment by name
            // using a list of valid names from the E-Invoiceing standard.
            name := ValidNamesTxt;
            MemoryStream := PdfAttachmentManager.GetInvoiceAttachment('');
        end;

        if IsNull(MemoryStream)
            then
            exit(false);

        MemoryStream.Position := 0;
        MemoryStream.CopyTo(PdfAttachmentOutStream);
        exit(true);
    end;

    procedure SaveAllAttachments(pdfStream: InStream)
    var
        PdfAttachmentManager: DotNet PdfAttachmentManager;
        PdfAttachment: DotNet PdfAttachment;
        AttachmentStream: InStream;
        AttachmentName: Text;
    begin
        PdfAttachmentManager := PdfAttachmentManager.PdfAttachmentManager(pdfStream);
        foreach PdfAttachment in PdfAttachmentManager
        do begin
            AttachmentName := PdfAttachment.Name;
            AttachmentStream := PdfAttachment.Contents;

            SaveFileContent(AttachmentStream, GetOutputPath() + AttachmentName);
        end;
        Message('All attachments saved to %1', GetOutputPath());
    end;

    procedure GetZipArchive(pdfStream: InStream)
    var
        PdfAttachmentManager: DotNet PdfAttachmentManager;
        AttachmentStream: InStream;
        ZipFilename: Text;
    begin
        PdfAttachmentManager := PdfAttachmentManager.PdfAttachmentManager(pdfStream);
        AttachmentStream := PdfAttachmentManager.GetZipArchiveWithAttachments();

        ZipFilename := 'Attachments.zip';
        DownloadFromStream(AttachmentStream, 'zip file', '', '', ZipFilename);

    end;

    procedure ShowNames(pdfStream: InStream): text
    var
        PdfAttachmentManager: DotNet PdfAttachmentManager;
        PdfAttachment: DotNet PdfAttachment;
        names: Text;
        name: Text;
    begin
        PdfAttachmentManager := PdfAttachmentManager.PdfAttachmentManager(pdfStream);
        names := '';

        foreach PdfAttachment in PdfAttachmentManager
        do begin
            name := PdfAttachment.Name;
            if (name = '') then
                continue;
            if names <> '' then
                names += ', ';
            names += name;
        end;
        exit(names)
    end;

    local procedure GetPdfPropoerties(DocumentStream: InStream): JsonObject
    var
        PdfDocumentInfoInstance: DotNet PdfDocumentInfo;
        PdfConverterInstance: DotNet PdfConverter;
        PdfTargetDevice: DotNet PdfTargetDevice;
        MemoryStream: DotNet MemoryStream;
        TextValue: text;
        IntegerValue: Integer;
        DecimalValue: Decimal;
        DateTimeValue: DateTime;
        DurationValue: Duration;
        JsonContainer: JsonObject;
    begin
        MemoryStream := MemoryStream.MemoryStream();
        MemoryStream := DocumentStream;

        PdfConverterInstance := PdfConverterInstance.PdfConverter(MemoryStream);
        PdfDocumentInfoInstance := PdfConverterInstance.DocumentInfo();

        DecimalValue := PdfDocumentInfoInstance.PageWidth;
        JsonContainer.Add('pageWidth', DecimalValue);

        DecimalValue := PdfDocumentInfoInstance.PageHeight;
        JsonContainer.Add('pageHeight', DecimalValue);

        IntegerValue := PdfDocumentInfoInstance.PageCount;
        JsonContainer.Add('pagecount', IntegerValue);

        TextValue := PdfDocumentInfoInstance.Author;
        JsonContainer.Add('author', TextValue);

        DateTimeValue := PdfDocumentInfoInstance.CreationDate;
        JsonContainer.Add('creationDate', DateTimeValue);

        DurationValue := PdfDocumentInfoInstance.CreationTimeZone;
        JsonContainer.Add('creationTimeZone', DurationValue);

        TextValue := PdfDocumentInfoInstance.Creator;
        JsonContainer.Add('creator', TextValue);

        TextValue := PdfDocumentInfoInstance.Producer;
        JsonContainer.Add('producer', TextValue);

        TextValue := PdfDocumentInfoInstance.Subject;
        JsonContainer.Add('subject', TextValue);

        TextValue := PdfDocumentInfoInstance.Title;
        JsonContainer.Add('title', TextValue);
        exit(JsonContainer);
    end;

    procedure SaveFileContent(var DocumentStream: InStream; FileName: Text)
    var
        FileObject: File;
        DocumentOutStream: OutStream;
    begin
        FileObject.CREATE(FileName);
        FileObject.CREATEOUTSTREAM(DocumentOutStream);
        CopyStream(DocumentOutStream, DocumentStream);
        FileObject.CLOSE;
    end;
}
