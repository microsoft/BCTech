page 58212 MSFTTenantMediaIDsAPI
{
    PageType = API;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Tenant Media";
    EntityName = 'tenantMediaIds';
    EntitySetName = 'tenantMediaIds';
    APIPublisher = 'MSFT';
    APIGroup = 'moveData';
    APIVersion = 'v1.0';
    DelayedInsert = true;
    Permissions = tabledata "Tenant Media" = rmid;

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