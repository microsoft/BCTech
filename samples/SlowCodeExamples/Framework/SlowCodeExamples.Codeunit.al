// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Provides an example of slowly running code.
/// </summary>
codeunit 50100 "Slow Code Examples"
{
    Access = Internal;

    procedure Get(var SlowCodeExample: Record "Slow Code Example")
    var
        ISlowCodeExample: Interface "Slow Code Example";
        CodeExample: Enum "Slow Code Examples";
        ExampleName: Text;
    begin
        foreach CodeExample in CodeExample.Ordinals() do begin
            ISlowCodeExample := CodeExample;
            ExampleName := CodeExample.Names().Get(CodeExample.AsInteger());

            SlowCodeExample."Slow Code Example" := CodeExample;
            SlowCodeExample."Display Text" := ExampleName;
            SlowCodeExample."Entry Type" := SlowCodeExample."Entry Type"::Name;
            SlowCodeExample.Insert();

            SlowCodeExample."Display Text" := 'Run scenario';
            SlowCodeExample."Entry Type" := SlowCodeExample."Entry Type"::RunCode;
            SlowCodeExample.Insert();

            SlowCodeExample."Display Text" := 'Hint';
            SlowCodeExample."Entry Type" := SlowCodeExample."Entry Type"::Hint;
            SlowCodeExample.Insert();
        end;

        SlowCodeExample.FindFirst();
    end;
}