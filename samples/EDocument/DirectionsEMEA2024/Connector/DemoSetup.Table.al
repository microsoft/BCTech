namespace DefaultPublisher.EDocDemo;

table 50100 "Demo Setup"
{
    InherentEntitlements = X;
    InherentPermissions = X;
    DataClassification = ToBeClassified;

    fields
    {
        field(1; "API Url"; Text[250])
        {
            DataClassification = CustomerContent;
        }
        field(2; "API Key"; Text[250])
        {
            DataClassification = CustomerContent;
            ExtendedDatatype = Masked;
        }
        field(3; "Service Name"; Text[250])
        {
            DataClassification = CustomerContent;
        }
    }

    keys
    {
        key(key1; "API Url")
        {
            Clustered = true;
        }
    }

    fieldgroups
    {
        // Add changes to field groups here
    }

}