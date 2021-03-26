---
title: Analyzing Posting Telemetry 
description: Learn about the posting telemetry in Business Central  
author: jswymer
ms.service: dynamics365-business-central
ms.topic: conceptual
ms.devlang: na
ms.tgt_pltfrm: na
ms.workload: na
ms.search.keywords: administration, tenant, admin, environment, sandbox, telemetry
ms.date: 04/01/2020
ms.author: jswymer
---

# Analyzing Posting Telemetry

**APPLIES TO:** [!INCLUDE[prod_short](../includes/prod_short.md)] 2020 release wave 2, update 17.2, and later

Posting telemetry gathers data about the posting of
- Purchase documents
- Sales documents
- Sales invoices


## Purchase document posted:  

Occurs when a purchase document was posted in the application.

### General dimensions

|Dimension|Description or value|
|---------|-----|
|message|**Purchace document posted: <document number> **|
|severityLevel|**1**|

### Custom dimensions

|Dimension|Description or value|
|---------|-----|
|aadTenantId|Specifies that Azure Active Directory (Azure AD) tenant ID used for Azure AD authentication. For on-premises, if you aren't using Azure AD authentication, this value is **common**. |
|alDataClassification|**SystemMetadata**|
|alDocumentNumber|Specifies the document number of the document that was posted. NB! only available from version 18.0 and later |
|alNumberOfLines|Specifies the the number of lines in the document. NB! This dimension is only available from version 18.0 and later. In prior versions, the dimension is called *alNumber of lines* |
|component|**Dynamics 365 Business Central Server**.|
|componentVersion|Specifies the version number of the component that emits telemetry (see the component dimension.)|
|environmentName|Specifies the name of the tenant environment. See [Managing Environments](tenant-admin-center-environments.md). This dimension isn't included for [!INCLUDE[prod_short](../includes/prod_short.md)] on-premises environments.|
|environmentType|Specifies the environment type for the tenant, such as **Production**, **Sandbox**, **Trial**. See [Environment Types](tenant-admin-center-environments.md#types-of-environments)|
|eventId|**AL0000CST**|
|extensionVersion|Version of the base app|
|telemetrySchemaVersion|Specifies the version of the [!INCLUDE[prod_short](../developer/includes/prod_short.md)] telemetry schema.|


## Sales document posted:  

Occurs when a sales document was posted in the application.

### General dimensions

|Dimension|Description or value|
|---------|-----|
|message|**Sales document posted: <document number> **|
|severityLevel|**1**|

### Custom dimensions

|Dimension|Description or value|
|---------|-----|
|aadTenantId|Specifies that Azure Active Directory (Azure AD) tenant ID used for Azure AD authentication. For on-premises, if you aren't using Azure AD authentication, this value is **common**. |
|alDataClassification|**SystemMetadata**|
|alDocumentNumber|Specifies the document number of the document that was posted. NB! only available from version 18.0 and later |
|alNumberOfLines|Specifies the the number of lines in the document. NB! This dimension is only available from version 18.0 and later. In prior versions, the dimension is called *alNumber of lines* |
|component|**Dynamics 365 Business Central Server**.|
|componentVersion|Specifies the version number of the component that emits telemetry (see the component dimension.)|
|environmentName|Specifies the name of the tenant environment. See [Managing Environments](tenant-admin-center-environments.md). This dimension isn't included for [!INCLUDE[prod_short](../includes/prod_short.md)] on-premises environments.|
|environmentType|Specifies the environment type for the tenant, such as **Production**, **Sandbox**, **Trial**. See [Environment Types](tenant-admin-center-environments.md#types-of-environments)|
|eventId|**AL0000CSU**|
|extensionVersion|Version of the base app|
|telemetrySchemaVersion|Specifies the version of the [!INCLUDE[prod_short](../developer/includes/prod_short.md)] telemetry schema.|


## Sales invoice posted:  

Occurs when a sales document was posted in the application.

### General dimensions

|Dimension|Description or value|
|---------|-----|
|message|**Sales document posted: <document number> **|
|severityLevel|**1**|

### Custom dimensions

|Dimension|Description or value|
|---------|-----|
|aadTenantId|Specifies that Azure Active Directory (Azure AD) tenant ID used for Azure AD authentication. For on-premises, if you aren't using Azure AD authentication, this value is **common**. |
|alDataClassification|**SystemMetadata**|
|alDocumentNumber|Specifies the document number of the invoice that was posted. NB! only available from version 18.0 and later |
|alNumberOfLines|Specifies the the number of sales lines in the invoice. NB! This dimension is only available from version 18.0 and later. In prior versions, the dimension is called *alNumber of lines* |
|component|**Dynamics 365 Business Central Server**.|
|componentVersion|Specifies the version number of the component that emits telemetry (see the component dimension.)|
|environmentName|Specifies the name of the tenant environment. See [Managing Environments](tenant-admin-center-environments.md). This dimension isn't included for [!INCLUDE[prod_short](../includes/prod_short.md)] on-premises environments.|
|environmentType|Specifies the environment type for the tenant, such as **Production**, **Sandbox**, **Trial**. See [Environment Types](tenant-admin-center-environments.md#types-of-environments)|
|eventId|**AL0000CZ0**|
|extensionVersion|Version of the base app|
|telemetrySchemaVersion|Specifies the version of the [!INCLUDE[prod_short](../developer/includes/prod_short.md)] telemetry schema.|


## See also

[Monitoring and Analyzing Telemetry](telemetry-overview.md)  
[Enabling Application Insights for Tenant Telemetry On-Premises](telemetry-enable-application-insights.md)  
