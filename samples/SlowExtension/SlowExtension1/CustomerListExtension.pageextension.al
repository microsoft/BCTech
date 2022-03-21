pageextension 50101 CustomerListExtension extends "Customer List"
{
    layout
    {
        // Add changes to page layout here
    }

    actions
    {
        addafter(Service)
        {
            action(AddManyCustomers)
            {
                ApplicationArea = All;
                Caption = 'TODO';
            }
        }
    }
}