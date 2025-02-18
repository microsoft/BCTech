from __future__ import annotations
from enum import Enum
from typing import List, Optional
from pydantic import BaseModel, Field

class Item(BaseModel):
    reason: str = Field(..., title="Thought process that describes how and why this element was generated.")
    name: str = Field(..., title="Specifies the product name of an item.")
    type: Type = Field(..., title="Specifies if the item card represents a physical inventory unit (Inventory), a labor time unit (Service), or a physical unit that is not tracked in inventory (Non-Inventory).")
    attributes: Optional[list[Attribute]] = Field(..., title="Specifies a list of attributes that describe the item.")

class ItemList(BaseModel):
    reason: str = Field(..., title="Thought process that describes how and why this element was generated.")
    items: List[Item] = Field(..., title="Specifies a list of items.")

class Type(str, Enum):
    INVENTORY = "Inventory"
    SERVICE = "Service"
    NON_INVENTORY = "Non-Inventory"
    
class Attribute(BaseModel):
    name: str = Field(..., title="Specifies the name of the item attribute.")
    value: str = Field(..., title="Specifies the value of the attribute.")
