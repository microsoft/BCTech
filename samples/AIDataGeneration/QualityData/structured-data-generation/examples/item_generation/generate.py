from __future__ import annotations
from enum import Enum
from structured_data_generation import ElementCreator
from pydantic import BaseModel, Field
from structured_data_generation.util import dump_as_yaml

if __name__ == "__main__":
  # Define a domain model for the data you wish to generate
  class Item(BaseModel):
      name: str = Field(..., title="Specifies the product name of an item.")
      type: Type = Field(..., title="Specifies if the item card represents a physical inventory unit (Inventory), a labor time unit (Service), or a physical unit that is not tracked in inventory (Non-Inventory).")
      attributes: list[Attribute] = Field(..., title="Specifies a list of attributes that describe the item.")

  class ItemList(BaseModel):
      items: list[Item] = Field(..., title="Specifies a list of items.")

  class Type(str, Enum):
      INVENTORY = "Inventory"
      SERVICE = "Service"
      NON_INVENTORY = "Non-Inventory"

  class Attribute(BaseModel):
      name: str = Field(..., title="Specifies the name of the item attribute.")
      value: str = Field(..., title="Specifies the value of the attribute.")

  # Define a creator that generates instances of the domain model
  items_creator = ElementCreator(
    response_format=ItemList,
    request_prompt="""
      Create a list of 5 items for a company that uses Business Central and sells silly stuffed animals. 
      Include emojis in the item names and vary the number of attributes per item.
  """)
  items = items_creator.create()

  print(dump_as_yaml(items))
  
  """
  items:
  - name: 🦄 Magical Unicorn
  type: Inventory
  attributes:
  - name: Color
    value: Multicolor
  - name: Material
    value: Soft Fabric
  ...
  """
