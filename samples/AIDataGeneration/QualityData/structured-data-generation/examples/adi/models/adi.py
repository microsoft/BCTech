"""
Generated using GPT-4o from 
    https://raw.githubusercontent.com/Azure-Samples/document-intelligence-code-samples/refs/heads/main/schema/2024-07-31-preview/invoice.md

Currency and Address are adapted from azure.ai.documentintelligence.models._models.py    
"""
from pydantic import BaseModel, Field
from typing import Optional, List

class Currency(BaseModel):
    amount: float = Field(..., description="Currency amount. Example: 29,520.00")
    currency_symbol: Optional[str] = Field(None, alias="currencySymbol", description="Currency symbol label, if any. Example: $")
    currency_code: Optional[str] = Field(None, alias="currencyCode", description="Resolved currency code (ISO 4217), if any. Example: USD")

class Address(BaseModel):
    houseNumber: Optional[str] = Field(None, description="House or building number.")
    poBox: Optional[str] = Field(None, description="Post office box number.")
    road: Optional[str] = Field(None, description="Street name.")
    city: Optional[str] = Field(None, description="Name of city, town, village, etc.")
    state: Optional[str] = Field(None, description="First-level administrative division.")
    postalCode: Optional[str] = Field(None, description="Postal code used for mail sorting.")
    countryRegion: Optional[str] = Field(None, description="Country/region.")
    streetAddress: Optional[str] = Field(None, description="Street-level address, excluding city, state, countryRegion, and postalCode.")
    unit: Optional[str] = Field(None, description="Apartment or office number.")
    cityDistrict: Optional[str] = Field(None, description="Districts or boroughs within a city, such as Brooklyn in New York City or City of Westminster in London.")
    stateDistrict: Optional[str] = Field(None, description="Second-level administrative division used in certain locales.")
    suburb: Optional[str] = Field(None, description="Unofficial neighborhood name, like Chinatown.")
    house: Optional[str] = Field(None, description="Building name, such as World Trade Center.")
    level: Optional[str] = Field(None, description="Floor number, such as 3F.")

class PaymentDetail(BaseModel):
    IBAN: Optional[str] = Field(None, description="International bank account number. Example: DE 94 700 700 100 029 49 00 00")
    SWIFT: Optional[str] = Field(None, description="ISO9362, an international standard for Business Identifier Codes (BIC). Example: DEUTDEMMXXX")
    BankAccountNumber: Optional[str] = Field(None, description="Bank account number, a unique identifier for a bank account. Example: 123456")
    BPayBillerCode: Optional[str] = Field(None, description="Biller code for BPay, an alphanumeric identifier unique to a biller or their product/service. Example: 123456")
    BPayReference: Optional[str] = Field(None, description="Reference number for BPay, a unique identifier for a specific customer's bill transaction. Example: 1234567")

class TaxDetail(BaseModel):
    Amount: Optional[Currency] = Field(None, description="The amount of the tax detail. Example: 29,520.00")
    Rate: Optional[str] = Field(None, description="The rate of the tax detail. Example: 18 %")

class PaidInFourInstallment(BaseModel):
    Amount: Optional[Currency] = Field(None, description="The installment amount due. Example: 29,520.00")
    DueDate: Optional[str] = Field(None, description="The installment due date. Example: 2024/01/01")

class Item(BaseModel):
    Amount: Currency = Field(None, description="The amount of the line item. Example: $60.00")
    Date: Optional[str] = Field(None, description="Date corresponding to each line item. Often it is a date the line item was shipped. Example: 3/4/2021")
    Description: Optional[str] = Field(None, description="The text description for the invoice line item. Example: Consulting service")
    Quantity: Optional[float] = Field(None, description="The quantity for this invoice line item. Example: 2")
    ProductCode: Optional[str] = Field(None, description="Product code, product number, or SKU associated with the specific line item. Example: A123")
    Tax: Currency = Field(None, description="Tax associated with each line item. Possible values include tax amount, tax %, and tax Y/N. Example: $6.00")
    TaxRate: Optional[str] = Field(None, description="Tax rate associated with each line item. Example: 18 %")
    Unit: Optional[str] = Field(None, description="The unit of the line item, e.g, kg, lb, etc. Example: hours")
    UnitPrice: Currency = Field(None, description="The net or gross price of one unit of this item. Example: $30.00")

