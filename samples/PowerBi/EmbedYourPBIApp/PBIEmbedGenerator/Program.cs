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

            sb.AppendLine($"// Auto-generated al file for PBI embed page {query.Attributes["id"].Value}");
            sb.AppendLine($"// Report: {query.Attributes["PBIReportName"].Value}");
            sb.AppendLine($"// Page: {query.Attributes["PBIReportPage"].Value}");
            sb.AppendLine("");
            sb.AppendLine($"namespace {alNamespace};");
            sb.AppendLine("");
            sb.AppendLine("using Microsoft.PowerBIReports;");
            sb.AppendLine("using System.Integration.PowerBI;");
            sb.AppendLine("using System.Environment.Configuration;");
            sb.AppendLine("");
            sb.AppendLine($"page {query.Attributes["id"].Value} \"{query.Attributes["name"].Value}\"");
            sb.AppendLine("{");
            sb.AppendLine("    UsageCategory = ReportsAndAnalysis;");
            sb.AppendLine($"    Caption = '{query.Attributes["caption"].Value}';");
            sb.AppendLine($"    AboutTitle = '{query.Attributes["aboutTitle"].Value}';");
            sb.AppendLine($"    AboutText = '{query.Attributes["aboutText"].Value}';");
            sb.AppendLine("");
            sb.AppendLine("    layout");
            sb.AppendLine("    {");
            sb.AppendLine("        area(Content)");
            sb.AppendLine("        {");
            sb.AppendLine("            usercontrol(PowerBIAddin; PowerBIManagement)");
            sb.AppendLine("            {");
            sb.AppendLine("                ApplicationArea = All;");
            sb.AppendLine("");
            sb.AppendLine("                trigger ControlAddInReady()");
            sb.AppendLine("                begin");
            sb.AppendLine("                    InitializeAddIn();");
            sb.AppendLine("                end;");
            sb.AppendLine("");
            sb.AppendLine("                trigger ErrorOccurred(Operation: Text; ErrorText: Text)");
            sb.AppendLine("                begin");
            sb.AppendLine("                    ShowErrorNotification(Operation, ErrorText);");
            sb.AppendLine("                end;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    actions");
            sb.AppendLine("    {");
            sb.AppendLine("        area(processing)");
            sb.AppendLine("        {");
            sb.AppendLine("            action(FullScreen)");
            sb.AppendLine("            {");
            sb.AppendLine("                ApplicationArea = All;");
            sb.AppendLine("                Caption = 'Fullscreen';");
            sb.AppendLine("                ToolTip = 'Shows the Power BI element as full screen.';");
            sb.AppendLine("                Image = View;");
            sb.AppendLine("                Visible = false;");
            sb.AppendLine("");
            sb.AppendLine("                trigger OnAction()");
            sb.AppendLine("                begin");
            sb.AppendLine("                    CurrPage.PowerBIAddin.FullScreen();");
            sb.AppendLine("                end;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("");
            sb.AppendLine("    var");
            sb.AppendLine("        ReportId: Guid;");
            sb.AppendLine($"        ReportPageTok: Label '{query.Attributes["PBIReportPage"].Value}', Locked = true;");
            sb.AppendLine("        ErrorNotificationMsg: Label 'An error occurred while loading Power BI. Your Power BI embedded content might not work. Here are the error details: \"%1: %2\"', Comment = '%1: a short error code. %2: a verbose error message in english';");
            sb.AppendLine("        PowerBIEmbedReportUrlTemplateTxt: Label 'https://app.powerbi.com/reportEmbed?reportId=%1', Locked = true;");
            sb.AppendLine("");
            sb.AppendLine("    trigger OnOpenPage()");
            sb.AppendLine("    var");
            sb.AppendLine("        PowerBIReportsSetup: Record \"PowerBI Reports Setup\";");
            sb.AppendLine("    begin");
            sb.AppendLine("        PowerBIReportsSetup.EnsureUserAcceptedPowerBITerms();");
            sb.AppendLine("");
            sb.AppendLine("        // Replace with your own report id");
            sb.AppendLine($"        ReportId := PowerBIReportsSetup.GetReportIdAndEnsureSetup(CurrPage.Caption(), PowerBIReportsSetup.FieldNo(\"{query.Attributes["PBIReportIdFieldName"].Value}\"));");
            sb.AppendLine("    end;");
            sb.AppendLine("");
            sb.AppendLine("    local procedure ShowErrorNotification(ErrorCategory: Text; ErrorMessage: Text)");
            sb.AppendLine("    var");
            sb.AppendLine("        PowerBIContextSettings: Record \"Power BI Context Settings\";");
            sb.AppendLine("        NotificationLifecycleMgt: Codeunit \"Notification Lifecycle Mgt.\";");
            sb.AppendLine("        Notify: Notification;");
            sb.AppendLine("    begin");
            sb.AppendLine("        Notify.Id := CreateGuid();");
            sb.AppendLine("        Notify.Message(StrSubstNo(ErrorNotificationMsg, ErrorCategory, ErrorMessage));");
            sb.AppendLine("        Notify.Scope := NotificationScope::LocalScope;");
            sb.AppendLine("");
            sb.AppendLine("        NotificationLifecycleMgt.SendNotification(Notify, PowerBIContextSettings.RecordId());");
            sb.AppendLine("    end;");
            sb.AppendLine("");
            sb.AppendLine("    local procedure InitializeAddIn()");
            sb.AppendLine("    var");
            sb.AppendLine("        PowerBIServiceMgt: Codeunit \"Power BI Service Mgt.\";");
            sb.AppendLine("    begin");
            sb.AppendLine("        PowerBiServiceMgt.InitializeAddinToken(CurrPage.PowerBIAddin);");
            sb.AppendLine("        CurrPage.PowerBIAddin.SetSettings(false, true, false, false, false, false, true);");
            sb.AppendLine("");
            sb.AppendLine("        CurrPage.PowerBIAddin.EmbedPowerBIReport(");
            sb.AppendLine("            StrSubstNo(PowerBIEmbedReportUrlTemplateTxt, ReportId),");
            sb.AppendLine("            ReportId,");
            sb.AppendLine("            ReportPageTok);");
            sb.AppendLine("    end;");
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
