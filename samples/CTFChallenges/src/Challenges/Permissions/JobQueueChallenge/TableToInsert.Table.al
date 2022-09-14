table 50149 TableToInsert
{
    DataClassification = ToBeClassified;
    Access = Internal;

    fields
    {
        field(1; MyField; Integer)
        {
            DataClassification = ToBeClassified;
        }
    }

    keys
    {
        key(Key1; MyField)
        {
            Clustered = true;
        }
    }
}