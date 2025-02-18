import asyncio
import json
import os
import sys
from typing import Any, Dict, List
from azure.ai.evaluation import AzureAIProject
from azure.ai.evaluation.simulator import AdversarialSimulator, AdversarialScenario, DirectAttackSimulator, IndirectAttackSimulator
from azure.identity import DefaultAzureCredential

requests = []

async def callback(
    request: List[Dict],
    stream: bool = False,
    session_state: Any = None,) -> dict:
    
    request["messages"].append({
        "content": "OK",
        "role": "assistant",
    })
    
    requests.append(request)
    
    return {
        "messages": request["messages"],
        "stream": stream,
        "session_state": session_state
    }
    
async def generate_qa_harms(azure_ai_project: AzureAIProject, max_results):
    simulator = AdversarialSimulator(azure_ai_project=azure_ai_project, credential=DefaultAzureCredential())
    outputs = await simulator(
        scenario=AdversarialScenario.ADVERSARIAL_QA,
        target=callback,
        max_simulation_results=max_results,
    )

    results = []
    for output in outputs:
        result = {
            "category": output["template_parameters"]["category"],
            "content": output["messages"][0]["content"],
        }
        if "filename" in output["template_parameters"] and "file_content" in output["template_parameters"]:
            result["filename"] = output["template_parameters"]["filename"]
            result["file_content"] = output["template_parameters"]["file_content"]
            
        results.append(result)
   
    return results

async def generate_xpia_harms(azure_ai_project: AzureAIProject, max_results):
    global requests
    requests = []
    
    simulator = IndirectAttackSimulator(azure_ai_project=azure_ai_project, credential=DefaultAzureCredential())
    await simulator(
        target=callback,
        max_conversation_turns=1,
        max_simulation_results=max_results,
    )
    
    results = []
    for request in requests:
        result = {
            "category": request["template_parameters"]["xpia_attack_type"],
            "content": request["messages"][0]["content"],
            "xpia_attack": request["template_parameters"]["xpia_attack_sentence"],
        }

        results.append(result)
        
    return results

async def generate_upia_harms(azure_ai_project: AzureAIProject, max_results):
    global requests
    requests = []
    
    simulator = DirectAttackSimulator(azure_ai_project=azure_ai_project, credential=DefaultAzureCredential())
    outputs = await simulator(
        target=callback,
        scenario=AdversarialScenario.ADVERSARIAL_QA,
        max_conversation_turns=1,
        max_simulation_results=max_results,
    )
    
    results = []
    for category in ["jailbreak", "regular"]:
        for output in outputs[category]:
            result = {
                "category": category,
                "content": output["messages"][0]["content"],
            }
            if "filename" in output["template_parameters"] and "file_content" in output["template_parameters"]:
                result["filename"] = output["template_parameters"]["filename"]
                result["file_content"] = output["template_parameters"]["file_content"]

            results.append(result)
   
    return results

async def main():
    default_max_results = 3
    qa_scenario = "qa"
    xpia_scenario = "xpia"
    upia_scenario = "upia"

    azure_ai_project = {
        "subscription_id": os.getenv("AAIF_SUBSCRIPTION_ID"),
        "resource_group_name": os.getenv("AAIF_RESOURCE_GROUP_NAME"),
        "project_name": os.getenv("AAIF_PROJECT_NAME"),
    }
    
    scenario = (len(sys.argv) > 1 and sys.argv[1]) or qa_scenario
    max_results = (len(sys.argv) > 2 and int(sys.argv[2])) or default_max_results
    
    if scenario == qa_scenario:
        results = await generate_qa_harms(azure_ai_project, max_results)
    elif scenario == xpia_scenario:
        results = await generate_xpia_harms(azure_ai_project, max_results)
    elif scenario == upia_scenario:
        results = await generate_upia_harms(azure_ai_project, max_results)
    else:
        raise ValueError(f"Invalid scenario: {scenario}")
      
    for result in results:
        print(json.dumps(result))
    
if __name__ == "__main__":
    asyncio.run(main())
