using System;
using System.Xml;
using System.Text;
using System.IO;
using Mono.Options;
using System.Collections.Generic;
using System.Net.Sockets;

namespace APIQueryGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html
            bool show_help = false;
            string inputfile = "";
            string outputdir = "";
            bool nosystemfields = false;

            var p = new OptionSet() { 
                { "h|help",  "show this message and exit", v => show_help = v != null },
                { "i|inputfile=",  "input xml file (required)", v => inputfile = v },
                { "o|outputdir=",  "output directory (if not specified, then input file name will be used)", v => outputdir = v },
                { "s|nosystemfields",  "Do not add system fields to API queries (default is that they will be added)", v => nosystemfields = v != null }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try using --help' for more information.");
                Environment.Exit(0);
            }

            if (show_help)
            {
                ShowHelp(p);
                Environment.Exit(0);
            }

            if (inputfile.Equals(""))
            {
                ShowHelp(p);
                Environment.Exit(0);
            }

            Console.WriteLine("API query payload generator");

            Console.WriteLine("------------------------");
            Console.WriteLine("Reading input file: " + inputfile);
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(inputfile);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read input file as a xml file.");
                Environment.Exit(0);
            }



            if (outputdir.Equals(""))
            {
                string fileNameNoExtension = Path.GetFileNameWithoutExtension(inputfile);
                outputdir = System.IO.Directory.GetCurrentDirectory() + "\\" + fileNameNoExtension;
            }
            Console.WriteLine("Using directory for output files: " + outputdir);

            try
            {
                _ = System.IO.Directory.CreateDirectory(outputdir);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not create directory: " + outputdir);
                return;
            }

            Console.WriteLine("Add system fields: " + (!nosystemfields).ToString());

            Console.WriteLine("Generating AL files...");
            XmlNode queries = doc.SelectSingleNode("APIQueries");
            var APIPublisher = queries.Attributes["APIPublisher"].Value;
            var APIGroup = queries.Attributes["APIGroup"].Value;
            var APIVersion = queries.Attributes["APIVersion"].Value;
            foreach (XmlNode query in queries.SelectNodes("query"))
            {
                var content = GenerateAPIQueryCode(query, APIPublisher, APIGroup, APIVersion, !nosystemfields);
                var filename = query.Attributes["filename"].Value;
                SaveFile(outputdir, filename, content);
                Console.WriteLine(filename);
            }
            Console.WriteLine("-------------------");
        }


        public static string GenerateAPIQueryCode(XmlNode query, string APIPublisher, string APIGroup, string APIVersion, bool addSystemFields)
        {
            StringBuilder sb = new StringBuilder();
            string indent = "    ";

            sb.AppendLine("// Auto-generated al file for query " + query.Attributes["id"].Value);

            sb.AppendLine ("query " + query.Attributes["id"].Value + " \"" + query.Attributes["name"].Value + "\"");
            sb.AppendLine("{");

            sb.Append(indent);
            sb.AppendLine("QueryType = API;");
            sb.Append(indent);
            sb.AppendLine("APIPublisher = " + APIPublisher.ToString() + ";");
            sb.Append(indent);
            sb.AppendLine("APIGroup = " + APIGroup.ToString() + "; ");
            sb.Append(indent);
            sb.AppendLine("APIVersion = " + APIVersion.ToString() + "; ");
            sb.Append(indent);
            sb.Append("Caption = '" + query.Attributes["Caption"].Value + "', ");
            sb.AppendLine("Locked = " + query.Attributes["Locked"].Value + ";");
            sb.Append(indent);
            sb.AppendLine("EntityName = " + query.Attributes["EntityName"].Value + ";");
            sb.Append(indent);
            sb.AppendLine("EntitySetName = " + query.Attributes["EntitySetName"].Value + ";");
            sb.Append(indent);
            sb.AppendLine("DataAccessIntent = ReadOnly;");

            sb.AppendLine("");

            sb.Append(indent);
            sb.AppendLine("elements");
            sb.Append(indent);
            sb.AppendLine("{");

            sb.Append(indent);
            sb.Append(indent);
            sb.AppendLine("dataitem(" + query.Attributes["DataItemName"].Value + "; \"" + query.Attributes["TableName"].Value + "\")");
            sb.Append(indent);
            sb.Append(indent);
            sb.AppendLine("{");

            string SystemCreatedOnFieldName = "SystemCreatedOn";
            bool systemFieldSystemCreatedOnIncluded = false;
            string SystemLastModifiedOnFieldName = "SystemLastModifiedOn";
            bool systemFieldSystemLastModifiedOnIncluded = false;
            string SystemRowversionFieldName = "SystemRowversion";
            bool systemFieldSystemRowversionIncluded = false;
            foreach (XmlNode field in query.SelectNodes("field"))
            {
                var DataItemFieldName = field.Attributes["DataItemFieldName"].Value;
                var FieldName = field.Attributes["FieldName"].Value;
                var Caption = field.Attributes["Caption"].Value;
                var Locked = field.Attributes["Locked"].Value;

                systemFieldSystemCreatedOnIncluded = FieldName == SystemCreatedOnFieldName;
                systemFieldSystemLastModifiedOnIncluded = FieldName == SystemLastModifiedOnFieldName;
                systemFieldSystemRowversionIncluded = FieldName == SystemRowversionFieldName;
                GenerateFieldCode(sb, indent, DataItemFieldName, FieldName, Caption, Locked);
            }

            if (addSystemFields)
            {
                if (!systemFieldSystemCreatedOnIncluded)
                {
                    GenerateFieldCode(sb, indent, "SystemCreatedOn_APIQueryField", SystemCreatedOnFieldName, "Datetime of when the record was created", "True");
                }
                if (!systemFieldSystemLastModifiedOnIncluded)
                {
                    GenerateFieldCode(sb, indent, "SystemLastModifiedOn_APIQueryField", SystemLastModifiedOnFieldName, "Datetime of when the record was last modified", "True");
                }
                if(!systemFieldSystemRowversionIncluded)
                {
                    GenerateFieldCode(sb, indent, "SystemRowVersion_APIQueryField", SystemRowversionFieldName, "The database rowversion of when the record", "True");
                }
            }

            sb.Append(indent);
            sb.Append(indent);
            sb.AppendLine("}");

            sb.Append(indent);
            sb.AppendLine("}");

            sb.AppendLine("}");

            return sb.ToString();
        }

        static void GenerateFieldCode(StringBuilder sb, string indent, string DataItemFieldName, string FieldName, string Caption, string Locked)
        {
            sb.Append(indent);
            sb.Append(indent);
            sb.Append(indent);
            sb.AppendLine("column(" + DataItemFieldName + "; \"" + FieldName + "\")");

            sb.Append(indent);
            sb.Append(indent);
            sb.Append(indent);
            sb.AppendLine("{");

            sb.Append(indent);
            sb.Append(indent);
            sb.Append(indent);
            sb.Append(indent);
            sb.Append("Caption = '" + Caption + "', ");
            sb.AppendLine("Locked = " + Locked + ";");

            sb.Append(indent);
            sb.Append(indent);
            sb.Append(indent);
            sb.AppendLine("}");

        }

        // https://epsicodecom.wordpress.com/2019/08/12/tutorial-generate-seperate-files-from-a-t4-template/
        public static void SaveFile(string folder, string fileName, string content)
        {
            using (FileStream fs = new FileStream(Path.Combine(folder, fileName.Trim() + ".al"), FileMode.Create))
            {
                using (StreamWriter str = new StreamWriter(fs))
                {
                    str.WriteLine(content);
                    str.Flush();
                }
            }
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: APIQueryGenerator [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);

            Console.WriteLine("-------------------");
            var exampleXML = """
<?xml version="1.0" encoding="utf-8" ?>
<APIQueries APIPublisher="Contoso" APIGroup="datawarehouse" APIVersion="1.0">
  <query id ="50000" name="QueryForTable1" filename="50000_QueryForTable1" Caption="customers" Locked="true" EntityName="Customer" EntitySetName="Customers" DataItemName="Customer" TableName ="Customers">
    <field DataItemFieldName="customerId" FieldName="Id" Caption="Customer Id" Locked="true"/>
    <field DataItemFieldName="customerNumber" FieldName="No." Caption="No" Locked="true"/>
    <field DataItemFieldName="customerName" FieldName="name" Caption="Customer Name" Locked="true"/>
  </query>
  <query id ="50001" name="QueryForTable2" filename="50001_QueryForTable2" Caption="vendors" Locked="false" EntityName="Vendor" EntitySetName="Vendors" DataItemName="Vendor" TableName ="Vendors" >
    <field DataItemFieldName="vendorId" FieldName="Id" Caption="Vendor Id" Locked="true"/>
    <field DataItemFieldName="vendorNumber" FieldName="No." Caption="No" Locked="true"/>
    <field DataItemFieldName="vendorName" FieldName="name" Caption="Vendor Name" Locked="true"/>
  </query>
</APIQueries>
""";

            Console.WriteLine("Example input file");
            Console.WriteLine(exampleXML);
            Console.WriteLine("-------------------");
        }

    }
}
