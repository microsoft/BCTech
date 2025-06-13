import os
from pydantic import BaseModel, Field
from openai import AzureOpenAI
from azure.identity import DefaultAzureCredential, get_bearer_token_provider

class VerificationResult(BaseModel):
    """Structured response for LLM verification"""
    verified: bool = Field(description="Whether the expected data matches the email request")
    reason: str = Field(description="Explanation of the verification result")

class TestSuiteValidator:
    """Handles validation of generated test suites"""
    
    def __init__(self):
        self._init_azure_openai()
    
    def _init_azure_openai(self):
        """Initialize Azure OpenAI client using environment variables"""
        try:
            endpoint = os.getenv('AZURE_OPENAI_ENDPOINT')
            deployment = os.getenv('AZURE_OPENAI_DEPLOYMENT')
            api_version = os.getenv('OPENAI_API_VERSION')
            
            if not endpoint or not deployment:
                self.az_client = None
                print("⚠️  Azure OpenAI environment variables not set. LLM verification will be skipped.")
                return
                
            token_provider = get_bearer_token_provider(
                DefaultAzureCredential(), 
                "https://cognitiveservices.azure.com/.default"
            )
            
            self.az_client = AzureOpenAI(
                api_version=api_version,
                azure_endpoint=endpoint,
                azure_ad_token_provider=token_provider
            )
            self.deployment_name = deployment
            
        except Exception as e:
            self.az_client = None
            print(f"⚠️  Failed to initialize Azure OpenAI client: {e}")
    
    def validate_test_suite(self, test_suite):
        """Validate the generated test suite for consistency and correctness"""
        for test in test_suite.tests:
            validation_passed = self._validate_single_test(test)
            self._print_test_result(test.name, validation_passed)
    
    def _validate_single_test(self, test) -> bool:
        """Validate a single test and return True if validation passed"""
        if not test.test_setup:
            return True
            
        setup_data = self._extract_setup_data(test.test_setup)
        
        for turn in test.turns:
            if not turn.expected_data:
                continue
                
            if not self._validate_turn_data(turn, setup_data):
                return False
                
        return True
    
    def _validate_turn_data(self, turn, setup_data) -> bool:
        """Validate quotes and sales orders in a turn"""
        # Validate quotes
        if turn.expected_data.quotes:
            for quote in turn.expected_data.quotes:
                if not self._validate_document(quote, turn.incoming_email.from_, setup_data):
                    return False
        
        # Validate sales orders
        if turn.expected_data.orders:
            for order in turn.expected_data.orders:
                if not self._validate_document(order, turn.incoming_email.from_, setup_data):
                    return False
        
        # Validate attachment property
        if not self._validate_attachment_property(turn.expected_data):
            return False
        
        # Validate expected data against email content using LLM
        if not self._validate_expected_data_with_llm(turn.expected_data, turn.incoming_email, setup_data):
            return False
        
        return True
    
    def _validate_document(self, document, from_email, setup_data) -> bool:
        """Validate a single document (quote or sales order)"""
        items_valid = self._validate_items(document.lines or [], setup_data['items'])
        customer_valid = self._validate_customer(document.customerName, from_email, 
                                                setup_data['customers'], setup_data['contacts'])
        return items_valid and customer_valid

    def _validate_items(self, expected_items, item_setup) -> bool:
        """Validate that expected items exist in the setup and have correct properties"""
        for expected_item in expected_items:
            if not self._validate_single_item(expected_item, item_setup):
                return False
        return True
    
    def _validate_single_item(self, expected_item, item_setup) -> bool:
        """Validate a single item against the setup data"""
        # Find the item in setup
        item_selected = self._find_item_in_setup(expected_item, item_setup)
        if not item_selected:
            return False  # Error already printed in _find_item_in_setup
        
        # Validate item properties
        return (self._validate_item_description(expected_item, item_selected) and
                self._validate_unit_of_measure(expected_item, item_selected) and
                self._validate_variant_codes(expected_item, item_selected))
    
    def _find_item_in_setup(self, expected_item, item_setup):
        """Find an item in the setup by itemNo or description"""
        if hasattr(expected_item, 'itemNo') and expected_item.itemNo:
            item_selected = next((i for i in item_setup if i.itemNo == expected_item.itemNo), None)
            if not item_selected:
                self._print_red(f"Item {expected_item.itemNo} not found in items list")
            return item_selected
            
        elif hasattr(expected_item, 'itemDescription') and expected_item.itemDescription:
            item_selected = next((i for i in item_setup if i.description == expected_item.itemDescription), None)
            if not item_selected:
                self._print_red(f"Item {expected_item.itemDescription} not found in items list")
            return item_selected
            
        return None
    
    def _validate_item_description(self, expected_item, item_selected) -> bool:
        """Validate item description consistency"""
        if (hasattr(expected_item, 'itemNo') and expected_item.itemNo and 
            hasattr(expected_item, 'itemDescription') and expected_item.itemDescription and
            not hasattr(expected_item, 'variantCode')):
            
            if item_selected.description != expected_item.itemDescription:
                self._print_red(f"Item description {expected_item.itemDescription} is not correct for item {item_selected.itemNo}")
                return False
        return True
    
    def _validate_unit_of_measure(self, expected_item, item_selected) -> bool:
        """Validate unit of measure against available units"""
        if not (hasattr(expected_item, 'unitOfMeasure') and expected_item.unitOfMeasure):
            return True
        
        valid_units = self._get_valid_units(item_selected)
        
        # Validate that the expected unit exists in valid units
        if not valid_units:
            self._print_red(f"No valid units of measure found for item {item_selected.itemNo}")
            return False
            
        if expected_item.unitOfMeasure not in valid_units:
            self._print_red(f"Unit of measure {expected_item.unitOfMeasure} not found in item {item_selected.itemNo}")
            return False
            
        return True
    
    def _get_valid_units(self, item_selected):
        """Get all valid units of measure for an item"""
        valid_units = []
        if hasattr(item_selected, 'baseUnitOfMeasure') and item_selected.baseUnitOfMeasure:
            valid_units.append(item_selected.baseUnitOfMeasure)
        if hasattr(item_selected, 'unitsOfMeasure') and item_selected.unitsOfMeasure:
            valid_units.extend([unit.code for unit in item_selected.unitsOfMeasure])
        return valid_units
    
    def _validate_variant_codes(self, expected_item, item_selected) -> bool:
        """Validate variant codes against available variants"""
        if not (hasattr(expected_item, 'variantCode') and expected_item.variantCode):
            return True
        
        if not (hasattr(item_selected, 'variants') and item_selected.variants):
            self._print_red(f"Variants not found in item {item_selected.itemNo}")
            return False
        
        expected_variants = set(expected_item.variantCode.split('|'))
        selected_variants = set([variant.code for variant in item_selected.variants])
        
        if not expected_variants.issubset(selected_variants):
            self._print_red(f"Variant code {expected_item.variantCode} not found in item {item_selected.itemNo}")
            return False
            
        return True

    def _validate_attachment_property(self, expected_data) -> bool:
        """Validate that attachment property is true if quotes or orders exist"""
        has_documents = (expected_data.quotes and len(expected_data.quotes) > 0) or (expected_data.orders and len(expected_data.orders) > 0)
        
        if has_documents and not expected_data.attachments:
            self._print_red("Attachment property should be true when quotes or orders are created")
            return False
            
        return True

    def _validate_customer(self, expected_customer_name, customer_email, customers, contacts):
        """Validate that the customer exists and has the correct email or contact"""
        if not expected_customer_name:
            return True  # No customer specified is valid
            
        customer_selected = next((c for c in customers if c.name == expected_customer_name), None)
        if not customer_selected:
            self._print_red(f"Customer {expected_customer_name} not found in customer list")
            return False
        
        # Check if email matches customer or their contacts
        if customer_selected.email != customer_email:
            contact_selected = next((c for c in contacts
                                   if c.email == customer_email
                                   and c.companyName == expected_customer_name),
                                   None)
            if not contact_selected:
                self._print_red(f"Customer name {expected_customer_name} does not have the email or any corresponding contact with email {customer_email}")
                return False
        
        return True  # Validation passed

    def _validate_expected_data_with_llm(self, expected_data, incoming_email, setup_data) -> bool:
        """Use LLM to verify that the expected data matches the email request."""

        if not hasattr(self, 'az_client') or not self.az_client:
            print("📧 LLM email verification - SKIPPED (client not available)")
            return True
        try:
            prompt = f"""Verify that the expected sales data correctly responds to the incoming email.

INCOMING EMAIL:
From: {incoming_email.from_}
Subject: {incoming_email.subject}
Body: {incoming_email.body}

EXPECTED DATA:
{expected_data.model_dump() if hasattr(expected_data, 'model_dump') else expected_data}

VALIDATION CRITERIA:
1. Expected quotes/orders match what was requested in the incoming email
2. Expected outgoing email response is appropriate for the incoming email and generated quotes/orders

Return verified=true only if the expected data properly responds to the incoming email."""

            response = self.az_client.beta.chat.completions.parse(
                model=self.deployment_name,
                messages=[
                    {"role": "system", "content": "You are a data validation expert. Verify if expected sales data properly responds to incoming emails."},
                    {"role": "user", "content": prompt}
                ],
                response_format=VerificationResult
            )
            
            verification_result = response.choices[0].message.parsed
            
            if not verification_result.verified:
                self._print_red(f"📧 LLM Verification failed: {verification_result.reason}")
                return False
            
            print(f"📧 LLM email verification - PASSED: {verification_result.reason}")
            return True
            
        except Exception as e:
            self._print_red(f"📧 LLM Verification error: {e}")
            return False
    
    def _print_test_result(self, test_name: str, validation_passed: bool):
        """Print the validation result"""
        print("\n")
        if validation_passed:
            print(f"✅ Test '{test_name}'")
        else:
            print(f"❌ Test '{test_name}'")

    def _extract_setup_data(self, test_setup):
        """Extract and organize setup data for validation"""
        return {
            'items': test_setup.itemsToCreate or [],
            'customers': test_setup.customersToCreate or [],
            'contacts': test_setup.contactsToCreate or []
        }
    
    def _print_red(self, message):
        """Print error message in red color"""
        print(f"\033[91m{message}\033[0m")