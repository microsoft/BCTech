// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

codeunit 51302 "Json Key/Value Pair Impl."
{
    Access = Internal;

    procedure ParseJObjectToKeyValuePair(JObject: JsonObject; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary)
    begin
        JsonKeyValuePair.Reset(); // clear filter before delete all
        JsonKeyValuePair.DeleteAll();
        Clear(JsonKeyValuePair); // clear current rec values to reset Entry No.

        ParseJObjectToKeyValuePairInternal(JObject, JsonKeyValuePair, 0);
        if JsonKeyValuePair.findfirst then;
    end;

    local procedure ParseJObjectToKeyValuePairInternal(JObject: JsonObject; var JsonKeyValuePair: Record "Json Key/Value Pair" temporary; Indent: Integer)
    var
        JsonDatatypeImpl: Codeunit "Json Datatype Impl.";
        JToken: JsonToken;
        JValue: JsonValue;
        KeyList: List of [Text];
        JKey: Text;
    begin
        if Indent = 0 then begin
            JsonKeyValuePair.Reset(); // clear filter before delete all
            JsonKeyValuePair.DeleteAll();
            Clear(JsonKeyValuePair); // clear current rec values to reset Entry No.
        end;

        KeyList := JObject.Keys;
        foreach JKey in KeyList do begin
            JObject.Get(JKey, JToken);
            JsonKeyValuePair.Init(); // recursive function so use init
            JsonKeyValuePair."Entry No." += 1;
            JsonKeyValuePair."Key" := JKey;
            JsonKeyValuePair.Indent := Indent * 2;
            if JToken.IsValue then begin
                JValue := Jtoken.AsValue();
                JsonKeyValuePair.ValueType := JsonDatatypeImpl.GetJsonValueDatatype(JValue);
                case JsonKeyValuePair.ValueType of
                    JsonKeyValuePair.ValueType::BigInteger:
                        JsonKeyValuePair.Value := Format(JValue.AsBigInteger());
                    JsonKeyValuePair.ValueType::Boolean:
                        JsonKeyValuePair.Value := Format(JValue.AsBoolean());
                    JsonKeyValuePair.ValueType::Byte:
                        JsonKeyValuePair.Value := Format(JValue.AsByte());
                    JsonKeyValuePair.ValueType::Char:
                        JsonKeyValuePair.Value := Format(JValue.AsChar());
                    JsonKeyValuePair.ValueType::Code:
                        JsonKeyValuePair.Value := Format(JValue.AsCode());
                    JsonKeyValuePair.ValueType::Date:
                        JsonKeyValuePair.Value := Format(JValue.AsDate());
                    JsonKeyValuePair.ValueType::Datetime:
                        JsonKeyValuePair.Value := Format(JValue.AsDateTime());
                    JsonKeyValuePair.ValueType::Decimal:
                        JsonKeyValuePair.Value := Format(JValue.AsDecimal());
                    JsonKeyValuePair.ValueType::Duration:
                        JsonKeyValuePair.Value := Format(JValue.AsDuration());
                    JsonKeyValuePair.ValueType::Integer:
                        JsonKeyValuePair.Value := Format(JValue.AsInteger());
                    JsonKeyValuePair.ValueType::Option:
                        JsonKeyValuePair.Value := Format(JValue.AsOption());
                    JsonKeyValuePair.ValueType::Text:
                        JsonKeyValuePair.Value := DelChr(Format(JValue.AsText()), '=', '"');
                    JsonKeyValuePair.ValueType::Time:
                        JsonKeyValuePair.Value := Format(JValue.AsTime());
                    else
                        JsonKeyValuePair.Value := Format(JValue);
                end;
                JsonKeyValuePair.Insert;
            end else begin
                JsonKeyValuePair.Insert;
                ParseJObjectToKeyValuePairInternal(JToken.AsObject(), JsonKeyValuePair, Indent + 1);
            end;
        end;
    end;

    procedure CreateJsonFromKeyValuePair(var JsonKeyValuePair: Record "Json Key/Value Pair" temporary): JsonObject
    begin
        if JsonKeyValuePair.findset then
            exit(CreateJsonFromKeyValuePairPartial(JsonKeyValuePair))
    end;

    procedure CreateJsonFromKeyValuePairPartial(var JsonKeyValuePair: Record "Json Key/Value Pair" temporary) JObject: JsonObject
    var
        JKey: Text;
    begin
        JKey := JsonKeyValuePair.Key;
        if JsonKeyValuePair.Next() <> 0 then
            repeat
                JObject.Add(JKey, CreateJsonTokenFromKeyValuePair(JsonKeyValuePair));
                JKey := JsonKeyValuePair.Key;
            until JsonKeyValuePair.Next() = 0;
    end;

    local procedure CreateJsonTokenFromKeyValuePair(var JsonKeyValuePair: Record "Json Key/Value Pair" temporary): JsonToken
    var
        JsonDatatypeImpl: Codeunit "Json Datatype Impl.";
        JObject: JsonObject;
        Jvalue: JsonValue;
        JKey: Text;
    begin
        JKey := JsonKeyValuePair.Key;
        if JsonKeyValuePair.Value <> '' then begin
            JValue := JsonDatatypeImpl.SetJsonValueAsDatatype(JsonKeyValuePair.Value, JsonKeyValuePair.ValueType);
            JObject.Add(Jkey, JValue.AsToken());
            JsonKeyValuePair.Next()
        end else begin
            JsonKeyValuePair.Next();
            JObject.add(Jkey, CreateJsonTokenFromKeyValuePair(JsonKeyValuePair));
        end;
        exit(JObject.AsToken);
    end;

    [EventSubscriber(ObjectType::Table, Database::"Json Key/Value Pair", 'OnBeforeInsertEvent', '', true, true)]
    local procedure EnsureTemporaryOnBeforeInsertJsonKeyValuePair(var Rec: Record "Json Key/Value Pair")
    begin
        if not Rec.IsTemporary then
            error('%1 record variable must be temporary.', Rec.TableCaption)
    end;
}