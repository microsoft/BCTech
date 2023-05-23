# Introduction

Microsoft Dynamics 365 Business Central includes the capability to track email message exchanges between salespeople and contacts so that the organization can manage customer relationships better. It's easy enough to set up, but it requires the organization to have enabled public folders on their mail server.  

Use this sample script as inspiration for how to set up public folders and mail flow rules in Exchange Online that are needed for the email logging functionality Business Central. For more information, see [Set up public folders and rules for email logging](https://learn.microsoft.com/dynamics365/business-central/marketing-set-up-email-logging#set-up-public-folders-and-rules-for-email-logging-in-exchange-online).

## Usage example

```powershell
SetupEmailLogging.ps1 -EmailLoggingUser emailLogging@sampleDomain.onmicrosoft.com
```

## Disclaimer

THE SAMPLE CODE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  
