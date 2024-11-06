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

All API calls must have Bearer token. The token itself is "secret"

All API calls must have Service header. Pick a service name for your team.

Example:
#### Authorization: Bearer secret
#### Service: MyTeamName

# Tasks to complete

## Implement Send Async with API.
Then post sales invoice for customer with EDoc Doc Sending profile. 
## Implement GetResponse. 
Then for a edocument with PendingResponse status, run Get Response job queue.

## Implement Receive. 
On the EDoc service page, you can click receive, or use auto import 

## Implement Approve
On Sent EDocument click approve.


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


## Get Response
 
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

## Approval

GET /demo-api/approve
 
Get if edoucment is approved

Headers:

Authorization: Bearer <token>

Service: <service_name>

Response:
- 200 OK: 


## CustomAction

GET /demo-api/customaction
 
CustomAction endpoint

Headers:

Authorization: Bearer <token>

Service: <service_name>

Response:
- 200 OK: 
