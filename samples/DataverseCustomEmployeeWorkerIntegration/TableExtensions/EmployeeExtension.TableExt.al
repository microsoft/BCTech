tableextension 50100 "Employee Extesion" extends Employee
{
    fields
    {
        field(50100; "Coupled to CRM"; Boolean)
        {
            Caption = 'Coupled to Dataverse';
            Editable = false;
        }
    }
}