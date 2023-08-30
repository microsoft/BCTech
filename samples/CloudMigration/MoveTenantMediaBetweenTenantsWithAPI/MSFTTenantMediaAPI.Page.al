page 58211 MSFTTenantMediaAPI
{
    PageType = API;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Tenant Media";
    EntityName = 'tenantMedia';
    EntitySetName = 'tenantMedia';
    APIPublisher = 'MSFT';
    APIGroup = 'moveData';
    APIVersion = 'v1.0';
    DelayedInsert = true;
    Permissions = tabledata "Tenant Media" = rmid;

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
                field(description; Rec.Description)
                {
                    Caption = 'Description';
                }

                field(base64ContentTxt; Base64ContentTxt)
                {
                    Caption = 'Base64ContentTxt';
                }
                field(mimeType; Rec."Mime Type")
                {
                    Caption = 'Mime Type';
                }
                field(height; Rec.Height)
                {
                    Caption = 'Height';
                }
                field(width; Rec.Width)
                {
                    Caption = 'Width';
                }
                field(companyName; Rec."Company Name")
                {
                    Caption = 'Company Name';
                    TableRelation = Company.Name;
                }
                field(expirationDate; Rec."Expiration Date")
                {
                    Caption = 'Expiration Date';
                }
                field(prohibitCache; Rec."Prohibit Cache")
                {
                    Caption = 'Prohibit Cache';
                }
                field(fileName; Rec."File Name")
                {
                    Caption = 'File Name';
                }
                field(securityToken; Rec."Security Token")
                {
                    Caption = 'Security Token';
                }
                field(creatingUser; Rec."Creating User")
                {
                    Caption = 'Creating User';
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
    begin
        Clear(Base64ContentTxt);
        Rec.CalcFields(Content);
        if not Rec.Content.HasValue() then
            exit;

        Rec.Content.CreateInStream(ContentInStream);
        Base64ContentTxt := Base64Convert.ToBase64(ContentInStream);
    end;

    local procedure SetBase64Text()
    var
        Base64Convert: Codeunit "Base64 Convert";
        ContentOutStream: OutStream;
    begin
        Rec.Content.CreateOutStream(ContentOutStream);
        Base64Convert.FromBase64(Base64ContentTxt, ContentOutStream);
    end;

    var
        Base64ContentTxt: text;
}