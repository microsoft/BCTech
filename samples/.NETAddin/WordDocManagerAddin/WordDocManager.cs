// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WordDocManagerAddin
{
    public class WordDocManager
    {
        public Stream CreateCustomerDirectoryDocument(List<Object> customers)
        {
            MemoryStream result = new MemoryStream();

            using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(result, WordprocessingDocumentType.Document))
            {
                MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());

                foreach (var customer in customers.Cast<List<Object>>())
                {
                    Paragraph para = body.AppendChild(new Paragraph());
                    Run run = para.AppendChild(new Run());

                    foreach (var item in customer.Cast<string>())
                    {
                        run.AppendChild(new Text(item));
                        CarriageReturn cr = new CarriageReturn();
                        run.AppendChild(cr);
                    }
                }

                wordDocument.Close();
            }


            result.Position = 0;
            return result;
        }
    }
}