class Document(BaseModel):
    CustomerName: Optional[str] = Field(None, description="Customer being invoiced. Example: Microsoft Corp")
    CustomerId: Optional[str] = Field(None, description="Reference ID for the customer. Example: CID-12345")
    PurchaseOrder: Optional[str] = Field(None, description="A purchase order reference number. Example: PO-3333")
    InvoiceId: Optional[str] = Field(None, description="ID for this specific invoice (often 'Invoice Number'). Example: INV-100")
    InvoiceDate: Optional[str] = Field(None, description="Date the invoice was issued. Example: 11/15/2019")
    DueDate: Optional[str] = Field(None, description="Date payment for this invoice is due. Example: 12/15/2019")
    VendorName: Optional[str] = Field(None, description="Vendor who has created this invoice. Example: CONTOSO LTD.")
    VendorAddress: Optional[Address] = Field(None, description="Mailing address for the Vendor. Example: 123 456th St, New York, NY 10001")
    VendorAddressRecipient: Optional[str] = Field(None, description="Name associated with the VendorAddress. Example: Contoso Headquarters")
    CustomerAddress: Optional[Address] = Field(None, description="Mailing address for the Customer. Example: 123 Other St, Redmond WA, 98052")
    CustomerAddressRecipient: Optional[str] = Field(None, description="Name associated with the CustomerAddress. Example: Microsoft Corp")
    BillingAddress: Optional[Address] = Field(None, description="Explicit billing address for the customer. Example: 123 Bill St, Redmond WA, 98052")
    BillingAddressRecipient: Optional[str] = Field(None, description="Name associated with the BillingAddress. Example: Microsoft Services")
    ShippingAddress: Optional[Address] = Field(None, description="Explicit shipping address for the customer. Example: 123 Ship St, Redmond WA, 98052")
    ShippingAddressRecipient: Optional[str] = Field(None, description="Name associated with the ShippingAddress. Example: Microsoft Delivery")
    SubTotal: Optional[Currency] = Field(None, description="Subtotal field identified on this invoice. Example: $100.00")
    TotalDiscount: Optional[Currency] = Field(None, description="Total discount field identified on this invoice. Example: $5.00")
    TotalTax: Optional[Currency] = Field(None, description="Total tax field identified on this invoice. Example: $10.00")
    InvoiceTotal: Optional[Currency] = Field(None, description="Total new charges associated with this invoice. Example: $110.00")
    AmountDue: Optional[Currency] = Field(None, description="Total Amount Due to the vendor. Example: $610.00")
    PreviousUnpaidBalance: Optional[Currency] = Field(None, description="Explicit previously unpaid balance. Example: $500.00")
    RemittanceAddress: Optional[Address] = Field(None, description="Explicit remittance or payment address for the customer. Example: 123 Remit St New York, NY, 10001")
    RemittanceAddressRecipient: Optional[str] = Field(None, description="Name associated with the RemittanceAddress. Example: Contoso Billing")
    ServiceAddress: Optional[Address] = Field(None, description="Explicit service address or property address for the customer. Example: 123 Service St, Redmond WA, 98052")
    ServiceAddressRecipient: Optional[str] = Field(None, description="Name associated with the ServiceAddress. Example: Microsoft Services")
    ServiceStartDate: Optional[str] = Field(None, description="First date for the service period. Example: 10/14/2019")
    ServiceEndDate: Optional[str] = Field(None, description="End date for the service period. Example: 11/14/2019")
    VendorTaxId: Optional[str] = Field(None, description="The government ID number associated with the vendor. Example: 123456-7")
    CustomerTaxId: Optional[str] = Field(None, description="The government ID number associated with the customer. Example: 765432-1")
    PaymentTerm: Optional[str] = Field(None, description="The terms under which the payment is meant to be paid. Example: Net90")
    KVKNumber: Optional[str] = Field(None, description="A unique identifier for businesses registered in the Netherlands. Example: 12345678")
    PaymentDetails: Optional[List[PaymentDetail]] = Field(None, description="List of payment details.")
    TaxDetails: Optional[List[TaxDetail]] = Field(None, description="List of tax details.")
    PaidInFourInstallments: Optional[List[PaidInFourInstallment]] = Field(None, description="List of installment details.")
    Items: Optional[List[Item]] = Field(None, description="List of line items.")
    description: str = Field(..., description="Description of the generated invoice.")
    

