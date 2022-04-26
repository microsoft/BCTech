// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// Inteface for slow running code examples.
/// </summary>
interface "Slow Code Example"
{
    Access = Internal;

    procedure RunSlowCode();
    procedure GetHint(): Text;
    procedure IsBackground(): Boolean;
}