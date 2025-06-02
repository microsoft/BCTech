using System.Xml.Linq;

namespace PayloadGenerator
{
    internal class ActionDefinition
    {
        public string rcExtensionName { get; set; }
        public string name { get; set; }       
        string caption { get; set; }
        string pbipagename { get; set; }
        string tooltip { get; set; }
        string obsolete { get; set; }

        public ActionDefinition(string rcExtenionName, string name, string caption, string pbipagename, string tooltip, string obsolete)
        {
            this.rcExtensionName = rcExtenionName;
            this.name = name;
            this.caption = caption;
            this.pbipagename = pbipagename;
            this.tooltip = tooltip;
            this.obsolete = obsolete;
        }

        public XElement asXElement()
        {
            XElement action = new XElement("action",
                new XAttribute("name", name),
                new XAttribute("caption", caption),
                new XAttribute("pbipagename", pbipagename),
                new XAttribute("tooltip", tooltip),
                new XAttribute("obsolete", obsolete)
            );

            return action;
        }
    }
}