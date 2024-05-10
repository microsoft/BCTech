// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

page 50101 "Demo Page"
{
    PageType = List;
    ApplicationArea = All;
    UsageCategory = Administration;
    SourceTable = "Copilot Test Table";

    layout
    {
        area(Content)
        {
        }
    }

    actions
    {
        area(Prompting)
        {
            action(Demo1)
            {
                ApplicationArea = All;
                Image = Sparkle;
                RunObject = Page Demo1;
            }

            action(Demo2)
            {
                ApplicationArea = All;
                Image = Sparkle;
                RunObject = Page Demo2;
            }

            action(Demo3)
            {
                ApplicationArea = All;
                Image = Sparkle;
                RunObject = Page Demo3;
            }
        }
    }
}