pageextension 50000 "Import Contact From Ocr" extends "Contact List"
{
    layout
    {
        // Add changes to page layout here
    }

    actions
    {
        // Add changes to page actions here
        addlast(processing)
        {
            action(NewFromOcr)
            {
                ApplicationArea = All;
                Caption = 'Create Contact from Picture';
                Image = NewCustomer;
                Promoted = true;
                PromotedCategory = Process;

                trigger OnAction()
                begin
                    message('Hello')
                end;
            }
        }
    }
}