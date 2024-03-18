table 50115 "Entitlement List"
{
    TableType = Temporary;

    fields
    {
        field(1; "Name/Id"; Text[200]) { }
        field(2; "Owning AppId"; Guid) { }
        field(3; "Is Entitled"; Boolean) { }
    }
}
