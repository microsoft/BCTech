page 58214 MSFTTenantMediaSetIDsAPI
{
    PageType = API;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Tenant Media Set";
    EntityName = 'tenantMediaSetIds';
    EntitySetName = 'tenantMediaSetIds';
    APIPublisher = 'MSFT';
    APIGroup = 'moveData';
    APIVersion = 'v1.0';
    DelayedInsert = true;
    Permissions = tabledata "Tenant Media Set" = rmid;

    layout
    {
        area(Content)
        {
            repeater(TenantMedia)
            {
                field(id; Rec.ID)
                {
                    Caption = 'id';
                }
            }
        }
    }
}