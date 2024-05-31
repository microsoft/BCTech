using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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
        string PBIReportId { get; set; }
        string PBIReportName { get; set; }
        string PBIReportPage {  get; set; }

        public PBIPageDefinition(
            string id, string name, string fileName, string caption, string aboutTitle
            , string aboutText, string PBIReportId, string PBIReportName, string PBIReportPage
        )
        {
            this.id = id;
            this.name = name;
            this.filename = fileName;
            this.caption = caption;
            this.aboutTitle = aboutTitle;
            this.aboutText = aboutText;
            this.PBIReportId = PBIReportId;
            this.PBIReportName = PBIReportName;
            this.PBIReportPage = PBIReportPage;
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
                new XAttribute("PBIReportId", PBIReportId),
                new XAttribute("PBIReportName", PBIReportName),
                new XAttribute("PBIReportPage", PBIReportPage)
            );

            return page;
        }
    }
}
