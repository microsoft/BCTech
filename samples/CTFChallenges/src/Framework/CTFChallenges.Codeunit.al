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
        CategoriesWithChallenges: Dictionary of [Enum "CTF Category", List of [Enum "CTF Challenge"]];
        Challenge: Enum "CTF Challenge";
        CurrentCategory: Enum "CTF Category";
        CurrentCategoryText: Text;
        TreeViewIndex: Integer;
    begin
        CTFChallenge.DeleteAll();

        foreach Challenge in Challenge.Ordinals() do
            AddChallengePerCategory(CategoriesWithChallenges, Challenge);

        foreach CurrentCategory in CategoriesWithChallenges.Keys() do begin
            CurrentCategoryText := CurrentCategory.Names.Get(CurrentCategory.AsInteger());
            InsertTreeViewRow(CTFChallenge, Challenge, CurrentCategoryText, CTFChallenge."Entry Type"::Category, TreeViewIndex);

            foreach Challenge in CategoriesWithChallenges.Get(CurrentCategory) do
                InsertCTFChallenge(CTFChallenge, Challenge, TreeViewIndex);
        end;

        CTFChallenge.FindFirst();
    end;

    procedure Get(var CTFChallenge: Record "CTF Challenge"; CTFChallengeFilter: Enum "CTF Challenge")
    var
        TreeViewIndex: Integer;
    begin
        CTFChallenge.DeleteAll();
        InsertCTFChallenge(CTFChallenge, CTFChallengeFilter, TreeViewIndex);
        CTFChallenge.FindFirst();
    end;

    local procedure InsertCTFChallenge(var CTFChallenge: Record "CTF Challenge"; Challenge: Enum "CTF Challenge"; var TreeViewIndex: Integer)
    var
        ICTFChallenge: Interface "CTF Challenge";
        ChallengeName: Text;
        HintIndex: Integer;
    begin
        ICTFChallenge := Challenge;
        ChallengeName := Challenge.Names().Get(Challenge.AsInteger());

        InsertTreeViewRow(CTFChallenge, Challenge, ChallengeName, CTFChallenge."Entry Type"::Name, TreeViewIndex);
        InsertTreeViewRow(CTFChallenge, Challenge, 'Run scenario', CTFChallenge."Entry Type"::RunCode, TreeViewIndex);

        for HintIndex := 1 to ICTFChallenge.GetHints().Count() do
            InsertTreeViewRow(CTFChallenge, Challenge, StrSubstNo('Hint %1', HintIndex), CTFChallenge."Entry Type"::Hint, TreeViewIndex);
    end;

    local procedure InsertTreeViewRow(var CTFChallenge: Record "CTF Challenge"; Challenge: Enum "CTF Challenge"; DisplayText: Text; EntryType: Option Category,Name,RunCode,Hint; var TreeViewIndex: Integer)
    begin
        CTFChallenge."No." := TreeViewIndex;
        CTFChallenge."CTF Challenge" := Challenge;
        CTFChallenge."Display Text" := DisplayText;
        CTFChallenge."Entry Type" := EntryType;
        CTFChallenge.Insert();
        TreeViewIndex += 1;
    end;

    local procedure AddChallengePerCategory(CategoriesWithChallenges: Dictionary of [Enum "CTF Category", List of [Enum "CTF Challenge"]]; Challenge: Enum "CTF Challenge")
    var
        ICTFChallenge: Interface "CTF Challenge";
        SpecificCategoryList: List of [Enum "CTF Challenge"];
        CurrentCategory: Enum "CTF Category";
    begin
        ICTFChallenge := Challenge;
        CurrentCategory := ICTFChallenge.GetCategory();

        if (CategoriesWithChallenges.ContainsKey(CurrentCategory)) then begin
            SpecificCategoryList := CategoriesWithChallenges.Get(CurrentCategory);
            SpecificCategoryList.Add(Challenge);
        end else begin
            SpecificCategoryList.Add(Challenge);
            CategoriesWithChallenges.Set(CurrentCategory, SpecificCategoryList);
        end;
    end;
}