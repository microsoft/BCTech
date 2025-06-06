# ---------------------------------------------------------
# Copyright (c) Microsoft Corporation. All rights reserved.
# ---------------------------------------------------------

from datetime import datetime
import os

from azure.ai.evaluation import AzureOpenAIModelConfiguration, AzureAIProject

def handle_json_serialization(o: any) -> any:
    """
    Handle datetime values in JSON serialization.

    Args:
        o: Object.

    Returns:
        any: Object without datetimes values.
    """
    if isinstance(o, datetime):
        return o.isoformat()
    raise TypeError

def get_azure_ai_project():
    return AzureAIProject(
        subscription_id = os.getenv("AZURE_AI_SUBSCRIPTION_ID"),
        resource_group_name = os.getenv("AZURE_AI_RESOURCE_GROUP"),
        project_name = os.getenv("AZURE_AI_PROJECT"))

def get_model_config():
    return AzureOpenAIModelConfiguration(
        azure_endpoint = os.getenv("AZURE_AI_ENDPOINT"),
        azure_deployment = os.getenv("AZURE_OPENAI_DEPLOYMENT") or "gpt-4o",
        api_version= os.getenv("OPENAI_API_VERSION") or "2024-08-01-preview"
    )
