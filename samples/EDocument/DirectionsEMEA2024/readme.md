# Getting Started

Look at the connector and format specifc code. 

- Open project in VS Code
- Run “Download Symbols”
- Select “Microsoft Clould Sandbox”
- Change “environmentName” in launch.json
- Run “Download Symbols” again
- Follow steps to authenticate vscode with your sandbox
- Then you can publish
- Additional information for sandbox setup : [Get started with AL - Business Central | Microsoft Learn](https://learn.microsoft.com/en-us/dynamics365/business-central/dev-itpro/developer/devenv-get-started#steps-to-set-up-a-sandbox-environment-and-visual-studio-code)
- Follow the Tasks To Complete 
- Do the capture the flag - Optional

# Demo API 
https://bc-edoc-workshop.azurewebsites.net/demo-api/

### Important information 

When you call the API, you must provide two headers. Authorization and Service. Authorization is a bearer token. 
Service is a name to identify your connector. You can pick anything. Example: MyConnector123 

All API calls must have Bearer token. The token itself is "secret"

All API calls must have Service header. Pick a service name for your team.

Example:
#### Authorization: Bearer secret
#### Service: MyTeamName

It is also a good idea to read the Interface documentation. You can control click the interfaces that the codeunit implements to read more.

# Tasks to complete

## Implement OpenServiceIntegrationSetupPage to open demo setup page
On the EDocument service the action "Open Integration Setup". On the setup page, set the field API Key to "secret". Then set the service name to your team name. If you use the functions in the IntegrationHelpers.Codeunit.al, the fields from the demo service will be added as headers automatically.

## Implement Send Async with API.
Then post sales invoice for customer with "EDocuments" Doc Sending profile. You need to change this for the customer you are sending to. 
Go to the E-Document and check the logs. If you have GetResposne status, then you completed sending async. Check the communication logs for the first flag. 

You need to communicate to the service endpoint by sending a http request.
Use the IntegrationHelpers.Codeunit.al use the PrepareRequestMessage to make it easier, and use HttpClient to send it.

You also need to use WriteBlobToRequestMessage to send the XML to the service.

## Implement GetResponse. 
Then for a edocument with PendingResponse status, run Get Response job queue manually. If the e-document is not Sent status, then you completed sending and get response. Check the communication logs for the second flag.

Similar to Send, you need to communicate to the service endpoint by sending a http request.
Use the IntegrationHelpers.Codeunit.al use the PrepareRequestMessage to make it easier, and use HttpClient to send it.

## Implement Approve
On Sent EDocument click approve. The implementation should return true to mark edocument approved. Check the communication logs for the third flag.

Similar to Send, you need to communicate to the service endpoint by sending a http request.
Use the IntegrationHelpers.Codeunit.al use the PrepareRequestMessage to make it easier, and use HttpClient to send it.


## Implement Receive. 
On the EDoc service page, you can click receive, or use auto import. The service will get one of the documents you sent to it. 


# Capture the flag - Win a Microsoft t-shirt and some Merch 

If you want, we added some flags in the communication logs response messages. 
If you collect them and get the right right url, and can show that to the instructors. 
You will get the flags by completing the above actions. 

Url looks like
https://bc-edoc-workshop.azurewebsites.net/{Flag1}/{Flag2}/{Flag3}

There is 1 t-shirt for the first.

We also have stickers.

# Endpoints 
 
##  Root Endpoint - 
GET /
 
Returns a simple greeting message.

Response:

200 OK: Returns "Hello World".

## Send XML
 
POST /demo-api/send
 
Saves the provided XML data to a file based on the Service header and the cbc:ID element in the XML.

Headers:

Authorization: Bearer

Service: <service_name> 

Request Body:
XML data with cbc:ID element.

Response:
- 200 OK: Returns {"message": "Received invoice <invoice_id>"} if the XML is successfully saved.
- 400 Bad Request: If the content type is not application/xml.
- 401 Unauthorized: If the Bearer token is missing or invalid.
- 401 Unauthorized: If the Service header is missing.


## Get Response from service
 
GET /demo-api/getresponse
 
Checks if a specific XML file exists based on the provided invoice_id and Service header.

Headers:

Authorization: Bearer

Service: <service_name>

Query Parameters:
invoice_id: The ID of the invoice to check.

Response:
- 200 OK: Returns {"message": "Success"} if the file exists.
- 200 OK: Returns {"message": "Not Received"} if the file does not exist.
- 400 Bad Request: Returns {"message": "invoice_id parameter is required"} if invoice_id is not provided.
- 401 Unauthorized: If the Bearer token is missing or invalid.
- 401 Unauthorized: If the Service header is missing.

## Approval of sent edocument

GET /demo-api/approve
 
Get if edoucment is approved

Headers:

Authorization: Bearer <token>

Service: <service_name>

Response:
- 200 OK: 


## Receive File

GET /demo-api/receive
 
Retrieves the most recent XML file for the specified service.

Headers:

Service: <service_name>

Response:
- 200 OK: Returns the content of the most recent XML file.
- 404 Not Found: Returns {"message": "No files found in upload directory"} if no files are found.
- 500 Internal Server Error: Returns {"message": "Error reading file", "error": "<error_message>"} if there is an error reading the file.
- 401 Unauthorized: If the Service header is missing.


