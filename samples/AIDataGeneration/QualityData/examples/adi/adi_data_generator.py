import sys
from typing import Type
from pydantic import BaseModel
from models import adi

from structured_data_generation import ElementCreatorAgent, DataGeneratorSwarm

class VendorCreatorAgent(ElementCreatorAgent):
    element_type: Type[BaseModel] = adi.Vendor

    def instructions(context_variables: dict) -> str:
        customers = ElementCreatorAgent._get_elements_of_type(context_variables, adi.Customer)

        return f"""
        You are a synthetic vendor creator agent.
        You create vendors that sell products and services to one of the available customers.
        In the description of the vendor, explain how the vendor is related to the customer.

        These are the available customers: {customers}.
        """

class CustomerCreatorAgent(ElementCreatorAgent):
    element_type: Type[BaseModel] = adi.Customer

    instructions: str = """
        You are a synthetic customer data creator agent.
        You create customers that buy products and services.
        Provide data for the required fields in the customer data.",
        """

class ProductCreatorAgent(ElementCreatorAgent):
    element_type: Type[BaseModel] = adi.Product

    def instructions(context_variables: dict) -> str:
        customers = ElementCreatorAgent._get_elements_of_type(context_variables, adi.Customer)
        vendors = ElementCreatorAgent._get_elements_of_type(context_variables, adi.Vendor)

        return f"""
        You are a synthethic product/item/service inventory generator agent.
        The items/services have to be relevant to the customers, {customers}.
        The item/service is provided by these vendors {vendors}.
        """

class InvoiceCreatorAgent(ElementCreatorAgent):
    element_type: Type[BaseModel] = adi.Invoice

    def instructions(context_variables: dict) -> str:
        customers = ElementCreatorAgent._get_elements_of_type(context_variables, adi.Customer)
        vendors = ElementCreatorAgent._get_elements_of_type(context_variables, adi.Vendor)
        items = ElementCreatorAgent._get_elements_of_type(context_variables, adi.Product)

        return f"""
        f"You are an invoice creator agent. The customers are {customers}."
        f"You can create invoices from the the following vendors: {vendors}."
        f"The vendors sells the following items/services: {items}."
        "You need to come up for data for each of the required fields in the invoice. Make up any data as needed."
        """
    
if __name__ == "__main__":
    swarm = DataGeneratorSwarm(
        creators = [
            CustomerCreatorAgent(),
            VendorCreatorAgent(),
            ProductCreatorAgent(),
            InvoiceCreatorAgent()],
        debug=False
    )

    request = sys.argv[1]
    result = swarm.run(request)

    print(result.model_dump_json(indent=2))
"""
Example: 
    python .\adi_data_generator.py 'Generate the customer "The Bean Boutique" (Located in Washington State, US.). Create an invoice for them'
    python .\adi_data_generator.py 'Generate the customer "The Bean Boutique" (Located in Washington State, US.). Create two invoices for 2026 for the customer.'
    python .\adi_data_generator.py 'Generate the customer "The Bean Boutique" (At The Bean Boutique, we offer more than just coffee beans. Explore unique accessories, stylish drinkware, and gift sets that celebrate the art of coffee. Located in Washington State, US.). Generate 2 vendors for them. For each vendor create 3 items that they could sell. Finally create 5 invoices from the vendors to the customer'
"""
