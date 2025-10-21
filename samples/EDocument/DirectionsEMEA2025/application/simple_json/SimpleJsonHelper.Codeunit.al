// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Microsoft.EServices.EDocument.Format;

using System.Utilities;

/// <summary>
/// Helper codeunit with pre-written methods for JSON operations.
/// These methods are provided to save time during the workshop.
/// </summary>
codeunit 81001 "SimpleJson Helper"
{
    Access = Internal;

    /// <summary>
    /// Writes a JSON property to the output stream.
    /// </summary>
    /// <param name="OutStr">The output stream to write to.</param>
    /// <param name="PropertyName">The name of the JSON property.</param>
    /// <param name="PropertyValue">The value of the JSON property.</param>
    procedure WriteJsonProperty(var OutStr: OutStream; PropertyName: Text; PropertyValue: Text)
    begin
        OutStr.WriteText('"' + PropertyName + '": "' + PropertyValue + '"');
    end;

    /// <summary>
    /// Writes a JSON numeric property to the output stream.
    /// </summary>
    procedure WriteJsonNumericProperty(var OutStr: OutStream; PropertyName: Text; PropertyValue: Decimal)
    begin
        OutStr.WriteText('"' + PropertyName + '": ' + Format(PropertyValue, 0, 9));
    end;

    /// <summary>
    /// Gets a text value from a JSON token.
    /// </summary>
    procedure GetJsonTokenValue(JsonToken: JsonToken): Text
    var
        JsonValue: JsonValue;
    begin
        if JsonToken.IsValue then begin
            JsonValue := JsonToken.AsValue();
            exit(JsonValue.AsText());
        end;
        exit('');
    end;

    /// <summary>
    /// Gets a decimal value from a JSON token.
    /// </summary>
    procedure GetJsonTokenDecimal(JsonToken: JsonToken): Decimal
    var
        JsonValue: JsonValue;
        DecimalValue: Decimal;
    begin
        if JsonToken.IsValue then begin
            JsonValue := JsonToken.AsValue();
            if JsonValue.AsDecimal(DecimalValue) then
                exit(DecimalValue);
        end;
        exit(0);
    end;

    /// <summary>
    /// Gets a date value from a JSON token.
    /// </summary>
    procedure GetJsonTokenDate(JsonToken: JsonToken): Date
    var
        JsonValue: JsonValue;
        DateValue: Date;
    begin
        if JsonToken.IsValue then begin
            JsonValue := JsonToken.AsValue();
            if Evaluate(DateValue, JsonValue.AsText()) then
                exit(DateValue);
        end;
        exit(0D);
    end;

    /// <summary>
    /// Selects a JSON token from a JSON object by path.
    /// </summary>
    procedure SelectJsonToken(JsonObject: JsonObject; Path: Text; var JsonToken: JsonToken): Boolean
    begin
        exit(JsonObject.SelectToken(Path, JsonToken));
    end;

    /// <summary>
    /// Reads JSON content from a TempBlob into a JsonObject.
    /// </summary>
    procedure ReadJsonFromBlob(var TempBlob: Codeunit "Temp Blob"; var JsonObject: JsonObject): Boolean
    var
        InStr: InStream;
        JsonText: Text;
    begin
        TempBlob.CreateInStream(InStr, TextEncoding::UTF8);
        InStr.ReadText(JsonText);
        exit(JsonObject.ReadFrom(JsonText));
    end;
}
