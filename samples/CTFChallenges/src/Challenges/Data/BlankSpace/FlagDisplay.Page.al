page 50106 "Flag Display"
{
    PageType = StandardDialog;
    ApplicationArea = All;
    UsageCategory = Administration;

    layout
    {
        area(Content)
        {
            group(FlagShowcase)
            {
                ShowCaption = false;

                field(TheFlagIs; TheFlagIs)
                {
                    ApplicationArea = All;
                    ShowCaption = false;

                    trigger OnValidate()
                    begin
                        if TheFlagIs = 'The flag is:' then
                            TheFlagIsCorrect := 'The flag is: Flag_d27d3329'
                        else
                            TheFlagIsCorrect := TheFlagIs;
                    end;
                }

                field(TheFlagIsCorrect; TheFlagIsCorrect)
                {
                    ApplicationArea = All;
                    Editable = false;
                    ShowCaption = false;
                }
            }
        }
    }

    trigger OnOpenPage()
    var
        TypeHelper: Codeunit "Type Helper";
    begin
        TheFlagIs := 'The flag' + TypeHelper.LFSeparator() + 'is:';
        TheFlagIsCorrect := 'The flag is:';
    end;


    var
        TheFlagIs: Text;
        TheFlagIsCorrect: Text;
}