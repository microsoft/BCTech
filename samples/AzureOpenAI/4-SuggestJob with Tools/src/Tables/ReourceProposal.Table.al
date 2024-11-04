table 54397 "Resource Proposal"
{
    TableType = Temporary;
    Caption = 'Copilot Resource Proposal';

    fields
    {
        field(1; "No."; Code[20])
        {
            Caption = 'No.';
            Editable = false;
        }
        field(3; Name; Text[100])
        {
            Caption = 'Name';
            Editable = false;
        }
        field(10; "Job Title"; Text[30])
        {
            Caption = 'Job Title';
            Editable = false;
        }
        field(20; Explanation; Text[2048])
        {
            Caption = 'Explanation';
            Editable = false;
        }
    }

    keys
    {
        key(Key1; "No.")
        {
            Clustered = true;
        }
    }
}