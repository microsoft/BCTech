using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace PayloadGenerator
{
    internal class PermSetDefinition
    {
        public string id { get; set; }
        public string name { get; set; }
        public string filename { get; set; }
        public string caption { get; set; }

        //id = "50140" 
        //name = "PBI EMBED VIEW" 
        //filename = "PBIEmbedView.PermissionSet.al"
        //caption = "PBI Embed - View"

        public PermSetDefinition(
            string id,
            string name,
            string filename,
            string caption
        )
        {
            this.id = id;
            this.name = name;
            this.filename = filename;
            this.caption = caption;
        }


        public XElement asXElement()
        {
            XElement permSet = new XElement("permissionset",
                new XAttribute("id", id),
                new XAttribute("name", name),
                new XAttribute("filename", filename),
                new XAttribute("caption", caption)
            );

            return permSet;
        }
    }
}
