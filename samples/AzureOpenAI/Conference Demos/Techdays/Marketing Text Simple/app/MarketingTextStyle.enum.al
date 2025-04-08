// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

namespace Techdays.AITestToolkitDemo;

enum 50100 "Marketing Text Style"
{
    Extensible = false;
    Access = Internal;

    value(0; Formal)
    {
        Caption = 'Formal';
    }
    value(1; Verbose)
    {
        Caption = 'Verbose';
    }
    value(2; Casual)
    {
        Caption = 'Casual';
    }
}