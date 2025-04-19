# Datasets

This article explains how to define datasets for use in AI testing in Business Central.

## Contents
1. [Overview](01-overview.md)
2. [Creating Datasets](02-datasets.md)
3. [Writing AI Tests](03-tests.md)
4. [AI Test Tool](04-ai-test-tool.md)
5. [Best Practices](05-best-practices.md)

---

## What is a Dataset?

A **dataset** is the foundation of AI testing in Business Central. Since AI tests are inherently data-driven, datasets allow us to simulate various user scenarios and interactions. By using diverse and comprehensive datasets, we can thoroughly evaluate AI features to ensure they meet high standards for correctness, safety, and accuracy.

## How to Create a Dataset

> [!TIP]
> The full source code for the example used in this article can be found in the [Marketing Text Simple](#) demo project.

AI tests in Business Central rely on datasets defined in either **JSONL** or **YAML** format. These datasets contain both test input and expected data values used by the AI Test Tool.

### Defining a JSONL Dataset

While there's no rigid schema required, the AI Test Tool supports certain common elements like `test_setup` and `expected_data`. Using these keywords helps create a consistent structure.

Here's an example of a valid JSONL dataset:

```json
{"test_setup": {"item_no": "C-10000", "description": "Contoso Coffee Machine", "uom": "PCS"}, "expected_data": {"tagline_max_length": 20}}
{"test_setup": {"item_no": "C-10001", "description": "Contoso Toaster", "uom": "PCS"}, "expected_data": {"tagline_max_length": 20}}
{"test_setup": {"item_no": "C-10002", "description": "Contoso Microwave Oven", "uom": "PCS"}, "expected_data": {"tagline_max_length": 20}}
```

Each line represents a distinct test case with inputs and expected outputs.

### Defining a YAML Dataset

You can also define the same dataset in YAML format for improved readability:

```yaml
tests:
  - test_setup:
      item_no: "C-10000"
      description: "Contoso Coffee Machine"
      uom: "PCS"
    expected_data:
      tagline_max_length: 20

  - test_setup:
      item_no: "C-10001"
      description: "Contoso Toaster"
      uom: "PCS"
    expected_data:
      tagline_max_length: 20

  - test_setup:
      item_no: "C-10002"
      description: "Contoso Microwave Oven"
      uom: "PCS"
    expected_data:
      tagline_max_length: 20
```

## How to Get Data for Your Tests

When creating AI tests, the data you use is just as important as the AI features you're testing. Quality, consistency, and realism of data are critical for ensuring that your tests are comprehensive and meaningful. 

> [!TIP]
> See the [Best practices](#) section for additional considerations when creating datasets.

### Sources of Data for Your Tests

1. **Public Datasets**:
   - There are many publicly available datasets that you can leverage for AI testing.

2. **Synthetic Data**:
   - In cases where real-world data is difficult to obtain or too sensitive, you can generate synthetic data. 
   - Synthetic data can be especially useful for testing edge cases or generating large volumes of data quickly.

3. **Customer or Internal Data**:
   - If you have access to anonymized customer data or internal business datasets, this can be a valuable source for realistic AI testing.
   - Ensure that the data is appropriately anonymized and that you comply with privacy regulations.

4. **Crowdsourced Data**:
   - Certain platforms allow you to gather custom data by leveraging crowdsourcing. 

5. **Simulated Data from Domain Experts**:
   - In certain domains, domain experts can provide valuable insights into generating realistic and relevant test data.
   - This approach is helpful when real-world data is not readily available or too sensitive to share.

### Additional Tips for Collecting Test Data

- **Start with small datasets**: Especially when testing new AI features, begin with small, manageable datasets to avoid overwhelming your testing process.
- **Incrementally increase complexity**: As you refine your tests, increase the complexity of the datasets to better simulate real-world scenarios.
- **Document data sources**: Always document the origins of your test data, including any transformations made, to ensure traceability and transparency in your testing process.