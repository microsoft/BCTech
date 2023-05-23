// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 
// ------------------------------------------------------------------------------------------------

codeunit 50138 SharedAccessTokenGenerator
{
    Access = Internal;

    // 
    // SharedAccessToken Generator for ServiceBus
    //
    // Documentation: https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-sas
    // 
    [NonDebuggable]
    procedure GetSasToken(ResourceUri: Text; SharedKeyName: Text; SharedKeyValue: Text): Text;
    var
        TypeHelper: codeunit "Type Helper";
        EncryptionManagement: codeunit "Cryptography Management";
        EncodedResourceUri: Text;
        expiry: Text;
        stringToSign: Text;
        signature: Text;
        Cr: Text[1];
        TimeToLive: Integer;
    begin
        EncodedResourceUri := TypeHelper.UrlEncode(resourceUri);
        TimeToLive := 10000;

        Cr[1] := 10;

        expiry := GetExpiry(TimeToLive);
        stringToSign := EncodedResourceUri + Cr + expiry;
        signature := EncryptionManagement.GenerateHashAsBase64String(stringToSign, SharedKeyValue, 2 /* HMACSHA256 */);

        exit(StrSubstNo('SharedAccessSignature sr=%1&sig=%2&se=%3&skn=%4',
            EncodedResourceUri,
            TypeHelper.UrlEncode(signature),
            expiry,
            SharedKeyName));
    end;

    local procedure GetExpiry(ttl: Integer): Text;
    var
        expirySinceEpoch: BigInteger;
    begin
        expirySinceEpoch := (CurrentDateTime() - CreateDateTime(19700101D, 0T)) div 1000 + ttl;
        exit(Format(expirySinceEpoch));
    end;
}

