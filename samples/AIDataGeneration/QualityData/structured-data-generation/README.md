# structured-data-generation

Generation of synthetic structured data using Azure OpenAI-deployed Large Language Models (LLMs).

The overall idea is to combine OpenAI constrained decoding/[structured outputs](https://openai.com/index/introducing-structured-outputs-in-the-api/) with multi-agent systems (specifically [Swarm](https://github.com/openai/swarm)).

## Getting Started

Prerequisites
- an Azure OpenAI LLM deployment that supports structured outputs and for which you have Entra ID-based access. (For this you will need the `Cognitive Services OpenAI User` role assignment for the Azure OpenAI resource). You can provision this through [Azure AI Foundry](https://learn.microsoft.com/en-us/azure/ai-services/connect-services-ai-foundry-portal).

The following commands will create a virtual Python environment and allow you to run the examples:

```shell
# Create a virtual environment in the structured-data-generation directory
python -m venv venv
.\venv\Scripts\Activate.ps1

# Install required dependencies
poetry install

# Set environment variables
$env:AZURE_OPENAI_ENDPOINT = "<URL of an Azure OpenAI endpoint to which you have Entra ID-based access>" # e.g., https://bctechdays2025.cognitiveservices.azure.com
$env:AZURE_OPENAI_DEPLOYMENT = "<name of an Azure OpenAI deployment that supports structured outputs>" # e.g., gpt-4.1
$env:OPENAI_API_VERSION = "<API version>" # e.g., 2025-01-01-preview
```

## Example

Here's a simple example of creating a title and an abstract for a textbook: 

```python
from __future__ import annotations
import json
from structured_data_generation import ElementCreator
from pydantic import BaseModel, Field

# Define a domain model for the data you wish to generate
class TextBook(BaseModel):
    title: str = Field(..., description="Title of the book")
    authors: list[Author] = Field(..., description="Authors of the book")
    isbn: str = Field(..., description="ISBN of the book")
    abstract: str = Field(..., description="Abstract of the book")

class Author(BaseModel):
    first_name: str = Field(..., description="First name of the author")
    last_name: str = Field(..., description="Last name of the author")

# Define a creator that generates instances of the domain model
textbook_creator = ElementCreator(response_format=TextBook)
textbook = textbook_creator.create()

print(json.dumps(textbook.model_dump(), indent=2))
"""
{
  "title": "Synthetic Data Generation: Techniques and Applications",
  "authors": [
    {
      "first_name": "Emily",
      "last_name": "Reed"
    },
    {
      "first_name": "Jordan",
      "last_name": "Patel"
    }
  ],
  "isbn": "978-3-16-148410-0",
  "abstract": "This comprehensive textbook provides an in-depth exploration of synthetic data generation techniques, encompassing both traditional statistical methods and cutting-edge machine learning approaches. It offers insights into the advantages, challenges, and ethical considerations of using synthetic data in various fields, including data science, healthcare, cybersecurity, and more. Readers will gain practical skills through step-by-step tutorials, case studies, and real-world applications, making this an essential resource for students, researchers, and professionals interested in harnessing the power of synthetic data."
}
"""
```

For further examples, see the `examples` subdirectory.