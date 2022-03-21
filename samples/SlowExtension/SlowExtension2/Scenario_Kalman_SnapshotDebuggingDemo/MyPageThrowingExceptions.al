page 60654 MyPageWithExceptions
{
    PageType = Card;
    ApplicationArea = All;
    UsageCategory = Administration;

    actions
    {
        area(Processing)
        {
            action(InvokeCodeUnit)
            {
                ApplicationArea = All;

                trigger OnAction()
                var
                    AcodeUnit: Codeunit TestExceptions;
                begin
                    AcodeUnit.Run();
                end;
            }
        }
    }

    var
        myInt: Integer;
}