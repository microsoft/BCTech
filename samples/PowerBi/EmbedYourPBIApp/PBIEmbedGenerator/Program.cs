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
            Version version = new Version();

            var p = new OptionSet() {
                { "h|help",  "show this message and exit", v => show_help = v != null },
                { "i|inputfile=",  "input xml file (required)", v => inputfile = v },
                { "o|outputdir=",  "output directory (if not specified, then input file name will be used)", v => outputdir = v },
                { "v|version=",    "target Business Central version", v => version = new Version(v) }
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

            string alNamespace = doc.SelectSingleNode("/objects/Namespace").Attributes["namespace"].Value;
            Console.WriteLine("Using namespace {0}", alNamespace);

            Console.WriteLine("Generating AL files...");
            XmlNode pages = doc.SelectSingleNode("/objects/PBIEmbedPages");
            Console.WriteLine("Found {0} pages", pages.SelectNodes("page").Count.ToString());
            foreach (XmlNode page in pages.SelectNodes("page"))
            {
                var content = GeneratePBIEmbedPageCode(alNamespace, page, version);
                var filename = page.Attributes["filename"].Value;
                SaveFile(outputdir, filename, content);
                Console.WriteLine(filename);
            }
            Console.WriteLine("-------------------");

            XmlNode rcExtensions = doc.SelectSingleNode("/objects/RCExtensions");
            Console.WriteLine("Found {0} RC extensions", rcExtensions.SelectNodes("pageextension").Count.ToString());
            foreach (XmlNode rc in rcExtensions.SelectNodes("pageextension"))
            {
                var content = GeneratePBIRCExtensionCode(alNamespace, rc);
                var filename = rc.Attributes["filename"].Value;
                SaveFile(outputdir, filename, content);
                Console.WriteLine(filename);
            }
            Console.WriteLine("-------------------");

            XmlNode permSets = doc.SelectSingleNode("/objects/PermissionSets");
            Console.WriteLine("Found {0} permission sets", permSets.SelectNodes("permissionset").Count.ToString());
            foreach (XmlNode permSet in permSets.SelectNodes("permissionset"))
            {
                var content = GeneratePBIEmbedPagePerm(alNamespace, permSet, pages);
                var filename = permSet.Attributes["filename"].Value;
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

        private static void indentAppendLine(StringBuilder sb, int indentLevel, string s)
        {
            indents(sb, indentLevel);
            sb.AppendLine(s);
        }

        public static string GeneratePBIRCExtensionCode(string alNamespace, XmlNode rcExt)
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

            sb.AppendLine("namespace " + alNamespace);
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


        public static string GeneratePBIEmbedPageCode(string alNamespace, XmlNode query, Version version)
        {
            StringBuilder sb = new StringBuilder();
            string context = query.Attributes["id"].Value + "-" + query.Attributes["name"].Value;

            /*
            // Auto-generated al file for PBI embed page <id>
            // 
            // Adding a PBI embed page for this PBI content:
            // PBI ReportId = '<PBIReportId>'
            // PBI ReportName = '<PBIReportName>'

            namespace Microsoft.PowerBIApps

            page <id> "<name>"
            {
                UsageCategory = ReportsAndAnalysis;
                Caption = '<caption>';
                AboutTitle = '<aboutTitle>';
                AboutText = '<aboutText>';

                layout
                {
                    area(Content)
                    {
                        part(EmbeddedReport; "Power BI Embedded Report Part")
                        {
                            Caption = 'Financial Overview';
                            SubPageView = where(Context = const('<id>-<name>'));
                            ApplicationArea = All;
                        }
                    }
                }
             */

            sb.AppendLine("// Auto-generated al file for PBI embed page " + query.Attributes["id"].Value);
            sb.AppendLine("// ");
            sb.AppendLine("// Adding a PBI embed page for this PBI content:");
            sb.AppendLine("// PBI ReportId = '" + query.Attributes["PBIReportId"].Value);
            sb.AppendLine("// PBI ReportName = '" + query.Attributes["PBIReportName"].Value);
            sb.AppendLine("");

            sb.AppendLine("namespace " + alNamespace);
            sb.AppendLine("");

            sb.AppendLine("page " + query.Attributes["id"].Value + " \"" + query.Attributes["name"].Value + "\"");
            sb.AppendLine("{");

            indentAppendLine(sb, 1, "UsageCategory = ReportsAndAnalysis;");
            indentAppendLine(sb, 1, "Caption = '" + query.Attributes["caption"].Value + "';");
            indentAppendLine(sb, 1, "AboutTitle = '" + query.Attributes["aboutTitle"].Value + "'; ");
            indentAppendLine(sb, 1, "AboutText = '" + query.Attributes["aboutText"].Value + "'; ");
            sb.AppendLine("");

            indentAppendLine(sb, 1, "layout");
            indentAppendLine(sb, 1, "{");

            indentAppendLine(sb, 2, "area(Content)");
            indentAppendLine(sb, 2, "{");

            indentAppendLine(sb, 3, "part(EmbeddedReport; \"Power BI Embedded Report Part\")");
            indentAppendLine(sb, 3, "{");

            indentAppendLine(sb, 4, "ApplicationArea = All;");
            indentAppendLine(sb, 4, "Caption = '" + query.Attributes["caption"].Value + "';");
            indentAppendLine(sb, 4, "SubPageView = where(Context = const ('" + context + "'));");

            indentAppendLine(sb, 3, "}");

            indentAppendLine(sb, 2, "}");

            indentAppendLine(sb, 1, "}");
            sb.AppendLine("");

            /*
            trigger OnOpenPage()
            var
                PowerBIDisplayedElement: Record "Power BI Displayed Element";
                PowerBIServiceMgt: Codeunit "Power BI Service Mgt.";
            begin
                EnsureUserAcceptedPowerBITerms();

                // Cleanup previously added reports for this context
                PowerBIDisplayedElement.SetRange(Context, vContext);
                PowerBIDisplayedElement.DeleteAll();
                PowerBIDisplayedElement.SetRange(Context);

                // Add the report
                PowerBIServiceMgt.AddReportForContext(vReportId, vContext);

                // Customize page and other settings
                PowerBIDisplayedElement.Get(UserSecurityId(), vContext, PowerBIDisplayedElement.MakeReportKey(vReportId),
                    PowerBIDisplayedElement.ElementType::Report);
                PowerBIDisplayedElement.ReportPage := vReportPage;
                PowerBIDisplayedElement.ShowPanesInExpandedMode := true;
                PowerBIDisplayedElement.ShowPanesInNormalMode := true;
                PowerBIDisplayedElement.Modify();

                CurrPage.EmbeddedReport.Page.SetFullPageMode(true);
            end;
             */
            indentAppendLine(sb, 1, "trigger OnOpenPage()");
            indentAppendLine(sb, 1, "var");

            indentAppendLine(sb, 2, "PowerBIDisplayedElement: Record \"Power BI Displayed Element\";");
            indentAppendLine(sb, 2, "PowerBIServiceMgt: Codeunit \"Power BI Service Mgt.\";");

            indentAppendLine(sb, 1, "begin");

            indentAppendLine(sb, 2, "EnsureUserAcceptedPowerBITerms();");
            sb.AppendLine("");
            indentAppendLine(sb, 2, "// Cleanup previously added reports for this context");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.SetRange(Context, vContext);");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.DeleteAll();");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.SetRange(Context);");
            sb.AppendLine("");
            indentAppendLine(sb, 2, "// Add the report");
            indentAppendLine(sb, 2, "PowerBIServiceMgt.AddReportForContext(vReportId, vContext);");
            sb.AppendLine("");
            indentAppendLine(sb, 2, "// Customize page and other settings");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.Get(UserSecurityId(), vContext, PowerBIDisplayedElement.MakeReportKey(vReportId), PowerBIDisplayedElement.ElementType::Report);");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.ReportPage := vReportPage;");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.ShowPanesInExpandedMode := true;");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.ShowPanesInNormalMode := true;");
            indentAppendLine(sb, 2, "PowerBIDisplayedElement.Modify();");
            sb.AppendLine("");

            if (version == new Version() | version >= new Version("24.2"))
            {
                indentAppendLine(sb, 2, "CurrPage.EmbeddedReport.Page.SetFullPageMode(true);");
            }

            indentAppendLine(sb, 1, "end;");
            sb.AppendLine("");

            /*
            local procedure EnsureUserAcceptedPowerBITerms()
            var
                PowerBIContextSettings: Record "Power BI Context Settings";
                PowerBIEmbedSetupWizard: Page "Power BI Embed Setup Wizard";
            begin
                PowerBIContextSettings.SetRange(UserSID, UserSecurityId());
                if PowerBIContextSettings.IsEmpty() then begin
                    PowerBIEmbedSetupWizard.SetContext(vContext);
                    if PowerBIEmbedSetupWizard.RunModal() <> Action::OK then;

                    if PowerBIContextSettings.IsEmpty() then
                        Error(PowerBiNotSetupErr);
                end;

                PowerBIContextSettings.CreateOrReadForCurrentUser(vContext);
                if not PowerBIContextSettings.LockToSelectedElement then begin
                    PowerBIContextSettings.LockToSelectedElement := true;
                    PowerBIContextSettings.Modify();
                end;
            end;
             */
            indentAppendLine(sb, 1, "local procedure EnsureUserAcceptedPowerBITerms()");
            indentAppendLine(sb, 1, "var");

            indentAppendLine(sb, 2, "PowerBIContextSettings: Record \"Power BI Context Settings\";");
            indentAppendLine(sb, 2, "PowerBIEmbedSetupWizard: Page \"Power BI Embed Setup Wizard\";");

            indentAppendLine(sb, 1, "begin");

            indentAppendLine(sb, 2, "PowerBIContextSettings.SetRange(UserSID, UserSecurityId());");
            indentAppendLine(sb, 2, "if PowerBIContextSettings.IsEmpty() then begin");

            indentAppendLine(sb, 3, "PowerBIEmbedSetupWizard.SetContext(vContext);");
            indentAppendLine(sb, 3, "if PowerBIEmbedSetupWizard.RunModal() <> Action::OK then;");
            sb.AppendLine("");
            indentAppendLine(sb, 3, "if PowerBIContextSettings.IsEmpty() then");

            indentAppendLine(sb, 4, "Error(PowerBiNotSetupErr);");

            indentAppendLine(sb, 2, "end;");
            sb.AppendLine("");
            indentAppendLine(sb, 2, "PowerBIContextSettings.CreateOrReadForCurrentUser(vContext);");
            indentAppendLine(sb, 2, "if not PowerBIContextSettings.LockToSelectedElement then begin");

            indentAppendLine(sb, 3, "PowerBIContextSettings.LockToSelectedElement := true;");
            indentAppendLine(sb, 3, "PowerBIContextSettings.Modify();");

            indentAppendLine(sb, 2, "end;");

            indentAppendLine(sb, 1, "end;");
            sb.AppendLine("");

            /* 
            var
                PowerBiNotSetupErr: Label 'Power BI is not set up. You need to set up Power BI in order to see this report.';
                vReportId: Label '<PBIReportId>', Locked = true;
                vReportPage: Label '<PBIReportPage>', Locked = true;
                vContext: Label '<id>-<name>', MaxLength = 30, Locked = true, Comment = 'IMPORTANT: keep it unique across pages. Also, make sure this value is the same used in the SubPageView above.';
             */
            indentAppendLine(sb, 1, "var");

            indentAppendLine(sb, 2, "PowerBiNotSetupErr: Label 'Power BI is not set up. You need to set up Power BI in order to see this report.';");
            indentAppendLine(sb, 2, "vReportId: Label '" + query.Attributes["PBIReportId"].Value + "', Locked = true;");
            indentAppendLine(sb, 2, "vReportPage: Label '" + query.Attributes["PBIReportPage"].Value + "', Locked = true;");
            indentAppendLine(sb, 2, "vContext: Label '" + context + "', MaxLength = 30, Locked = true, Comment = 'IMPORTANT: keep it unique across pages. Also, make sure this value is the same used in the SubPageView above.';");
            
            sb.AppendLine("");
            sb.AppendLine("}");

            return sb.ToString();
        }

        public static string GeneratePBIEmbedPagePerm(string alNamespace, XmlNode permSet, XmlNode pages)
        {
            StringBuilder sb = new StringBuilder();

            /*
            // Auto-generated al file for PBI permissions 50120

            namespace Microsoft.PowerBIApps

            permissionset 50120 "Power BI Finance App - Objects"
            {
                Access = Internal;
                Assignable = false;
                Permissions =
                    page "PBIFinancialOverview" = X,
                    page "PBIIncomeStatementbyMonth" = X,
                    page "PBIBalanceSheetbyMonth" = X,
                    page "PBIBudgetComparison" = X,
                    page "PBILiquidityKPIs" = X,
                    page "PBIProfitability" = X,
            } 
            */

            indentAppendLine(sb, 0, "// Auto-generated al file for PBI permissions " + permSet.Attributes["id"].Value);
            indentAppendLine(sb, 0, "");
            indentAppendLine(sb, 0, "namespace " + alNamespace);
            indentAppendLine(sb, 0, "");
            indentAppendLine(sb, 0, "permissionset " + permSet.Attributes["id"].Value + " \"" + permSet.Attributes["name"].Value + "\"");
            indentAppendLine(sb, 0, "{");
            indentAppendLine(sb, 1, "Access = Internal;");
            indentAppendLine(sb, 1, "Assignable = false;");
            indentAppendLine(sb, 1, "Permissions =");

            foreach (XmlNode page in pages.SelectNodes("page"))
            {
                indentAppendLine(sb, 2, "page \"" + page.Attributes["name"].Value + "\" = X,");
            }

            indentAppendLine(sb, 0, "}");

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
