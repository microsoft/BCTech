codeunit 50951 ReportRenderingCompleteHandler
{
    trigger OnRun()
    begin
        // Initialize the attachment lists and other related global variables to a known state.
        this.ResetAttachmentLists();
        clear(this.AdditionalDocumenNames);
        clear(this.UserCode);
        clear(this.AdminCode);
    end;

    /// <summary>
    /// Configure the attachment lists. Empty name will reset the list.
    /// </summary>
    /// <param name="name">Attahcment name.</param>
    /// <param name="MimeType">Attachment mimetype.</param>
    /// <param name="Filename">Attachment filename.</param>
    procedure AddAttachment(name: Text; DataType: enum PdfAttachmentDataRelationShip; MimeType: Text; Filename: Text; Description: Text; PrimaryDocument: Boolean)
    begin
        if (name = '') then begin
            ResetAttachmentLists();
            exit;
        end;
        if this.AttachmentNames.Contains(name) then begin
            Error('Attachment with name %1 already exists.', name);
        end;

        this.AttachmentNames.Add(name);
        this.AttachmentDataTypes.Add(DataType);
        this.AttachmentMimeTypes.Add(MimeType);
        this.AttachmentFilenames.Add(Filename);
        this.Attachmentdescriptions.Add(Description);
        if (PrimaryDocument) then begin
            this.PrimaryDocumentName := name;
            this.SaveFormat := PdfSaveFormat::Einvoice;
        end;
    end;

    /// <summary>
    /// Add a file to the list of files to append to the rendered document. Empty name will reset the list.
    /// </summary>
    /// <param name="fileName">Path to the file to append. Platform will remove the file when rendering has been completed.</param>
    procedure AddFilesToAppend(fileName: Text)
    begin
        if (fileName = '') then begin
            if (this.AppendCount() = 0) then
                exit;
            Clear(this.AdditionalDocumenNames);
            exit;
        end;

        this.AdditionalDocumenNames.Add(fileName);
    end;

    /// <summary>
    /// Add a stream to the list of files to append to the rendered document using a temporary file name.
    /// </summary>
    /// <param name="fileName">Stream with file content. Platform will remove the temporary file when rendering has been completed.</param>
    procedure AddStreamToAppend(FileInStream: InStream)
    var
        TempFile: File;
        TempFileName: Text;
        LocalInStream: InStream;
        FileOutStream: OutStream;
    begin
        if (FileInStream.Length = 0) then
            exit;
        LocalInStream := FileInStream;
        LocalInStream.ResetPosition();
        TempFile.CreateTempFile();
        TempFileName := TempFile.Name;
        TempFile.Close();
        TempFile.Create(TempFileName);
        TempFile.CreateOutStream(FileOutStream);

        CopyStream(FileOutStream, LocalInStream);
        TempFile.Close();
        this.AddFilesToAppend(TempFileName);
    end;

    /// <summary>
    /// Protect the document with a user and admin code using text data type.
    /// </summary>
    /// <param name="user">User code.</param>
    /// <param name="admin">Admin code.</param>
    [NonDebuggable]
    procedure ProtectDocument(user: Text; admin: Text)
    begin
        UserCode := user;
        AdminCode := admin;
    end;

    /// <summary>
    /// Protect the document with a user and admin code using secrettext data type.
    /// </summary>
    /// <param name="user">User code.</param>
    /// <param name="admin">Admin code.</param>
    [NonDebuggable]
    procedure ProtectDocument(user: SecretText; admin: SecretText)
    begin
        UserCode := user;
        AdminCode := admin;
    end;

    procedure AttachmentCount(): Integer
    begin
        if ((AttachmentNames.Count() <> AttachmentMimeTypes.Count()) OR
            (AttachmentNames.Count() <> AttachmentDataTypes.Count()) OR
            (AttachmentNames.Count() <> AttachmentFilenames.Count()) OR
            (AttachmentNames.Count() <> AttachmentDescriptions.Count())) then
            Error('Attachment information lists are not in sync.');

        exit(AttachmentNames.Count());
    end;

    procedure AppendCount(): Integer
    begin
        exit(AdditionalDocumenNames.Count());
    end;

    local procedure ResetAttachmentLists()
    begin
        Clear(this.AttachmentNames);
        Clear(this.AttachmentMimeTypes);
        Clear(this.AttachmentFilenames);
        Clear(this.AttachmentDataTypes);
        Clear(this.AttachmentDescriptions);
        clear(this.PrimaryDocumentName);
        clear(this.SaveFormat);
    end;

    //[NonDebuggable]
    procedure ToJson(RenderingPayload: JsonObject): JsonObject
    var
        JsonElement: JsonObject;
        json: JsonObject;
        jsonDataArray: JsonArray;
        SourceDataArray: JsonArray;
        JsonTokenElement: JsonToken;
        JsonTextToken: JsonToken;
        i: Integer;
        name: Text;
        DataType: enum PdfAttachmentDataRelationShip;
        MimeType: Text;
        FileName: Text;
        Description: Text;
        TextVar: Text;
        user, admin : SecretText;
        HasProtection: Boolean;
        ProtectionOverrideError: Label 'The rendering payload already contains protection. This cannot be overwritten.';
        PrimaryDocumentOverrideError: Label 'The rendering payload already contains a primary document. This cannot be overwritten.';
        // DO NOT modify the following labels. They are used by the platform to identify the properties in the JSON payload.
        PrimaryDocumentToken: Label 'primaryDocument', Locked = true;
        SaveFormatToken: Label 'saveformat', Locked = true;
        AttachmentsToken: Label 'attachments', Locked = true;
        AdditionalDocumentsToken: Label 'additionalDocuments', Locked = true;
        VersionToken: Label 'version', Locked = true;
        ProtectionToken: Label 'protection', Locked = true;
        NameToken: Label 'name', Locked = true;
        RelationshipToken: Label 'relationship', Locked = true;
        MimeTypeToken: Label 'mimetype', Locked = true;
        FileNameToken: Label 'filename', Locked = true;
        DescriptionToken: Label 'description', Locked = true;
        UserToken: Label 'user', Locked = true;
        AdminToken: Label 'admin', Locked = true;
        JsonVersionTxt: Label '1.0', Locked = true;
    begin
        json := RenderingPayload;
        if not json.Contains(VersionToken) then
            json.Add(VersionToken, JsonVersionTxt);

        if StrLen(this.PrimaryDocumentName) > 0 then begin
            if json.Contains(PrimaryDocumentToken) then begin
                json.Get(PrimaryDocumentToken, JsonTextToken);
                if JsonTextToken.WriteTo(TextVar) then
                    // The rendering payload already contains a primary document. This cannot be overwritten. Fail with an error.
                    if TextVar <> '' then
                        Error(PrimaryDocumentOverrideError);

                json.Replace(PrimaryDocumentToken, this.PrimaryDocumentName);
            end else
                json.Add(PrimaryDocumentToken, this.PrimaryDocumentName);

            if json.Contains(SaveFormatToken) then
                json.Replace(SaveFormatToken, format(this.SaveFormat, 0, 1))
            else
                json.Add(SaveFormatToken, format(this.SaveFormat, 0, 1));
        end;

        for i := 1 to this.AttachmentCount() do begin
            this.FetchAttachment(i, name, DataType, MimeType, FileName, Description);
            clear(JsonElement);
            JsonElement.Add(NameToken, name);
            JsonElement.Add(DescriptionToken, Description);
            JsonElement.Add(RelationshipToken, format(DataType, 0, 1));
            JsonElement.Add(MimeTypeToken, MimeType);
            JsonElement.Add(FileNameToken, FileName);
            jsonDataArray.Add(JsonElement);
        end;

        if (json.Contains(AttachmentsToken)) then begin
            // The rendering payload already contains attachments. We need to add the new ones to the existing list.

            SourceDataArray := json.GetArray(AttachmentsToken);

            for i := 0 to jsonDataArray.Count() - 1 do begin
                jsonDataArray.Get(i, JsonTokenElement);
                SourceDataArray.Add(JsonTokenElement);
            end;
            json.Replace(AttachmentsToken, SourceDataArray);
        end else
            json.Add(AttachmentsToken, jsonDataArray);

        Clear(jsonDataArray);
        Clear(SourceDataArray);

        if (json.Contains(AdditionalDocumentsToken)) then begin
            SourceDataArray := json.GetArray(AdditionalDocumentsToken);
            for i := 1 to this.AppendCount() do begin
                SourceDataArray.Add(this.AdditionalDocumenNames.Get(i));
            end;
            json.Replace(AdditionalDocumentsToken, SourceDataArray);
        end else begin
            for i := 1 to this.AppendCount() do begin
                jsonDataArray.Add(this.AdditionalDocumenNames.Get(i));
            end;
            json.Add(AdditionalDocumentsToken, jsonDataArray);
        end;

        HasProtection := this.FetchDocumentProtection(user, admin);
        if (json.Contains(ProtectionToken)) then begin

            // The rendering payload already contains protection. This cannot be overwritten. Fail with an error. 
            if (HasProtection) then
                Error(ProtectionOverrideError);
        end
        else begin
            Clear(JsonElement);
            JsonElement.Add(UserToken, user.Unwrap());
            JsonElement.Add(AdminToken, admin.Unwrap());
            Json.Add(ProtectionToken, JsonElement);
        end;
        exit(json);
    end;

    [NonDebuggable]
    local procedure FetchDocumentProtection(var user: SecretText; var admin: SecretText) HasProtection: Boolean
    begin
        user := UserCode;
        admin := AdminCode;
        HasProtection := NOT (user.IsEmpty() AND admin.IsEmpty());
    end;

    local procedure FetchAttachment(AttachmentIndex: Integer; var name: Text; var DataType: enum PdfAttachmentDataRelationShip; var MimeType: Text; var FileName: Text; var Description: Text): Boolean
    begin
        if (AttachmentIndex < 1) then
            Error('Attachment index must be greater than 0.');

        if (AttachmentIndex > this.AttachmentCount()) then
            Error('Attachment index is out of range.');

        name := this.AttachmentNames.Get(AttachmentIndex);
        DataType := this.AttachmentDataTypes.Get(AttachmentIndex);
        MimeType := this.AttachmentMimeTypes.Get(AttachmentIndex);
        fileName := this.AttachmentFilenames.Get(AttachmentIndex);
        Description := this.AttachmentDescriptions.Get(AttachmentIndex);
        if not File.Exists(fileName) then
            exit(false);

        exit(true);
    end;

    // Global data for the codeunit
    var
        // The following properties are used to store the attachment information and must be kept in sync.
        AttachmentNames: List of [Text];
        AttachmentDataTypes: List of [enum PdfAttachmentDataRelationShip];
        AttachmentMimeTypes: List of [Text];
        AttachmentFilenames: List of [Text];
        AttachmentDescriptions: List of [Text];

        // Store the files names to be used for appending to the rendered document.
        AdditionalDocumenNames: List of [Text];

        UserCode: SecretText;
        AdminCode: SecretText;
        PrimaryDocumentName: Text; // The primary document that will be used at the alternative representation of the PDF contents.
        SaveFormat: enum PdfSaveFormat;
}
