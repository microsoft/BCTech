pageextension 50100 "Employee List Extension" extends "Employee List"
{
    layout
    {
        addlast(Control1)
        {
            field("Coupled to CRM"; Rec."Coupled to CRM")
            {
                ApplicationArea = All;
                Caption = 'Coupled to Dataverse';
                Editable = false;
            }
        }
    }
}