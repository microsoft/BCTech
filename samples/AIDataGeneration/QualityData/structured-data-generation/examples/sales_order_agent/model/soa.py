from __future__ import annotations

from typing import List, Optional, Type
from pydantic import BaseModel, Field

# Company
class Company(BaseModel):
    name: str = Field(None, description="Name of the company")
    description: str = Field(None, description="Description of the company")

# Test setup
class UnitOfMeasure(BaseModel):
    code: str
    quantityPerUnitOfMeasure: int

class Item(BaseModel):
    description: str = Field(None, description="Description of the item in singular.")
    itemNo: str
    baseUnitOfMeasure: str
    unitsOfMeasure: Optional[List[UnitOfMeasure]] = Field(None, description="Units of measure for the item")
    unitPrice: float

class Customer(BaseModel):
    name: str
    address: str
    countryRegionCode: str
    postCode: int
    city: str
    phoneNo: str
    email: str

class Contact(BaseModel):
    name: str
    companyName: str
    phoneNo: str
    email: str

class TestSetup(BaseModel):
    itemsToCreate: Optional[List[Item]] = Field(None, description="Items to create in the test setup")
    customersToCreate: Optional[List[Customer]] = Field(None, description="Customers to create in the test setup")
    contactsToCreate: Optional[List[Contact]] = Field(None, description="Contacts to create in the test setup")
    quotesToCreate: Optional[List[Quote]] = Field(None, description="Quotes to create in the test setup")

# Test cases
class QuoteLine(BaseModel):
    """Represents a line in a quote in a test for the Sales Order Agent"""
    itemDescription: str = Field(None, description="Description of the item")
    quantity: int = Field(None, description="Quantity of the item")
    unitOfMeasure: str = Field(None, description="Unit of measure of the item")

class Quote(BaseModel):
    """Represents a quote in a test for the Sales Order Agent"""
    customerName: str = Field(None, description="Name of the customer")
    lines: List[QuoteLine] = Field(None, description="Lines in the quote")

class OrderLine(BaseModel):
    """Represents a line in a sales order in a test for the Sales Order Agent"""
    itemDescription: str = Field(None, description="Description of the item")
    quantity: int = Field(None, description="Quantity of the item")
    unitOfMeasure: str = Field(None, description="Unit of measure of the item")

class Order(BaseModel):
    """Represents a sales order in a test for the Sales Order Agent"""
    customerName: str = Field(None, description="Name of the customer")
    totalExclVAT: float = Field(None, description="Total amount of the quote excluding VAT")
    lines: List[OrderLine] = Field(None, description="Lines in the sales order")

class ExpectedData(BaseModel):
    """Represents the expected output for a turn in a test for the Sales Order Agent"""
    attachments: bool = Field(None, description="Whether an attachment is expected. If a quote or order is created, this should be True.")
    quotes: Optional[List[Quote]] = Field(None, description="Expected quotes that were created in the turn")
    orders: Optional[List[Order]] = Field(None, description="Expected orders that were created in the turn")
    email: str = Field(None, description="Description of the expected email output from the Sales Order Agent")

class IncomingEmail(BaseModel):
    """Represents an incoming email for the Sales Order Agent"""
    from_: str = Field(..., description="Email address of the sender", alias="from")
    subject: str = Field(None, description="Subject of the email")
    body: str = Field(None, description="Body of the email")

class Turn(BaseModel):
    """Represents a turn in a test for the Sales Order Agent"""
    incoming_email: IncomingEmail = Field(..., description="Incoming email for the turn")
    expected_data: ExpectedData = Field(None, description="Expected output for the turn")

class TestCase(BaseModel):
    """Represents a test of the Sales Order Agent in Business Central"""
    name: str = Field(None, description="Name of the test")
    description: str = Field(None, description="Description of the test")
    test_setup: Optional[TestSetup] = Field(None, description="Setup for the test suite")
    turns: List[Turn]

class TestSuite(BaseModel):
    """Represents a suite of tests for the Sales Order Agent in Business Central"""
    name: str = Field(None, description="Name of the test suite")
    tests: List[TestCase]
