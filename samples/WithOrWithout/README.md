# With or without – that is not a question

In the Business Central team, we are laser-focused on running Business Central Online service smoothly without any interruptions.

Our powerful extensibility model makes it easy to add functionality on top of the Business Central platform and base application, but it is also the same extensibility model that makes continuous upgrade and delivery of new platform and application features a challenge. 

The extensibility model and the AL programming language is a successor to the C/AL language. We have a great legacy, but AL language comes with some built-in pitfalls from the past. One of these pitfalls has a name – the `with` statement". 

## TL;DR
This document describes why and more importantly how, we are going to remove this pitfall from Business Central development in the future. 

If you already feel intimidated by the length of this document please [see for yourself and try the sample code](README.md#see-for-yourself---try-the-sample-code) to at least get an understanding of the underlying problem and take look at the changes in the [Business Central 2020 Release Wave 2](README.md#business-central-2020-release-wave-2).

## The `with` statement
The `with` statement has always been controversial and a topic for almost religious discussions on readability. It brings the members of a variable into the closest [scope](README.md#symbol-lookup). While being convenient when writing code, it can make code harder to read (subject to personal preferences), but it can also prevent code in Business Central online from being upgraded without changes to the code or even worse - upgraded but with changed behavior.

Let's take look at a simple example:

```AL 
codeunit 50140 MyCodeunit
{
    procedure DoStuff()
    var
        Customer: Record Customer;
    begin
        with Customer do begin
            // Do some work on the Customer record.
            Name := 'Foo';

            if IsDirty() then 
                Modify();
        end;
    end; 

    local procedure IsDirty(): Boolean;
    begin
        exit(false);
    end;
} 
```

The `DoStuff()` procedure does some work on the `Customer` record and calls a local procedure `IsDirty()` to check whether to update the record or not. Looking at the code above it looks like it does absolutely nothing since the `IsDirty()` is returning `false` - assuming that the `IsDirty()` call in line 11 is in fact calling the local `IsDirty()` procedure.

I can of course write a test or hook up a debugger to check that the code is doing what I expect and then I'm fine, right? Not so fast! Maybe in an old and more static world, but in the SaaS world things are changing in a more rapid pace.

In Business Central online we recompile your code when we upgrade the platform and application. We recompile to ensure that it is working and to regenerate the runtime artifacts to match the new platform. 

We have made a promise not to break you without due warning, but the use of the `with` statement makes it impossible for us, as Microsoft, to make even additive changes in a completely non-breaking way. This problem is not isolated to changes made by Microsoft, any additive change has the potential to break a `with` statement in consuming code.

## Symbol lookup
Let's look at the code from above again. What would happen to that code if `IsDirty` was added to the base application between two releases? To understand that we need to take a look at how compilers turn syntax into symbols. When the AL compiler meets the `IsDirty` call it must bind the syntax name to a procedure symbol.   

When the AL compiler searches for the symbol `IsDirty()` in the sample above it will search in following order

1.	`Customer` table 
    1.	User-defined members on the Customer table and Customer table extensions
    2.	Platform-defined members e.g. `FindFirst()` or `Modify()`.
2.	`MyCodeunit` codeunit
    1.	User-defined members e.g. `IsDirty()`
    2.	Platform-defined members
3.	Globally defined members. 

The first time the search for ‘IsDirty’ finds the name ‘IsDirty’, it will not continue to the next top-level group. That means that if a procedure named `IsDirty` is introduced in the `Customer` table (platform or app) that procedure will be found instead of the procedure in `MyCodeunit`.

The procedure `IsDirty` is something we wanted to add to the built-in record functions in Business Central 2020 release wave 1, but it turned that it was already being used in several extensions that would break if we went ahead. We had to settle with only adding the method to the `RecordRef` which is a less optimal solution. 

> The solution for the explicit `with` is to stop using it. Using the `with` statement can make your code vulnerable to upstream changes. 

```AL 
// Safe version
codeunit 50140 MyCodeunit
{
    procedure DoStuff()
    var
        Customer: Record Customer;
    begin
        // Do some work on the Customer record.
        Customer.Name := 'Foo';

        if IsDirty() then 
            Customer.Modify();
    end; 

    local procedure IsDirty(): Boolean;
    begin
        exit(false);
    end;
} 
```

## The Implicit `with`
We sometimes refer to the `with` statement as the explicit `with`, because it is an explicit choice from the developer. The implicit `with` is even more insidious than the explicit `with`, because it is injected automatically by the compiler in certain situations. 

### Codeunits
When a codeunit has the `TableNo` property set, then there is an implicit `with` around the code inside the `OnRun` trigger. This is indicated with the comments in the code below.

```AL 
codeunit 50140 MyCodeunit
{
    TableNo = Customer;

    trigger OnRun()
    begin
        // with Rec do begin
        SetRange("No.", '10000', '20000');
        if Find() then
            repeat
            until Next() = 0;

        if IsDirty() then
            Message('Something is not clean');
        // end;
    end;

    local procedure IsDirty(): Boolean;
    begin
        exit(false);
    end;
} 
```

Similar to the previous code sample, the code looks like it will call the local `IsDirty`, but depending on the Customer table, extensions to the Customer table, and built-in methods it may not be the case. 

>The implicit `with` on the Rec variable can be mitigated by extracting the content of the `OnRun` into a separate procedure that takes the record as a parameter (see the code below). Doing that isolates the implicit `with` to the `OnRun` and only a name clash with the extracted method name can cause issues.

```AL 
codeunit 50140 MyCodeunit
{
    TableNo = Customer;

    trigger OnRun()
    begin
        MyOwnOnRunTrigger(Rec);
    end;

    local procedure MyOwnOnRunTrigger(Customer: Record Customer)
    begin
        Customer.SetRange("No.", '10000', '20000');
        if Customer.Find() then
            repeat
            until Customer.Next() = 0;

        if IsDirty() then
            Message('Something is not clean');
    end;

    local procedure IsDirty(): Boolean;
    begin
        exit(false);
    end;
} 
```
### Pages
Pages on tables have the same type of implicit `with`, but around the entire object. Everywhere inside the page object the fields and methods from the source tables are directly available without any prefix.

```AL 
page 50143 ImplicitWith
{
    SourceTable = Customer;

    layout
    {
        area(Content)
        {
            field("No."; "No.") { }
            field(Name; Name)
            {
                trigger OnValidate()
                begin
                    Name := 'test';
                end;
            }
        }
    }

    trigger OnInit()
    begin
        if IsDirty() then Insert()
    end;

    local procedure IsDirty(): Boolean
    begin
        exit(Name <> '');
    end;
} 
```

On pages it is not only the code in triggers and procedures that is spanned by the implicit `with` on the source Rec; also the source expressions for the fields are covered.

>Where the solutions for explicit `with` and implicit `with` in codeunits also works on existing version, there is no good solution for pages until Business Central 2020 Wave 2. 

## See for yourself - try the sample code
Open the sample code using your favorite editor (Visual Studio Code) and language extension (AL Language).

The code contains a [codeunit](ExplicitWith.Codeunit.al) with an explicit `with` statement and a [codeunit](ImplicitWith.Codeunit.al) and a [page](ImplicitWith.Page.al) with implicit `with`. 

There is also a [tableextension](UpstreamChange.al) that simulates changes made to the Customer table outside this project. The file contains two different versions of `IsDirty`; one with the same signature as in three other files and one with a different signature.

### Same signature 
If the signature in upstream change matches the original signature, the code will still compile, but the behavior of the called method may have changed. 

Try uncomment the first version of the IsDirty method in the UpstreamChange file. You can verify what method the compiler resolves the IsDirty call to by putting the cursor on the method call and Go to definition (F12). It will now go to the method from the UpstreamChange.

This type of upstream change can change the runtime behavior of your code without any warning. 

### Different signature
Make sure that the method with the same signature is commented out again and uncomment the second version of the IsDirty method in the UpstreamChange file. Instantly all the calls to IsDirty are now flagged as AL0135 errors because of a missing argument.

In the case of a different signature your code won't compile which can prevent your code from being upgraded.

## Business Central 2020 Release Wave 2 
From Business Central 2020 release wave 2 we will begin to warn about the use of explicit/implicit `with` for extensions with targeting the cloud. There will be two different warnings: AL0604 and AL0606.

That will for some of you result in a lot of warnings that you eventually will have to fix. 

>It is important to note that the warnings are only warnings for now, but we will at some point in the future turn them into errors. We will at the earliest remove `with` statement support from the Business Central 2021 release wave 2.

#### AL0606 - use of explicit `with` 
The warning has a QuickFix code-action that allows you to convert the statement(s) inside the `with` statement to fully-qualified statements. Same as described above.

#### AL0604 - use of implicit `with`
Just qualifying with `Rec.` will not solve the problem. The `IsDirty()` will still be vulnerable to upstream change. We need to get rid of the implicit `with`, but we would also like an opt-in model to avoid forcing everyone to upgrade their code at once. 

The solution for that is to introduce pragmas in AL. A pragma is an instruction to the compiler on how it should understand the code. The pragma instructs the compiler not to create an implicit `with` for the `Rec` variable.

Syntax for the implicit with pragma. The pragma must be used before the beginning of the codeunit or page.

```AL 
#pragma implicitwith disable|restore
```

The fixed page looks like this

```AL 
#pragma implicitwith disable
page 50143 ImplicitWith
{
    SourceTable = Customer;

    layout
    {
        area(Content)
        {
            field("No."; Rec."No.") { }
            field(Name; Rec.Name)
            {
                trigger OnValidate()
                begin
                    Rec.Name := 'test';
                end;
            }
        }
    }

    trigger OnInit()
    begin
        if IsDirty() then Rec.Insert()
    end;

    local procedure IsDirty(): Boolean
    begin
        exit(Name <> '');
    end;
} 
#pragma implicitwith restore
```

The QuickFix code-actions will automatically insert the pragma before and after the fixed object.

>Tip: Remember to enable CodeActions in the settings for the AL Language extension.

### "Can I please hide the warnings for now. I promise to fix them later?"
No developers like 1000 new warnings poured out over their code overnight. We are aware of this and for that reason have we added two new ways of suppressing warnings in Business Central 2020 release wave 2. You can use them to unclutter your warnings while working on other issues and fix the warnings at your own pace.

Warnings can be suppressed globally in an extension in app.json. The syntax is:

```AL 
"suppressWarnings": [ "AL0606", "AL0604" ]
```

It is also possible to use a pragma to suppress individual warnings for one or more lines of code.  

```AL 
#pragma warning disable AL0606
    // No AL0606 will be shown for code here.
    with Customer do begin
        Name := 'Foo';
        Insert();
    end;

#pragma warning restore AL0606
// Suppression of AL0606 is restored to global state.
```

## Without a question
The `with` statement is problematic and there is no way to fix it. It has been with (no pun intended) us for many years, but times have changed and we have to let it go.

Please help us by helping yourself and your customers. Start removing the use `with` statement from your code. You can get the full effect already from the Business Central 2020 release wave 2.	
