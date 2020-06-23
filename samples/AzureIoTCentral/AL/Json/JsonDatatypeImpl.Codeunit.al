// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------------

codeunit 51301 "Json Datatype Impl."
{
    Access = Internal;

    procedure SetJsonValueAsDatatype(Value: Text; Datatype: Enum JsonDatatype): JsonValue;
    var
        Jvalue: JsonValue;
        Integer: Integer;
        BigInteger: BigInteger;
        Decimal: Decimal;
        Date: Date;
        Datetime: Datetime;
        Boolean: Boolean;
        Text: Text;
        Time: Time;
    begin
        Case Datatype of
            Datatype::Integer:
                begin
                    Evaluate(Integer, Value);
                    JValue.SetValue(Integer);
                end;
            Datatype::BigInteger:
                begin
                    Evaluate(BigInteger, Value);
                    JValue.SetValue(BigInteger);
                end;
            Datatype::Decimal:
                begin
                    Evaluate(Decimal, Value);
                    JValue.SetValue(Decimal);
                end;
            Datatype::Date:
                begin
                    Evaluate(Date, Value);
                    JValue.SetValue(Date);
                end;
            Datatype::Datetime:
                begin
                    Evaluate(Datetime, Value);
                    JValue.SetValue(Datetime);
                end;
            Datatype::Time:
                begin
                    Evaluate(Time, Value);
                    JValue.SetValue(Time);
                end;
            Datatype::Boolean:
                begin
                    Evaluate(Boolean, Value);
                    JValue.SetValue(Boolean);
                end;
            Datatype::Text:
                JValue.SetValue(Value);
            else
                JValue.SetValue(Value);
        end;
        exit(JValue)
    end;

    procedure GetJsonValueDatatype(Jvalue: JsonValue) Datatype: Enum JsonDatatype
    begin
        Case true of
            TryIntegerType(Jvalue):
                exit(Datatype::Integer);
            TryBigIntegerType(Jvalue):
                exit(Datatype::BigInteger);
            TryDecimalType(Jvalue):
                exit(Datatype::Decimal);
            TryDateType(Jvalue):
                exit(Datatype::Date);
            TryDatetimeType(Jvalue):
                exit(Datatype::Datetime);
            TryTimeType(Jvalue):
                exit(Datatype::Time);
            TryBooleanType(Jvalue):
                exit(Datatype::Boolean);
            TryTextType(Jvalue):
                exit(Datatype::Text);
            else
                exit(Datatype::Text);
        end;
    end;

    [TryFunction]
    local procedure TryIntegerType(JValue: JsonValue)
    var
        Integer: Integer;
    begin
        if StrLen(DelChr(format(JValue), '=', '0123456789 ')) <> 0 then
            error('not an integer');
        Integer := JValue.AsInteger();
    end;

    [TryFunction]
    local procedure TryBigIntegerType(JValue: JsonValue)
    var
        BigInteger: BigInteger;
    begin
        if StrLen(DelChr(format(JValue), '=', '0123456789 ')) <> 0 then
            error('not a big integer');
        BigInteger := JValue.AsBigInteger();
    end;

    [TryFunction]
    local procedure TryDecimalType(JValue: JsonValue)
    var
        Decimal: Decimal;
    begin
        if StrLen(DelChr(format(JValue), '=', '.,0123456789 ')) <> 0 then
            error('not a decimal');
        Decimal := JValue.AsDecimal();
    end;

    [TryFunction]
    local procedure TryDateType(JValue: JsonValue)
    var
        Date: Date;
    begin
        Date := JValue.AsDate();
    end;

    [TryFunction]
    local procedure TryDatetimeType(JValue: JsonValue)
    var
        Datetime: Datetime;
    begin
        Datetime := JValue.AsDateTime();
    end;

    [TryFunction]
    local procedure TryTimeType(JValue: JsonValue)
    var
        Time: Time;
    begin
        Time := JValue.AsTime();
    end;

    [TryFunction]
    local procedure TryBooleanType(JValue: JsonValue)
    var
        Boolean: Boolean;
    begin
        Boolean := JValue.AsBoolean();
    end;

    [TryFunction]
    local procedure TryTextType(JValue: JsonValue)
    var
        Text: Text;
    begin
        Text := JValue.AsText();
    end;
}