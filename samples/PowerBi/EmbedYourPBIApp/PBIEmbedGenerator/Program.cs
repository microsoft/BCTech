using System;
using System.Xml;
using System.Text;
using System.IO;
using Mono.Options;
using System.Collections.Generic;

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

            var p = new OptionSet() { 
                { "h|help",  "show this message and exit", v => show_help = v != null },
                { "i|inputfile=",  "input xml file (required)", v => inputfile = v },
                { "o|outputdir=",  "output directory (if not specified, then input file name will be used)", v => outputdir = v }
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

            Console.WriteLine("PBI embed page AL code generator");

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

            Console.WriteLine("Generating AL files...");
            XmlNode pages = doc.SelectSingleNode("/objects/PBIEmbedPages");
            Console.WriteLine("Found {0} pages", pages.SelectNodes("page").Count.ToString());
            foreach (XmlNode page in pages.SelectNodes("page"))
            {
                var content = GeneratePBIEmbedPageCode(page);
                var filename = page.Attributes["filename"].Value;
                SaveFile(outputdir, filename, content);
                Console.WriteLine(filename);
            }
            Console.WriteLine("-------------------");

            XmlNode rcExtensions = doc.SelectSingleNode("/objects/RCExtensions");
            Console.WriteLine("Found {0} RC extensions", rcExtensions.SelectNodes("pageextension").Count.ToString());
            foreach (XmlNode rc in rcExtensions.SelectNodes("pageextension"))
            {
                var content = GeneratePBIRCExtensionCode(rc);
                var filename = rc.Attributes["filename"].Value;
                SaveFile(outputdir, filename, content);
                Console.WriteLine(filename);
            }
            Console.WriteLine("-------------------");

        }

        private static void indents(StringBuilder sb, int indentLevel)
        {
            var indent = "    ";

            for (int i = 0; i < indentLevel; i++)
            {
                sb.Append(indent);
            }
        }

        public static string GeneratePBIRCExtensionCode(XmlNode rcExt)
        {
            StringBuilder sb = new StringBuilder();

            /*
             pageextension 50100 BusinessManagerRoleCenterExt extends "Business Manager Role Center"
             */

            sb.AppendLine("// Auto-generated al file for PBI role centre extension " + rcExt.Attributes["id"].Value);
            sb.AppendLine("// ");
            sb.AppendLine("// Adding actions for the following AL PBI pages:");
            foreach (XmlNode action in rcExt.SelectNodes("action"))
            {
                sb.AppendLine("// * " + action.Attributes["pbipagename"].Value );
            }
            sb.AppendLine("");


            sb.AppendLine("pageextension " + rcExt.Attributes["id"].Value + " \"" + rcExt.Attributes["name"].Value + "\" extends \"" + rcExt.Attributes["extends"].Value + "\"");
            sb.AppendLine("{");

            indents(sb, 1);
            sb.AppendLine("actions");

            indents(sb, 1);
            sb.AppendLine("{");

            indents(sb, 2);
            sb.AppendLine(rcExt.Attributes["where"].Value);

            indents(sb, 2);
            sb.AppendLine("{");

            indents(sb, 3);
            sb.AppendLine("group(\"PBIReports\")");

            indents(sb, 3);
            sb.AppendLine("{");

            indents(sb, 4);
            sb.AppendLine("Caption = 'Power BI Reports';");

            indents(sb, 4);
            sb.AppendLine("Image = PowerBI;");

            indents(sb, 4);
            sb.AppendLine("ToolTip = '" + rcExt.Attributes["tooltip"].Value + "';");

            foreach (XmlNode action in rcExt.SelectNodes("action"))
            {
                var content = GeneratePBIRCExtensionActionCode(action, sb);
            }

            indents(sb, 3);
            sb.AppendLine("}");

            indents(sb, 2);
            sb.AppendLine("}");

            indents(sb, 1);
            sb.AppendLine("}");

            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GeneratePBIRCExtensionActionCode(XmlNode action, StringBuilder sb)
        {
            //action(PBIFinancialOverview)
            //{
            //    ApplicationArea = Basic, Suite;
            //    Caption = 'Financial Overview';
            //    Image = "PowerBI";
            //    RunObject = page PBIFinancialOverview;
            //    ToolTip = 'Open a Power BI report that shows your company''s assets, liabilities, and equity.';
            //}

            sb.AppendLine("");

            indents(sb, 4);
            sb.AppendLine("action(" + action.Attributes["name"].Value + ")");

            indents(sb, 4);
            sb.AppendLine("{");

            indents(sb, 5);
            sb.AppendLine("ApplicationArea = Basic, Suite;");

            indents(sb, 5);
            sb.AppendLine("Caption = '" + action.Attributes["caption"].Value + "';");

            indents(sb, 5);
            sb.AppendLine("Image = \"PowerBI\";");

            indents(sb, 5);
            sb.AppendLine("RunObject = page \"" + action.Attributes["pbipagename"].Value + "\";");

            indents(sb, 5);
            sb.AppendLine("Tooltip = '" + action.Attributes["tooltip"].Value + "';");

            indents(sb, 4);
            sb.AppendLine("}");

            return sb.ToString();
        }


        public static string GeneratePBIEmbedPageCode(XmlNode query)
        {
            StringBuilder sb = new StringBuilder();

            /*
             page <id> <page name>
            {
                UsageCategory = ReportsAndAnalysis;
                Caption = '<page caption>';
                AboutTitle = '<teaching tip title>';
                AboutText = '<teaching tip text>';

                layout
                {
                    area(Content)
                    {
                        part("partname"; "Power BI Embedded Report Part")
                        {
                            Caption = '<page caption>';
                            SubPageView = where(Context = const('LockedCustomerReport'));
                        }
                    }
                }

                trigger OnOpenPage()
                var
                    PowerBIContextSettings: Record "Power BI Context Settings";
                    PowerBIDisplayedElement: Record "Power BI Displayed Element";
                    vReportId: Guid;
                    vReportName: Text[200];
                begin
                    PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
                    if PowerBIContextSettings.IsEmpty() then
                        exit; // User has not set up the Power BI integration

                    // some of boiler plate code here

                    vReportId := 'TODO';
                    vReportName := 'TODO';

                    // TODO: which method to use for a single page in a report
                    PowerBIDisplayedElement.ElementId := PowerBIDisplayedElement.MakeReportVisualKey(vReportId, vReportName, 'ab1fcfce118c0d14d565');

                    // more of boiler plate code here
                end;
            }
             */
            sb.AppendLine("// Auto-generated al file for PBI embed page " + query.Attributes["id"].Value);
            sb.AppendLine("// ");
            sb.AppendLine("// Adding a PBI embed page for this PBI content:");
            sb.AppendLine("// PBI ReportId = '" + query.Attributes["PBIReportId"].Value);
            sb.AppendLine("// PBI ReportName = '" + query.Attributes["PBIReportName"].Value);
            sb.AppendLine("");

            sb.AppendLine ("page " + query.Attributes["id"].Value + " \"" + query.Attributes["name"].Value + "\"");
            sb.AppendLine("{");

            indents(sb, 1);
            sb.AppendLine("UsageCategory = ReportsAndAnalysis;");

            indents(sb, 1);
            sb.AppendLine("Caption = '" + query.Attributes["caption"].Value + "';");

            indents(sb, 1);
            sb.AppendLine("AboutTitle = '" + query.Attributes["aboutTitle"].Value + "'; ");

            indents(sb, 1);
            sb.AppendLine("AboutText = '" + query.Attributes["aboutText"].Value + "'; ");

            sb.AppendLine("");

            indents(sb, 1);
            sb.AppendLine("layout");

            indents(sb, 1);
            sb.AppendLine("{");

            indents(sb, 2);
            sb.AppendLine("area(Content)");

            indents(sb, 2);
            sb.AppendLine("{");

            indents(sb, 3);
            sb.AppendLine("part(\"partname\"; \"Power BI Embedded Report Part\")");

            indents(sb, 3);
            sb.AppendLine("{");

            indents(sb, 4);
            sb.AppendLine("Caption = '" + query.Attributes["caption"].Value + "';");

            indents(sb, 4);
            sb.AppendLine("SubPageView = where(Context = const ('LockedCustomerReport'));");

            indents(sb, 3);
            sb.AppendLine("}");

            indents(sb, 2);
            sb.AppendLine("}");

            indents(sb, 1);
            sb.AppendLine("}");

            sb.AppendLine("");

            indents(sb, 1);
            sb.AppendLine("trigger OnOpenPage()");

            indents(sb, 1);
            sb.AppendLine("var");

            indents(sb, 2);
            sb.AppendLine("PowerBIContextSettings: Record \"Power BI Context Settings\";");

            indents(sb, 2);
            sb.AppendLine("PowerBIDisplayedElement: Record \"Power BI Displayed Element\";");

            indents(sb, 2);
            sb.AppendLine("vReportId: Guid;");

            indents(sb, 2);
            sb.AppendLine("vReportName: Text[200];");

            indents(sb, 1);
            sb.AppendLine("begin");

            indents(sb, 2);
            sb.AppendLine("PowerBIContextSettings.SetRange(UserSID, UserSecurityId());");

            indents(sb, 2);
            sb.AppendLine("if PowerBIContextSettings.IsEmpty() then");

            indents(sb, 3);
            sb.AppendLine("exit; // User has not set up the Power BI integration");

            sb.AppendLine("");

            /*
                    if not PowerBIDisplayedElement.Get(UserSecurityId(), 'LockedCustomerReport', PowerBIDisplayedElement.MakeReportVisualKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565'), PowerBIDisplayedElement.ElementType::"Report Visual") then begin
                        PowerBIDisplayedElement.Init();
                        PowerBIDisplayedElement.ElementType := PowerBIDisplayedElement.ElementType::"Report Visual";
                        PowerBIDisplayedElement.ElementId := PowerBIDisplayedElement.MakeReportVisualKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565');
                        PowerBIDisplayedElement.ElementEmbedUrl := 'https://app.powerbi.com/reportEmbed?reportId=061ce0f5-3918-44ee-b820-a8d0d384fb2e&config=eyJjbHVzdGVyVXJsIjoiaHR0cHM6Ly9XQUJJLVdFU1QtVVMzLUEtUFJJTUFSWS1yZWRpcmVjdC5hbmFseXNpcy53aW5kb3dzLm5ldCIsImVtYmVkRmVhdHVyZXMiOnsibW9kZXJuRW1iZWQiOnRydWUsInVzYWdlTWV0cmljc1ZOZXh0Ijp0cnVlfX0%3d';
                        PowerBIDisplayedElement.Context := 'LockedCustomerReport'; // Use here the same context that you specified in the SubPageView of the part;
                        PowerBIDisplayedElement.UserSID := UserSecurityId();
                        PowerBIDisplayedElement.ShowPanesInExpandedMode := true;
                        PowerBIDisplayedElement.ShowPanesInNormalMode := false;
                        PowerBIDisplayedElement.Insert();
                    end;

            */

            indents(sb, 2);
            sb.AppendLine("if not PowerBIDisplayedElement.Get(UserSecurityId(), 'LockedCustomerReport', PowerBIDisplayedElement.MakeReportVisualKey('061ce0f5-3918-44ee-b820-a8d0d384fb2e', 'ReportSection1', 'ab1fcfce118c0d14d565'), PowerBIDisplayedElement.ElementType::\"Report Visual\") then begin");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.Init();");

            indents(sb, 3);
            sb.AppendLine("// some of boiler plate code here");

            sb.AppendLine("");

            indents(sb, 3);
            sb.AppendLine("vReportId := '" + query.Attributes["PBIReportId"].Value + "';");

            indents(sb, 3);
            sb.AppendLine("vReportName := '" + query.Attributes["PBIReportName"].Value + "';");

            sb.AppendLine("");

            indents(sb, 3);
            sb.AppendLine("// TODO: which method to use for a single page in a report");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.ElementId := PowerBIDisplayedElement.MakeReportVisualKey(vReportId, vReportName, 'ab1fcfce118c0d14d565');");

            sb.AppendLine("");

            indents(sb, 3);
            sb.AppendLine("// more of boiler plate code here");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.Context := 'LockedCustomerReport'; // Use here the same context that you specified in the SubPageView of the part;");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.UserSID := UserSecurityId();");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.ShowPanesInExpandedMode := true;");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.ShowPanesInNormalMode := false;");

            indents(sb, 3);
            sb.AppendLine("PowerBIDisplayedElement.Insert();");

            indents(sb, 2);
            sb.AppendLine("end;");

            indents(sb, 1);
            sb.AppendLine("end;");

            sb.AppendLine("");

            sb.AppendLine("}");

            return sb.ToString();
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
