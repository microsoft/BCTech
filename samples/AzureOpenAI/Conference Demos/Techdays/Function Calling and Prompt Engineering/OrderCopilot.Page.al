// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

using Microsoft.Sales.Document;

page 50100 "Order Copilot"
{
    Caption = 'Order Copilot';
    DataCaptionExpression = PageCaptionTxt;
    PageType = PromptDialog;
    ApplicationArea = All;
    UsageCategory = Administration;
    Extensible = false;
    Editable = true;
    InherentEntitlements = X;
    InherentPermissions = X;

    layout
    {
        area(Prompt)
        {
            field(UserQueryTxt; UserQueryTxt)
            {
                ApplicationArea = All;
                MultiLine = true;
                ShowCaption = false;
                ToolTip = 'Enter your query here. You can use natural language to describe what you are looking for.';
                Caption = 'User Query';
                InstructionalText = 'Create sales quote or get order status by enter user query.';
            }
        }

        area(Content)
        {

            grid(MyGrid)
            {
                GridLayout = Rows;

                group(DocumentDetails)
                {
                    Caption = 'Order Copilot Response';
                    Editable = false;
                    field("Details"; Details)
                    {
                        Caption = 'Details';
                        ToolTip = 'Specifies the type of details shown below.';
                        StyleExpr = 'Bold';
                        Editable = false;
                        ShowCaption = false;
                    }

                    field("Document"; DocumentLine)
                    {
                        Caption = 'Document';
                        ToolTip = 'Specifies document type and number.';
                        Editable = false;

                        trigger OnDrillDown()
                        begin
                            TempResult.ShowSourceHeaderDocument();
                        end;
                    }

                    group(DocumentLineGroup)
                    {
                        ShowCaption = false;
                        Visible = IsStatusVisible;
                        Editable = false;

                        field("Status"; Status)
                        {
                            Caption = 'Status';
                            ToolTip = 'Specifies the status of the document.';
                            StyleExpr = StatusStyleTxt;
                            Editable = false;
                        }
                    }

                }


            }

        }

        area(PromptOptions)
        {

        }
    }

    actions
    {
        area(SystemActions)
        {
            systemaction(Generate)
            {
                Caption = 'Generate';
                ToolTip = 'Generate from Copilot.';

                trigger OnAction()
                begin
                    OrderCopilotImpl.StartOrderCopilot(UserQueryTxt, TempResult);
                    PageCaptionTxt := UserQueryTxt;
                    SetDocumentLine();
                end;

            }
            systemaction(OK)
            {
                Caption = 'Ok';
                ToolTip = 'Save the changes and close the page.';
            }
            systemaction(Cancel)
            {
                Caption = 'Discard';
                ToolTip = 'Discard the changes and close the page.';
            }
        }
        area(PromptGuide)
        {

            group(GetStatusGroup)
            {
                Caption = 'Get Status';
#pragma warning disable AW0005
                action(StatusFromOrderPrompt)
#pragma warning restore AW0005
                {
                    Caption = 'Get status of order [No.]';
                    ToolTip = 'Sample prompt for getting the status of a sales order. Text in brackets specifies the order no.';

                    trigger OnAction()
                    var
                        CopyFromLbl: Label 'Get status of order ';
                    begin
                        UserQueryTxt := CopyFromLbl;
                        CurrPage.Update(false);
                    end;
                }
#pragma warning disable AW0005
                action(StatusFromInvoicePrompt)
#pragma warning restore AW0005
                {
                    Caption = 'Order: [No.]';
                    ToolTip = 'Sample prompt for getting the status of a sales order. Text in brackets specifies the order no.';

                    trigger OnAction()
                    var
                        CopyFromLbl: Label 'Order: ';
                    begin
                        UserQueryTxt := CopyFromLbl;
                        CurrPage.Update(false);
                    end;
                }
            }
            group(CreateSalesQuote)
            {
                Caption = 'Create Sales Quote';
#pragma warning disable AW0005
                action(CreateSalesQuote1Prompt)
#pragma warning restore AW0005
                {
                    Caption = 'I need [Quantity] [Item Name] for [Customer Name]';
                    ToolTip = 'Sample prompt for creating a sales quote. Text in brackets specifies the quantity, item and customer.';

                    trigger OnAction()
                    var
                        CopyFromLbl: Label 'I need [Quantity] [Item Name] for [Customer Name] ';
                    begin
                        UserQueryTxt := CopyFromLbl;
                        CurrPage.Update(false);
                    end;
                }
#pragma warning disable AW0005
                action(CreateSalesQuote2Prompt)
#pragma warning restore AW0005
                {
                    Caption = 'Add item: [quantity] [description]';
                    ToolTip = 'Sample prompt for adding an item to a sales quote. Text in brackets specifies the quantity and description.';

                    trigger OnAction()
                    var
                        CopyFromLbl: Label 'Add item: ';
                    begin
                        UserQueryTxt := CopyFromLbl;
                        CurrPage.Update(false);
                    end;
                }

            }
        }
    }

    trigger OnOpenPage()
    begin
        IsStatusVisible := false;
    end;

    local procedure SetDocumentLine()
    var
        StatusLineDocSearchTxt: Label '%1 %2', Comment = '%1 = Document Type, %2 = Document No.';
    begin
        case TempResult.Type of
            TempResult.Type::"Order Status":
                begin
                    Details := 'Following is the status of the sales order you requested:';
                    DocumentLine := StrSubstNo(StatusLineDocSearchTxt, 'Sales Order', TempResult.DocumentNo);
                    Status := TempResult.GetStatus();
                    StatusStyleTxt := TempResult.GetStatusStyleText(Status);
                    IsStatusVisible := true;
                    CurrPage.Update(false);
                end;

            TempResult.Type::"Create Quote":
                begin
                    Details := 'Following is the sales quote created for you. Please review and confirm:';
                    DocumentLine := StrSubstNo(StatusLineDocSearchTxt, 'Sales Quote', TempResult.DocumentNo);
                    IsStatusVisible := false;
                    CurrPage.Update(false);
                end;
        end;
    end;

    trigger OnQueryClosePage(CloseAction: Action): Boolean
    begin
        if CloseAction <> CloseAction::OK then
            case TempResult.Type of
                TempResult.Type::"Create Quote":
                    TempResult.DeleteSalesQuote();
            end;
    end;

    var
        TempResult: Record "Order Copilot Response" temporary;
        OrderCopilotImpl: Codeunit "Order Copilot Impl";
        Status: Enum "Sales Document Status";
        UserQueryTxt: Text;
        DocumentLine: Text;
        StatusStyleTxt: Text;
        Details: Text;
        PageCaptionTxt: Text;
        IsStatusVisible: Boolean;
}