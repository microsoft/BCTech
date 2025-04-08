namespace CopilotToolkitDemo.ItemSubstitution;

table 54323 "Copilot Item Sub Proposal"
{
    TableType = Temporary;

    fields
    {
        field(1; "No."; Code[20])
        {
            Caption = 'No.';
            Editable = false;
        }
        field(3; Description; Text[100])
        {
            Caption = 'Name';
            Editable = false;
        }
        field(20; Explanation; Text[2048])
        {
            Caption = 'Explanation';
            Editable = false;
        }
        field(21; "Full Explanation"; Blob)
        {
            Caption = 'Full Explanation';
        }
        field(22; Select; Boolean)
        {
            Caption = 'Select';
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