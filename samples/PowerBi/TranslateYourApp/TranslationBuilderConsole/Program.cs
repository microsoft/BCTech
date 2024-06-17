using Mono.Options;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net;
using TranslationsBuilder;
using TranslationsBuilder.Services;
using System.Drawing;
using System.Resources;
using Microsoft.AnalysisServices.Tabular;
using TranslationsBuilder.Models;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Data.Common;

namespace TranslationsBuilderConsole
{
    internal class Program
    {
        static Model model;

        static void Main(string[] args)
        {
            // http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html
            bool export = false;
            bool import = false;
            string fileFormat = "";
            string fileName = "";
            string directory = "";
            string culture = "";
            string pbixFile = "";
            bool showHelp = false;

            var p = new OptionSet() {
                { "e|export", "export language", v => export = v != null },
                { "i|import", "import language", v => import = v != null },
                { "h|help",  "show this message and exit", v => showHelp = v != null },
                { "f|fileFormat=",  "file format (required)", v => fileFormat = v },
                { "n|fileName=",  "file name (required)", v => fileName = v },
                { "d|directory=", "directory (require)", v => directory = v },
                { "c|culture=", "culture", v => culture = v },
                { "v|pbixFile=",    "target PBIX file path (required)", v => pbixFile = v }
            };

            List<string> extra;
            try
            {
                extra = p.Parse(args);
            } catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Try using --help' for more information.");
                Environment.Exit(0);
            }

            // Assume Power BI Desktop is running

            var process = Process.GetProcessesByName("msmdsrv")[0];
            var tcpTable = ManagedIpHelper.GetExtendedTcpTable();
            var tcpRow = tcpTable.SingleOrDefault((r) => r.ProcessId == process.Id && r.State == TcpState.Listen && IPAddress.IsLoopback(r.LocalEndPoint.Address));
            TranslationsManager.Connect("localhost:" + tcpRow?.LocalEndPoint.Port.ToString());
            model = TranslationsManager.model;

            if (export)
            {
                ExportResX(fileName, directory, culture, pbixFile);
                Environment.Exit(0);
            }

            if (import)
            {

                Environment.Exit(0);
            }

            ShowHelp(p);
            Environment.Exit(0);



            TranslationsManager.ImportTranslations(fileName);

            // Assume Power BI Desktop saves changes and closes
        }

        static void ExportResX(string fileName, string directory, string cultures, string pbixFile)
        {
            var cultureNames = cultures.Split(',');

            try
            {
                var defaultCulture = model.Cultures[model.Culture];

                foreach (var cultureName in cultureNames)
                {
                    var writer = new ResXResourceWriter(Path.Combine(directory, String.Format("{0}.{1}.{2}", fileName, cultureName, "resx")));
                    List<ResourceRow> resources = new List<ResourceRow>();

                    var targetCulture = model.Cultures.Find(cultureName);
                    var blanks = false;
                    if (targetCulture == null)
                    {
                        targetCulture = defaultCulture;
                        blanks = true;
                    } 
                    var useDefault = defaultCulture.Name == targetCulture.Name;

                    foreach (Table table in model.Tables)
                    {
                        List<string> displayFoldersForTable = new List<string>();

                        string tableName = table.Name.ToLower();
                        if ((!table.IsHidden && !tableName.Contains("translated") && !tableName.Contains("translations")) || table.Name.Equals(TranslationsManager.LocalizedLabelsTableName))
                        {
                            if (!table.Name.Equals(TranslationsManager.LocalizedLabelsTableName))
                            {
                                resources.AddRange(GetTableRows(table, targetCulture, useDefault));
                            }

                            foreach (Column column in table.Columns)
                            {
                                if (!column.IsHidden && !column.Name.ToLower().Contains("translation") && !table.Name.Equals(TranslationsManager.LocalizedLabelsTableName) && column.Name != table.Name)
                                {
                                    resources.AddRange(GetColumnRows(table, column, targetCulture, displayFoldersForTable, useDefault));
                                }
                            }
                                
                            foreach (Measure measure in table.Measures)
                            {
                                if (!measure.IsHidden || table.Name.Equals(TranslationsManager.LocalizedLabelsTableName))
                                {
                                    resources.AddRange(GetMeasureRows(table, measure, targetCulture, displayFoldersForTable, useDefault));
                                }
                            }
                                
                            foreach (Hierarchy hierarchy in table.Hierarchies)
                            {
                                if (!hierarchy.IsHidden)
                                {
                                    resources.AddRange(GetHierarchyRows(table, hierarchy, targetCulture, displayFoldersForTable, useDefault));
                                }
                            }

                        }
                    }

                    foreach (var resource in resources)
                    {
                        ResXDataNode resourceNode = null;
                        if (blanks)
                        {
                            resourceNode = new ResXDataNode(resource.Id, "");
                        } 
                        else
                        {
                            resourceNode = new ResXDataNode(resource.Id, resource.Value);
                        }
                        resourceNode.Comment = resource.Comment;
                        writer.AddResource(resourceNode);
                    }

                    writer.Close();
                }
            } catch (Exception ex)
            {

            }
        }

        static void Import()
        {

        }


