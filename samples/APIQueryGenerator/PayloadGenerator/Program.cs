using System;
using System.Xml;
using System.Text;
using System.IO;
using Mono.Options;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using CsvHelper;
using System.Globalization;
using CsvHelper.Configuration;

namespace PayloadGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html
            bool show_help = false;
            string inputtablefile = "";
            string inputfieldfile = "";
            string outputdir = "";
            string outputfile = "";
            string APIPublisher = "", APIGroup = "", APIVersion = "";
            int idnumberstart = 50000;


            var p = new OptionSet() {
                { "apipublisher=",  "APIPublisher (required)", v => APIPublisher = v },
                { "apigroup=",  "APIGroup (required)", v => APIGroup = v },
                { "apiversion=",  "APIPublisher (required)", v => APIVersion = v },
                { "h|help",  "show this message and exit", v => show_help = v != null },
                { "inputtablefile=",  "input csv file for tables (required)", v => inputtablefile = v },
                { "inputfieldfile=",  "input csv file for fields (required)", v => inputfieldfile = v },
                { "outputdir=",  "output directory", v => outputdir = v },
                { "outputfile=",  "output file (required)", v => outputfile = v },
                { "idnumberstart=",  "Id numbers for AL objects starts at this number (default is 50000)", v => idnumberstart = int.Parse( v ) },
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

            if (inputtablefile.Equals(""))
            {
                ShowHelp(p);
                Environment.Exit(0);
            }

            Console.WriteLine("API query payload generator");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                NewLine = Environment.NewLine,
                Delimiter = ";",
                MissingFieldFound = null,
                HeaderValidated = null
            };

            // Get tables from csv file
            Console.WriteLine("------------------------");
            Console.WriteLine("Reading input file for tables: " + inputtablefile);
            List<QueryDefinition> queries = new List<QueryDefinition>();
            try
            {
                using (TextReader reader = File.OpenText(inputtablefile))
                {
                    CsvReader csv = new CsvReader(reader, config);
                    while (csv.Read())
                    {
                        QueryDefinition Record = csv.GetRecord<QueryDefinition>();
                        queries.Add(Record);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read input file for table definitions.");
                Environment.Exit(0);
            }
            Console.WriteLine("Read " + queries.Count.ToString() + " records");


            // Get fields from csv file
            Console.WriteLine("------------------------");
            Console.WriteLine("Reading input file for fields: " + inputfieldfile);
            List<FieldDefinition> fields = new List<FieldDefinition>();
            //try
            //{
                using (TextReader reader = File.OpenText(inputfieldfile))
                {
                    CsvReader csv = new CsvReader(reader, config);
                    while (csv.Read())
                    {
                        FieldDefinition Record = csv.GetRecord<FieldDefinition>();
                        fields.Add(Record);
                    }
                }
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("Could not read input file for field definitions.");
            //    Environment.Exit(0);
            //}
            Console.WriteLine("Read " + fields.Count.ToString() + " records");



            // add AL object ids
            Console.WriteLine("AL object ids start at: " + idnumberstart.ToString());
            int ALObjectId = idnumberstart;
            foreach (QueryDefinition query in queries)
            {
                query.id = ALObjectId;
                ALObjectId++;
            }

            // add fields to queries
            foreach (FieldDefinition field in fields)
            {
                var tableName = field.tableName;
                var query = queries.Find(q => q.tableName == tableName);
                query?.addField(field);
            }



            if (outputfile.Equals(""))
            {
                Console.WriteLine("Output file parameter required.");
                Environment.Exit(0);
            }


            if (outputdir.Equals(""))
            {
                outputdir = System.IO.Directory.GetCurrentDirectory();
            }
            Console.WriteLine("Using directory for output files: " + outputdir);


            Console.WriteLine("Generating payload file...");
            Console.WriteLine("APIPublisher = " + APIPublisher);
            Console.WriteLine("APIGroup = " + APIGroup);
            Console.WriteLine("APIVersion = " + APIVersion);

            XDocument payloaddoc = GeneratePayload(APIPublisher, APIGroup, APIVersion, queries);
            string content = payloaddoc.ToString();
            SaveFile(outputdir, outputfile, content);
        }

        public static XDocument GeneratePayload(string APIPublisher, string APIGroup, string APIVersion, IEnumerable<QueryDefinition> queries)
        {
            XDocument doc = new XDocument(

            );

            XElement APIQueries = new XElement("APIQueries",
                    new XAttribute("APIPublisher", APIPublisher),
                    new XAttribute("APIGroup", APIGroup),
                    new XAttribute("APIVersion", APIVersion)
                );
            doc.Add(APIQueries);

            foreach (QueryDefinition query in queries)
            {
                XElement el = query.asXElement();
                APIQueries.Add(el);
            }

            return doc;
        }



        // https://epsicodecom.wordpress.com/2019/08/12/tutorial-generate-seperate-files-from-a-t4-template/
        public static void SaveFile(string folder, string fileName, string content)
        {
            using (FileStream fs = new FileStream(Path.Combine(folder, fileName.Trim() + ".xml"), FileMode.Create))
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
            Console.WriteLine("Usage: PayloadGenerator [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);

            Console.WriteLine("-------------------");
        }

    }
}
