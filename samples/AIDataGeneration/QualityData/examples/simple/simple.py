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
