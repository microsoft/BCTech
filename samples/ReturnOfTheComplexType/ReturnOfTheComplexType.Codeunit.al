// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50130 ReturnOfTheComplexType
{
    procedure Test()
    var
        Customer: record Customer;
        Client: HttpClient;
    begin
        // Get first customer with name starting with 'spo'
        Customer := GetCustomerByName('spo');

        // Get and Show bing html.
        Message(GetBingHtml());
    end;



    /// <summary>
    /// Get the first customer with name starting with <paramref name="Name"/>
    /// </summary>
    /// <param name="Name">Name filter</param>
    /// <returns>First customer</returns>
    procedure GetCustomerByName(Name: Text): record Customer;
    var
        Customer: record Customer;
    begin
        Customer.SetFilter(Name, '@' + Name + '*');
        Customer.FindFirst();
        exit(Customer);
    end;

    /// <summary>
    /// Returns a bing-ready HttpClient
    /// </summary>
    /// <returns>Bing HttpClient</returns>
    procedure GetBingClient() Result: HttpClient;
    begin
        Result.SetBaseAddress('https://www.bing.com');
    end;

    /// <summary>
    /// Get the response from a request to bing.
    /// </summary>
    /// <returns>The response message</returns>
    procedure GetBingResponse() Response: HttpResponseMessage
    begin
        GetBingClient().Get('', Response)
    end;

    /// <summary>
    /// Get the response from www.bing.com as an html-string. 
    /// </summary>
    /// <returns>string with html</returns>
    procedure GetBingHtml() Result: Text;
    begin
        GetBingResponse().Content().ReadAs(Result);
    end;
}
