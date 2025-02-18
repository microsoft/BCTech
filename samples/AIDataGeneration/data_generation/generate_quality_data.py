import os
import sys
from azure.identity import DefaultAzureCredential, get_bearer_token_provider
from openai import AzureOpenAI

from model import model

def create_azure_openai_client(
    azure_endpoint: str,
    azure_deployment: str = "gpt-4o",
    api_key: str = None,
    api_version = "2024-08-01-preview") -> AzureOpenAI:

    if api_key:
        return AzureOpenAI(
            azure_endpoint=azure_endpoint,
            api_version=api_version,
            azure_deployment=azure_deployment,
            azure_api_key=api_key)
    else:
        return AzureOpenAI(
            azure_endpoint=azure_endpoint,
            api_version=api_version,
            azure_deployment=azure_deployment,
            azure_ad_token_provider=
                get_bearer_token_provider(
                    DefaultAzureCredential(),
                    "https://cognitiveservices.azure.com/.default"))    

if __name__ == "__main__":
    client = create_azure_openai_client(
        os.getenv("AZURE_OPENAI_ENDPOINT"),
        os.getenv("AZURE_OPENAI_DEPLOYMENT") or "gpt-4o",
        os.getenv("AZURE_OPENAI_API_KEY"))

    system_prompt = """
    You are a helpful synthetic data generator. 
    You are asked to generate a list of items with descriptions and attributes.
    """
    request_prompt = sys.argv[1]
    
    completion = client.beta.chat.completions.parse(
        model="...",
        messages=[
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": request_prompt},
        ],
        response_format=model.ItemList
    )
    
    result = completion.choices[0].message.parsed
    
    for item in result.items:
        print(item.model_dump_json())
