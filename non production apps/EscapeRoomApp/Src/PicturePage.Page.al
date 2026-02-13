page 73924 "Picture Page"
{
    PageType = UserControlHost;
    Caption = '';
    ApplicationArea = All;
    UsageCategory = Administration;
    Editable = false;

    layout
    {
        area(Content)
        {
            usercontrol("WebPageViewer"; WebPageViewer)
            {
                ApplicationArea = All;

                trigger ControlAddInReady(CallbackUrl: Text)
                begin
                    AddinIsReady := true;
                    LoadImage();
                end;

            }
        }
    }

    var
        Base64Image: Text;
        AddinIsReady: Boolean;
        HTML: Text;

    procedure SetImage(var Base64Image: Text)
    begin
        this.Base64Image := Base64Image;

        LoadImage();
    end;

    local procedure LoadImage()
    begin
        if not AddinIsReady then
            exit;

        LoadHTML();
    end;

    procedure LoadHTML()
    var
        HTMLTok: Label 'data:image/%1;base64,%2', Locked = true;
    begin
        HTML := '<img src=' + StrSubstNo(HTMLTok, 'png', Base64Image) + '>';
        CurrPage.WebPageViewer.SetContent(HTML);
    end;
}
