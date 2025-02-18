namespace Techdays.AITestToolkitDemo;
using System.TestTools.TestRunner;
using Microsoft.Inventory.Item;
using System.TestTools.AITestToolkit;

codeunit 50200 "Marketing Text With AI Tests"
{
    Subtype = Test;

    var
        IsInitialized: Boolean;

    local procedure Initialize()
    begin
        if IsInitialized then
            exit;

        // Register the feature for the test

        // Setup the items needed for the test
        CreateItem();

        IsInitialized := true;
    end;

    [Test]
    procedure TestTagLineLength()
    var
        TestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        TagLine: Text;
        MaxLength: Integer;
        ItemNo: Code[20];
        TaglineLengthErr: Label 'Tagline: %1; Length: %2; Exceeds maximum lenght of %3', Comment = '%1 = Tagline, %2 = Length, %3 = Maximum Length';
    begin
        // [GIVEN] The setup to create an item and required maximum length
        Initialize();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('ItemNo').ValueAsText(), 1, MaxStrLen(ItemNo));
        MaxLength := TestContext.GetTestSetup().Element('MaxLength').ValueAsInteger();

        // [WHEN] Generating the tagline with required maximum length
        TagLine := MarketingTextWithAI.GenerateTagLine(ItemNo, MaxLength);

        // [THEN] The tagline should be within the required maximum length
        if StrLen(TagLine) > MaxLength then
            Error(TaglineLengthErr, ItemNo, StrLen(TagLine), MaxLength);
    end;

    [Test]
    procedure TestMarketingTextContentFormal()
    var
        TestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        Style: Enum "Marketing Text Style";
        MarketingText: Text;
        ItemNo: Code[20];
    begin
        // [GIVEN] The setup to create an item
        Initialize();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('ItemNo').ValueAsText(), 1, MaxStrLen(ItemNo));

        // [WHEN] Generating the marketing text with formal tone
        MarketingText := MarketingTextWithAI.GenerateMarketingText(ItemNo, Style::Formal);

        // [THEN] Return the marketing text for external evaluation

    end;

    [Test]
    procedure TestMarketingTextContentVerbose()
    var
        TestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        Style: Enum "Marketing Text Style";
        MarketingText: Text;
        ItemNo: Code[20];
    begin
        // [GIVEN] The setup to create an item
        Initialize();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('ItemNo').ValueAsText(), 1, MaxStrLen(ItemNo));

        // [WHEN] Generating the marketing text with verbose tone
        MarketingText := MarketingTextWithAI.GenerateMarketingText(ItemNo, Style::Verbose);

        // [THEN] Return the marketing text for external evaluation
        TestContext.SetAnswerForQnAEvaluation(MarketingText);
    end;

    [Test]
    procedure TestMarketingTextContentCasual()
    var
        TestContext: Codeunit "AIT Test Context";
        MarketingTextWithAI: Codeunit "Marketing Text With AI";
        Style: Enum "Marketing Text Style";
        MarketingText: Text;
        ItemNo: Code[20];
    begin
        // [GIVEN] The setup to create an item
        Initialize();
        ItemNo := CopyStr(TestContext.GetTestSetup().Element('ItemNo').ValueAsText(), 1, MaxStrLen(ItemNo));

        // [WHEN] Generating the marketing text with casual tone
        MarketingText := MarketingTextWithAI.GenerateMarketingText(ItemNo, Style::Casual);

        // [THEN] Return the marketing text for external evaluation
        TestContext.SetAnswerForQnAEvaluation(MarketingText);
    end;

    local procedure CreateItem()
    var
        Item: Record Item;
        TestContext: Codeunit "AIT Test Context";
    begin
        Item."No." := CopyStr(TestContext.GetTestSetup().Element('ItemNo').ValueAsText(), 1, MaxStrLen(Item."No."));
        Item.Description := CopyStr(TestContext.GetTestSetup().Element('ItemDescription').ValueAsText(), 1, MaxStrLen(Item.Description));
        Item.Insert();
    end;
}