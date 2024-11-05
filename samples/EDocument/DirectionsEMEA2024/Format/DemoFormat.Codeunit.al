namespace DefaultPublisher.EDocDemo;

using Microsoft.eServices.EDocument;
using Microsoft.Sales.Document;
using Microsoft.Sales.Peppol;
using System.Utilities;
using Microsoft.eServices.EDocument.IO.Peppol;
using System.IO;
using Microsoft.Purchases.Document;

codeunit 50100 "Demo Format" implements "E-Document"
{
    InherentEntitlements = X;
    InherentPermissions = X;

    procedure Check(var SourceDocumentHeader: RecordRef; EDocumentService: Record "E-Document Service"; EDocumentProcessingPhase: enum "E-Document Processing Phase")
    var
        SalesHeader: Record "Sales Header";
    begin

        // What do you need to do:
        // Validate that document contains fields data needed for your export.
        // Example "External Document No."

        case SourceDocumentHeader."Number" of
            Database::"Sales Header":
                case EDocumentProcessingPhase of
                    EDocumentProcessingPhase::Release,
                    EDocumentProcessingPhase::Post:
                        begin
                            SourceDocumentHeader.Field(SalesHeader.FieldNo("External Document No.")).TestField();
                        end;
                end;
        end;
    end;

    procedure Create(EDocumentService: Record "E-Document Service"; var EDocument: Record "E-Document"; var SourceDocumentHeader: RecordRef; var SourceDocumentLines: RecordRef; var TempBlob: codeunit "Temp Blob")
    var
        DocOutStream: OutStream;
    begin

        // What do you need to do:
        // Take SourceDocumentHeader, SourceDocumentLines and transform that into a blob. 
        // Example Data -> XML -> Blob 

        TempBlob.CreateOutStream(DocOutStream);
        case EDocument."Document Type" of
            EDocument."Document Type"::"Sales Invoice",
            EDocument."Document Type"::"Service Invoice":
                GenerateInvoiceXMLFile(SourceDocumentHeader, DocOutStream);
            EDocument."Document Type"::"Sales Credit Memo",
            EDocument."Document Type"::"Service Credit Memo":
                GenerateCrMemoXMLFile(SourceDocumentHeader, DocOutStream);
        end;

    end;


    procedure CreateBatch(EDocumentService: Record "E-Document Service"; var EDocuments: Record "E-Document"; var SourceDocumentHeaders: RecordRef; var SourceDocumentsLines: RecordRef; var TempBlob: codeunit System.Utilities."Temp Blob")
    begin

    end;

    procedure GetBasicInfoFromReceivedDocument(var EDocument: Record "E-Document"; var TempBlob: codeunit "Temp Blob")
    var
        TempXMLBuffer: Record "XML Buffer" temporary;
        ImportPeppol: Codeunit "EDoc Import PEPPOL BIS 3.0";
        DocStream: InStream;
    begin
        // What do you need to do:
        // Read the data and parse it into an e-document
        // Header information and document meta data

        // TempBlob.CreateInStream(DocStream);
        // TempXMLBuffer.LoadFromStream(DocStream);
        // EDocument.Direction := EDocument.Direction::Incoming;
        // EDocument."Document Type" := EDocument."Document Type"::"Purchase Invoice";
        // EDocument."Incoming E-Document No." := CopyStr(GetNodeByPath(TempXMLBuffer, '/Invoice/cbc:ID'), 1, MaxStrLen(EDocument."Document No."));
        // // Complete implementation
        ImportPeppol.ParseBasicInfo(EDocument, TempBlob);

    end;

    procedure GetCompleteInfoFromReceivedDocument(var EDocument: Record "E-Document"; var CreatedDocumentHeader: RecordRef; var CreatedDocumentLines: RecordRef; var TempBlob: codeunit System.Utilities."Temp Blob")
    var
        TempPurchaseHeader: Record "Purchase Header" temporary;
        TempPurchaseLine: Record "Purchase Line" temporary;
        ImportPeppol: Codeunit "EDoc Import PEPPOL BIS 3.0";
    begin
        // What do you need to do:
        // Read the data and parse it into an e-document
        // Lines can be populated

        ImportPeppol.ParseCompleteInfo(EDocument, TempPurchaseHeader, TempPurchaseLine, TempBlob);
        CreatedDocumentHeader.GetTable(TempPurchaseHeader);
        CreatedDocumentLines.GetTable(TempPurchaseLine);

    end;


    //
    // Helper functions taken from PEPPOL 3.0 implementation. 
    //

    local procedure GetNodeByPath(var TempXMLBuffer: Record "XML Buffer" temporary; XPath: Text): Text
    begin
        TempXMLBuffer.Reset();
        TempXMLBuffer.SetRange(Type, TempXMLBuffer.Type::Element);
        TempXMLBuffer.SetRange(Path, XPath);

        if TempXMLBuffer.FindFirst() then
            exit(TempXMLBuffer.Value);
    end;


    local procedure GenerateInvoiceXMLFile(VariantRec: Variant; var OutStr: OutStream)
    var
        SalesInvoicePEPPOLBIS30: XMLport "Sales Invoice - PEPPOL BIS 3.0";
    begin
        SalesInvoicePEPPOLBIS30.Initialize(VariantRec);
        SalesInvoicePEPPOLBIS30.SetDestination(OutStr);
        SalesInvoicePEPPOLBIS30.Export();
    end;

    local procedure GenerateCrMemoXMLFile(VariantRec: Variant; var OutStr: OutStream)
    var
        SalesCrMemoPEPPOLBIS30: XMLport "Sales Cr.Memo - PEPPOL BIS 3.0";
    begin
        SalesCrMemoPEPPOLBIS30.Initialize(VariantRec);
        SalesCrMemoPEPPOLBIS30.SetDestination(OutStr);
        SalesCrMemoPEPPOLBIS30.Export();
    end;

    [EventSubscriber(ObjectType::Table, Database::"E-Document Service", 'OnAfterValidateEvent', 'Document Format', false, false)]
    local procedure OnAfterValidateDocumentFormat(var Rec: Record "E-Document Service"; var xRec: Record "E-Document Service"; CurrFieldNo: Integer)
    var
        EDocServiceSupportedType: Record "E-Doc. Service Supported Type";
    begin
        if Rec."Document Format" = Rec."Document Format"::"PEPPOL BIS 3.0" then begin
            EDocServiceSupportedType.SetRange("E-Document Service Code", Rec.Code);
            if EDocServiceSupportedType.IsEmpty() then begin
                EDocServiceSupportedType.Init();
                EDocServiceSupportedType."E-Document Service Code" := Rec.Code;
                EDocServiceSupportedType."Source Document Type" := EDocServiceSupportedType."Source Document Type"::"Sales Invoice";
                EDocServiceSupportedType.Insert();

                EDocServiceSupportedType."Source Document Type" := EDocServiceSupportedType."Source Document Type"::"Sales Credit Memo";
                EDocServiceSupportedType.Insert();

                EDocServiceSupportedType."Source Document Type" := EDocServiceSupportedType."Source Document Type"::"Service Invoice";
                EDocServiceSupportedType.Insert();

                EDocServiceSupportedType."Source Document Type" := EDocServiceSupportedType."Source Document Type"::"Service Credit Memo";
                EDocServiceSupportedType.Insert();
            end;
        end;
    end;

}
