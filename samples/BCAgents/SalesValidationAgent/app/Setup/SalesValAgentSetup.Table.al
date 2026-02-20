namespace SalesValidationAgent.Setup;

table 50100 "Sales Val. Agent Setup"
{
    Access = Internal;
    Caption = 'Sales Val. Agent Setup';
    DataClassification = CustomerContent;
    InherentEntitlements = RIMDX;
    InherentPermissions = RIMDX;
    ReplicateData = false;
    DataPerCompany = false;

    fields
    {
        field(1; "User Security ID"; Guid)
        {
            Caption = 'User Security ID';
            ToolTip = 'Specifies the unique identifier for the user.';
            DataClassification = EndUserPseudonymousIdentifiers;
            Editable = false;
        }
    }
    keys
    {
        key(Key1; "User Security ID")
        {
            Clustered = true;
        }
    }
}