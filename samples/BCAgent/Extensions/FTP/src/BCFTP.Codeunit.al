/// <summary>
/// Codeunit PTEBCFTP (ID 50130).
/// </summary>
codeunit 50124 PTEBCFTP
{
    var
        ServiceBusRelay: codeunit AzureServiceBusRelay;
        Host: Text;
        Usr: Text;
        Pwd: Text;
        RootFolder: Text;
        HostnameLbl: Label 'hostName';
        UsernameLbl: Label 'userName';
        PasswdLbl: Label 'passwd';
        RootFolderLbl: Label 'rootFolder';
        FtpPluginNameTok: Label '/ftp/V1.0', Locked = true;
        ConnectFtpTok: Label '/ConnectFtp?jsonsettings=%1', Locked = true, Comment = '%1 - JSettings';
        GetFileListFtpTok: Label '/GetFilesFtp?jsonsettings=%1&foldername=%2', Locked = true, Comment = '%1 - JSettings, %2 - Foldername';
        DownloadFileFtpTok: Label '/DownloadFileFtp?jsonsettings=%1&filearray=%2', Locked = true, Comment = '%1 - JSettings, %2 - Filename';
        GetWorkDirectoryFtpTok: Label '/GetWorkingDirectoryFtp?jsonsettings=%1', Locked = true, Comment = '%1 - JSettings';
        SetWorkingDirectoryFtpTok: Label '/SetWorkingDirectoryFtp?jsonsettings=%1&foldername=%2', Locked = true, Comment = '%1 - JSettings, %2 - Foldername';
        CombineTxt: Label '%1%2', Comment = '%1 - String1, %2 - String2';


    /// <summary>
    /// Connect.
    /// </summary>
    /// <param name="JSettings">JsonObject.</param>
    /// <returns>Return variable Result of type Text.</returns>
    procedure Connect(JSettings: JsonObject) Result: Text
    var
        SettingsString: Text;
    begin
        JSettings.WriteTo(SettingsString);
        ServiceBusRelay.Get(BuildRequest(ConnectFtpTok, SettingsString), Result);
    end;

    /// <summary>
    /// GetFilesList.
    /// </summary>
    /// <param name="JSettings">JsonObject.</param>
    /// <param name="FolderName">Text.</param>
    /// <returns>Return variable Result of type Text.</returns>
    procedure GetFilesList(JSettings: JsonObject; FolderName: Text) Result: Text
    var
        SettingsString: Text;
    begin
        JSettings.WriteTo(SettingsString);
        ServiceBusRelay.Get(BuildRequest(GetFileListFtpTok, SettingsString, Foldername), Result);
    end;

    /// <summary>
    /// DownLoadFile.
    /// </summary>
    /// <param name="JSettings">JsonObject.</param>
    /// <param name="FileArray">JsonArray.</param>
    /// <returns>Return variable Result of type Text.</returns>
    procedure DownLoadFile(JSettings: JsonObject; FileArray: JsonArray) Result: Text
    var
        ArrayString: Text;
        SettingsString: Text;
    begin
        FileArray.WriteTo(ArrayString);
        JSettings.WriteTo(SettingsString);
        ServiceBusRelay.Get(BuildRequest(DownloadFileFtpTok, SettingsString, ArrayString), Result);
    end;

    /// <summary>
    /// SetWorkingDirectory.
    /// </summary>
    /// <param name="JSettings">JsonObject.</param>
    /// <param name="FolderName">Text.</param>
    /// <returns>Return variable Result of type Text.</returns>
    procedure SetWorkingDirectory(JSettings: JsonObject; FolderName: Text) Result: Text
    var
        SettingsString: Text;
    begin
        JSettings.WriteTo(SettingsString);
        ServiceBusRelay.Get(BuildRequest(SetWorkingDirectoryFtpTok, SettingsString, FolderName), Result);
    end;

    /// <summary>
    /// GetWorkingDirectory.
    /// </summary>
    /// <param name="JSettings">JsonObject.</param>
    /// <returns>Return variable Result of type Text.</returns>
    procedure GetWorkingDirectory(JSettings: JsonObject) Result: Text
    var
        SettingsString: Text;
    begin
        JSettings.WriteTo(SettingsString);
        ServiceBusRelay.Get(BuildRequest(GetWorkDirectoryFtpTok, SettingsString), Result);
    end;

    /// <summary>
    /// SettingsToVars.
    /// </summary>
    /// <param name="JSettings">JsonObject.</param>
    procedure SettingsToVars(JSettings: JsonObject)
    var
        JToken: JsonToken;
    begin
        if JSettings.Get(HostNameLbl, JToken) then
            Host := JToken.AsValue().AsText();

        if JSettings.Get(UsernameLbl, JToken) then
            Usr := JToken.AsValue().AsText();

        if JSettings.Get(PasswdLbl, JToken) then
            Pwd := JToken.AsValue().AsText();

        if JSettings.Get(RootFolderLbl, JToken) then
            RootFolder := JToken.AsValue().AsText();
    end;

    local procedure BuildRequest(Method: Text; SettingsString: Text): Text
    begin
        exit(StrSubStno(CombineTxt, FtpPluginNameTok, StrSubstNo(Method, SettingsString)));
    end;

    local procedure BuildRequest(Method: Text; SettingsString: Text; Foldername: Text): Text
    begin
        exit(StrSubStno(CombineTxt, FtpPluginNameTok, StrSubstNo(Method, SettingsString, Foldername)));
    end;
}
