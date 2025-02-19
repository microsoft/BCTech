import os
import sys
from azure.identity import DefaultAzureCredential, get_bearer_token_provider
from openai import AzureOpenAI

from model import model

def create_azure_openai_client(azure_deployment: str = "gpt-4o") -> AzureOpenAI:
    api_key = os.environ.get("AZURE_OPENAI_API_KEY")
    
    if api_key:
        return AzureOpenAI(
            azure_deployment=azure_deployment)
    else:
        return AzureOpenAI(
            azure_deployment=azure_deployment,
            azure_ad_token_provider=
                get_bearer_token_provider(
                    DefaultAzureCredential(),
                    "https://cognitiveservices.azure.com/.default"))    

if __name__ == "__main__":
    client = create_azure_openai_client()

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
