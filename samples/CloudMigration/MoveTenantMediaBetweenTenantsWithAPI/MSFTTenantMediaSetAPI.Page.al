page 58213 MSFTTenantMediaSetAPI
{
    PageType = API;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Tenant Media Set";
    EntityName = 'tenantMediaSet';
    EntitySetName = 'tenantMediaSet';
    APIPublisher = 'MSFT';
    APIGroup = 'moveData';
    APIVersion = 'v1.0';
    DelayedInsert = true;
    Permissions = tabledata "Tenant Media Set" = rmid;

    layout
    {
        area(Content)
        {
            repeater(TenantMedia)
            {
                field(id; Rec.ID)
                {
                    Caption = 'id';
                }
                field(mediaID; Rec."Media Index")
                {
                    Caption = 'Media Index';
                }

                field(base64ContentTxt; Base64ContentTxt)
                {
                    Caption = 'Base64ContentTxt';
                }

                field(companyName; Rec."Company Name")
                {
                    Caption = 'Company Name';
                    TableRelation = Company.Name;
                }

            }
        }
    }

    trigger OnInsertRecord(BelowxRec: Boolean): Boolean
    begin
        SetBase64Text();
        Rec.Insert();
        exit(false);
    end;

    trigger OnAfterGetRecord()
    begin
        GetBase64Text();
    end;

    local procedure GetBase64Text()
    var
        Base64Convert: Codeunit "Base64 Convert";
        ContentInStream: InStream;
        ContentOutStream: OutStream;
        TempBlob: Codeunit "Temp Blob";
    begin
        Clear(Base64ContentTxt);
        Rec.CalcFields("Media ID");
        if not Rec."Media ID".HasValue() then
            exit;

        TempBlob.CreateOutStream(ContentOutStream);
        Rec."Media ID".ExportStream(ContentOutStream);
        TempBlob.CreateInStream(ContentInStream);
        Base64ContentTxt := Base64Convert.ToBase64(ContentInStream);
    end;

    local procedure SetBase64Text()
    var
        Base64Convert: Codeunit "Base64 Convert";
        ContentInStream: InStream;
        ContentOutStream: OutStream;
        TempBlob: Codeunit "Temp Blob";
    begin
        TempBlob.CreateOutStream(ContentOutStream);
        Base64Convert.FromBase64(Base64ContentTxt, ContentOutStream);
        TempBlob.CreateInStream(ContentInStream);
        Rec."Media ID".ImportStream(ContentInStream, '');
    end;

    var
        Base64ContentTxt: text;
}