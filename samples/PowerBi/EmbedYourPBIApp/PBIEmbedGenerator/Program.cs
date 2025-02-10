using PBIEmbedGenerator.Properties;
using System;
using System.CommandLine;
using System.IO;
using System.Text;
using System.Xml;

namespace APIQueryGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputFileOption = new Option<FileInfo>("--inputfile", "The XML payload input file")
            {
                IsRequired = true
            };
            inputFileOption.AddAlias("--i");
            var outputDirOption = new Option<DirectoryInfo>("--outputdir", "The output directory where the AL files will be generated")
            {
                IsRequired = true
            };
            outputDirOption.AddAlias("--o");

            var rootCommand = new RootCommand("Power BI content - AL generator");
            rootCommand.AddOption(inputFileOption);
            rootCommand.AddOption(outputDirOption);
            rootCommand.SetHandler((inputFile, outputDir) =>
            {
                Run(inputFile.FullName, outputDir.FullName);
            }, inputFileOption, outputDirOption);

            try
            {
                rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }

        static void Run(string inputfile, string outputdir)
        {
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
                Environment.Exit(-1);
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
                var content = GeneratePBIEmbedPageCode(alNamespace, page);
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

        public static string GeneratePBIRCExtensionCode(string alNamespace, XmlNode rcExt)
        {
            StringBuilder actionssb = new StringBuilder();
            foreach (XmlNode action in rcExt.SelectNodes("action"))
            {
                GeneratePBIRCExtensionActionCode(action, actionssb);
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(Encoding.UTF8.GetString(Resources.RoleCenterExtensionTemplate),
                alNamespace, // 0
                rcExt.Attributes["rolecenternamespace"].Value, // 1
                rcExt.Attributes["id"].Value, // 2
                EscapeALTextString(rcExt.Attributes["name"].Value), // 3
                EscapeALTextString(rcExt.Attributes["extends"].Value), // 4
                rcExt.Attributes["where"].Value, // 5
                rcExt.Attributes["image"].Value, // 6
                rcExt.Attributes["tooltip"].Value, // 7
                actionssb.ToString() // 8
                );

            return sb.ToString();
        }

        public static void GeneratePBIRCExtensionActionCode(XmlNode action, StringBuilder sb)
        {
            if (action.Attributes["obsolete"].Value.Length > 0)
            {
                sb.AppendLine($"#if not CLEAN{action.Attributes["obsolete"].Value}");
            }
            StringBuilder additionalActionProperties = new StringBuilder();
            if (!string.IsNullOrEmpty(action.Attributes["tooltip"].Value)){
                additionalActionProperties.AppendLine($"                    Tooltip = '{EscapeALTextString(action.Attributes["tooltip"].Value)}';");
            }
            if (action.Attributes["obsolete"].Value.Length > 0)
            {
                var indentationInAction = "                    ";
                additionalActionProperties.AppendFormat(Encoding.UTF8.GetString(Resources.ObsoletePropertiesTemplate),
                    indentationInAction,
                    EscapeALTextString(action.Attributes["obsolete"].Value)
                );
                additionalActionProperties.AppendLine();
                additionalActionProperties.AppendLine(indentationInAction + "Visible = false;");
            }
            sb.AppendFormat(Encoding.UTF8.GetString(Resources.RoleCenterActionTemplate),
                action.Attributes["name"].Value, // 0
                EscapeALTextString(action.Attributes["caption"].Value), // 1
                action.Attributes["pbipagename"].Value, // 2
                additionalActionProperties // 3
                );
            if (action.Attributes["obsolete"].Value.Length > 0)
            {
                sb.AppendLine("#endif");
            }
        }

        public static string GeneratePBIEmbedPageCode(string alNamespace, XmlNode query)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder additionalProperties = new StringBuilder();
            if (query.Attributes["obsolete"].Value.Length > 0)
            {
                var indentationInAction = "    ";
                sb.AppendLine($"#if not CLEAN{query.Attributes["obsolete"].Value}");
                additionalProperties.AppendFormat(Encoding.UTF8.GetString(Resources.ObsoletePropertiesTemplate),
                    indentationInAction,
                    EscapeALTextString(query.Attributes["obsolete"].Value)
                    );
            }

            var applicationArea = query.Attributes["applicationArea"].Value.Length > 0 ? query.Attributes["applicationArea"].Value : "All" ;
            
            sb.AppendFormat(Encoding.UTF8.GetString(Resources.ReportPageTemplate),
                alNamespace, // 0
                query.Attributes["id"].Value, // 1
                query.Attributes["name"].Value, // 2
                EscapeALTextString(query.Attributes["caption"].Value), // 3
                EscapeALTextString(query.Attributes["aboutTitle"].Value), // 4
                EscapeALTextString(query.Attributes["aboutText"].Value), // 5
                query.Attributes["PBIReportPage"].Value, // 6
                query.Attributes["PBIReportIdFieldName"].Value, // 7
                additionalProperties, // 8
                applicationArea // 9
                );
            if (query.Attributes["obsolete"].Value.Length > 0)
            {
                sb.AppendLine("#endif");
            }
            return sb.ToString();
        }

        public static string GeneratePBIEmbedPagePerm(string alNamespace, XmlNode permSet, XmlNode pages)
        {
            StringBuilder pageStringBuilder = new StringBuilder();
            foreach (XmlNode page in pages.SelectNodes("page"))
            {
                if (page.Attributes["name"].Value == pages.LastChild.Attributes["name"].Value)
                {
                    pageStringBuilder.AppendLine("        page \"" + page.Attributes["name"].Value + "\" = X;");
                }
                else
                {
                    if (page.NextSibling.Attributes["obsolete"].Value.Length > 0 && page.NextSibling.Attributes["name"].Value == pages.LastChild.Attributes["name"].Value)
                    {
                        pageStringBuilder.AppendLine($"#if not CLEAN{page.NextSibling.Attributes["obsolete"].Value}");
                        pageStringBuilder.AppendLine("        page \"" + page.Attributes["name"].Value + "\" = X,");
                        pageStringBuilder.AppendLine("#pragma warning disable AL0432");
                        pageStringBuilder.AppendLine("        page \"" + page.NextSibling.Attributes["name"].Value + "\" = X;");
                        pageStringBuilder.AppendLine("#pragma warning restore AL0432");
                        pageStringBuilder.AppendLine("#else");
                        pageStringBuilder.AppendLine("        page \"" + page.Attributes["name"].Value + "\" = X;");
                        pageStringBuilder.AppendLine("#endif");
                        break;
                    }

                    if (page.Attributes["obsolete"].Value.Length > 0)
                    {
                        pageStringBuilder.AppendLine($"#if not CLEAN{page.Attributes["obsolete"].Value}");
                        pageStringBuilder.AppendLine("#pragma warning disable AL0432");
                    }
                    pageStringBuilder.AppendLine("        page \"" + page.Attributes["name"].Value + "\" = X,");
                    if (page.Attributes["obsolete"].Value.Length > 0)
                    {
                        pageStringBuilder.AppendLine("#pragma warning restore AL0432");
                        pageStringBuilder.AppendLine("#endif");
                    }
                }
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(Encoding.UTF8.GetString(Resources.PermissionSetTemplate),
                alNamespace, // 0
                permSet.Attributes["id"].Value, // 1
                permSet.Attributes["name"].Value, // 2
                pageStringBuilder.ToString() // 3
                );

            return sb.ToString();
        }

        public static string EscapeALTextString(string s)
        {
            return s.Replace("'", "''");
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
    }
}
