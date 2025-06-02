/// <summary>
/// Enum for the relationship between the attachment and the data.
/// Factur-X Version 1.07.2 (ZUGFeRD v. 2.3.2) | November 15th, 2024, section 6.2.2 Data relationship 
/// TODO: Replace standard reference with the Adope PDF v2.0 reference move this file to system app.
/// </summary>
enum 50950 PdfAttachmentDataRelationShip
{
    Extensible = true;

    /// <summary>
    /// The embedded file contains data which is used for the visual representation in the PDF part, 
    /// e.g. for a table or a graph. 
    /// </summary>
    value(0; Data)
    {
    }

    /// <summary>
    /// The embedded file contains the source data for the visual representation derived therefrom in 
    /// the PDF part, e.g. a PDF file created via an XSL transformation from an (embedded) XML source file or the 
    /// MS Word file from which the PDF file was created. 
    /// </summary>    
    value(1; Source)
    {
    }

    /// <summary>
    /// This data relationship should be used if the embedded data are an alternative 
    /// representation of the PDF contents. 
    /// </summary>
    value(2; Alternative)
    {
    }

    /// <summary>
    /// This data relationship is used if the embedded file serves neither as the source nor as 
    /// the alternative representation, but the file contains additional information, e.g. on easier automatic 
    /// processing.
    /// </summary>
    value(3; Supplement)
    {
    }

    /// <summary>
    /// This data relationship term applies where none of the data relationships above 
    /// apply, or where there is an unknown data relationship.
    /// </summary>  
    value(4; Unspecified)
    {
    }
}


