// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50171 AzureBlobStorage
{
    // 
    // Azure Blob Storage REST Api
    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/blob-service-rest-api
    //
    var
        AccountName: Text;
        ResourceUri: Text;
        [NonDebuggable]
        SharedKey: Text;
        StorageApiVersionTok: label '2015-07-08', locked = true;
        PutVerbTok: label 'PUT', locked = true;
        GetVerbTok: label 'GET', locked = true;
        DeleteVerbTok: label 'DELETE', locked = true;
        StorageNotEnabledErr: label 'Azure Storage is not set-up. Please go to Service Connections to set-up';
        IsInitialized: Boolean;

    //
    // Initialize the Blob Storage Api
    //
    procedure Initialize();
    var
        AzureStorageSetup: Record AzureStorageSetup;
    begin
        if not AzureStorageSetup.Get() then
            Error(StorageNotEnabledErr);

        if not AzureStorageSetup.IsEnabled then
            Error(StorageNotEnabledErr);

        AccountName := AzureStorageSetup.AccountName;
        ResourceUri := StrSubstNo('https://%1.blob.core.windows.net', AzureStorageSetup.AccountName);
        SharedKey := AzureStorageSetup.GetSharedAccessKey();

        IsInitialized := true;
    end;

    local procedure CheckInitialized();
    begin
        if not IsInitialized then
            Initialize();
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/list-containers2
    // 
    procedure ListContainers(var containers: XmlDocument);
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
        content: Text;
    begin
        CheckInitialized();

        InitializeRequest(request, GetVerbTok, '/', 'comp=list');
        client.Send(request, response);
        CheckResponseCode(response);

        response.Content().ReadAs(content);
        XmlDocument.ReadFrom(content, containers);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/list-containers2
    // 
    procedure ListContainers(var Containers: record AzureBlobStorageContainer temporary)
    var
        TypeHelper: Codeunit "Type Helper";
        ContainersXml: XmlDocument;
        ContainerList: XmlNodeList;
        ContainerNode: XmlNode;
        Node: XmlNode;
        Properties: XmlElement;
        i: Integer;
    begin
        ListContainers(ContainersXml);

        containers.DeleteAll();
        if ContainersXml.SelectNodes('/EnumerationResults/Containers/Container', ContainerList) then
            for i := 1 to ContainerList.Count() do begin
                Containers.Init();

                ContainerList.Get(i, ContainerNode);

                ContainerNode.SelectSingleNode('Name', Node);
                Containers.Name := CopyStr(Node.AsXmlElement().InnerText(), 1, MaxStrLen(Containers.Name));

                if ContainerNode.SelectSingleNode('Properties', Node) then begin
                    Properties := Node.AsXmlElement();

                    if Properties.SelectSingleNode('Last-Modified', Node) then
                        Containers."Last-Modified" := TypeHelper.EvaluateUTCDateTime(Node.AsXmlElement().InnerText());

                    if Properties.SelectSingleNode('Etag', Node) then
                        Containers.Etag := CopyStr(Node.AsXmlElement().InnerText(), 1, MaxStrLen(Containers.Etag));

                    if Properties.SelectSingleNode('LeaseState', Node) then
                        Evaluate(Containers.LeaseState, Node.AsXmlElement().InnerText());

                    if Properties.SelectSingleNode('LeaseStatus', Node) then
                        Evaluate(Containers.LeaseStatus, Node.AsXmlElement().InnerText());
                end;

                Containers.Insert();
            end;
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/create-container
    // 
    procedure CreateContainer(ResourcePath: Text);
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
    begin
        CheckInitialized();

        InitializeRequest(request, PutVerbTok, ResourcePath, 'restype=container');
        client.Send(request, response);
        CheckResponseCode(response);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/delete-container
    // 
    procedure DeleteContainer(ResourcePath: Text): Boolean;
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
    begin
        CheckInitialized();

        InitializeRequest(request, DeleteVerbTok, ResourcePath, 'restype=container');
        client.Send(request, response);
        CheckResponseCode(response);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/list-blobs
    // 
    procedure ListBlobs(ResourcePath: Text; Prefix: Text; var blobs: XmlDocument);
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
        content: Text;
        parameters: Text;
    begin
        CheckInitialized();

        parameters := 'restype=container&comp=list';
        if prefix <> '' then
            parameters += '&prefix=' + Prefix; // TODO: Escaping of Prefix?
        InitializeRequest(request, GetVerbTok, ResourcePath, parameters);
        client.Send(request, response);
        CheckResponseCode(response);

        response.Content().ReadAs(content);
        XmlDocument.ReadFrom(content, blobs);
    end;

    procedure ListBlobs(ResourcePath: Text; var blobs: XmlDocument);
    begin
        ListBlobs(ResourcePath, '', blobs);
    end;

    procedure ListBlobs(ResourcePath: Text; Prefix: Text; var Blobs: record AzureBlobStorageBlob)
    var
        TypeHelper: Codeunit "Type Helper";
        BlobsXml: XmlDocument;
        BlobList: XmlNodeList;
        BlobNode: XmlNode;
        Node: XmlNode;
        Properties: XmlElement;
        i: Integer;
    begin
        ListBlobs(ResourcePath, Prefix, BlobsXml);

        Blobs.DeleteAll();
        if BlobsXml.SelectNodes('/EnumerationResults/Blobs/Blob', BlobList) then
            for i := 1 to BlobList.Count() do begin
                Blobs.Init();

                BlobList.Get(i, BlobNode);

                BlobNode.SelectSingleNode('Name', Node);
                Blobs.Name := CopyStr(Node.AsXmlElement().InnerText(), 1, MaxStrLen(Blobs.Name));

                if BlobNode.SelectSingleNode('Properties', Node) then begin
                    Properties := Node.AsXmlElement();

                    if Properties.SelectSingleNode('Last-Modified', Node) then
                        Blobs."Last-Modified" := TypeHelper.EvaluateUTCDateTime(Node.AsXmlElement().InnerText());

                    if Properties.SelectSingleNode('Etag', Node) then
                        Blobs.Etag := CopyStr(Node.AsXmlElement().InnerText(), 1, MaxStrLen(Blobs.Etag));

                    if Properties.SelectSingleNode('LeaseState', Node) then
                        Evaluate(Blobs.LeaseState, Node.AsXmlElement().InnerText());

                    if Properties.SelectSingleNode('LeaseStatus', Node) then
                        Evaluate(Blobs.LeaseStatus, Node.AsXmlElement().InnerText());

                    if Properties.SelectSingleNode('Content-Length', Node) then
                        Evaluate(Blobs."Content-Length", Node.AsXmlElement().InnerText());

                    if Properties.SelectSingleNode('Content-Type', Node) then
                        Blobs."Content-Type" := CopyStr(Node.AsXmlElement().InnerText(), 1, MaxStrLen(Blobs.Etag));

                    if Properties.SelectSingleNode('Content-Encoding', Node) then
                        Blobs."Content-Encoding" := CopyStr(Node.AsXmlElement().InnerText(), 1, MaxStrLen(Blobs.Etag));
                end;

                Blobs.Container := CopyStr(ResourcePath, 1, MaxStrLen(Blobs.Container));

                Blobs.Insert();
            end;
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/get-blob
    // 
    procedure GetBlob(ResourcePath: Text; var blob: InStream): Boolean;
    var
        response: HttpResponseMessage;
    begin
        GetBlob(ResourcePath, response);

        response.Content().ReadAs(blob);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/get-blob
    // 
    procedure GetBlob(ResourcePath: Text; var blob: Text): Boolean;
    var
        response: HttpResponseMessage;
    begin
        GetBlob(ResourcePath, response);

        response.Content().ReadAs(blob);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/get-blob
    // 
    local procedure GetBlob(ResourcePath: Text; var response: HttpResponseMessage)
    var
        client: HttpClient;
        request: HttpRequestMessage;
    begin
        CheckInitialized();

        InitializeRequest(request, GetVerbTok, ResourcePath, '');
        client.Send(request, response);
        CheckResponseCode(response);
    end;


    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/delete-blob
    // 
    procedure DeleteBlob(ResourcePath: Text)
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
    begin
        CheckInitialized();

        InitializeRequest(request, DeleteVerbTok, ResourcePath, '');
        client.Send(request, response);
        CheckResponseCode(response);
    end;

    procedure DeleteBlobs(ResourcePath: Text; Prefix: Text);
    var
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
        BlobList: XmlNodeList;
        BlobNode: XmlNode;
        Node: XmlNode;
        BlobsXml: XmlDocument;
        i: Integer;
        DeleteResourcePath: Text;
    begin
        ListBlobs(ResourcePath, Prefix, BlobsXml);

        if BlobsXml.SelectNodes('/EnumerationResults/Blobs/Blob', BlobList) then begin
            for i := 1 to BlobList.Count() do begin
                BlobList.Get(i, BlobNode);
                BlobNode.SelectSingleNode('Name', Node);

                DeleteResourcePath := ResourcePath + '/' + Node.AsXmlElement().InnerText();
                DeleteBlob(DeleteResourcePath);
            end;
        end;
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/put-blob
    // 
    procedure PutBlob(ResourcePath: Text; blob: InStream; ContentType: Text)
    var
        TempBlob: codeunit "Temp Blob";
        request: HttpRequestMessage;
        ins: InStream;
        outs: OutStream;
    begin
        TempBlob.CreateOutStream(outs);
        CopyStream(outs, blob);
        TempBlob.CreateInStream(ins);

        request.Content().WriteFrom(ins);
        PutBlob(ResourcePath, TempBlob.Length(), ContentType, request);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/put-blob
    // 
    procedure PutBlob(ResourcePath: Text; blob: codeunit "Temp Blob"; ContentType: Text)
    var
        request: HttpRequestMessage;
        ins: InStream;
    begin
        blob.CreateInStream(ins);
        request.Content().WriteFrom(ins);
        PutBlob(ResourcePath, blob.Length(), ContentType, request);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/put-blob
    // 
    procedure PutBlob(ResourcePath: Text; blob: Text)
    begin
        PutBlob(ResourcePath, blob, 'text/plain; charset=utf-8')
    end;
    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/put-blob
    // 
    procedure PutBlob(ResourcePath: Text; blob: Text; ContentType: Text)
    var
        request: HttpRequestMessage;
    begin
        request.Content().WriteFrom(blob);
        PutBlob(ResourcePath, StrLen(blob), ContentType, request);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/put-blob
    // 
    local procedure PutBlob(ResourcePath: Text; ContentLength: Integer; ContentType: Text; var request: HttpRequestMessage)
    var
        client: HttpClient;
        response: HttpResponseMessage;
        headers: HttpHeaders;
    begin
        CheckInitialized();

        request.Content().GetHeaders(headers);
        headers.Add('Content-Length', Format(ContentLength));
        if headers.Contains('Content-Type') then
            headers.Remove('Content-Type');
        headers.Add('Content-Type', ContentType);

        InitializeRequest(request, PutVerbTok, ResourcePath, '', 'x-ms-blob-type:BlockBlob', Format(ContentLength), ContentType);
        client.Send(request, response);
        CheckResponseCode(response);
    end;

    //
    // Api Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/copy-blob
    // 
    procedure CopyBlob(SourcePath: Text; DestinationPath: Text)
    var
        TypeHelper: codeunit "Type Helper";
        client: HttpClient;
        request: HttpRequestMessage;
        response: HttpResponseMessage;
        headers: HttpHeaders;
        SourceUri: Text;
    begin
        CheckInitialized();

        SourceUri := ResourceUri + SourcePath;

        InitializeRequest(request, PutVerbTok, DestinationPath, '', 'x-ms-copy-source:' + SourceUri, '', '');
        client.Send(request, response);
        CheckResponseCode(response);
    end;

    local procedure CheckResponseCode(response: HttpResponseMessage)
    begin
        if not response.IsSuccessStatusCode() then
            Error(response.ReasonPhrase());
    end;

    local procedure InitializeRequest(var request: HttpRequestMessage; methodVerb: Text; resourcePath: Text; parameters: Text)
    begin
        InitializeRequest(request, methodVerb, resourcePath, parameters, '', '', '');
    end;

    local procedure InitializeRequest(var request: HttpRequestMessage; methodVerb: Text; resourcePath: Text; parameters: Text; xHeaders: Text; ContentLength: Text; ContentType: Text)
    var
        TypeHelper: codeunit "Type Helper";
        headers: HttpHeaders;
        UtcDateTime: Text;
        token: Text;
        xHeaderList: List of [Text];
        header: Text;
        index: Integer;
    begin

        UtcDateTime := TypeHelper.GetCurrUTCDateTimeAsText();

        request.GetHeaders(headers);

        // Add x-ms-??? headers
        headers.Add('x-ms-date', UtcDateTime);
        headers.Add('x-ms-version', StorageApiVersionTok);
        if xHeaders <> '' then begin
            xHeaderList := xHeaders.Split(';');
            foreach header in xHeaderList do begin
                index := header.IndexOf(':');
                token := header.Substring(1, index - 1);
                token := header.Substring(index);
                headers.Add(header.Substring(1, index - 1), header.Substring(index + 1));
            end;
        end;

        // Add Authorization header
        resourcePath := TypeHelper.UriEscapeDataString(resourcePath).Replace('%2F', '/');

        token := GetSasToken(methodVerb, ResourcePath, UtcDateTime, ContentLength, ContentType, CreateCanonicalizedParameters(parameters), xHeaderList);
        headers.Add('Authorization', token);

        request.Method := methodVerb;
        if parameters <> '' then
            resourcePath := resourcePath + '?' + parameters.Replace('/', '%2F');

        request.SetRequestUri(ResourceUri + resourcePath);
    end;

    local procedure CreateCanonicalizedParameters(parameters: Text): Text;
    var
        builder: TextBuilder;
        paramList: List of [Text];
        i: Integer;
        Cr: Text[1];
    begin
        if parameters = '' then
            exit;

        Cr[1] := 10;
        paramList := parameters.Split('&');
        SortList(paramList);

        for i := 1 to paramList.Count() do begin
            if builder.Length() > 0 then
                builder.Append(Cr);
            builder.Append(paramList.Get(i).Replace('=', ':'));
        end;

        exit(builder.ToText());
    end;

    local procedure SortList(var list: List of [Text])
    var
        i: Integer;
        j: Integer;
        s: Text;
    begin
        for i := 1 to list.Count() - 1 do
            for j := i + 1 to list.Count() do
                if list.Get(i) > list.Get(j) then begin
                    s := list.Get(i);
                    list.Set(i, list.Get(j));
                    list.Set(j, s);
                end;
    end;

    //
    // Shared Access Key Generation
    //
    // Documentation: https://learn.microsoft.com/en-us/rest/api/storageservices/authorize-with-shared-key
    //
    // Some digested documentation here: https://www.red-gate.com/simple-talk/cloud/platform-as-a-service/azure-blob-storage-part-5-blob-storage-rest-api/
    //
    local procedure GetSasToken(Verb: Text; ResourcePath: Text; Date: Text; ContentLength: Text; ContentType: Text; query: Text; xHeaderList: List of [Text]): Text;
    var
        EncryptionManagement: codeunit "Cryptography Management";
        Cr: Text[1];
        stringToSign: TextBuilder;
        Signature: Text;
        header: Text;
    begin
        if ContentLength = '0' then
            ContentLength := '';

        Cr[1] := 10;

        stringToSign.Append(Verb + Cr +
              Cr + /*Content-Encoding*/
              Cr + /*Content-Language*/
              ContentLength + Cr + /*Content-Length*/
              Cr + /*Content-MD5*/
              ContentType + Cr + /*Content-Type*/
              Cr + /*Date*/
              Cr + /*If-Modified-Since*/
              Cr + /*If-Match*/
              Cr + /*If-None-Match*/
              Cr + /*If-Unmodified-Since*/
              Cr); /*Range*/

        xHeaderList.Add('x-ms-date:' + Date);
        xHeaderList.Add('x-ms-version:' + StorageApiVersionTok);
        SortList(xHeaderList);
        foreach header in xHeaderList do begin
            stringToSign.Append(header);
            stringToSign.Append(Cr);
        end;

        stringToSign.Append('/' + AccountName + ResourcePath);
        if query <> '' then
            stringToSign.Append(Cr + query);

        header := stringToSign.ToText();
        signature := EncryptionManagement.GenerateBase64KeyedHashAsBase64String(stringToSign.ToText(), SharedKey, 2 /* HMACSHA256 */);

        exit(StrSubstNo('SharedKey %1:%2', AccountName, Signature));
    end;

    procedure GetContentTypeFromFileName(Filename: Text): Text;
    var
        Extension: Text;
        IndexOfLastDot: Integer;
    begin
        IndexOfLastDot := Filename.LastIndexOf('.');
        if IndexOfLastDot < 1 then
            exit('');

        Extension := Filename.Substring(IndexOfLastDot + 1).ToLower();
        case Extension of
            'aac':
                exit('audio/aac'); // AAC audio
            'abw':
                exit('application/x-abiword'); // AbiWord document
            'arc':
                exit('application/x-freearc'); // Archive document (multiple files embedded)
            'avi':
                exit('video/x-msvideo'); // AVI: Audio Video Interleave
            'azw':
                exit('application/vnd.amazon.ebook'); // Amazon Kindle eBook format
            'bin':
                exit('application/octet-stream'); // Any kind of binary data
            'bmp':
                exit('image/bmp'); // Windows OS/2 Bitmap Graphics
            'bz':
                exit('application/x-bzip'); // BZip archive
            'bz2':
                exit('application/x-bzip2'); // BZip2 archive
            'csh':
                exit('application/x-csh'); // C-Shell script
            'css':
                exit('text/css'); // Cascading Style Sheets (CSS)
            'csv':
                exit('text/csv'); // Comma-separated values (CSV)
            'doc':
                exit('application/msword'); // Microsoft Word
            'docx':
                exit('application/vnd.openxmlformats-officedocument.wordprocessingml.document'); // Microsoft Word (OpenXML)
            'eot':
                exit('application/vnd.ms-fontobject'); // MS Embedded OpenType fonts
            'epub':
                exit('application/epub+zip'); // Electronic publication (EPUB)
            'gz':
                exit('application/gzip'); // GZip Compressed Archive
            'gif':
                exit('image/gif'); // Graphics Interchange Format (GIF)
            'htm':
                exit('text/html'); // HyperText Markup Language (HTML)
            'html':
                exit('text/html'); // HyperText Markup Language (HTML)
            'ico':
                exit('image/vnd.microsoft.icon'); // Icon format
            'ics':
                exit('text/calendar'); // iCalendar format
            'jar':
                exit('application/java-archive'); // Java Archive (JAR)
            'jpeg':
                exit('image/jpeg'); // JPEG images
            'jpg':
                exit('image/jpeg'); // JPG images
            'js':
                exit('text/javascript'); // JavaScript
            'json':
                exit('application/json'); // JSON format
            'jsonld':
                exit('application/ld+json'); // JSON-LD format
            'mid':
                exit('audio/midi audio/x-midi'); // Musical Instrument Digital Interface (MIDI)
            'midi':
                exit('audio/midi audio/x-midi'); // Musical Instrument Digital Interface (MIDI)
            'mjs':
                exit('text/javascript'); // JavaScript module
            'mp3':
                exit('audio/mpeg'); // MP3 audio
            'mpeg':
                exit('video/mpeg'); // MPEG Video
            'mpkg':
                exit('application/vnd.apple.installer+xml'); // Apple Installer Package
            'odp':
                exit('application/vnd.oasis.opendocument.presentation'); // OpenDocument presentation document
            'ods':
                exit('application/vnd.oasis.opendocument.spreadsheet'); // OpenDocument spreadsheet document
            'odt':
                exit('application/vnd.oasis.opendocument.text'); // OpenDocument text document
            'oga':
                exit('audio/ogg'); // OGG audio
            'ogv':
                exit('video/ogg'); // OGG video
            'ogx':
                exit('application/ogg'); // OGG
            'opus':
                exit('audio/opus'); // Opus audio
            'otf':
                exit('font/otf'); // OpenType font
            'png':
                exit('image/png'); // Portable Network Graphics
            'pdf':
                exit('application/pdf'); // Adobe Portable Document Format (PDF)
            'php':
                exit('application/php'); // Hypertext Preprocessor (Personal Home Page)
            'ppt':
                exit('application/vnd.ms-powerpoint'); // Microsoft PowerPoint
            'pptx':
                exit('application/vnd.openxmlformats-officedocument.presentationml.presentation'); // Microsoft PowerPoint (OpenXML)
            'rar':
                exit('application/x-rar-compressed'); // RAR archive
            'rtf':
                exit('application/rtf'); // Rich Text Format (RTF)
            'sh':
                exit('application/x-sh'); // Bourne shell script
            'svg':
                exit('image/svg+xml'); // Scalable Vector Graphics (SVG)
            'swf':
                exit('application/x-shockwave-flash'); // Small web format (SWF) or Adobe Flash document
            'tar':
                exit('application/x-tar'); // Tape Archive (TAR)
            'tif':
                exit('image/tiff'); // Tagged Image File Format (TIFF)
            'tiff':
                exit('image/tiff'); // Tagged Image File Format (TIFF)
            'ts':
                exit('video/mp2t'); // MPEG transport stream
            'ttf':
                exit('font/ttf'); // TrueType Font
            'txt':
                exit('text/plain'); // Text, (generally ASCII or ISO 8859-n)
            'vsd':
                exit('application/vnd.visio'); // Microsoft Visio
            'wav':
                exit('audio/wav'); // Waveform Audio Format
            'weba':
                exit('audio/webm'); // WEBM audio
            'webm':
                exit('video/webm'); // WEBM video
            'webp':
                exit('image/webp'); // WEBP image
            'woff':
                exit('font/woff'); // Web Open Font Format (WOFF)
            'woff2':
                exit('font/woff2'); // Web Open Font Format (WOFF)
            'xhtml':
                exit('application/xhtml+xml'); // XHTML
            'xls':
                exit('application/vnd.ms-excel'); // Microsoft Excel
            'xlsx':
                exit('application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'); // Microsoft Excel (OpenXML)
            'xml':
                exit('application/xml if not readable from casual users (RFC 3023, section 3)'); // XML
            'xul':
                exit('application/vnd.mozilla.xul+xml'); // XUL
            'zip':
                exit('application/zip'); // ZIP archive
            '3gp':
                exit('video/3gpp'); // 3GPP audio/video container
            '3g2':
                exit('video/3gpp2'); // 3GPP2 audio/video container
            '7z':
                exit('application/x-7z-compressed'); // 7-zip archive
            else
                exit('');
        end;
    end;
}
