// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
///
/// </summary
codeunit 51300 Json
{
    Access = Public;

    /// <summary>
    ///
    /// </summary
    procedure SetJsonValueAsDatatype(Value: Text; Datatype: Enum JsonDatatype): JsonValue;
    var
        JsonDatatypeImpl: Codeunit "Json Datatype Impl.";
    begin
        exit(JsonDatatypeImpl.SetJsonValueAsDatatype(Value, Datatype))
    end;

    /// <summary>
    ///
    /// </summary
    procedure GetJsonValueDatatype(Jvalue: JsonValue) Datatype: Enum JsonDatatype
    var
        JsonDatatypeImpl: Codeunit "Json Datatype Impl.";
    begin
        exit(JsonDatatypeImpl.GetJsonValueDatatype(Jvalue))
    end;

    /// <summary>
    ///
    /// </summary
    procedure ParseJObjectToKeyValuePair(JObject: JsonObject; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    var
        JsonKeyValuePairImpl: Codeunit "Json Key/Value Pair Impl.";
    begin
        JsonKeyValuePairImpl.ParseJObjectToKeyValuePair(JObject, JsonKeyValuePair)
    end;

    /// <summary>
    ///
    /// </summary
    procedure CreateJsonFromKeyValuePair(var JsonKeyValuePair: Record "Json Key/Value Pair" temporary): JsonObject
    var
        JsonKeyValuePairImpl: Codeunit "Json Key/Value Pair Impl.";
    begin
        exit(JsonKeyValuePairImpl.CreateJsonFromKeyValuePair(JsonKeyValuePair))
    end;

    /// <summary>
    ///
    /// </summary
    procedure CreateJsonFromKeyValuePairPartial(var JsonKeyValuePair: Record "Json Key/Value Pair" temporary): JsonObject
    var
        JsonKeyValuePairImpl: Codeunit "Json Key/Value Pair Impl.";
    begin
        exit(JsonKeyValuePairImpl.CreateJsonFromKeyValuePairPartial(JsonKeyValuePair))
    end;
}