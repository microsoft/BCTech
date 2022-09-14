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

    procedure GetCTFChallengesBanner(): Text
    var
        BannerText: TextBuilder;
    begin
        BannerText.AppendLine('Each scenario listed below contains a "bad implementation" of some kind, e.g., an implementation that leads to poor performance.');
        BannerText.AppendLine('What you should do:');
        BannerText.AppendLine('    1. Run a scenario - and observe that it''s not working as expected.');
        BannerText.AppendLine('    2. Use available troubleshooting tools to find out why.');
        BannerText.AppendLine('');
        BannerText.AppendLine('When you see the flag - which is on the format "Flag_xxxxxxxx" - then you know you have found the problem.');
        exit(BannerText.ToText());
    end;

    procedure GetExternalCTFChallengesBanner(): Text
    var
        BannerText: TextBuilder;
    begin
        BannerText.AppendLine('Please follow the instructions in the CTF portal to get the challenge names.');
        BannerText.Append('After that, you can enter the name of the challenge in the field below.');
        exit(BannerText.ToText());
    end;

    procedure GetHintKeys(): Text
    var
        CryptographyManagement: Codeunit "Cryptography Management";
        Challenge: Enum "CTF Challenge";
        ICTFChallenge: Interface "CTF Challenge";
        ChallengeName: Text;
        HintText: Text;
        HintIndex: Integer;
        HintKeysText: TextBuilder;
        HashAlgorithmType: Option MD5,SHA1,SHA256,SHA384,SHA512;
        HashText: Text;
    begin
        foreach Challenge in Challenge.Ordinals() do begin
            ChallengeName := Challenge.Names().Get(Challenge.AsInteger());
            ICTFChallenge := Challenge;

            for HintIndex := 1 to ICTFChallenge.GetHints().Count() do begin
                HintText := ICTFChallenge.GetHints().Get(HintIndex);
                HashText := CryptographyManagement.GenerateHash(HintText, HashAlgorithmType::SHA256);
                HintKeysText.AppendLine(StrSubstNo('%1 Hint %2: CTF_%3', ChallengeName, HintIndex, HashText.Substring(1, 8)));
            end;
        end;

        exit(HintKeysText.ToText());
    end;

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