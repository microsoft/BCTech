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
    internal class FieldDefinition
    {
        // <field DataItemFieldName = "customerId" FieldName="Id" Caption="Customer Id" Locked="true"/>

        [Index(0)]
        public string tableName { get; set; }
        [Index(1)]
        public string fieldName { get; set; }
        string caption { get; set; }
        bool locked { get; set; } = true;
        string dataItemFieldName { get; set; }


        public FieldDefinition(string tableName, string fieldName)
        {
            this.tableName = tableName;
            this.fieldName = fieldName;
            this.caption = "API query for field " + fieldName;
            this.dataItemFieldName = fieldName + "_APIQueryField";
        }


        public XElement asXElement()
        {
            XElement query = new XElement("field",
                new XAttribute("FieldName", fieldName.ToString()),
                new XAttribute("Caption", caption.ToString()),
                new XAttribute("Locked", locked.ToString()),
                new XAttribute("DataItemFieldName", dataItemFieldName.ToString())
            );

            return query;
        }
    }
}
