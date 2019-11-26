table 50001 "Questionnaire OCR Setup"
{
    DataClassification = CustomerContent;

    fields
    {
        field(1; "Primary Key"; Code[10])
        {
            DataClassification = SystemMetadata;

        }
        field(2; "Profile Code"; code[20])
        {
            DataClassification = CustomerContent;
            Caption = 'Profile Code';
            TableRelation = "Profile Questionnaire Header";
            ValidateTableRelation = true;
        }
        field(3; "OCR Url"; text[200])
        {
            DataClassification = CustomerContent;
            Caption = 'OCR Url';
        }
        field(4; "OCR Subscription Key"; text[50])
        {
            DataClassification = CustomerContent;
            Caption = 'OCR Subscription Key';
        }
    }

    keys
    {
        key(key1; "Primary Key")
        {
            Clustered = true;
        }
    }


    trigger OnInsert()
    begin

    end;

    trigger OnModify()
    begin

    end;

    trigger OnDelete()
    begin

    end;

    trigger OnRename()
    begin

    end;

}