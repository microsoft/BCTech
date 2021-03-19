// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;
using System.IO;

namespace WordDocManagerAzureFunc
{
    internal class WordDocManager
    {
        // Create a Word document using the OpenXML library
        internal Stream CreateCustomerDirectoryDocument(List<Customer> customers)
        {
            MemoryStream result = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(result, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                foreach (var customer in customers)
                {
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());

                    run.AppendChild(new Text(customer.name));
                    run.AppendChild(new CarriageReturn());
                    run.AppendChild(new Text(customer.street));
                    run.AppendChild(new CarriageReturn());
                    run.AppendChild(new Text(string.Format("{0} {1}", customer.zipCode, customer.city)));
                    run.AppendChild(new CarriageReturn());
                }

                wordDocument.Close();
            }

            result.Position = 0;
            return result;
        }
    }
}
