This extension allows AL developers to quickly get started with completions on the Azure OpenAI service.

## Getting Started

After publishing the extension, search for the new "Azure OpenAI Setup" page within Business Central. From there you can enter the path to the completions endpoint.

The completions endpoint typically has the format of: `https://<resource name>.openai.azure.com/openai/deployments/<deployment name>/completions?api-version=2022-12-01`.
This sample has been tested with the `text-davinci-003` model. Using another model may require tweaking of the prompts (for example with turbo or gpt4).

## Calling Azure OpenAI

Check the ItemCard page extension for an example of how to call Azure OpenAI.
Here we generate the call responsibly by grounding the prompt in metadata (item categories), and validating that the suggested category is one that exists in the system.
Finally, a confirmation dialog is shown to the user to let them decide whether or not to accept the suggestion. The suggestion is not applied automatically by design.

When implementing your own scenarios, ensure you follow Responsible AI design patterns. For more information, check out https://aka.ms/RAI
