# Adversarial Simulation

The idea of the code in this folder is to enable adversarial simulation in Business Central.

There are two parts:

- A _BC app_, in the `AdversarialSimulation` folder that interacts with the BC simulation/evaluation API
- A _BC simulation/evaluation API_ written in Python that wraps the [Azure AI Foundry evaluation SDK](https://learn.microsoft.com/en-us/azure/ai-foundry/how-to/develop/simulator-interaction-data)

There is no Azure AI Foundry API per se for simulation as the SDK uses a callback method in which the SDK assumes that it can call an AI feature directly. This is abstracted away using the BC app and BC simulation API.

## Usage

First, start the BC simulation/evaluation API (see [README](bc-evaluation/README.md)).

Next, add the `AdversarialSimulation` app to your AI test project. The typical interaction steps for a simple Q&A interaction is:
- `AdversarialSimulation.Start()`
- `AdversarialSimulation.GetHarm()`

The idea is then that your AI feature should catch/not react to the adversarial input. For additional details, see [the Azure AI Foundry documentation](https://learn.microsoft.com/en-us/azure/ai-foundry/how-to/develop/simulator-interaction-data).
