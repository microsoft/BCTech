This extension allows AL developers to quickly get started with completions on the Azure OpenAI service.

Notice: this app is provided as sample/demo, and should not be used in production.

## Getting Started

Before publishing the extension, make sure to initialize the secrets in the codeunit `SecretsAndCapabilitiesSetup.Codeunit.al` and the app ID in `app.json`.

The completions endpoint typically has the format of: `https://<resource name>.openai.azure.com/openai/deployments/<deployment name>/completions?api-version=2022-12-01`.
This sample has been tested with the `gpt-35-turbo-16k` model. Using another model may require tweaking of the prompts (for example with turbo or gpt4).

## Calling Azure OpenAI

You can find a minimal example on how to call the Chat Completion endpoint inside `GenerateItemSubProposal.Codeunit.al`, procedure `Chat`.

In general, large language models sometimes provide information that is not accurate. Make sure you implement all the necessary steps to ensure you avoid these cases, including but not limited to:
- Check grounding for the facts that Azure OpenAI returns (in this example, checking that the suggested item exists is our grounding step)
- Always make the user review any suggestion from Copilot before you save the changes
- When implementing your own scenarios, ensure you follow Responsible AI design patterns. For more information, check out https://aka.ms/RAI
