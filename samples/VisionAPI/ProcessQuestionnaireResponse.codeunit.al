codeunit 50000 "Process Questionnaire Response"
{
    TableNo = 50002; // database::"Questionnaire Response Header";
    trigger OnRun()
    var
        jsonresult: JsonObject;
        lines: Dictionary of [text, text];
        OutStr: OutStream;
    begin
        TestField("Profile Code");
        if SendToOCR(Rec, jsonresult) then begin
            clear("OCR json");
            "OCR json".CreateOutStream(OutStr);
            jsonresult.WriteTo(OutStr);
            Modify;
            commit;
            CreateLines(jsonresult, "Profile Code", "Entry No.", lines);
        end else
            error('Could not connect to ocr service');
    end;

    var
        myInt: Integer;

    local procedure SendToOCR(var QuestionnaireResponseHeader: Record "Questionnaire Response Header"; var jsonresult: JsonObject): Boolean
    var
        QuestionnaireOCRSetup: record "Questionnaire OCR Setup";
        TempBlob: Codeunit "Temp Blob";
        uri: Text;
        OCRhttpclient: HttpClient;
        content: HttpContent;
        contentHeaders: HttpHeaders;
        OCRHttpResponse: HttpResponseMessage;
        InStr: InStream;
        OutStr: OutStream;
        httpStatusCode: Integer;
        T0: DateTime;
        t: text;
        ResultLink: text;
        ArrayOfText: array[1] of text;
    begin
        // test
        QuestionnaireResponseHeader.TestField(Picture);
        QuestionnaireOCRSetup.get;
        QuestionnaireOCRSetup.testfield("OCR Url");
        QuestionnaireOCRSetup.TestField("OCR Subscription Key");
        TempBlob.CreateOutStream(OutStr);
        QuestionnaireResponseHeader.Picture.ExportStream(OutStr);

        TempBlob.CreateInStream(InStr);
        content.WriteFrom(InStr);
        content.GetHeaders(contentHeaders);
        contentHeaders.Clear();
        contentHeaders.Add('Content-Type', 'application/octet-stream');

        uri := QuestionnaireOCRSetup."OCR Url" + 'vision/v2.1/recognizeText?mode=Handwritten';
        OCRhttpclient.DefaultRequestHeaders.Add('Ocp-Apim-Subscription-Key', QuestionnaireOCRSetup."OCR Subscription Key");
        if not OCRhttpclient.Post(uri, content, OCRHttpResponse) then
            exit(false);
        OCRHttpResponse.Headers.GetValues('Operation-Location', ArrayOfText);
        ResultLink := ArrayOfText[1];


        if ResultLink = '' then
            error('missing result link');
        T0 := CurrentDateTime;
        while (CurrentDateTime < T0 + 30000) and (httpStatusCode <> 200) do begin
            sleep(1000);
            OCRhttpclient.get(ResultLink, OCRHttpResponse);
            httpStatusCode := OCRHttpResponse.HttpStatusCode;
        end;
        if httpStatusCode <> 200 then
            exit(false);
        OCRHttpResponse.Content.ReadAs(t);
        jsonresult.ReadFrom(t);
        exit(true);
    end;

    local procedure CreateLines(var jsonInput: jsonObject; ProfileCode: code[20]; ResponseNo: integer; var lines: Dictionary of [text, text])
    var
        ProfileQuestionnaireLine: Record "Profile Questionnaire Line";
        PrevQuestionLine: Record "Profile Questionnaire Line";
        PrevAnswerLine: Record "Profile Questionnaire Line";
        TempProfileQuestionnaireLine: Record "Profile Questionnaire Line" temporary;
        QuestionnaireResponseLine: Record "Questionnaire Response Line";
        TypeHelper: Codeunit "Type Helper";
        jtoken: JsonToken;
        jtoken2: JsonToken;
        jarray: JsonArray;
        jarray2: JsonArray;
        RootToken: JsonToken;
        jobject: jsonobject;
        tokenList: List of [jsontoken];
        t: text;
        i: Integer;
        BoundingBox1: array[8] of Integer;
        BoundingBox2: array[8] of Integer;
        QuestionFound: Boolean;
        AnswerFound: Boolean;

    begin
        ProfileQuestionnaireLine.SetRange("Profile Questionnaire Code", ProfileCode);
        if not ProfileQuestionnaireLine.FindSet() then
            exit;
        repeat
            TempProfileQuestionnaireLine := ProfileQuestionnaireLine;
            TempProfileQuestionnaireLine.Insert();
        until ProfileQuestionnaireLine.next = 0;
        QuestionnaireResponseLine.setrange("Header Entry No.", ResponseNo);
        QuestionnaireResponseLine.DeleteAll();

        jsonInput.SelectToken('recognitionResult', jtoken);
        jobject := jtoken.AsObject();
        jobject.SelectToken('lines', jtoken);
        if not jtoken.IsArray then
            exit;
        jarray := jtoken.AsArray();
        foreach jtoken in jarray do begin
            jobject := jtoken.AsObject();
            jobject.SelectToken('text', jtoken2);
            if jtoken2.IsValue then begin
                t := jtoken2.AsValue.AsText();
                t := DelChr(t, '>', '. ');
                if FindQuestionnaireLine(t, TempProfileQuestionnaireLine) then begin
                    GetBoundingBox(jobject, BoundingBox1);
                    if TempProfileQuestionnaireLine.Type = TempProfileQuestionnaireLine.Type::Question then begin
                        QuestionFound := true;
                        AnswerFound := false;
                        PrevQuestionLine := TempProfileQuestionnaireLine;
                    end else
                        if TempProfileQuestionnaireLine.Type = TempProfileQuestionnaireLine.Type::Answer then begin
                            AnswerFound := true;
                            PrevAnswerLine := TempProfileQuestionnaireLine;
                        end;
                end else
                    if QuestionFound then begin
                        GetBoundingBox(jobject, BoundingBox2);
                        if BoundingBoxesOnSameLine(BoundingBox1, BoundingBox2) then begin
                            QuestionnaireResponseLine.init;
                            if AnswerFound then begin // previous description is the answer to the question
                                QuestionnaireResponseLine."Response Text" := CopyStr(PrevAnswerLine.Description, 1, MaxStrLen(QuestionnaireResponseLine."Response Text"));
                            end else // current description is the answer to the question
                                QuestionnaireResponseLine."Response Text" := CopyStr(t, 1, MaxStrLen(QuestionnaireResponseLine."Response Text"));
                            if QuestionnaireResponseLine."Response Text" <> '' then begin
                                QuestionnaireResponseLine."Header Entry No." := ResponseNo;
                                QuestionnaireResponseLine."Line No." := PrevquestionLine."Line No.";
                                QuestionnaireResponseLine."Profile Code" := TempProfileQuestionnaireLine."Profile Questionnaire Code";
                                QuestionnaireResponseLine."Response Selection" := AnswerFound;
                                QuestionnaireResponseLine.Insert(true);
                            end;
                        end;
                        QuestionFound := false;
                        AnswerFound := false;
                    end;
            end;
        end;
    end;

    local procedure FindQuestionnaireLine(Description: text; var TempProfileQuestionnaireLine: Record "Profile Questionnaire Line" temporary): Boolean
    var
        TypeHelper: Codeunit "Type Helper";
        TextDistance: Integer;
    begin
        if strlen(Description) <= 2 then
            exit(false);
        if TempProfileQuestionnaireLine.FindSet() then
            repeat
                if TypeHelper.TextDistance(UpperCase(Description), UpperCase(TempProfileQuestionnaireLine.Description)) <= 2 then
                    exit(true);
            until TempProfileQuestionnaireLine.next = 0;
        exit(false);
    end;

    local procedure GetBoundingBox(var jobject: jsonObject; var BoundingBox: array[8] of Integer): Boolean
    var
        i: Integer;
        jtoken: JsonToken;
        jarray: JsonArray;
    begin
        clear(BoundingBox);
        jobject.SelectToken('boundingBox', jtoken);
        if jtoken.IsArray then begin
            jarray := jtoken.AsArray();
            i := 0;
            foreach jtoken in jarray do begin
                i += 1;
                if i <= ArrayLen(BoundingBox) then
                    BoundingBox[i] := jtoken.AsValue().AsInteger();
            end;
        end;
        exit(i = 8);
    end;

    local procedure BoundingBoxesOnSameLine(BoundingBox1: array[8] of Integer; BoundingBox2: array[8] of Integer): Boolean
    begin
        // Boundingbox: (x,y): left-upper, right-upper, right-lower, left-lower. (0,0) is the top-left  corner of the canvas
        exit((BoundingBox1[2] < BoundingBox2[8]) and (BoundingBox1[8] > BoundingBox2[2])); // only consider Y-values and assume parallel sides
    end;
}