        static List<ResourceRow> GetTableRows(Table table, Culture culture, bool useDefault)
        {
            List<ResourceRow> rows = new List<ResourceRow>();
            var caption = culture.ObjectTranslations[table, TranslatedProperty.Caption]?.Value;
            if (useDefault)
            {
                caption ??= table.Name;
            }
            rows.Add(new ResourceRow(table.LineageTag, caption, String.Format("table [{0}]", table.Name)));

            if (!string.IsNullOrEmpty(table.Description))
            {
                var description = culture.ObjectTranslations[table, TranslatedProperty.Description]?.Value;
                if (useDefault)
                {
                    description ??= table.Description;
                }
                rows.Add(new ResourceRow(String.Format("[{0}]-description", table.Name), description, String.Format("table [{0}]", table.Name)));
            }

            return rows;
        }

        static List<ResourceRow> GetColumnRows(Table table, Column column, Culture culture, List<string> displayFoldersForTable, bool useDefault)
        {
            List<ResourceRow> rows = new List<ResourceRow>();

            var caption = culture.ObjectTranslations[column, TranslatedProperty.Caption]?.Value;
            if (useDefault)
            {
                caption ??= column.Name;
            }
            rows.Add(new ResourceRow(column.LineageTag, caption, String.Format("table [{0}] column [{1}]", table.Name, column.Name)));

            if (!string.IsNullOrEmpty(column.DisplayFolder) && !displayFoldersForTable.Contains(column.DisplayFolder))
            {
                rows.Add(new ResourceRow(String.Format("[{0}]-displayFolder", column.LineageTag), culture.ObjectTranslations[column, TranslatedProperty.DisplayFolder]?.Value, String.Format("table [{0}] column [{1}]", table.Name, column.Name)));
                displayFoldersForTable.Add(column.DisplayFolder);
            }

            if (!string.IsNullOrEmpty(column.Description))
            {
                rows.Add(new ResourceRow(String.Format("[{0}]-description", column.Name), culture.ObjectTranslations[column, TranslatedProperty.Description]?.Value, String.Format("table [{0}] column [{1}]", table.Name, column.Name)));
            }

            return rows;
        }

        static List<ResourceRow> GetMeasureRows(Table table, Measure measure, Culture culture, List<string> displayFoldersForTable, bool useDefault)
        {
            List<ResourceRow> rows = new List<ResourceRow>();

            var caption = culture.ObjectTranslations[measure, TranslatedProperty.Caption]?.Value;
            if (useDefault)
            {
                caption ??= measure.Name;
            }
            rows.Add(new ResourceRow(measure.LineageTag, caption, String.Format("table [{0}] measure [{1}]", table.Name, measure.Name)));

            if (!string.IsNullOrEmpty(measure.DisplayFolder) && !displayFoldersForTable.Contains(measure.DisplayFolder))
            {
                rows.Add(new ResourceRow(String.Format("[{0}]-displayFolder", measure.LineageTag), culture.ObjectTranslations[measure, TranslatedProperty.DisplayFolder]?.Value, String.Format("table [{0}] measure [{1}]", table.Name, measure.Name)));
                displayFoldersForTable.Add(measure.DisplayFolder);
            }

            if (!string.IsNullOrEmpty(measure.Description))
            {
                rows.Add(new ResourceRow(String.Format("[{0}]-description", measure.Name), culture.ObjectTranslations[measure, TranslatedProperty.Description]?.Value, String.Format("table [{0}] measure [{1}]", table.Name, measure.Name)));
            }

            return rows;
        }

        static List<ResourceRow> GetHierarchyRows(Table table, Hierarchy hierarchy, Culture culture, List<string> displayFoldersForTable, bool useDefault)
        {
            List<ResourceRow> rows = new List<ResourceRow>();

            var caption = culture.ObjectTranslations[hierarchy, TranslatedProperty.Caption]?.Value;
            if (useDefault)
            {
                caption ??= hierarchy.Name;
            }
            rows.Add(new ResourceRow(hierarchy.LineageTag, caption, String.Format("table [{0}] hierarchy [{1}]", table.Name, hierarchy.Name)));

            if (!string.IsNullOrEmpty(hierarchy.DisplayFolder) && !displayFoldersForTable.Contains(hierarchy.DisplayFolder))
            {
                rows.Add(new ResourceRow(String.Format("[{0}]-displayFolder", hierarchy.LineageTag), culture.ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder]?.Value, String.Format("table [{0}] hierarchy [{1}]", table.Name, hierarchy.Name)));
            }

            foreach (Level hierarchyLevel in hierarchy.Levels)
            {
                var hierarchyLevelCaption = culture.ObjectTranslations[hierarchyLevel, TranslatedProperty.Description]?.Value;
                if (useDefault)
                {
                    hierarchyLevelCaption ??= hierarchyLevel.Name;
                }
                rows.Add(new ResourceRow(String.Format("[{0}]-hierarchyLevel", hierarchyLevel.Name), hierarchyLevelCaption, String.Format("table [{0}] hierarchyLevel [{1}]", table.Name, hierarchyLevel.Name)));
            }


            if (!string.IsNullOrEmpty(hierarchy.Description))
            {
                rows.Add(new ResourceRow(String.Format("[{0}]-description", hierarchy.Name), culture.ObjectTranslations[hierarchy, TranslatedProperty.Description]?.Value, String.Format("table [{0}] measure [{1}]", table.Name, hierarchy.Name)));
            }

            return rows;
        }

        static void ShowHelp(OptionSet p)
        {
            Console.WriteLine("Usage: TranslationBuilderConsole [OPTIONS]+");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
            Console.WriteLine("-------------------");
        }

        [Serializable()]
        class ResourceRow
        {
            public string Id { get; }
            public string Value { get;  }
            public string Comment { get; }

            public ResourceRow(string id, string value, string comment)
            {
                this.Id = id;
                this.Value = value;
                this.Comment = comment;
            }

        }
    }
}