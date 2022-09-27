# Multiple Power BI reports on the Role Center

Since Microsoft Dynamics 365 Business Central version 2022 Wave 2 (21.0), it's possible to have multiple Power BI reports embedded in a Role Center at the same time.

Even though none of the built-in Role Centers takes advantage of this functionality as of today, you can create a new Role Center (or extend an existing one) to leverage it.

## Teaser: end result

![A screenshot of a Business Central Role Center with 4 Power BI reports](./Screenshot.png 'Teaser: end result')

## How it worked before

If you worked with Power BI embedded into Business Central, you know reports have always been visualized inside a `part` (and will continue to be). This part can be hosted in a factbox (such as in `page 31 "Item List"`), in a Role Center part (such as in `page 9022 "Business Manager Role Center"`), or as part in a list or card page (such as in `page 1156 "Company Detail"`).

These parts behave all in the same way: each part must be initialized with a `context`, which represents a keyword to identify the set of reports you want to visualize in that specific `part`. If no context is provided, the current user's `profile/role` is used instead.

Until today, the way to specify a `context` for a Power BI part was to call some code from the main page (usually in the trigger `OnOpenPage`), for example:

```csharp  <!-- For lack of better syntax highlighting -->
page 50000 "Company Detail"
{
    layout
    {
        area(content)
        {
            group("Power BI")
            {
                part(PowerBIPartOne; "Power BI Embedded Report Part")
                {
                    ApplicationArea = All;
                }
            }
        }
    }

    trigger OnOpenPage()
    begin
        CurrPage.PowerBIPartOne.PAGE.SetPageContext('DetailedCompanyReports'); 
        // If you are using older versions, this might look like:
        // CurrPage.PowerBIPartOne.PAGE.SetContext('DetailedCompanyReports'); 
    end;
}
```

This has a few consequences:
1. If you want to visualize the same set of reports in two different pages (for example, in both the List page and Card page for a certain record), you just need to use the same `context` for the two pages, and the same reports will show up.
1. If you want to have two parts hosted in the same page, showing a different set of reports, you just need to use two different `context`'s for them.
1. Role Center pages have no triggers, which means no way to call the `SetPageContext` function as in the snippet above. As a consequence, any Power BI part in the role center would have the same `context`: the current user's `profile/role`.

This is until Wave 2 2022 (21.0).

## How it works now

Since Wave 2 2022 (21.0), we added a small new feature that allows for different `context`'s to be specified for different Role Center parts.

The way to achieve this is by using the `SubPageView` property. The following snippet is completely equivalent to the snippet above, and will show exactly the same reports (but, unlike the snippet above, this can be used in a Role Center as well).

```csharp  <!-- For lack of better syntax highlighting -->
page 50000 "Company Detail"
{
    layout
    {
        area(content)
        {
            group("Power BI")
            {
                part(PowerBIPartOne; "Power BI Embedded Report Part")
                {
                    ApplicationArea = All;
                    SubPageView = where(Context = const('DetailedCompanyReports'));

                }
            }
        }
    }
}
```

Notice, this is backwards compatible: if you selected some reports using the old way of specifying a `context`, you will see these same reports when specifying a `context` using the new way.

In this folder, you can find an example of the `Business Manager Role Center` adapted to host 4 different Power BI report parts (using different `context`'s).

### Limitations
There are currently some limitations on how the `SubPageView` property is handled.
1. The `SubPageView` property for the Power BI parts only supports constant values, NOT filters.
1. If you use both ways of specifying a `context` at the same time, the `SetPageContext` value will overwrite the value specified in the `SubPageView` property.
1. Dynamically changing the context (for example, by calling `SetPageContext` in the `OnAfterGetCurrRecord`) is not fully supported and could lead to unexpected consequences.


