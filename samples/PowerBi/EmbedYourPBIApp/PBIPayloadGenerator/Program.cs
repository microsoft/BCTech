using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.CommandLine;


namespace PayloadGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var rootCommand = new RootCommand("Power BI content - AL payload generator");
            var inputFileOption = new Option<FileInfo>("--inputfile", "The input Excel file")
            {
                IsRequired = true
            };
            inputFileOption.AddAlias("--i");
            var outputFileOption = new Option<FileInfo>("--outputfile", "The output file to be used as payload for the AL generator")
            {
                IsRequired = true
            };
            var outputDirOption = new Option<DirectoryInfo>(
                "--outputdir",
                description: "The output directory for the output file",
                getDefaultValue: () => new DirectoryInfo(System.IO.Directory.GetCurrentDirectory())
            );
            rootCommand.AddOption(inputFileOption);
            rootCommand.AddOption(outputFileOption);
            rootCommand.AddOption(outputDirOption);

            rootCommand.SetHandler((inputFile, outputFile, outputDir) =>
            {
                Run(inputFile.FullName, outputFile.FullName, outputDir.FullName);
            }, inputFileOption, outputFileOption, outputDirOption);

            try
            {
                rootCommand.InvokeAsync(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Try using --help' for more information.");
                Environment.Exit(-1);
            }
        }
        public static void Run(string inputfile, string outputfile, string outputdir)
        { 

            Console.WriteLine("PBI embed app payload generator");

            // Get data from Excel file
            Console.WriteLine("------------------------");
            Console.WriteLine("Reading input for PBI embed app: " + inputfile);

            var NamespaceSheetName = "Namespace";
            List<string[]> NamespaceRows = readExcelWorksheet(inputfile, NamespaceSheetName);
            if (NamespaceRows.Count > 2)
            {
                throw new Exception("Only one namespace definition is supported.");
            }
            Console.WriteLine("Worksheet '{0}'. Found : {1} rows", NamespaceSheetName, NamespaceRows.Count.ToString());
            string ALNamespace = "";
            if (NamespaceRows.Count >= 1 && NamespaceRows[0].Length > 0)
            {
                ALNamespace = NamespaceRows[0][0];
            }

            var PBIReportsSheetName = "PBIReports";
            List<string[]> PBIReportRows = readExcelWorksheet(inputfile, PBIReportsSheetName);
            Console.WriteLine("Worksheet '{0}'. Found : {1} rows", PBIReportsSheetName, PBIReportRows.Count.ToString());
            List<PBIPageDefinition> pages = new List<PBIPageDefinition>();
            ExcelRowstoPBIPageDefinitions(PBIReportRows, pages);

            var RCExtensionsSheetName = "RCExtensions";
            List<string[]> RCExtensionRows = readExcelWorksheet(inputfile, RCExtensionsSheetName);
            Console.WriteLine("Worksheet '{0}'. Found : {1} rows", RCExtensionsSheetName, RCExtensionRows.Count.ToString());
            List<RCExtensionDefinition> rcExtensions = new List<RCExtensionDefinition>();
            ExcelRowstoRCExtensionDefinitions(RCExtensionRows, rcExtensions);

            var RCExtensionActionsSheetName = "RCExtensionActions";
            List<string[]> RCExtensionActionRows = readExcelWorksheet(inputfile, RCExtensionActionsSheetName);
            Console.WriteLine("Worksheet '{0}'. Found : {1} rows", RCExtensionActionsSheetName, RCExtensionActionRows.Count.ToString() );
            List<ActionDefinition> rcExtensionActions = new List<ActionDefinition>();
            ExcelRowstoRCExtensionActionDefinitions(RCExtensionActionRows, rcExtensionActions);

            var permSetSheetName = "PermissionSets";
            List<string[]> permSetRows = readExcelWorksheet(inputfile, permSetSheetName);
            Console.WriteLine("Worksheet '{0}'. Found : {1} rows", permSetSheetName, permSetRows.Count.ToString());
            List<PermSetDefinition> permSets = new List<PermSetDefinition>();
            ExcelRowstoPermSetDefinitions(permSetRows, permSets);

            foreach ( var ext in rcExtensions)
            {
                var extName = ext.name;

                foreach (var action in rcExtensionActions)
                {
                    if(action.rcExtensionName == extName)
                    {
                        ext.addAction(action);
                    }
                }
            }

            if (outputfile.Equals(""))
            {
                Console.WriteLine("Output file parameter required.");
                Environment.Exit(-1);
            }
            Console.WriteLine("Using directory for output files: " + outputdir);
            Console.WriteLine("Generating payload file...");

            XDocument payloaddoc = GeneratePayload(ALNamespace, pages, rcExtensions, permSets);
            string content = payloaddoc.ToString();
            SaveFile(outputdir, outputfile, content);
            Console.WriteLine("Wrote file: " + outputfile);
        }

        public static XDocument GeneratePayload(string ALNamespaceString, IEnumerable<PBIPageDefinition> pages, IEnumerable<RCExtensionDefinition> rcExtensions, IEnumerable<PermSetDefinition> permSets)
        {
            XDocument doc = new XDocument(

            );

            XElement objects = new XElement("objects");
            doc.Add(objects);

            XElement ALNamespace = new XElement("Namespace");
            objects.Add(ALNamespace);
            ALNamespace.SetAttributeValue("namespace", ALNamespaceString);

            XElement PBIEmbedPages = new XElement("PBIEmbedPages");
            objects.Add(PBIEmbedPages);
            foreach (PBIPageDefinition page in pages)
            {
                XElement el = page.asXElement();
                PBIEmbedPages.Add(el);
            }

            XElement RCExtensions = new XElement("RCExtensions");
            objects.Add(RCExtensions);
            foreach (RCExtensionDefinition ext in rcExtensions)
            {
                XElement el = ext.asXElement();
                RCExtensions.Add(el);
            }

            XElement PermissionSets = new XElement("PermissionSets");
            objects.Add(PermissionSets);
            foreach (PermSetDefinition permSet in permSets)
            {
                XElement el = permSet.asXElement();
                PermissionSets.Add(el);
            }

            return doc;
        }



        // https://epsicodecom.wordpress.com/2019/08/12/tutorial-generate-seperate-files-from-a-t4-template/
        public static void SaveFile(string folder, string fileName, string content)
        {
            using (FileStream fs = new FileStream(Path.Combine(folder, fileName.Trim()), FileMode.Create))
            {
                using (StreamWriter str = new StreamWriter(fs))
                {
                    str.WriteLine(content);
                    str.Flush();
                }
            }
        }

        static private List<string[]> readExcelWorksheet(string inputfile, string worksheetName)
        {
            List<string[]> rows = new List<string[]>();

            using (var document = SpreadsheetDocument.Open(inputfile, isEditable: false))
            {
                var workbookPart = document.WorkbookPart;
                var sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == worksheetName);
                var worksheetPart = (WorksheetPart)(workbookPart.GetPartById(sheet.Id));
                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                var columnRefs = new List<string>();
                var regex = new Regex("\\d+");

                foreach (var row in sheetData.Elements<Row>())
                {
                    if (row.RowIndex == 1)
                    {
                        foreach (var cell in row.Elements<Cell>())
                        {
                            columnRefs.Add(regex.Replace(cell.CellReference, ""));
                        }
                    } 
                    else
                    {
                        List<string> cells = new List<string>();

                        foreach(var columnRef in columnRefs)
                        {
                            var query = row.Elements<Cell>().Where(cell => regex.Replace(cell.CellReference, "") == columnRef).FirstOrDefault();

                            if (query == null)
                            {
                                cells.Add("");
                            }
                            else
                            {
                                cells.Add(GetCellValue(query, workbookPart));
                            }
                        }

                        var cellArray = cells.ToArray();
                        rows.Add(cellArray);
                    }
                }
            }
            return rows;
        }

        static private void ExcelRowstoPBIPageDefinitions(List<string[]> PBIReportRows, List<PBIPageDefinition> pages)
        {
            foreach (var row in PBIReportRows)
            {
                PBIPageDefinition Record = new PBIPageDefinition(
                    row[0],
                    row[1],
                    row[2],
                    row[3],
                    row[4],
                    row[5],
                    row[6],
                    row[7],
                    row[8],
                    row[9],
                    row[10]
                ); 

                if (!row[0].Equals("id"))
                {
                    pages.Add(Record);
                    Console.WriteLine("Read PBI page metadata for: " + Record.name);
                }
            }
        }

        static private void ExcelRowstoRCExtensionDefinitions(List<string[]> RCExtensionRows, List<RCExtensionDefinition> extensions)
        {
            foreach (var row in RCExtensionRows)
            {
                RCExtensionDefinition Record = new RCExtensionDefinition(
                    row[0],
                    row[1],
                    row[2],
                    row[3],
                    row[4],
                    row[5],
                    row[6],
                    row[7]
                );

                if (!row[0].Equals("id"))
                {
                    extensions.Add(Record);
                    Console.WriteLine("Read Role center extension metadata for: " + Record.name);
                }
            }
        }

        static private void ExcelRowstoRCExtensionActionDefinitions(List<string[]> RCExtensionActionRows, List<ActionDefinition> actions)
        {
            foreach (var row in RCExtensionActionRows)
            {
                ActionDefinition Record = new ActionDefinition(
                    row[0],
                    row[1],
                    row[2],
                    row[3],
                    row[4],
                    row[5]
                );

                if (!row[0].Equals("rcextensionname"))
                {
                    actions.Add(Record);
                    Console.WriteLine("Read Role center extension action metadata for: " + Record.name);
                }
            }
        }
        static private void ExcelRowstoPermSetDefinitions(List<string[]> PermSetRows, List<PermSetDefinition> extensions)
        {
            foreach (var row in PermSetRows)
            {
                PermSetDefinition Record = new PermSetDefinition(
                    row[0],
                    row[1],
                    row[2]
                );

                if (!row[0].Equals("id"))
                {
                    extensions.Add(Record);
                    Console.WriteLine("Read permission set metadata for: " + Record.name);
                }
            }
        }

        // https://learn.microsoft.com/en-us/office/open-xml/spreadsheet/how-to-retrieve-the-values-of-cells-in-a-spreadsheet?tabs=cs-0%2Ccs-2%2Ccs-3%2Ccs-4%2Ccs-5%2Ccs-6%2Ccs-7%2Ccs-8%2Ccs-9%2Ccs-10%2Ccs-11%2Ccs#sample-code
        static private string GetCellValue(Cell cell, WorkbookPart workbookPart)
        {
            if (cell == null)
            {
                return "";
            }

            var value = cell.CellFormula != null
                ? cell.CellValue.InnerText
                : cell.InnerText.Trim();

            // If the cell represents an integer number, you are done. 
            // For dates, this code returns the serialized value that 
            // represents the date. The code handles strings and 
            // Booleans individually. For shared strings, the code 
            // looks up the corresponding value in the shared string 
            // table. For Booleans, the code converts the value into 
            // the words TRUE or FALSE.
            if (cell.DataType == null)
            {
                return value;
            }

            if(cell.DataType.Value == CellValues.SharedString)
            {
                // For shared strings, look up the value in the
                // shared strings table.
                var stringTable =
                    workbookPart.GetPartsOfType<SharedStringTablePart>()
                        .FirstOrDefault();

                // If the shared string table is missing, something 
                // is wrong. Return the index that is in
                // the cell. Otherwise, look up the correct text in 
                // the table.
                if (stringTable != null)
                {
                    value =
                        stringTable.SharedStringTable
                            .ElementAt(int.Parse(value)).InnerText;
                }
                else if (cell.DataType.Value == CellValues.Boolean)
                {
                    switch (value)
                    {
                        case "0":
                            value = "FALSE";
                            break;
                        default:
                            value = "TRUE";
                            break;
                    }
                }
            }

            return value;
        }


    }
}
