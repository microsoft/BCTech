// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

table 50131 Drives
{
    fields
    {
        field(1; Name; Text[512])
        {
        }
    }

    [IntegrationEvent(true, false)]
    procedure MyCrazeEvent();
    begin

    end;
}