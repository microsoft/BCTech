
page 50950 PdfAttachments
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = Company;

    layout
    {
        area(Content)
        {
            group(GroupName)
            {
                field(Name; CompanyName)
                {
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action("LoadInvoiceXmlFromPdf - text")
            {
                trigger OnAction()
                var
                    PdfAttachmentManager: Codeunit PdfHelper;
                    PdfInStream: InStream;
                    PdfAttachmentStream: InStream;

                    PdfFile: File;
                    TempBlob: Codeunit "Temp Blob";
                    PdfAttachmentOutStream: OutStream;
                    XmlFileName: Text;
                begin
                    LoadFileResource();
                    PdfFile.Open(TestFileName);
                    PdfFile.CREATEINSTREAM(PdfInStream);
                    if not PdfAttachmentManager.GetInvoiceAttachmentStream(PdfInStream, TempBlob)
                    then
                        Message('No invoice attachment found in the PDF file.');

                    PdfFile.Close();
                    TempBlob.CreateInStream(PdfAttachmentStream);
                    Message(' Stream length = %1, %2', PdfAttachmentStream.Length, PdfAttachmentStream.Position);
                    PdfAttachmentManager.SaveFileContent(PdfAttachmentStream, PdfAttachmentManager.GetOutputPath() + 'Invoice.xml');
                    PdfAttachmentStream.ResetPosition();
                    XmlFileName := 'invoice.xml';
                    downloadFromStream(PdfAttachmentStream, 'xml file', '', '', XmlFileName);
                end;
            }
            action(SaveAllAttachments)
            {
                trigger OnAction()
                var
                    PdfAttachmentManager: Codeunit PdfHelper;
                    PdfInStream: InStream;
                    PdfAttachmentStream: InStream;
                    PdfFile: File;
                begin
                    LoadFileResource();
                    PdfFile.Open(TestFileName);
                    PdfFile.CREATEINSTREAM(PdfInStream);
                    PdfAttachmentManager.SaveAllAttachments(PdfInStream);
                    PdfFile.Close();
                end;
            }
            action(DownloadZipArchive)
            {
                trigger OnAction()
                var
                    PdfAttachmentManager: Codeunit PdfHelper;
                    PdfInStream: InStream;
                    PdfAttachmentStream: InStream;
                    PdfFile: File;
                begin
                    LoadFileResource();
                    PdfFile.Open(TestFileName);
                    PdfFile.CREATEINSTREAM(PdfInStream);
                    PdfAttachmentManager.GetZipArchive(PdfInStream);
                    PdfFile.Close();
                end;
            }
            action(ShowNames)
            {
                trigger OnAction()
                var
                    PdfAttachmentManager: Codeunit PdfHelper;
                    PdfInStream: InStream;
                    PdfAttachmentStream: InStream;
                    PdfFile: File;
                    attachments: Text;
                begin
                    LoadFileResource();
                    PdfFile.Open(TestFileName);
                    PdfFile.CREATEINSTREAM(PdfInStream);
                    attachments := PdfAttachmentManager.ShowNames(PdfInStream);
                    PdfFile.Close();
                    Message('Attachments: %1', attachments);
                end;
            }
            action(RunReportAndAttachFiles)
            {
                trigger OnAction()
                var
                    PdfAttachmentsReport: Report PdfAttachments;
                    PdfAttachmentManager: Codeunit PdfHelper;
                    PdfInStream: InStream;
                    PdfAttachmentStream: InStream;
                    PdfFilename: Text;
                    outputFile: Text;
                    PdfFile: File;
                begin
                    PdfFilename := 'PdfAttachmentTest.pdf';
                    PdfAttachmentsReport.SaveAsPdf(PdfFilename);

                    PdfFile.Open(PdfFilename);
                    PdfFile.CREATEINSTREAM(PdfInStream);
                    PdfAttachmentManager.GetZipArchive(PdfInStream);
                    PdfFile.Close();
                    outputFile := PdfFilename;
                    Download(PdfFilename, 'Download file', 'c:\temp\', 'Pdf File(*.pdf)|*.pdf', outputFile);
                end;
            }
        }
    }

    procedure LoadFileResource()
    var
        resourceStream: Instream;
        FileStream: OutStream;
        TestFile: File;
    begin
        TestFileName := TemporaryPath + '\' + TestResourceName;

        if File.Exists(TestFileName) then exit;

        NavApp.GetResource(TestResourceName, resourceStream);
        TestFile.CREATE(TestFileName);
        TestFile.CREATEOUTSTREAM(FileStream);
        CopyStream(FileStream, resourceStream);
        TestFile.Close();
    end;

    var
        TestFileName: Text;
        TestResourceName: TextConst ENU = 'XRECHNUNG_Elektron.pdf';
}

