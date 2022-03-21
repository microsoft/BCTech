table 50100 "Database Operation Statistics"
{
    DataClassification = SystemMetadata;
    
    fields
    {
        field(1; "Table ID"; Integer)
        {
        }
        field(2; Inserts; Integer)
        {
        }
        field(3; Modifies; Integer)
        {
        }
        field(4; Deletes; Integer)
        {
        }
        field(5; "Table Name"; Text[30])
        {
            FieldClass = FlowField;
            CalcFormula = lookup(AllObjWithCaption."Object Name" where("Object Type" = const(Table), "Object ID" = Field("Table ID")));
            Editable = false;
        }
    }
    
    keys
    {
        key(PK; "Table ID")
        {
            Clustered = true;
        }
    }
}