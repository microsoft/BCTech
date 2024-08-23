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
using System.Collections;

namespace TranslationsBuilderConsole
{
    internal class Program
    {
        const string IMPORT = "import";
        const string EXPORT = "export";
        const string CAPTION = "caption";
        const string DESCRIPTION = "description";
        const string TABLE = "table";
        const string COLUMN = "column";
        const string DISPLAYFOLDER = "displayFolder";
        const string MEASURE = "measure";
        const string HIERARCHY = "hierarchy";
        const string LEVEL = "level";

        static Model model;

        static void Main(string[] args)
        {
            // http://www.ndesk.org/doc/ndesk-options/NDesk.Options/OptionSet.html
            string operation = "";
            string fileFormat = "";
            string filePath = "";
            string filePrefix = "";
            string cultures = "";
            string defaultCulture = "";
            string directory = "";
            string pbixFile = "";
            bool showHelp = false;

            var p = new OptionSet() {
                { "o|operation=", "operation(required) [export,import]", v => operation = v },
                { "f|fileFormat=",  "file format(required) [csv,resx,json]", v => fileFormat = v },
                { "h|help",  "show this message and exit", v => showHelp = v != null },
                { "filePath=", "file to export/import", v => filePath = v },
                { "filePrefix=",  "file prefix when exporting/importing", v => filePrefix = v },
                { "directory=", "directory for exporting/importing", v => directory = v },
                { "cultures=", "cultures to export", v => cultures = v },
                { "defaultCulture=", "default culture when importing", v => defaultCulture = v },
                { "pbixFile=", "target PBIX file path", v => pbixFile = v }
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

            if (showHelp)
            {
                ShowHelp(p);
                Environment.Exit(0);
            }

            switch (fileFormat)
            {
                case "csv":
                    switch (operation)
                    {
                        case IMPORT:
                            Console.WriteLine("Importing csv translations...");
                            if (String.IsNullOrEmpty(filePath)) { Console.WriteLine("Missing filePath."); }
                            TryConnectPBIDesktop();
                            TranslationsManager.ImportTranslations(filePath);
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Unsupported operation.");
                            break;
                    }
                    break;
                case "resx":
                    if (String.IsNullOrEmpty(filePrefix)) { Console.WriteLine("Missing filePrefix."); }
                    if (String.IsNullOrEmpty(directory)) { Console.WriteLine("Missing directory."); }
                    switch(operation)
                    {
                        case IMPORT:
                            Console.WriteLine("Importing resx translations...");
                            if (String.IsNullOrEmpty(defaultCulture)) { Console.WriteLine("Missing defaultCulture."); }
                            TryConnectPBIDesktop();
                            ImportResX(filePrefix, directory, defaultCulture, pbixFile);
                            Environment.Exit(0);
                            break;
                        case EXPORT:
                            Console.WriteLine("Exporting resx translations...");
                            if (String.IsNullOrEmpty(cultures)) { Console.WriteLine("Missing cultures."); }
                            TryConnectPBIDesktop();
                            ExportResX(filePrefix, directory, cultures, pbixFile);
                            Environment.Exit(0);
                            break;
                        default:
                            Console.WriteLine("Unsupported operation.");
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Unsupported fileFormat.");
                    break;
            }

            ShowHelp(p);
            Environment.Exit(0);
        }

        static void TryConnectPBIDesktop()
        {
            try
            {
                // Assume Power BI Desktop is running
                Console.WriteLine("Connecting to Power BI Desktop...");
                var process = Process.GetProcessesByName("msmdsrv")[0];
                var tcpTable = ManagedIpHelper.GetExtendedTcpTable();
                var tcpRow = tcpTable.SingleOrDefault((r) => r.ProcessId == process.Id && r.State == TcpState.Listen && IPAddress.IsLoopback(r.LocalEndPoint.Address));
                TranslationsManager.Connect("localhost:" + tcpRow?.LocalEndPoint.Port.ToString());
            } catch (Exception e)
            {
                Console.WriteLine("Error connecting to Power BI Desktop");
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
            model = TranslationsManager.model;
        }

        static void ImportResX(string fileName, string directory, string defaultCulture, string pbixFile)
        {
            if (!TranslationsManager.DoesTableExistInModel(TranslationsManager.LocalizedLabelsTableName))
            {
                TranslationsManager.CreateLocalizedLabelsTable(false);
            }
            var filePaths = Directory.GetFiles(directory, String.Format("{0}.*.resx", fileName));
            foreach (var filePath in filePaths)
            {
                var cultureName = filePath.Split('.')[1];
                if (cultureName == defaultCulture)
                {
                    continue;
                }
                if (!model.Cultures.Contains(cultureName))
                {
                    model.Cultures.Add(new Culture { Name = cultureName });
                    model.SaveChanges();
                }
                var reader = new ResXResourceReader(filePath);
                foreach (DictionaryEntry resource in reader)
                {
                    var name = ((string)resource.Key).Split('.');
                    var value = (string)resource.Value;
                    if (String.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    var table = model.Tables.FindByLineageTag(name[0]);
                    if (table == null)
                    {
                        continue;
                    }
                    switch (name[1])
                    {
                        case CAPTION:
                            model.Cultures[cultureName].ObjectTranslations.SetTranslation(table, TranslatedProperty.Caption, value);
                            break;
                        case DESCRIPTION:
                            model.Cultures[cultureName].ObjectTranslations.SetTranslation(table, TranslatedProperty.Description, value);
                            break;
                        case COLUMN:
                            var column = table.Columns.FindByLineageTag(name[2]);
                            if (column == null)
                            {
                                continue;
                            }
                            switch (name[3])
                            {
                                case CAPTION:
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(column, TranslatedProperty.Caption, value);
                                    break;
                                case DESCRIPTION:
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(column, TranslatedProperty.Description, value);
                                    break;
                                case DISPLAYFOLDER:
                                    UpdateDisplayFolderForTable(table, column.DisplayFolder, cultureName, value);
                                    break;
                            }
                            break;
                        case MEASURE:
                            var measure = table.Measures.FindByLineageTag(name[2]);
                            if (measure == null)
                            {
                                continue;
                            }
                            switch (name[3])
                            {
                                case CAPTION:
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(measure, TranslatedProperty.Caption, value);
                                    break;
                                case DESCRIPTION:
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(measure, TranslatedProperty.Description, value);
                                    break;
                                case DISPLAYFOLDER:
                                    UpdateDisplayFolderForTable(table, measure.DisplayFolder, cultureName, value);
                                    break;
                            }
                            break;
                        case HIERARCHY:
                            var hierarchy = table.Hierarchies.FindByLineageTag(name[2]);
                            if (hierarchy == null)
                            {
                                continue;
                            }
                            switch (name[3])
                            {
                                case CAPTION:
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.Caption, value);
                                    break;
                                case DESCRIPTION:
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.Description, value);
                                    break;
                                case DISPLAYFOLDER:
                                    UpdateDisplayFolderForTable(table, hierarchy.DisplayFolder, cultureName, value);
                                    break;
                                case LEVEL:
                                    var level = hierarchy.Levels.FindByLineageTag(name[4]);
                                    if (level == null)
                                    {
                                        continue;
                                    }
                                    model.Cultures[cultureName].ObjectTranslations.SetTranslation(level, TranslatedProperty.Caption, value);
                                    break;
                            }
                            break;
                    }
                }
                model.SaveChanges();
            }
            TranslationsManager.GenerateTranslatedLocalizedLabelsTable();
        }

        static void UpdateDisplayFolderForTable(Table table, string folderName, string targetCulure, string value)
        {
            foreach (Column column in table.Columns)
            {
                if (column.DisplayFolder.Equals(folderName))
                {
                    model.Cultures[targetCulure].ObjectTranslations.SetTranslation(column, TranslatedProperty.DisplayFolder, value);
                }
            }
            foreach (Measure measure in table.Measures)
            {
                if (measure.DisplayFolder.Equals(folderName))
                {
                    model.Cultures[targetCulure].ObjectTranslations.SetTranslation(measure, TranslatedProperty.DisplayFolder, value);
                }
            }
            foreach (Hierarchy hierarchy in table.Hierarchies)
            {
                if (hierarchy.DisplayFolder.Equals(folderName))
                {
                    model.Cultures[targetCulure].ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.DisplayFolder, value);
                }
            }
        }

        static void ExportResX(string fileName, string directory, string cultures, string pbixFile)
        {
            var cultureNames = cultures.Split(',');

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
        }

        static List<ResourceRow> GetTableRows(Table table, Culture culture, bool useDefault)
        {
            List<ResourceRow> rows = new List<ResourceRow>();
            var caption = culture.ObjectTranslations[table, TranslatedProperty.Caption]?.Value;
            if (useDefault)
            {
                caption ??= table.Name;
            }
            rows.Add(new ResourceRow($"{table.LineageTag}.{CAPTION}", caption, $"table [table.Name]"));

            if (!string.IsNullOrEmpty(table.Description))
            {
                var description = culture.ObjectTranslations[table, TranslatedProperty.Description]?.Value;
                if (useDefault)
                {
                    description ??= table.Description;
                }
                rows.Add(new ResourceRow($"{table.LineageTag}.{DESCRIPTION}", description, $"table [{table.Name}]"));
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
            rows.Add(new ResourceRow($"{table.LineageTag}.{COLUMN}.{column.LineageTag}.{CAPTION}", caption, $"{TABLE} [{table.Name}] {COLUMN} [{column.Name}]"));

            if (!string.IsNullOrEmpty(column.DisplayFolder) && !displayFoldersForTable.Contains(column.DisplayFolder))
            {
                rows.Add(new ResourceRow($"{table.LineageTag}.{COLUMN}.{column.LineageTag}.{DISPLAYFOLDER}", culture.ObjectTranslations[column, TranslatedProperty.DisplayFolder]?.Value, $"{TABLE} [{table.Name}] {COLUMN} [{column.Name}]"));
                displayFoldersForTable.Add(column.DisplayFolder);
            }

            if (!string.IsNullOrEmpty(column.Description))
            {
                rows.Add(new ResourceRow($"{table.LineageTag}.{COLUMN}.{column.LineageTag}.{DESCRIPTION}", culture.ObjectTranslations[column, TranslatedProperty.Description]?.Value, $"{TABLE} [{table.Name}] {COLUMN} [{column.Name}]"));
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
            rows.Add(new ResourceRow($"{table.LineageTag}.{MEASURE}.{measure.LineageTag}.{CAPTION}", caption, $"{TABLE} [{table.Name}] {MEASURE} [{measure.Name}]"));

            if (!string.IsNullOrEmpty(measure.DisplayFolder) && !displayFoldersForTable.Contains(measure.DisplayFolder))
            {
                rows.Add(new ResourceRow($"{table.LineageTag}.{MEASURE}.{measure.LineageTag}.{DISPLAYFOLDER}", culture.ObjectTranslations[measure, TranslatedProperty.DisplayFolder]?.Value, $"{TABLE} [{table.Name}] {MEASURE} [{measure.Name}]"));
                displayFoldersForTable.Add(measure.DisplayFolder);
            }

            if (!string.IsNullOrEmpty(measure.Description))
            {
                rows.Add(new ResourceRow($"{table.LineageTag}.{MEASURE}.{measure.LineageTag}.{DESCRIPTION}", culture.ObjectTranslations[measure, TranslatedProperty.Description]?.Value, $"{TABLE} [{table.Name}] {MEASURE} [{measure.Name}]"));
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
            rows.Add(new ResourceRow($"{table.LineageTag}.{HIERARCHY}.{hierarchy.LineageTag}.{CAPTION}", caption, $"{TABLE} [{table.Name}] {HIERARCHY} [{hierarchy.Name}]"));

            if (!string.IsNullOrEmpty(hierarchy.DisplayFolder) && !displayFoldersForTable.Contains(hierarchy.DisplayFolder))
            {
                rows.Add(new ResourceRow($"{table.LineageTag}.{HIERARCHY}.{hierarchy.LineageTag}.{DISPLAYFOLDER}", culture.ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder]?.Value, $"{TABLE} [{table.Name}] {HIERARCHY} [{hierarchy.Name}]"));
            }

            foreach (Level hierarchyLevel in hierarchy.Levels)
            {
                var hierarchyLevelCaption = culture.ObjectTranslations[hierarchyLevel, TranslatedProperty.Description]?.Value;
                if (useDefault)
                {
                    hierarchyLevelCaption ??= hierarchyLevel.Name;
                }
                rows.Add(new ResourceRow($"{table.LineageTag}.{HIERARCHY}.{hierarchy.LineageTag}.{LEVEL}.{hierarchyLevel.LineageTag}", hierarchyLevelCaption, $"{TABLE} [{table.Name}] {HIERARCHY} [{hierarchy.Name}] {LEVEL} [{hierarchyLevel.Name}]"));
            }


            if (!string.IsNullOrEmpty(hierarchy.Description))
            {
                rows.Add(new ResourceRow($"{table.LineageTag}.{HIERARCHY}.{hierarchy.LineageTag}.{DESCRIPTION}", culture.ObjectTranslations[hierarchy, TranslatedProperty.Description]?.Value, $"{TABLE} [{table.Name}] {HIERARCHY} [{hierarchy.Name}]"));
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
            Console.WriteLine("Examples:");
            Console.WriteLine("\t--operation export --fileFormat resx --directory \"<path>\" --filePrefix \"FinanceApp\" --cultures \"en-US,fr-FR,da-DK\"");
            Console.WriteLine("\t--operation import --fileFormat resx --directory \"<path>\" --filePrefix \"FinanceApp\" --defaultCulture \"en-US\"");
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