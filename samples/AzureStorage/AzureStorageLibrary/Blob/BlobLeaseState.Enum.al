// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

enum 50181 BlobLeaseState
{
    value(0; available) { }
    value(1; leased) { }
    value(2; expired) { }
    value(3; breaking) { }
    value(4; broken) { }
}