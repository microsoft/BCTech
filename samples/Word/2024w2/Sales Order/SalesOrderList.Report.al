report 52801 "Sales Order - List"
{
    DefaultRenderingLayout = "Sales Order List Layout";

    dataset
    {
        dataitem(SalesOrder; "Sales Orders")
        {
            column(SalesOrder_Id; SalesOrder.Id)
            {
                IncludeCaption = true;
            }
            column(SalesOrder_Description; SalesOrder.Description)
            {
                IncludeCaption = true;
            }
            dataitem(SalesOrderLine; "Sales Order Lines")
            {
                DataItemLink = ParentId = FIELD(Id);
                column(SalesOrderLine_Id; SalesOrderLine.Id)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_Description; SalesOrderLine.Description)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_Quantity; SalesOrderLine.Quantity)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_Amount; SalesOrderLine.Amount)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_LineNo; SalesOrderLine.LineNo)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_FieldToHide; SalesOrderLine.FieldToHide)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_RowToHide; SalesOrderLine.RowToHide)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderLine_ColumnToHide; SalesOrderLine.ColumnToHide)
                {
                    IncludeCaption = true;
                }
            }
            dataitem(SalesOrderAdditionalInfo; "Sales Order Additional Info")
            {
                DataItemLink = ParentId = FIELD(Id);
                column(SalesOrderAdditionalInfo_Id; SalesOrderAdditionalInfo.Id)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderAdditionalInfo_Description; SalesOrderAdditionalInfo.Description)
                {
                    IncludeCaption = true;
                }
                column(SalesOrderAdditionalInfo_Miscellaneous; SalesOrderAdditionalInfo.Miscellaneous)
                {
                    IncludeCaption = true;
                }
            }
        }
    }

    requestpage
    {
        SaveValues = true;

        layout
        {
            area(content)
            {
                group(requestpage)
                {
                }
            }
        }

        actions
        {
        }


        trigger OnInit()
        begin
        end;
    }

    rendering
    {
        layout("Sales Order List Layout")
        {
            Type = Word;
            LayoutFile = './Layout/SalesOrderList.docx';
            Caption = 'Sales Order List Layout';
            Summary = 'Word Layout for Sales Orders';
        }
    }

    labels
    {
    }
}

