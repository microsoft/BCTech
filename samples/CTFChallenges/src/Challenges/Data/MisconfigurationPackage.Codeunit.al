// ------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// ------------------------------------------------------------------------------------------------

/// <summary>
/// A data CTF challenge.
/// </summary>
codeunit 50113 "Misconfiguration Package" implements "CTF Challenge"
{
    Access = Internal;

    procedure RunChallenge()
    var
        TempBlob: Codeunit "Temp Blob";
        XMLDOMManagement: Codeunit "Config. XML Exchange";
        FlagOutStream: OutStream;
        BlobInStream: InStream;
    begin
        TempBlob.CreateOutStream(FlagOutStream);
        FlagOutStream.WriteText(GetCtfRapidStartXML());

        TempBlob.CreateInStream(BlobInStream);
        XMLDOMManagement.ImportPackageXMLFromStream(BlobInStream);
    end;

    procedure GetHints(): List of [Text]
    var
        Hints: List of [Text];
    begin
        Hints.Add('Running the challenge imports a configuration package. Try applying it.');
        Hints.Add('Try navigating to the configuration package card to see what tables in contains.');
        exit(Hints);
    end;

    procedure GetCategory(): Enum "CTF Category"
    begin
        exit(Enum::"CTF Category"::Data);
    end;


    local procedure GetCtfRapidStartXML(): Text
    begin
        exit(
        '<?xml version="1.0" standalone="yes"?>' +
        '<DataList MinCountForAsyncImport="5" ExcludeConfigTables="1" LanguageID="1033" ProductVersion="NAV16.0" PackageName="Microsoft Dynamics 365 Business Central" Code="CTF Package">' +
            '<FinanceChargeTermsList>' +
                '<TableID>5</TableID>' +
                '<FinanceChargeTerms>' +
                    '<Code PrimaryKey="1" ProcessingOrder="1">CTF</Code>' +
                    '<InterestRate ProcessingOrder="2">1.5</InterestRate>' +
                    '<MinimumAmountLCY ProcessingOrder="3">10</MinimumAmountLCY>' +
                    '<AdditionalFeeLCY ProcessingOrder="4">10</AdditionalFeeLCY>' +
                    '<Description ProcessingOrder="5">Flag_8c54c28e</Description>' +
                    '<InterestCalculationMethod ProcessingOrder="6">0</InterestCalculationMethod>' +
                    '<InterestPeriodDays ProcessingOrder="7">30</InterestPeriodDays>' +
                    '<GracePeriod ProcessingOrder="8">&lt;5D&gt;</GracePeriod>' +
                    '<DueDateCalculation ProcessingOrder="9">&lt;1M&gt;</DueDateCalculation>' +
                    '<InterestCalculation ProcessingOrder="10">0</InterestCalculation>' +
                    '<PostInterest ProcessingOrder="11">1</PostInterest>' +
                    '<PostAdditionalFee ProcessingOrder="12">1</PostAdditionalFee>' +
                    '<LineDescription ProcessingOrder="13">%4% finance charge of %6</LineDescription>' +
                    '<AddLineFeeinInterest ProcessingOrder="14">0</AddLineFeeinInterest>' +
                    '<DetailedLinesDescription ProcessingOrder="15">Sum finance charge of %5</DetailedLinesDescription>' +
                '</FinanceChargeTerms>' +
            '</FinanceChargeTermsList>' +
        '</DataList>'
        )
    end;
}