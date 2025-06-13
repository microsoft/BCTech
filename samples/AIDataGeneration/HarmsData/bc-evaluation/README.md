# Business Central evaluation and simulation API

## Prerequisites

You will need an [Azure AI Foundry (AAIF) project](https://learn.microsoft.com/en-us/azure/ai-foundry/what-is-azure-ai-foundry) (in addition to Python and Poetry).

## Usage

First, create a virtual Python environment to run the simulation API:
```shell
# Create a virtual environment in the bc-evaluation directory
python -m venv venv
.\venv\Scripts\Activate.ps1

# Install required dependencies
poetry install

# Set environment variables
$env:AZURE_AI_SUBSCRIPTION_ID = "<ID of the subscription in which you AAIF project was created>" # e.g., a39c810f-475d-491c-8b5f-b06ce875989f
$env:AZURE_AI_RESOURCE_GROUP = "<name of the resource group of the AAIF project>" # e.g., "techdays-2025"
$env:AZURE_AI_PROJECT = "<name of the AAIF project>" # e.g., BCTechDays2025Project
```

Next, run the simulation API, e.g.,

```shell
bcevaluation api start --port 8000
```
