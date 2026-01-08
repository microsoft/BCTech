page 73920 "Rich Text Box Page"
{
    PageType = StandardDialog;
    Caption = '';
    ApplicationArea = All;
    UsageCategory = Administration;
    Editable = false;

    layout
    {
        area(Content)
        {
            group(Group1)
            {
                ShowCaption = false;
                field(RichTextBox; this.HTML)
                {
                    ShowCaption = false;
                    MultiLine = true;
                    Editable = false;
                    ExtendedDatatype = RichContent;
                }
            }
        }
    }


    var
        HTML: Text;

    procedure Initialize(Caption: Text; HTML: text)
    begin
        this.Caption := Caption;
        This.HTML := HTML;
    end;

}