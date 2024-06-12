using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace TranslationsBuilder.Services {

  class ExcelUtilities {

    public static void OpenCsvInExcel(string FilePath) {

      ProcessStartInfo startInfo = new ProcessStartInfo();

      string excelLocation1 = @"C:\Program Files\Microsoft Office\root\Office16\EXCEL.EXE";
      string excelLocation2 = @"C:\Program Files (x86)\Microsoft Office\root\Office16\EXCEL.EXE";
      bool excelFound = false;

      if (File.Exists(excelLocation1)) {
        startInfo.FileName = excelLocation1;
        excelFound = true;
      }
      else {
        if (File.Exists(excelLocation2)) {
          startInfo.FileName = excelLocation2;
          excelFound = true;
        }
      }
      if (excelFound) {
        startInfo.Arguments = FilePath;
        Process.Start(startInfo);
      }
      else {
        System.Console.WriteLine("Coud not find Microsoft Excel on this PC.");
      }
    }

  }

}
