// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Helper functions for dealing with CTF challenges.
/// </summary>
codeunit 50100 "CTF Challenges"
{
    Access = Internal;

    procedure Get(var CTFChallenge: Record "CTF Challenge")
    var
        ICTFChallenge: Interface "CTF Challenge";
        CodeExample: Enum "CTF Challenge";
        ExampleName: Text;
    begin
        foreach CodeExample in CodeExample.Ordinals() do begin
            ICTFChallenge := CodeExample;
            ExampleName := CodeExample.Names().Get(CodeExample.AsInteger());

            CTFChallenge."CTF Challenge" := CodeExample;
            CTFChallenge."Display Text" := ExampleName;
            CTFChallenge."Entry Type" := CTFChallenge."Entry Type"::Name;
            CTFChallenge.Insert();

            CTFChallenge."Display Text" := 'Run scenario';
            CTFChallenge."Entry Type" := CTFChallenge."Entry Type"::RunCode;
            CTFChallenge.Insert();

            CTFChallenge."Display Text" := 'Hint';
            CTFChallenge."Entry Type" := CTFChallenge."Entry Type"::Hint;
            CTFChallenge.Insert();
        end;

        CTFChallenge.FindFirst();
    end;
}