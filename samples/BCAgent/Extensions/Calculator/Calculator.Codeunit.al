// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50130 Calculator
{
    var
        ServiceBusRelay: codeunit AzureServiceBusRelay;
        CalculatorPluginNameTok: Label '/calculator/V1.0', Locked = true;
        AddFuncDefTok: Label '/Add?a=%1&b=%2', Locked = true;
        SubtractFuncDefTok: Label '/Subtract?a=%1&b=%2', Locked = true;

    procedure Add(a: Decimal; b: Decimal) Result: Decimal;
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(CalculatorPluginNameTok + StrSubstNo(AddFuncDefTok, Format(a, 0, 9), Format(b, 0, 9)), ResultText);
        Evaluate(Result, ResultText, 9);
    end;

    procedure Subtract(a: Decimal; b: Decimal) Result: Decimal;
    var
        ResultText: Text;
    begin
        ServiceBusRelay.Get(CalculatorPluginNameTok + StrSubstNo(SubtractFuncDefTok, Format(a, 0, 9), Format(b, 0, 9)), ResultText);
        Evaluate(Result, ResultText, 9);
    end;
}
