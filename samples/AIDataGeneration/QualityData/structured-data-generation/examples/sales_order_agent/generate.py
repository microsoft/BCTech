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







#region Generate test suites from test suites file
    # print("\n🌱 Generating test suites from test suites file...")
    # script_dir = os.path.dirname(__file__)
    # test_suites_file_path = os.path.join(script_dir, "seeds/test_suites.yaml")
    
    # if not os.path.exists(test_suites_file_path):
    #     print(f"❌ Test suites file not found at {test_suites_file_path}")
    #     exit()
    
    # with open(test_suites_file_path, 'r') as f:
    #     test_suites_data = yaml.safe_load(f)
    #   # Parse test_suites structure
    # test_suites_list = test_suites_data.get('test_suites', [])
    # test_suites = {}
    # total_tests = 0
    
    # for test_suite_obj in test_suites_list:
    #     suite_name = test_suite_obj.get('name')
    #     if not suite_name:
    #         raise ValueError(f"Test suite missing required 'name' field: {test_suite_obj}")
            
    #     test_cases = test_suite_obj.get('test_cases', [])
    #     test_suites[suite_name] = test_cases
    #     total_tests += len(test_cases)
        
    # print(f"📊 Found {total_tests} test cases across {len(test_suites)} test suites")
    #   # Generate test suites for each test suite
    # for test_suite_name, test_cases in test_suites.items():
    #     print(f"\n📂 Processing test suite: {test_suite_name} ({len(test_cases)} test cases)")
        
    #     # Generate all test cases for this test suite
    #     generated_tests = []
    #     for i, test_case in enumerate(test_cases):
    #         test_name = test_case.get('name', f'Test {i+1}')
    #         test_description = test_case.get('description', '')
            
    #         if not test_description:
    #             print(f"⚠️ Skipping {test_name} - no description found")
    #             continue
                
    #         print(f"🧪 Generating {i+1}/{len(test_cases)}: {test_name}")
            
    #         test_result = TestSuiteCreator().create(test_description)
    #         # Extract the individual tests from the generated test suite
    #         if hasattr(test_result, 'tests') and test_result.tests:
    #             generated_tests.extend(test_result.tests)
        
    #     # Create one test suite containing all generated tests
    #     if generated_tests:
    #         suite_result = {
    #             'test_suite': {
    #                 'name': test_suite_name,
    #                 'tests': [test.model_dump(by_alias=True, exclude_none=True) for test in generated_tests]
    #             }
    #         }
            
    #         # Save the complete test suite to one file
    #         suite_filename = "".join(c.lower() if c.isalnum() else '-' for c in test_suite_name).strip('-')
    #         suite_output_file = os.path.join(output_dir, f"{suite_filename}.yaml")
            
    #         print(f"💾 Saving to: {suite_output_file}")
    #         try:
    #             with open(suite_output_file, 'w') as f:
    #                 yaml.dump(suite_result, f, default_flow_style=False, sort_keys=False)
    #             print(f"✅ Test suite with {len(generated_tests)} tests saved to {suite_output_file}")
    #         except Exception as e:
    #             print(f"❌ Error saving file: {e}")
    #             print(f"Debug - test suite: '{test_suite_name}', filename: '{suite_filename}'")
    
    # print(f"\n✅ Completed generating test suites for {len(test_suites)} test suites")

#endregion