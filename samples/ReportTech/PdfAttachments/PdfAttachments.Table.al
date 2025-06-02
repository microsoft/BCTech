table 50951 PdfAttachments
{
    DataClassification = ToBeClassified;
    TableType = Temporary;

    fields
    {
        field(1; Id; Integer)
        {
            DataClassification = ToBeClassified;
        }
        field(2; Name; Text[250])
        {
            DataClassification = ToBeClassified;
        }
        field(3; Comment; Text[250])
        {
            DataClassification = ToBeClassified;
        }
        field(4; Type; enum PdfAttachmentDataRelationShip)
        {
            DataClassification = ToBeClassified;
        }
        field(5; Mimetype; Text[250])
        {
            DataClassification = ToBeClassified;
        }
        field(6; Data; Blob)
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(Key1; Id)
        {
            Clustered = true;
        }
    }
}

