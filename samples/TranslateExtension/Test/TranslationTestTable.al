table 50101 TranslationTestTable
{
    DataClassification = ToBeClassified;

    fields
    {
        field(50130; "Customer name"; Text[20])
        {
            DataClassification = ToBeClassified;

        }

        field(50131; Balance; Text[20])
        {
            DataClassification = ToBeClassified;

        }
        field(50132; "Total sales"; Text[20])
        {
            DataClassification = ToBeClassified;

        }
        field(50133; "Phone no."; Text[20])
        {
            DataClassification = ToBeClassified;

        }
        field(50134; Address; Text[20])
        {
            DataClassification = ToBeClassified;

        }
        field(50135; City; Text[20])
        {
            DataClassification = ToBeClassified;

        }
    }

    keys
    {
        key(PrimaryKey; "Customer name")
        {
            Clustered = true;
        }
    }
}

