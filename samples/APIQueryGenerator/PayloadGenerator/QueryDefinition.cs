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
    internal class QueryDefinition
    {
        //   <query id ="50000" name="QueryForTable1" filename="50000_QueryForTable1" Caption="customers" Locked="true" EntityName="Customer" EntitySetName="Customers" DataItemName="Customer" TableName ="Customers">

        [Index(0)]
        public string tableName { get; set; }
        public int id { get; set; }
        string name { get; set; }
        string filename { get; set; }
        string caption { get; set; }
        bool locked { get; set; } = true;
        string entityName { get; set; }
        string entitySetName { get; set; }
        string dataItemName { get; set; }

        //public QueryDefinition(int id, string name, string filename, string caption, bool locked, string entityName, string entitySetName, string dataItemName, string tableName)
        //{
        //    this.id = id;
        //    this.name = name;
        //    this.filename = filename;
        //    this.caption = caption;
        //    this.locked = locked;
        //    this.entityName = entityName;
        //    this.entitySetName = entitySetName;
        //    this.dataItemName = dataItemName;
        //    this.tableName = tableName;
        //}

        //public QueryDefinition(int id, string tableName)
        //{
        //    this.id = id;
        //    this.name = tableName + "_APIQuery";
        //    this.filename = tableName + "_APIQuery";
        //    this.caption = tableName;
        //    this.entityName = tableName;
        //    this.entitySetName = tableName;
        //    this.dataItemName = tableName;
        //    this.tableName = tableName;
        //}

        List<FieldDefinition> fields = new List<FieldDefinition>();

        public void addField(FieldDefinition field) { fields.Add(field); }


        public QueryDefinition(string tableName)
        {
            this.name = tableName + "_APIQuery";
            this.filename = tableName + "_APIQuery";
            this.caption = "API query for table " + tableName;
            this.entityName = tableName;
            this.entitySetName = tableName;
            this.dataItemName = tableName + "_APIQuery";
            this.tableName = tableName;
        }


        public XElement asXElement()
        {
            XElement query = new XElement("query",
                new XAttribute("TableName", tableName.ToString()),
                new XAttribute("id", id.ToString()),
                new XAttribute("name", name.ToString()),
                new XAttribute("filename", filename.ToString()),
                new XAttribute("Caption", caption.ToString()),
                new XAttribute("Locked", locked.ToString()),
                new XAttribute("EntityName", entityName.ToString()),
                new XAttribute("EntitySetName", entitySetName.ToString()),
                new XAttribute("DataItemName", dataItemName.ToString())
            );

            foreach(FieldDefinition field in fields)
            {
                XElement el = field.asXElement();
                query.Add(el);
            }            

            return query;
        }
    }
}
