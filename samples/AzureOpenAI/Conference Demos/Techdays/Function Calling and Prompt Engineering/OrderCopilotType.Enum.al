// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------
namespace Techdays.Copilot.Order;

/// <summary>
/// The order types that the copilot can handle.
/// </summary>
enum 50100 "Order Copilot Type"
{
    Extensible = false;

    value(1; "Order Status")
    {
    }
    value(2; "Create Quote")
    {
    }
}