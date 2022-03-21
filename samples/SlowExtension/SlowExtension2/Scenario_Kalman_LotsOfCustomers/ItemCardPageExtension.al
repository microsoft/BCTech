pageextension 50123 MyItemCardPageExtension extends "Item Card"
{
    actions
    {
        addlast(Functions)
        {            
            action(SlowCode)
            {
                ApplicationArea = All;
                Visible = true;
                Promoted = true;
                PromotedCategory = Category4;
                trigger OnAction()
                var
                begin                    
                    TraverseCustomersWithFilter('<>''''');
                    TraverseCustomers();
                end;
            }
        }
    }

    local procedure TraverseCustomersWithFilter(filterText: Code[20])
    var       
        Customer: Record Customer;
        counter: Integer;
    begin
        SelectLatestVersion();
        Customer.SetFilter(Customer."Address 2", filterText);
        Customer.FindFirst();
        counter := 0;
        repeat
            counter := counter + 1;
            Customer.CalcFields("Balance (LCY)");           
            Customer.CalcAvailableCredit();
            // Sleep(10);
        until Customer.Next() = 0;              
        
        Message('Traversed %1 customers, with a filter on Name 2, while calculating the "Balance (LCY)"', counter);
    end;

   local procedure TraverseCustomers()
    var       
        Customer: Record Customer;
        counter: Integer;
    begin      
        Customer.FindFirst();
        counter := 0;
        repeat
            counter := counter + 1;
            Customer.CalcFields("Balance (LCY)");   
            Customer.CalcAvailableCredit();   
            // Sleep(100);    
        until Customer.Next() = 0;

        Message('Traversed %1 customers, while calculating the "Balance (LCY)"', counter);           
    end;
}
