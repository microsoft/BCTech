import json
import os
from pydantic import BaseModel
import yaml
from model import soa
from structured_data_generation import ElementCreator

class TestSetupCreator(ElementCreator):
    response_format: BaseModel = soa.TestSetup

    def create(self) -> soa.TestSetup:
        script_dir = os.path.dirname(__file__)
        file_path = os.path.join(script_dir, "seeds/test_setup.yaml")
        with open(file_path, 'r') as f:
            data = yaml.safe_load(f)
        
        self.request_prompt =  f"""
        Create a test setup with items, customers, and contacts for the Sales Order Agent.
        Don't create quotes.

        The created element are for this customer: Phoenix Runners Supply. 
        Phoenix Runners Supply is a locally owned small business specializing in running gear and accessories, serving the Phoenix, AZ area.". 

        Create 10 items, 3 customers that might buy these items, and 5 contacts that work for the customers.

        Here's an example in YAML format: 
        {data}
        """
        # Units of measure: PCS, BOX, SET, PACK, GR, KG

        return self._create_internal()

class TestSuiteCreator(ElementCreator):
    response_format: BaseModel = soa.TestSuite

    def create(self, instructions: str) -> soa.TestSuite:
        script_dir = os.path.dirname(__file__)
        
        file_path = os.path.join(script_dir, "seeds/test_setup.yaml")
        with open(file_path, 'r') as f:
            test_setup = yaml.safe_load(f)
        
        file_path = os.path.join(script_dir, "seeds/p0.yaml")
        with open(file_path, 'r') as f:
            p0 = yaml.safe_load(f)
        
        self.request_prompt =  f"""
        You create test suites for the Sales Order Agent that processes emails in
        Dynamics 365 Business Central. The agent creates sales quotes and later converts
        these to sales orders.

        Here's an example of setup for a test suite in YAML format. Names and descriptions need to be realistic and specific.
        {test_setup}
        
        Here's an example of a test suite in YAML format:
        {p0}
        
        Be sure to: {instructions}
        """

        result = self._create_internal()
        
        self._validate_test_suite(result)
        
        return result

    def _validate_test_suite(self, test_suite: soa.TestSuite):
        item_prices = {item.description: item.unitPrice for item in test_suite.test_setup.itemsToCreate}

        for tests in test_suite.tests:
            for turn in tests.turns:
                if not turn.expected_data or not turn.expected_data.quotes:
                    continue
                for quote in turn.expected_data.quotes:
                    total = 0
                    for line in quote.lines:
                        line.lineAmount = line.quantity * item_prices.get(line.itemDescription)
                        total += line.lineAmount
                    total = round(total, 2)
                    if total != quote.totalExclVAT:
                        print(f"Error in test {tests.name}: total {quote.totalExclVAT} does not match calculated total {total} for quote {quote.customerName}")
                    quote.totalExclVAT = total

if __name__ == "__main__":
    creator = TestSuiteCreator()
    result = creator.create("Create a test suite where the company sells running supplies. The unit of meaures used are BOX and PCS. Generate a test with 2 turns.")
    print(yaml.dump(result.model_dump(exclude_none=True), default_flow_style=False))
"""
name: Running Supplies Agent
test_setup:
  contactsToCreate:
  - companyName: Trail Warriors
    email: elena.miles@trailwarriors.com
    name: Elena Miles
    phoneNo: (310) 555-1000
  customersToCreate:
  - address: 456 Terrain Drive
    city: Beverly Hills
    countryRegionCode: US
    email: info@trailwarriors.com
    name: Trail Warriors
    phoneNo: (310) 555-1000
    postCode: 90210
  itemsToCreate:
  - baseUnitOfMeasure: PRS
    description: Trail Running Shoes
    itemNo: RunSup-001
    unitPrice: 120.0
    unitsOfMeasure:
    - code: BOX
      quantityPerUnitOfMeasure: 2
  - baseUnitOfMeasure: PCS
  ...
"""