# Writing AI Tests

This article explains how to create automated AI tests for Copilot features in Business Central.

## Contents
1. [Overview](01-overview.md)
2. [Creating Datasets](02-datasets.md)
3. [Writing AI Tests](03-tests.md)
4. [AI Test Tool](04-ai-test-tool.md)
5. [Best Practices](05-best-practices.md)

---

## What is an AI Test?

An **AI Test** is a procedure designed to evaluate the accuracy, reliability, and safety of a Copilot feature. These tests involve running the Copilot through a series of predefined scenarios defined by datasets, then comparing the outputs against expected results.

## Step-by-Step: How to Write an AI Test

> [!TIP]
> The full source code for the example used in this article can be found in the [Marketing Text Simple](#) demo project.

Follow these steps to create an AI Test.

### 1. Define the Test

An AI Test in Business Central is a standard AL test that uses the `AIT Test Context` codeunit to interact with datasets. 

> [!NOTE]
> AI Tests depend on datasets. Learn how to create one in the [Dataset](#) article.

Example of a basic test method:

```al
[Test]
procedure TestTagLineLength()
var
    AITTestContext: Codeunit "AIT Test Context";
begin
    // Test logic goes here
end;
```

### 2. Access the Dataset

Use the `AIT Test Context` codeunit to retrieve inputs and expected values from your dataset.

- To get the full input as JSON:
  ```al
  AITTestContext.GetInput()
  ```
- To get specific setup values:
  ```al
  AITTestContext.GetTestSetup().Element('element_name').ValueAsText()
  ```
- To get expected result values:
  ```al
  AITTestContext.GetExpectedData().Element('element_name').ValueAsInteger()
  ```

> [!TIP]
> Review the `AIT Test Context` codeunit for all supported procedures.

Example:

```al
[Test]
procedure TestTagLineLength()
var
    AITTestContext: Codeunit "AIT Test Context";
    MaxLength: Integer;
    ItemNo: Code[20];
begin
    // [GIVEN] The item and required maximum length
    CreateItem();
    ItemNo := CopyStr(AITTestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));
    MaxLength := AITTestContext.GetExpectedData().Element('tagline_max_length').ValueAsInteger();

    // Continue with test logic
end;
```

### 3. Call the Copilot Feature

You can invoke your Copilot feature from AL code just like any other method.

Example of calling `GenerateTagLine`:

```al
[Test]
procedure TestTagLineLength()
var
    AITTestContext: Codeunit "AIT Test Context";
    MarketingTextWithAI: Codeunit "Marketing Text With AI";
    TagLine: Text;
    MaxLength: Integer;
    ItemNo: Code[20];
begin
    // [GIVEN]
    CreateItem();
    ItemNo := CopyStr(AITTestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));
    MaxLength := AITTestContext.GetExpectedData().Element('tagline_max_length').ValueAsInteger();

    // [WHEN]
    TagLine := MarketingTextWithAI.GenerateTagLine(ItemNo, MaxLength);

    // Continue with test logic
end;
```

### 4a. Evaluate Internally

Use AL assertions to verify that the generated results meet expectations.

```al
[Test]
procedure TestTagLineLength()
var
    AITTestContext: Codeunit "AIT Test Context";
    MarketingTextWithAI: Codeunit "Marketing Text With AI";
    TagLine: Text;
    MaxLength: Integer;
    ItemNo: Code[20];
    TaglineLengthErr: Label 'Tagline: %1; Length: %2; Exceeds maximum length of %3', Comment = '%1 = Tagline, %2 = Length, %3 = Maximum Length';
begin
    // [GIVEN]
    CreateItem();
    ItemNo := CopyStr(AITTestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));
    MaxLength := AITTestContext.GetExpectedData().Element('tagline_max_length').ValueAsInteger();

    // [WHEN]
    TagLine := MarketingTextWithAI.GenerateTagLine(ItemNo, MaxLength);

    // [THEN]
    if StrLen(TagLine) > MaxLength then
        Error(TaglineLengthErr, TagLine, StrLen(TagLine), MaxLength);
end;
```

> [!NOTE]
> Ensure all test methods in a codeunit can handle the same input. If not, split the tests across multiple codeunits.

### 4b. Evaluate Externally

For more complex validations (e.g., tone, relevance), it's often better to evaluate the results externally, using tools like Azure AI Foundry.

Use `SetTestOutput` to export output for external evaluation:

```al
[Test]
procedure TestMarketingTextContentCasual()
begin
    TestScenario(StyleEnum::Casual);
end;

local procedure TestScenario(Style: Enum "Marketing Text Style")
var
    Item: Record Item;
    TestContext: Codeunit "AIT Test Context";
    MarketingTextWithAI: Codeunit "Marketing Text With AI";
    TestOutputJson: Codeunit "Test Output Json";
    ContextOutputJson: Codeunit "Test Output Json";
    MarketingText: Text;
    QueryTxt: Text;
    ItemNo: Code[20];
begin
    // [GIVEN]
    CreateItem();
    ItemNo := CopyStr(TestContext.GetTestSetup().Element('item_no').ValueAsText(), 1, MaxStrLen(ItemNo));

    // [WHEN]
    MarketingText := MarketingTextWithAI.GenerateMarketingText(ItemNo, Style);

    // [THEN] External evaluation
    QueryTxt := 'Generate a marketing text for the given item in Business Central in ';
    case Style of
        StyleEnum::Formal:
            QueryTxt += '*Formal* Tone.';
        StyleEnum::Verbose:
            QueryTxt += '*Verbose* Tone.';
        StyleEnum::Casual:
            QueryTxt += '*Casual* Tone.';
    end;

    // Prepare test output
    TestOutputJson.Initialize();
    TestOutputJson.Add('query', QueryTxt);

    // Add context
    ContextOutputJson.Initialize();
    ContextOutputJson.Add('item_no', ItemNo);
    Item.Get(ItemNo);
    ContextOutputJson.Add('description', Item.Description);
    ContextOutputJson.Add('uom', Item."Base Unit of Measure");

    TestOutputJson.Add('context', ContextOutputJson.ToText());

    // Add generated response
    TestOutputJson.Add('response', MarketingText);

    // Set test output for external validation
    TestContext.SetTestOutput(TestOutputJson.ToText());
end;
```
