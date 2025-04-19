# Best Practices

This article outlines best practices for validating accuracy, ensuring safety, supporting multilingual users, and tracking model version changes.

## Contents
1. [Overview](01-overview.md)
2. [Creating Datasets](02-datasets.md)
3. [Writing AI Tests](03-tests.md)
4. [AI Test Tool](04-ai-test-tool.md)
5. [Best Practices](05-best-practices.md)

---

## Key considerations for LLM-based features

Unlike deterministic systems, LLM-based features require new testing approaches. Consider the following:

- **Non-determinism:** Identical prompts may produce different results  
- **Context sensitivity:** Small changes in input phrasing can significantly affect output quality  
- **Bias and safety:** Language models may reflect or amplify societal and cultural biases  

> [!NOTE]  
> Always include human-in-the-loop evaluation for user-facing or high-impact scenarios, even if you’ve automated parts of the test pipeline.

### 1. Measure accuracy at scale

To evaluate Copilot performance broadly:

1. Use the **AI Test Tool** to automate testing and verify thousands of prompts automatically  
2. Score outputs for **correctness**, **relevance**, and **completeness**  
3. Flag low-confidence responses for human review  

The AI Test Toolkit allows automating AI Testing at scale..

### 2. Create realistic test cases

Design tests that reflect actual usage:

- Base cases on real-world tasks and intents  
- Leverage anonymized user logs for representative prompts  
- Include a variety of phrasing styles and complexity levels  

Build test suites that cover both common and edge-case user scenarios.

### 3. Validate output safety and tone

A Copilot feature must be accurate—but also safe, respectful, and aligned with your organization’s voice. Outputs that appear correct can still fail due to inappropriate tone or harmful implications.

Test to ensure your Copilot:

- **Avoids bias and stereotyping:**  
  - Uses inclusive, non-gendered language  
  - Resists reproducing cultural or societal biases  
- **Maintains professional tone:**  
  - Aligns with your brand’s voice  
  - Avoids sarcasm or humor unless appropriate  
- **Filters harmful content:**  
  - Blocks hate speech, profanity, and explicit material  
  - Mitigates prompt abuse and adversarial input  
- **Handles adversarial prompts safely:**  
  - Defends against prompt injection and chaining  
  - Gracefully manages nonsense or confusing queries  

> [!TIP]  
> Integrate both quality and safety tests into your CI/CD pipeline using the AI Test Toolkit.

### 4. Test for cross-language compatibility

If your Copilot supports multiple languages:

1. Validate input/output handling in each supported locale  
2. Involve native speakers to assess linguistic and cultural accuracy  
3. Avoid assuming that English test results apply globally  

Localizing your test approach is essential to ensure consistent user experience across regions.

### 5. Track changes across model versions

LLMs evolve quickly—and updates can unintentionally affect feature behavior. Use regression testing to:

- Re-run existing test suites on updated models  
- Compare current vs. previous outputs side-by-side  
- Identify unexpected changes or regressions  

Maintain historical baselines for consistency across releases.

## Best practices checklist

| Task                   | Description                                |
|------------------------|--------------------------------------------|
| Automate testing       | Evaluate large-scale output with batch runs|
| Define realistic prompts| Reflect real-world user behavior           |
| Review for safety and tone| Detect harmful or biased content         |
| Localize testing       | Validate multilingual output accuracy       |
| Run version comparisons| Track regressions from model updates       |

