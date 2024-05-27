using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PayloadGenerator
{
    internal class RCExtensionDefinition
    {
        public string id { get; set; }
        public string name { get; set; }
        string filename { get; set; }
        string extends{ get; set; }
        string where { get; set; }
        string tooltip { get; set; }

        //id = "50140" 
        //name = "BusinessManagerRoleCenterExt" 
        //filename = "BusinessManagerRoleCenterExt.PageExtension" 
        //extends = "Business Manager Role Center"
        //where = 'addafter("Excel Reports")'
        //tooltip = 'Power BI reports for finance'


        List<ActionDefinition> actions = new List<ActionDefinition>();

        public void addAction(ActionDefinition action) { actions.Add(action); }


        public RCExtensionDefinition(
            string id,
            string name,
            string filename,
            string extends,
            string where,
            string tooltip             
        )
        {
            this.id = id;
            this.name = name;
            this.filename = filename;
            this.extends = extends;
            this.where = where;
            this.tooltip = tooltip;
        }


        public XElement asXElement()
        {
            XElement pageExtension = new XElement("pageextension",
                new XAttribute("id", id),
                new XAttribute("name", name),
                new XAttribute("filename", filename),
                new XAttribute("extends", extends),
                new XAttribute("where", where),
                new XAttribute("tooltip", tooltip)
            );

            foreach(ActionDefinition action in actions)
            {
                XElement el = action.asXElement();
                pageExtension.Add(el);
            }            

            return pageExtension;
        }
    }
}
