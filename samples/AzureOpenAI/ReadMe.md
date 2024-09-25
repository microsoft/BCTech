# Introduction
This folder contains several extensions that demonstrate how AL developers can use the Developer Tools for Copilot in Dynamics 365 Business Central.

Starting from these samples, you can quickly get started with implementing Copilot features in Business Central, leveraging the models on the Azure OpenAI service.

> **_NOTICE:_** These apps are provided as samples/demos, and should not be used in production.

> **_NOTICE:_** These apps have been tested with the Azure OpenAI models that were newest when the sample was created. If you install them today they might use newer models that require tweaking of the prompts to work properly.


## Calling Azure OpenAI
In general, large language models sometimes provide information that is not accurate. Make sure you implement all the necessary steps to ensure you avoid these cases, including but not limited to:
- Check grounding for the facts that Azure OpenAI returns (for example, checking that the suggested item exists is our grounding step, or make sure that the job/project suggestion has estimated completion dates in the future)
- Always make the user review any suggestion from Copilot before you save the changes
- When implementing your own scenarios, ensure you follow Responsible AI design patterns. For more information, check out https://aka.ms/RAI


# Folders
### 1-GetStarted
This folder contains the simplest example of an extension that adds a new Copilot feature/capability to Business Central.

This extension is ready for you to explore. If you are NOT using Managed AI Resources, you will get an error you get an error similar to "The authorization is not configured". You need to change the line in `DraftProject.Page.al`, and instead of `AzureOpenAI.SetManagedResourceAuthorization` call `AzureOpenAI.SetAuthorization` specifying your own Azure OpenAI endpoint, deployment, and API key. The endpoint typically has the format of: `https://<resource name>.openai.azure.com/`.

After you install the extension, you will be able to access its logic from the existing Base Application page `page 89 "Job List"`.


### 2-ItemSubstitution
This folder contains one of the early samples that uses the AL Copilot Toolkit. It demonstrates how to use the PromptDialog page type and its special capabilities, as well as how to set up and tweak the parameters to call Azure Open AI from AL.

This extension is ready for you to explore. Before you install it, you need to modify the file `SecretsAndCapabilitiesSetup.Codeunit.al`, and make sure Isolated Storage is set up with your Azure OpenAI endpoint, deployment, and API key.

If you are NOT using Managed AI Resources, you will get an error you get an error similar to "The authorization is not configured". You need to change the line in `GenerateItemSubProposal.Codeunit.al`, and instead of `AzureOpenAI.SetManagedResourceAuthorization` call `AzureOpenAI.SetAuthorization`.

After you install the extension, you will be able to access its logic from the existing Base Application page `page 30 "Item Card"`.


### 3-SuggestJob
This folder contains one of the early samples that uses the AL Copilot Toolkit. It demonstrates how to use the PromptDialog page type and its special capabilities, as well as how to set up and tweak the parameters to call Azure Open AI from AL.

This extension is ready for you to explore. Before you install it, you need to modify the file `SecretsAndCapabilitiesSetup.Codeunit.al`, and make sure Isolated Storage is set up with your Azure OpenAI endpoint, deployment, and API key. 

If you are NOT using Managed AI Resources, you will get an error you get an error similar to "The authorization is not configured". You need to change the line in `SimplifiedCopilotChat.Codeunit.al`, and instead of `AzureOpenAI.SetManagedResourceAuthorization` call `AzureOpenAI.SetAuthorization`.

After you install the extension, you will be able to access the first part of its logic (Suggest Job) from the existing Base Application page `page 89 "Job List"`, and the second part of its logic (Suggest Resource) from the existing Base Application page `page 1007 "Job Planning Lines"`.


### 4-SuggestJob with Tools
This folder contains a newer example of the Suggest Job Copilot feature. This sample is using Azure OpenAI Tools (formerly called Functions), and illustrates the usage of the AL interface `interface "AOAI Function"`.

This extension is ready for you to explore. If you are NOT using Managed AI Resources, you will get an error you get an error similar to "The authorization is not configured". You need to change the line in `SuggestJobGenerateProposal.Codeunit.al`, and instead of `AzureOpenAI.SetManagedResourceAuthorization` call `AzureOpenAI.SetAuthorization` specifying your own Azure OpenAI endpoint, deployment, and API key. The endpoint typically has the format of: `https://<resource name>.openai.azure.com/`.

After you install the extension, you will be able to access its logic from the existing Base Application page `page 89 "Job List"`.


### Conference Demos
This folder contains several demos that were shown in different conferences and events since we released the AL Copilot Toolkit in 2023 Release Wave 2.

