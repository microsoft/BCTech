/// <summary>
/// Page PTEFtpFiles (ID 50125).
/// </summary>
page 50125 PTEFtpFiles
{
    Caption = 'PTEFtpFiles';
    PageType = ListPart;
    SourceTable = "Name/Value Buffer";
    SourceTableTemporary = true;

    layout
    {
        area(content)
        {
            repeater(General)
            {
                field(FolderName; Rec.Name)
                {
                    ApplicationArea = All;
                    Caption = 'Name';
                    Editable = false;
                    DrillDown = true;
                    StyleExpr = StyleTxt;

                    trigger OnDrillDown()
                    begin
                        OnDrillDownName();
                    end;
                }

                field(Size; Rec.Value)
                {
                    ApplicationArea = All;
                    Caption = 'Size';
                    Editable = false;
                }
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(DownloadFile)
            {
                Caption = 'Download';
                ToolTip = 'Download selected file(s)';
                ApplicationArea = All;
                Image = Download;

                trigger OnAction()
                begin
                    DownloadFiles();
                end;
            }
        }
    }

    trigger OnAfterGetCurrRecord()
    begin
        SetStyle();
    end;

    trigger OnAfterGetRecord()
    begin
        SetStyle();
    end;


    var
        JSettings: JsonObject;
        [InDataSet]
        CurrentFolder: Text;
        RootFolder: Text;
        StyleTxt: Text;
        NameLbl: Label 'fullName';
        SizeLbl: Label 'fileSize';
        UpLevelLbl: Label '..';
        ZeroSizeLbl: Label '0';


    /// <summary>
    /// SetSettings.
    /// </summary>
    /// <param name="NewJSettings">JsonObject.</param>
    procedure SetSettings(NewJSettings: JsonObject)
    var
        JToken: JsonToken;
        RootFolderLbl: Label 'RootFolder';
    begin
        JSettings := NewJSettings;
        if NewJSettings.Get(RootFolderLbl, JToken) then
            CurrentFolder := JToken.AsValue().AsText();
    end;

    /// <summary>
    /// SetSource.
    /// </summary>
    /// <param name="NewSource">JsonArray.</param>
    procedure SetSource(NewSource: JsonArray)
    var
        JToken: JsonToken;
    begin
        Rec.DeleteAll(true);

        InsertNameValue(UpLevelLbl, 0);
        foreach JToken in NewSource do
            InsertNameValue(JToken, NewSource.IndexOf(JToken) + 1);

        CurrPage.Update(false);
    end;

    /// <summary>
    /// DownloadFiles.
    /// </summary>
    procedure DownloadFiles()
    var
        TempNameValueBuffer: Record "Name/Value Buffer" temporary;
        BCFtp: Codeunit PTEBCFTP;
        JObject: JsonObject;
        JArray: JsonArray;
        FilenameLbl: Label 'fileName';
    begin
        if (Rec.Value = ZeroSizeLbl) or (Rec.Name = UpLevelLbl) then
            exit;

        TempNameValueBuffer.Copy(Rec, true);

        CurrPage.SetSelectionFilter(TempNameValueBuffer);
        if TempNameValueBuffer.FindSet() then
            repeat
                JObject.Add(FilenameLbl, TempNameValueBuffer.Name);
                JArray.Add(JObject);
            until TempNameValueBuffer.Next() = 0;

        BCFtp.DownLoadFile(JSettings, JArray);
    end;

    local procedure InsertNameValue(JToken: JsonToken; Id: Integer)
    var
        JObject: JsonObject;
    begin
        Rec.Init();
        Rec.ID := Id;

        JObject := JToken.AsObject();

        if JObject.Get(NameLbl, JToken) then
            Rec.Name := CopyStr(JToken.AsValue().AsText(), 1, MaxStrLen(Rec.Name));

        if JObject.Get(SizeLbl, JToken) then
            Rec.Value := Format(JToken.AsValue().AsInteger());

        if JObject.Get(SizeLbl, JToken) then
            Rec.Value := Format(JToken.AsValue().AsInteger());

        Rec.Insert(false);
    end;

    local procedure InsertNameValue(Name: Text; Id: Integer)
    begin
        Rec.Init();
        Rec.ID := Id;
        Rec.Name := CopyStr(Name, 1, MaxStrLen(Rec.Name));
        Rec.Insert(false);
    end;

    local procedure NavigateUp()
    var
        TempRegExMatches: Record Matches temporary;
        BCFtp: Codeunit PTEBCFTP;
        RegExp: Codeunit Regex;
        JToken: JsonToken;
        RegExpLbl: Label '^(.*[\\\/])';
    begin
        if CurrentFolder <> RootFolder then begin
            RegExp.Match(CurrentFolder, RegExpLbl, TempRegExMatches);
            if TempRegExMatches.IsEmpty() then
                exit;

            TempRegExMatches.FindFirst();
            CurrentFolder := TempRegExMatches.ReadValue();
        end;
        if CurrentFolder.EndsWith('/') or CurrentFolder.EndsWith('\') then
            CurrentFolder := CopyStr(CurrentFolder, 1, StrLen(CurrentFolder) - 1);
        JToken.ReadFrom(BCFtp.GetFilesList(JSettings, CurrentFolder));
        if not JToken.AsObject().Get('items', JToken) then
            exit;

        SetSource(JToken.AsArray());
    end;

    local procedure NavigateDown()
    var
        BCFtp: Codeunit PTEBCFTP;
        JToken: JsonToken;
    begin
        JToken.ReadFrom(BCFtp.GetFilesList(JSettings, Rec.Name));
        if not JToken.AsObject().Get('items', JToken) then
            exit;

        CurrentFolder := Rec.Name;
        SetSource(JToken.AsArray());
    end;

    local procedure SetStyle()
    var
        StandardAccentLbl: Label 'StandardAccent';
    begin
        Clear(StyleTxt);
        if Rec.Value = ZeroSizeLbl then
            StyleTxt := StandardAccentLbl;
    end;

    local procedure OnDrillDownName()
    begin
        if (Rec.Value <> ZeroSizeLbl) and (Rec.Name <> UpLevelLbl) then
            exit;

        if Rec.Name = UpLevelLbl then
            NavigateUp()
        else
            NavigateDown();
    end;

}