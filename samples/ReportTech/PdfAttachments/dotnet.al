// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
dotnet
{
    assembly("Microsoft.BusinessCentral.DocumentProcessor")
    {
        Culture = 'neutral';
        PublicKeyToken = '31bf3856ad364e35';

        type("Microsoft.BusinessCentral.DocumentProcessor.WordTransformation"; "WordTransformation")
        {
        }

        type("Microsoft.BusinessCentral.DocumentProcessor.MailMerge"; "MailMerge")
        {
        }

        type("Microsoft.BusinessCentral.DocumentProcessor.PdfDocumentInfo"; "PdfDocumentInfo")
        {
        }

        type("Microsoft.BusinessCentral.DocumentProcessor.PdfConverter"; "PdfConverter")
        {
        }

        type("Microsoft.BusinessCentral.DocumentProcessor.PdfTargetDevice"; "PdfTargetDevice")
        {
        }

        type("Microsoft.BusinessCentral.DocumentProcessor.PdfAttachmentManager"; "PdfAttachmentManager")
        {
        }

        type("Microsoft.BusinessCentral.DocumentProcessor.PdfAttachment"; "PdfAttachment")
        {
        }
        type("Microsoft.BusinessCentral.DocumentProcessor.WordTransformation"; "WordHandler")
        {
        }
    }

    assembly("mscorlib")
    {
        type("System.Collections.Generic.Dictionary`2"; "SystemDictionary_Of_T_U")
        {
        }

        type("System.Array"; "SystemArray")
        {
        }

        type("System.Collections.Generic.List`1"; "SystemList_Of_T")
        {
        }

        type("System.Collections.Generic.IList`1"; "SystemIList_Of_T")
        {
        }

        type("System.IO.MemoryStream"; MemoryStream)
        {
        }
    }
}
