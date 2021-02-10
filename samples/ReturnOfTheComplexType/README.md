# Return of the Complex Type

It is not a secret that AL has its roots in C/AL. That heritage comes with millions of lines of already written code, but also with a list of design choices and limitations that comes from another era and runtime.
Our core priority has been to preserve as much of the existing C/AL and at the same time make AL a world-class modern language for modular cloud development.

We have already added various new extensibility features to the language, and we will continue to do that. With Business Central 2021 Wave 1, we will deliver the idea with most votes from the https://aka.ms/bcideas site; Report Extensibility. It will be a big step forward for our reporting story.

When you develop a programming language it will inevitably be compared against many other languages. Each computer language has its own strength and history. AL is great when it comes to building advanced business logic and UI, but it also has it shortcomings in other areas. It doesn’t mean that we are satisfied with the current state and won’t innovate/improve the core AL language – on the contrary. 

(C/)AL has always had a limited set of possible return types for procedures. Only simple types could be returned, but this is now changing. From Business Central 2021 Wave 1 most types can be returned from procedures - both user-defined types and most built-in types.
Look at the code sample below:

```AL
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
```

It is also possible to use a named return value. Internally the exit-statement causes an assignment to an allocated return value. The assignment will have a small performance cost based on the type. Since the record type is treated as a value-type it is better. 

```AL
    procedure GetCustomerByName(Name: Text) Customer: record Customer;
    begin
        Customer.SetFilter(Name, '@' + Name + '*');
        Customer.FindFirst();
    end;
```

The method GetCustomerByName() returns a Customer Record. It can be used as you would expect:
        
```AL
    // Get first customer with name starting with 'spo'
    Customer := GetCustomerByName('spo');
```

The returned value doesn’t have to be used in an assignment statement. It can be used as part of an expression like:

```AL
    // Use the returned value as an expression.
    DoSomethingWithSales(GetCustomerByName('spo').GetSalesLCY());
```

This is not only for user-defined types like records, codeunits, etc., but also for built-in types. Everyone who has tried to create reusable and maintainable code using the built-in HttpClient has felt the pain of not being able to return some of the built-in types. With this change it is possible to write code like this: 

```AL
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
```

This is not a new unique language feature for AL. In fact, it was probably more unique that AL didn’t support it, meaning “unique” in a bad way. 

It is not the goal to make AL a generic all-purpose language like C#. AL is a language built for writing Business Applications for the cloud and to be a leader in that field. AL will never get full parity with languages like C#, but we will continue to improve the language and remove cases where AL is “unique” in bad ways.
