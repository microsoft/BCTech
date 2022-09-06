report 50147 "My New Customer - List"
{
    DefaultRenderingLayout = "CustomerList.rdlc";
    ApplicationArea = Basic, Suite;
    Caption = 'Customer List';
    UsageCategory = ReportsAndAnalysis;
    DataAccessIntent = ReadOnly;

    dataset
    {
        dataitem(Customer; Customer)
        {

            column(COMPANYNAME; COMPANYPROPERTY.DisplayName())
            {
            }
            column(Customer_TABLECAPTION__________CustFilter; TableCaption + ': ' + CustFilter)
            {
            }
            column(CustFilter; CustFilter)
            {
            }
            column(Customer__No__; "No.")
            {
            }
            column(CustAddr_1_; CustAddr[1])
            {
            }
            
            trigger OnAfterGetRecord()
            begin
                CalcFields("Balance (LCY)");
                FormatAddr.FormatAddr(
                  CustAddr, Name, "Name 2", '', Address, "Address 2",
                  City, "Post Code", County, "Country/Region Code");
            end;
        }
    }

    requestpage
    {
        SaveValues = true;

        layout
        {
        }

        actions
        {
        }
    }

    rendering
    {
        layout("CustomerList.rdlc")
        {
            Type = RDLC;
            LayoutFile = 'CustomerList.rdlc';
        }
    }
      
    labels
    {
    }
   

    trigger OnPreReport()
    var
        FormatDocument: Codeunit "Format Document";
    begin
        CustFilter := FormatDocument.GetRecordFiltersWithCaptions(Customer);
    end;

    var
        FormatAddr: Codeunit "Format Address";
        CustFilter: Text;
        CustAddr: array[8] of Text[100];  
}

