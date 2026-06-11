namespace Contoso.Loyalty;

/// <summary>
/// Pattern B — step 5 of 5. Permission set covering the partner's own buffer and production
/// tables. Without it the cloud-migration UI and any end user reading the production table
/// hit "you do not have permission to read …" errors. Box-product migrators ship analogous
/// sets under <c>src/Permission/</c>.
/// </summary>
permissionset 50100 "Contoso Loyalty"
{
    Assignable = true;
    Caption = 'Contoso Loyalty Migration';

    Permissions =
        table "Contoso Loyalty Tier" = X,
        tabledata "Contoso Loyalty Tier" = RIMD,
        table "Contoso BC14 Loyalty Tier" = X,
        tabledata "Contoso BC14 Loyalty Tier" = RIMD;
}
