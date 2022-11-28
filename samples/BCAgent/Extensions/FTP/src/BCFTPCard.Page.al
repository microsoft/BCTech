/// <summary>
/// Page PTEBCFTP (ID 50124).
/// </summary>
page 50124 PTEBCFTP
{
    Caption = 'BC FTP';
    UsageCategory = Administration;
    ApplicationArea = All;


    layout
    {
        area(Content)
        {
            group(Server)
            {
                Caption = 'Server';

                field(FtpHost; FtpHost)
                {
                    Caption = 'FTP Host';
                    ApplicationArea = All;

                    trigger OnValidate()
                    begin
                        UpdateSettings();
                    end;
                }

                field(FtpUser; FtpUser)
                {
                    Caption = 'FTP User';
                    ApplicationArea = All;

                    trigger OnValidate()
                    begin
                        UpdateSettings();
                    end;
                }

                field(FtpPasswd; FtpPasswd)
                {
                    Caption = 'FTP Passwd';
                    ApplicationArea = All;
                    ExtendedDatatype = Masked;

                    trigger OnValidate()
                    begin
                        UpdateSettings();
                    end;
                }

                field(FtpFolder; FtpFolder)
                {
                    Caption = 'FTP Folder';
                    ApplicationArea = All;

                    trigger OnValidate()
                    begin
                        UpdateSettings();
                    end;
                }
            }

            group(Response)
            {
                Caption = 'Response';

                field(FtpResponse; FtpResponse)
                {
                    ApplicationArea = All;
                    Caption = 'FTP Response';
                    MultiLine = true;
                }
            }

            part(BCFtpFiles; PTEFtpFiles)
            {
                ApplicationArea = All;
            }
        }
    }

    actions
    {
        area(Processing)
        {
            action(Connect)
            {
                ApplicationArea = All;
                Caption = 'Connect';
                Image = Continue;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;

                trigger OnAction()
                begin
                    FtpResponse := BCFtp.Connect(JSettings);
                    CurrPage.Update(false);
                end;
            }

            action(GetFileList)
            {
                ApplicationArea = All;
                Caption = 'File List';
                ToolTip = 'Get list of files on FTP server';
                Image = Continue;
                Promoted = true;
                PromotedOnly = true;
                PromotedCategory = Process;

                trigger OnAction()
                var
                    JToken: JsonToken;
                    Source: JsonArray;
                begin
                    FtpResponse := BCFtp.GetFilesList(JSettings, FtpFolder);
                    JToken.ReadFrom(FtpResponse);
                    if not JToken.AsObject().Get('items', JToken) then
                        exit;

                    Source := JToken.AsArray();
                    CurrPage.BCFtpFiles.Page.SetSource(Source);
                    CurrPage.Update(false);
                end;
            }
            action(DownloadFile)
            {
                Caption = 'Download';
                ToolTip = 'Download selected file(s)';
                ApplicationArea = All;
                Image = Download;

                trigger OnAction()
                begin
                    CurrPage.BCFtpFiles.Page.DownloadFiles();
                end;
            }
        }
    }

    var
        BCFtp: Codeunit PTEBCFTP;
        JSettings: JsonObject;
        FtpHost: Text;
        FtpUser: Text;
        FtpPasswd: Text;
        FtpResponse: Text;
        FtpFolder: Text;
        HostnameLbl: Label 'hostName';
        UsernameLbl: Label 'userName';
        PasswdLbl: Label 'passwd';
        RootFolderLbl: Label 'rootFolder';


    local procedure UpdateSettings()
    begin
        if JSettings.Contains(HostnameLbl) then
            JSettings.Replace(hostNameLbl, FtpHost)
        else
            JSettings.Add(hostNameLbl, FtpHost);

        if JSettings.Contains(UsernameLbl) then
            JSettings.Replace(userNameLbl, FtpUser)
        else
            JSettings.Add(userNameLbl, FtpUser);

        if JSettings.Contains(PasswdLbl) then
            JSettings.Replace(passwdLbl, FtpPasswd)
        else
            JSettings.Add(passwdLbl, FtpPasswd);

        if JSettings.Contains(RootFolderLbl) then
            JSettings.Replace(rootFolderLbl, FtpFolder)
        else
            JSettings.Add(rootFolderLbl, FtpFolder);

        BCFTP.SettingsToVars(JSettings);
        CurrPage.BCFtpFiles.Page.SetSettings(JSettings);
    end;
}