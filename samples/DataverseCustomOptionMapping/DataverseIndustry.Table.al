table 50100 "Dataverse Industry"
{
    Caption = 'Dataverse Industry';
    Description = 'Industry';
    TableType = Temporary;

    fields
    {
        field(1; "Option Id"; Integer)
        {
            DataClassification = SystemMetadata;
        }
        field(2; "Code"; Text[250])
        {
            DataClassification = SystemMetadata;
        }
    }

    keys
    {
        key(Key1; "Option Id")
        {
            Clustered = true;
        }
        key(Key2; "Code")
        {
        }
    }

    procedure Load(): Boolean
    var
        TableMetadata: Record "Table Metadata";
        CDSIntegrationMgt: Codeunit "CDS Integration Mgt.";
        OptionSetMetadataDictionary: Dictionary of [Integer, Text];
        OptionValue: Integer;
    begin
        if TableMetadata.Get(Database::"CRM Account") then
            OptionSetMetadataDictionary := CDSIntegrationMgt.GetOptionSetMetadata(TableMetadata.ExternalName, 'industrycode');
        if OptionSetMetadataDictionary.Count() = 0 then
            exit(true);

        foreach OptionValue in OptionSetMetadataDictionary.Keys() do begin
            Clear(Rec);
            Rec."Option Id" := OptionValue;
            Rec."Code" := CopyStr(OptionSetMetadataDictionary.Get(OptionValue), 1, MaxStrLen(Rec."Code"));
            Rec.Insert();
        end;

        if not Rec.FindFirst() then
            exit(false);

        exit(true);
    end;
}