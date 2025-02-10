using System.Xml.Linq;

namespace PayloadGenerator
{
    internal class PBIPageDefinition
    {
//        [Index(0)]
        string id { get; set; }
        public string name { get; set; }
        string filename { get; set; }
        string caption { get; set; }
        string aboutTitle { get; set; }
        string aboutText { get; set; }
        string PBIReportIdFieldName { get; set; }
        string PBIReportName { get; set; }
        string PBIReportPage {  get; set; }
        string obsolete { get; set; }
        string applicationArea { get; set; }

        public PBIPageDefinition(
            string id, string name, string fileName, string caption, string aboutTitle,
            string aboutText, string PBIReportId, string PBIReportName, string PBIReportPage,
            string obsolete, string applicationArea
        )
        {
            this.id = id;
            this.name = name;
            this.filename = fileName;
            this.caption = caption;
            this.aboutTitle = aboutTitle;
            this.aboutText = aboutText;
            this.PBIReportIdFieldName = PBIReportId;
            this.PBIReportName = PBIReportName;
            this.PBIReportPage = PBIReportPage;
            this.obsolete = obsolete;
            this.applicationArea = applicationArea;
        }


        public XElement asXElement()
        {
            XElement page = new XElement("page",
                new XAttribute("id", id),
                new XAttribute("name", name),
                new XAttribute("filename", filename),
                new XAttribute("caption", caption),
                new XAttribute("aboutTitle", aboutTitle),
                new XAttribute("aboutText", aboutText),
                new XAttribute("PBIReportIdFieldName", PBIReportIdFieldName),
                new XAttribute("PBIReportName", PBIReportName),
                new XAttribute("PBIReportPage", PBIReportPage),
                new XAttribute("obsolete", obsolete),
                new XAttribute("applicationArea", applicationArea)
            );

            return page;
        }
    }
}
