// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

permissionsetextension 50100 "CTF Challenges" extends "System App - Basic"
{

    Permissions = tabledata Cereal = RIMD,
                  tabledata Milk = RIMD,
                  tabledata "Quick Item Flag_6e5b1753" = RIMD,
                  tabledata "CTF Challenges Setup" = RIMD,
                  tabledata Treasure = RI,
                  tabledata AlienDancer = RIMD;
}