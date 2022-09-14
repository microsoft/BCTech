table 50148 TableToRead
{
    DataClassification = ToBeClassified;
    Access = Internal;
    fields
    {
        field(1; ID; Guid)
        {
        }
        field(4; Value; Text[250])
        {
        }
    }

    keys
    {
        key(PK; ID)
        {
            Clustered = true;
        }
    }
}