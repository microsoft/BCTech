page 50107 "Security Check"
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

                field(UserSecId; UserSecId)
                {
                    ApplicationArea = All;
                    ShowCaption = false;

                    trigger OnValidate()
                    begin
                        if UserSecId = UserSecurityId() then
                            OutputText := 'Your identity is confirmed! Your flag: Flag_f4054824'
                    end;
                }

                field(Flag; OutputText)
                {
                    ApplicationArea = All;
                    Editable = false;
                    ShowCaption = false;
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        OutputText := 'We need to verify your identity before presenting the flag. Please enter your security ID.';
    end;


    var
        UserSecId: Text;
        OutputText: Text;
}