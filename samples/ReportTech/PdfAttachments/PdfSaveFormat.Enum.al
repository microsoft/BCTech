/// <summary>
/// Enum for the PDf formats that can be used.
/// Factur-X Version 1.07.2 (ZUGFeRD v. 2.3.2) | November 15th, 2024, section 6.2.2 Data relationship 
/// TODO: Replace standard reference with the Adope PDF v2.0 reference move this file to system app.
/// </summary>

enum 50951 PdfSaveFormat
{
    Extensible = false;

    /// <summary>
    /// Save the PDF in the default format (traditionally PDF version 1.7). Platform will decide the PDF version.
    /// </summary>
    value(0; Default)
    {
    }

    /// <summary>
    /// Save the PDF in the PDF/A-3B format. This will not update the embedded XMP metadata.
    /// </summary>
    value(1; PdfA3B)
    {
    }

    /// <summary>
    /// Save the PDF in the PDF/A-3B format and add the embedded XMP metadata requried by E-Invoice standards like ZUGFeRD/Facturec.
    /// </summary>
    value(2; Einvoice)
    {
    }
}
