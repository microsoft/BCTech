page 73925 "Hints ListPart"
{
    PageType = ListPart;
    ApplicationArea = All;
    SourceTable = "Escape Room Task";
    Editable = false;
    Caption = 'Hints';
    SourceTableView = sorting("Hint DateTime") order(descending);

    layout
    {
        area(Content)
        {
            repeater(General)
            {
                field(Hint; Rec.Hint)
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies the hint text.';
                    StyleExpr = 'Strong';
                    trigger OnDrillDown()
                    begin
                        message(Rec.Hint);
                    end;
                }
                field(DateTimeCreated; Rec."Hint DateTime")
                {
                    ApplicationArea = All;
                    ToolTip = 'Specifies when the hint was created.';
                    StyleExpr = 'Subordinate';
                }
            }
        }
    }
}
