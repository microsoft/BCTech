import os
from pydantic import BaseModel
import yaml
from model import soa
from structured_data_generation import ElementCreator
from validator import TestSuiteValidator

class TestSuiteCreator(ElementCreator):
    # Specify the model for the response format
    response_format: BaseModel = soa.TestSuite

    def create(self, instructions: str) -> soa.TestSuite:

#region Seed example: test_setup.yaml
        # script_dir = os.path.dirname(__file__)
        # file_path = os.path.join(script_dir, "seeds/test_setup.yaml")
        # with open(file_path, 'r') as f:
        #     test_setup = yaml.safe_load(f)

        # Here's the existing test setup in YAML format. Create the tests based on this setup:
        # {test_setup}
#endregion

        self.request_prompt =  f"""
        You create test suites for the Sales Order Agent that processes emails in Dynamics 365 Business Central. The agent creates sales quotes and later converts these to sales orders.
        
        Be sure to: {instructions}        
        """

        print("🤖 Generating test suite...")
        result = self._create_internal()
        
        print("🤖 Validating test suite...")
        validator = TestSuiteValidator()
        validator.validate_test_suite(result)
        
        return result

if __name__ == "__main__":
    # Create output directory if it doesn't exist
    output_dir = "output"
    os.makedirs(output_dir, exist_ok=True)

    creator = TestSuiteCreator()
    result = creator.create("Create a test suite where the company sells running supplies. The unit of measures used are BOX and PCS. Generate a test with 2 turns.")

    # Write to file
    output_file = os.path.join(output_dir, "soa_test_dataset.yaml")
    with open(output_file, 'w') as f:
        yaml.dump(result.model_dump(by_alias=True, exclude_none=True), f, default_flow_style=False, sort_keys=False)
    
    print(f"\n\nTest dataset saved to {output_file}. Please review the errors in the console output.\n\n")