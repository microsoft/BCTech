namespace SalesValidationAgent.Setup.KPI;

table 50101 "Sales Val. Agent KPI"
{
    Access = Internal;
    Caption = 'Sales Val. Agent KPI';
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
            ToolTip = 'Specifies the unique identifier for the agent user.';
            DataClassification = EndUserPseudonymousIdentifiers;
            Editable = false;
        }
        field(10; "Orders Released"; Integer)
        {
            Caption = 'Orders Released';
            ToolTip = 'Specifies the number of sales orders released by the agent.';
            DataClassification = CustomerContent;
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
