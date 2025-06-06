from __future__ import annotations
import os
from typing import Any, List, Type, get_args, get_origin
from openai import AzureOpenAI
from pydantic import BaseModel, Field, create_model
import json
import yaml
from openai import AzureOpenAI
from azure.identity import DefaultAzureCredential, get_bearer_token_provider

def read_from_yaml(yaml_file: str, type: Type[BaseModel]) -> BaseModel:
    with open(yaml_file, 'r') as f:
        data = yaml.safe_load(f)
        return type.model_validate(data)

def read_list_from_yaml(yaml_file: str, type: Type[BaseModel]) -> List[BaseModel]:
    with open(yaml_file, 'r') as f:
        elements = yaml.safe_load(f)
        return [type.model_validate(element) for element in elements]

def create_azureopenai_client() -> AzureOpenAI:
    return AzureOpenAI(
        azure_deployment=os.getenv("AZURE_OPENAI_DEPLOYMENT", "production"),
        azure_ad_token_provider=
            get_bearer_token_provider(
                DefaultAzureCredential(),
                "https://cognitiveservices.azure.com/.default"))

class ElementCreator(BaseModel):
    system_prompt: str = "You are a helpful synthetic data generator."
    request_prompt: str = "Create {element_name}(s)."
    client: AzureOpenAI = None
    response_format: Any = None

    class Config:
        arbitrary_types_allowed = True

    def __init__(self, **data):
        super().__init__(**data)
        if not self.client:
            self.client = create_azureopenai_client()

        if not self.request_prompt:
            self.request_prompt = self.request_prompt.format(
                element_name=self.response_format.__name__)
    
    def create(self) -> BaseModel:
        return self._create_internal()

    def _create_internal(self) -> BaseModel:
        response_format = self.response_format
        response_format_is_list = get_origin(response_format) == list

        if response_format_is_list:
            list_elements_name = get_args(response_format)[0].__name__
            response_format = create_model(
                'ResponseList',
                responses=(
                    response_format,
                    Field(None, description=f"List of {list_elements_name}(s) to create."))
            )

        completion = self.client.beta.chat.completions.parse(
            model="...",
            messages=[
                {"role": "system", "content": self.system_prompt},
                {"role": "user", "content": self.request_prompt},
            ],
            response_format=response_format
        )

        result = completion.choices[0].message.parsed

        if response_format_is_list:
            result = result.responses

        return result

def dump_as_yaml(data: BaseModel) -> str:
    yaml_data = json.loads(data.model_dump_json(exclude_none=True, by_alias=True))
    return yaml.dump(yaml_data, default_flow_style=False, allow_unicode=True, sort_keys=False)
