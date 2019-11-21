table 50003 "Questionnaire Response Line"
{
    DataClassification = CustomerContent;

    fields
    {
        field(1; "Header Entry No."; Integer)
        {
            DataClassification = SystemMetadata;
            Caption = 'Entry No.';
        }
        field(2; "Line No."; Integer)
        {
            DataClassification = SystemMetadata;
            Caption = 'Line No.';
        }
        field(3; "Profile Code"; Code[20])
        {
            DataClassification = CustomerContent;
            Caption = 'Profile Code';
        }
        field(4; "Contact No."; Code[20])
        {
            DataClassification = CustomerContent;
            Caption = 'Contact No.';
        }
        field(5; "Questionnaire Line Type"; Option)
        {
            OptionMembers = Question,Answer;
            DataClassification = CustomerContent;
            caption = 'Questionnaire Line Type';
        }
        field(6; "Response Text"; Text[1000])
        {
            DataClassification = CustomerContent;
            caption = 'Response Text';
        }
        field(7; "Response Selection"; Boolean)
        {
            DataClassification = CustomerContent;
            caption = 'Response Selection';
        }
        field(8; Description; text[250])
        {
            Caption = 'Question';
            FieldClass = FlowField;
            CalcFormula = lookup ("Profile Questionnaire Line".Description WHERE("Profile Questionnaire Code" = field("Profile Code"), "Line No." = field("Line No.")));
        }
    }

    keys
    {
        key(key1; "Header Entry No.", "Line No.")
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