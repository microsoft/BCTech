// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

permissionset 50100 "CTF Challenges Admin"
{
    Assignable = true;

    Permissions = tabledata "CTF Challenges Setup" = RIMD;
}