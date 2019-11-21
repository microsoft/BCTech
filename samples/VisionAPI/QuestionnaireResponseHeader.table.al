table 50002 "Questionnaire Response Header"
{
    DataClassification = CustomerContent;

    fields
    {
        field(1; "Entry No."; Integer)
        {
            DataClassification = SystemMetadata;
            AutoIncrement = true;
            Caption = 'Entry No.';
        }
        field(2; "Profile Code"; Code[20])
        {
            DataClassification = CustomerContent;
            Caption = 'Profile Code';
            TableRelation = "Profile Questionnaire Header";
        }
        field(3; "Contact No."; Code[20])
        {
            DataClassification = CustomerContent;
            Caption = 'Contact No.';
            TableRelation = Contact;
        }
        field(4; "Picture"; Media)
        {
            DataClassification = CustomerContent;
            caption = 'Picture';
        }
        field(5; "OCR json"; Blob)
        {
            DataClassification = CustomerContent;
            caption = 'OCR json';
        }
        field(6; "Created On"; DateTime)
        {
            DataClassification = CustomerContent;
            Caption = 'Created On';
        }
    }

    keys
    {
        key(key1; "Entry No.")
        {
            Clustered = true;
        }
        key(key2; "Profile Code")
        {

        }
        key(key3; "Contact No.")
        {

        }
    }


    trigger OnInsert()
    begin
        "Created On" := CurrentDateTime;
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