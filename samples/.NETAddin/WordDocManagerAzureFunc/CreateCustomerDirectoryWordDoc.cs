// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WordDocManagerAzureFunc
{
    public static class CreateCustomerDirectoryWordDoc
    {
        private static WordDocManager docManager = new WordDocManager();

        // Azure Function App 
        [FunctionName("CreateCustomerDirectoryWordDoc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("CreateCustomerDirectoryWordDocument HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // For debugging
            log.LogInformation("Request body:");
            log.LogInformation(requestBody);

            // Deserialize the customer list from JSON
            var customers = JsonConvert.DeserializeObject<List<Customer>>(requestBody);
            
            // Create the actual Word document in a stream
            Stream stream = docManager.CreateCustomerDirectoryDocument(customers);

            // Return the document as a binary stream
            stream.Position = 0;
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer, 0, (int)stream.Length);
            return new FileContentResult(buffer, "application/octet-stream");
        }
    }
}