"""
Model for generating data based on the model for ADI
"""
class Product(BaseModel):
    Description: Optional[str] = Field(None, description="The text description for the invoice line item. Example: Consulting service")
    ProductCode: Optional[str] = Field(None, description="Product code, product number, or SKU associated with the specific line item. Example: A123")
    TaxRate: Optional[str] = Field(None, description="Tax rate associated with each line item. Example: 18 %")
    Unit: Optional[str] = Field(None, description="The unit of the line item, e.g, kg, lb, etc. Example: hours")
    UnitPrice: Currency = Field(None, description="The net or gross price of one unit of this item. Example: $30.00")
    Vendor: str = Field(..., description="Vendor who provides this product/item/service. Example: CONTOSO LTD.")

class Invoice(BaseModel):
    CustomerName: Optional[str] = Field(None, description="Customer being invoiced. Example: Microsoft Corp")
    InvoiceId: Optional[str] = Field(None, description="ID for this specific invoice (often 'Invoice Number'). Example: INV-100")
    InvoiceDate: Optional[str] = Field(None, description="Date the invoice was issued. Example: 11/15/2019")
    DueDate: Optional[str] = Field(None, description="Date payment for this invoice is due. Example: 12/15/2019")
    VendorName: Optional[str] = Field(None, description="Vendor who has created this invoice. Example: CONTOSO LTD.")
    VendorAddress: Address = Field(None, description="Mailing address for the Vendor. Example: 123 456th St, New York, NY 10001")
    VendorAddressRecipient: Optional[str] = Field(None, description="Name associated with the VendorAddress. Example: Contoso Headquarters")
    PaymentTerm: Optional[str] = Field(None, description="The terms under which the payment is meant to be paid. Example: Net90")
    Items: List[Item] = Field(None, description="List of line items.")
    description: str = Field(..., description="Description of the generated invoice.")

class Vendor(BaseModel):
    VendorName: str = Field(None, description="Vendor who has created this invoice. Example: CONTOSO LTD.")
    VendorAddress: Address = Field(None, description="Mailing address for the Vendor. Example: 123 456th St, New York, NY 10001")
    VendorAddressRecipient: str = Field(None, description="Name associated with the VendorAddress. Example: Contoso Headquarters")
    VendorDescription: str = Field(None, description="Description of the vendor including the products and services they offer.")

class Customer(BaseModel):
    CustomerName: Optional[str] = Field(None, description="Customer being invoiced. Example: Microsoft Corp")
    CustomerId: Optional[str] = Field(None, description="Reference ID for the customer. Example: CID-12345")
    CustomerAddress: Optional[Address] = Field(None, description="Mailing address for the Customer. Example: 123 Other St, Redmond WA, 98052")
    CustomerAddressRecipient: Optional[str] = Field(None, description="Name associated with the CustomerAddress. Example: Microsoft Corp")
    BillingAddress: Optional[Address] = Field(None, description="Explicit billing address for the customer. Example: 123 Bill St, Redmond WA, 98052")
    BillingAddressRecipient: Optional[str] = Field(None, description="Name associated with the BillingAddress. Example: Microsoft Services")
    ShippingAddress: Optional[Address] = Field(None, description="Explicit shipping address for the customer. Example: 123 Ship St, Redmond WA, 98052")
    ShippingAddressRecipient: Optional[str] = Field(None, description="Name associated with the ShippingAddress. Example: Microsoft Delivery")
