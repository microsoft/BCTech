using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AnalysisServices.Tabular;
using TranslationsBuilder.Models;
using AMO = Microsoft.AnalysisServices;
using AdomdClient = Microsoft.AnalysisServices.AdomdClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Forms;
using System.Reflection.Emit;
using System.Data.Common;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using CsvHelper;
using System.Globalization;
using System.Security.Cryptography;

namespace TranslationsBuilder.Services {

  internal class TranslationsManager {

    public const string TranslatedDatasetObjectsTableName = "Translated Dataset Object Labels";
    public const string LocalizedLabelsTableName = "Localized Labels";
    public const string TranslatedLocalizedLabelsTableName = "Translated Localized Labels";
    private const string AppAnnotationName = "TranslationsBuilder";

    private static Server server = new Server();

    public static Database database;
    public static Model model;
    public static bool IsConnected = false;

    public static DatasetConnection ActiveConnection;

    static TranslationsManager() {

      var sessions = PowerBiDesktopUtilities.GetActiveDatasetConnections();

      if (!string.IsNullOrEmpty(AppSettings.Server)) {
        var session = sessions.Find((s) => s.ConnectString == AppSettings.Server);
        if (session != null) {
          server.Connect(AppSettings.Server);
          database = server.Databases[0];
          model = database.Model;
          IsConnected = true;
          ActiveConnection = session;
          UpdateAppDataModelAnnotation();
          return;
        }
      }

      AppSettings.Server = "";
      AppSettings.Database = "";
      IsConnected = false;
      ActiveConnection = null;

    }

    public static void Connect(string ConnectString) {

      if (IsConnected) {
        server.Disconnect(true);
      }

      server.Connect(ConnectString);
      database = server.Databases[0];
      model = database.Model;
      IsConnected = true;
      SetLocalActiveConnection();
      AppSettings.Server = ConnectString;
      UpdateAppDataModelAnnotation();
    }

    public static void UpdateAppDataModelAnnotation() {

      string annotationValue = "Version:" + GlobalConstants.VersionNumber + "-Updated:" + DateTime.Today.ToString("yyyy-MM-dd");

      if (model.Annotations.Contains(AppAnnotationName)) {
        model.Annotations[AppAnnotationName].Value = annotationValue;
      }
      else {
        model.Annotations.Add(new Annotation { Name = AppAnnotationName, Value = annotationValue });
      }
      model.SaveChanges();

    }

    private static void SetLocalActiveConnection() {
      var sessions = PowerBiDesktopUtilities.GetActiveDatasetConnections();
      ActiveConnection = sessions.Find((s) => s.ConnectString == server.ConnectionString);
    }

    public static void Disconnect() {
      IsConnected = false;
      ActiveConnection = null;
      server.Disconnect(true);
    }

    public static string DatasetName {
      get {
        return TranslationsManager.ActiveConnection.DatasetName;
      }
    }

    public static List<string> GetTables() {
      var tables = new List<string>();
      //*** enumerate through tables
      foreach (Table table in model.Tables) {
        tables.Add(table.Name);
      }
      return tables;
    }

    public static void RefreshDataFromServer() {
      try {
        model.Sync(new SyncOptions { DiscardLocalChanges = false });
      }
      catch { }
    }

    public static bool TranslationsExist() {

      foreach (Table table in model.Tables) {
        // exclude hidden tables and system tables
        if ((!table.IsHidden && !table.Name.ToLower().Contains("translated"))) {
          // do not include 'Localized Labels' table name but do include the measures inside
          if (!table.Name.Equals("Localized Labels")) {
            return true;
          }

          foreach (Column column in table.Columns) {
            if (!column.IsHidden && !column.Name.ToLower().Contains("translation") && !table.Name.Equals("Localized Labels") && column.Name != table.Name) {
              return true;
            }
          }

          foreach (Measure measure in table.Measures) {
            if (!measure.IsHidden) {
              return true;
            }
          }

          foreach (Hierarchy hierarchy in table.Hierarchies) {
            if (!hierarchy.IsHidden) {
              return true;
            }
          }

        }
      }
      return false;

    }

    public static TranslationsTable GetTranslationsTable(string targtLanguage = null) {

      TranslationSet translationSet = new TranslationSet {
        DefaultLangauge = SupportedLanguages.AllLanguages[model.Culture],
        SecondaryLanguages = new List<Language>()
      };

      if (targtLanguage != null) {
        // create table with a single secondary culture if targetLanguage is passed
        translationSet.SecondaryLanguages.Add(SupportedLanguages.AllLanguages[targtLanguage]);
      }
      else {
        // create table for all secondary cultures if no language is passed
        foreach (var culture in model.Cultures) {
          if (culture.Name != model.Culture) {
            translationSet.SecondaryLanguages.Add(SupportedLanguages.AllLanguages[culture.Name]);
          }
        }
      }

      int secondaryLanguageCount = translationSet.SecondaryLanguages.Count;
      int columnCount = 4 + secondaryLanguageCount;

      string[] Headers = new string[columnCount];

      // set column headers
      Headers[0] = "Object Type";
      Headers[1] = "Property";
      Headers[2] = "Name";
      Headers[3] = translationSet.DefaultLangauge.FullName;
      int index = 4;
      foreach (var language in translationSet.SecondaryLanguages) {
        Headers[index] = language.FullName;
        index += 1;
      }

      var defaultLanguage = model.Cultures[model.Culture];
      List<string[]> Rows = new List<string[]>();

      foreach (Table table in model.Tables) {
        // exclude hidden tables and system tables

        // track display folders for each table so they are only added oncce
        List<string> displayFoldersForTable = new List<string>();

        string tableName = table.Name.ToLower();
        if ((!table.IsHidden && !tableName.Contains("translated") && !tableName.Contains("translations")) || table.Name.Equals(LocalizedLabelsTableName)) {

          // do not include 'Localized Labels' table name but do include the measures inside
          if (!table.Name.Equals("Localized Labels")) {
            Rows.AddRange((GetTableRows(table, defaultLanguage, translationSet)));
          }

          foreach (Column column in table.Columns) {
            if (!column.IsHidden && !column.Name.ToLower().Contains("translation") && !table.Name.Equals("Localized Labels") && column.Name != table.Name) {
              Rows.AddRange(GetColumnRows(table, column, defaultLanguage, translationSet, displayFoldersForTable));
            }
          }

          foreach (Measure measure in table.Measures) {
            if (!measure.IsHidden || table.Name.Equals(LocalizedLabelsTableName)) {
              Rows.AddRange(GetMeasureRows(table, measure, defaultLanguage, translationSet, displayFoldersForTable));
            }
          }

          foreach (Hierarchy hierarchy in table.Hierarchies) {
            if (!hierarchy.IsHidden) {
              Rows.AddRange(GetHierarchyRows(table, hierarchy, defaultLanguage, translationSet, displayFoldersForTable));
            }
          }

        }
      }
      return new TranslationsTable {
        Headers = Headers,
        Rows = Rows
      };

    }

    public static List<string[]> GetTableRows(Table table, Culture defaultLanguage, TranslationSet translationSet) {

      List<string[]> rows = new List<string[]>();

      // add row for caption
      List<string> captionRowValues = new List<string> {
        "Table",
        "Caption",
        { table.Name },
        defaultLanguage.ObjectTranslations[table, TranslatedProperty.Caption]?.Value  ??  table.Name
      };

      foreach (var language in translationSet.SecondaryLanguages) {
        captionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[table, TranslatedProperty.Caption]?.Value);
      }
      rows.Add(captionRowValues.ToArray());

      if (!string.IsNullOrEmpty(table.Description)) {
        List<string> descriptionRowValues = new List<string> {
          "Table",
          "Description",
          table.Name,
          defaultLanguage.ObjectTranslations[table, TranslatedProperty.Description]?.Value ?? table.Name
        };
        foreach (var language in translationSet.SecondaryLanguages) {
          descriptionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[table, TranslatedProperty.Description]?.Value);
        }
        rows.Add(descriptionRowValues.ToArray());
      }

      return rows;
    }

    public static List<string[]> GetColumnRows(Table table, Column column, Culture defaultLanguage, TranslationSet translationSet, List<string> displayFoldersForTable) {

      List<string[]> rows = new List<string[]>();

      // always add row for caption
      List<string> captionRowValues = new List<string> {
        "Column",
        "Caption",
        $"{table.Name}[{column.Name}]",
        defaultLanguage.ObjectTranslations[column, TranslatedProperty.Caption]?.Value ?? column.Name
      };
      foreach (var language in translationSet.SecondaryLanguages) {
        captionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[column, TranslatedProperty.Caption]?.Value);
      }
      rows.Add(captionRowValues.ToArray());

      if (!string.IsNullOrEmpty(column.DisplayFolder) && !displayFoldersForTable.Contains(column.DisplayFolder)) {
        List<string> displayFolderRowValues = new List<string> { "Table", "DisplayFolder", $"{table.Name}[{column.Name}]{column.DisplayFolder}", defaultLanguage.ObjectTranslations[column, TranslatedProperty.DisplayFolder]?.Value };
        foreach (var language in translationSet.SecondaryLanguages) {
          displayFolderRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[column, TranslatedProperty.DisplayFolder]?.Value);
        }
        rows.Add(displayFolderRowValues.ToArray());
        displayFoldersForTable.Add(column.DisplayFolder);
      }

      if (!string.IsNullOrEmpty(column.Description)) {
        List<string> descriptionRowValues = new List<string> { "Column", "Description", $"{table.Name}[{column.Name}]", defaultLanguage.ObjectTranslations[column, TranslatedProperty.Description]?.Value };
        foreach (var language in translationSet.SecondaryLanguages) {
          descriptionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[column, TranslatedProperty.Description]?.Value);
        }
        rows.Add(descriptionRowValues.ToArray());
      }

      return rows;
    }

    public static List<string[]> GetMeasureRows(Table table, Measure measure, Culture defaultLanguage, TranslationSet translationSet, List<string> displayFoldersForTable) {

      List<string[]> rows = new List<string[]>();

      // always add row for caption
      List<string> captionRowValues = new List<string> {
        "Measure",
        "Caption",
        $"{table.Name}[{measure.Name}]",
        defaultLanguage.ObjectTranslations[measure, TranslatedProperty.Caption]?.Value ?? measure.Name
      };
      foreach (var language in translationSet.SecondaryLanguages) {
        captionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[measure, TranslatedProperty.Caption]?.Value);
      }
      rows.Add(captionRowValues.ToArray());

      if (!string.IsNullOrEmpty(measure.DisplayFolder) && !displayFoldersForTable.Contains(measure.DisplayFolder)) {
        List<string> displayFolderRowValues = new List<string> { "Table", "DisplayFolder", $"{table.Name}[{measure.DisplayFolder}]", defaultLanguage.ObjectTranslations[measure, TranslatedProperty.DisplayFolder]?.Value };
        foreach (var language in translationSet.SecondaryLanguages) {
          displayFolderRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[measure, TranslatedProperty.DisplayFolder]?.Value);
        }
        rows.Add(displayFolderRowValues.ToArray());
        displayFoldersForTable.Add(measure.DisplayFolder);
      }

      if (!string.IsNullOrEmpty(measure.Description)) {
        List<string> descriptionRowValues = new List<string> { "Measure", "Description", $"{table.Name}[{measure.Name}]{measure.Name}", defaultLanguage.ObjectTranslations[measure, TranslatedProperty.Description]?.Value };
        foreach (var language in translationSet.SecondaryLanguages) {
          descriptionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[measure, TranslatedProperty.Description]?.Value);
        }
        rows.Add(descriptionRowValues.ToArray());
      }

      return rows;
    }

    public static List<string[]> GetHierarchyRows(Table table, Hierarchy hierarchy, Culture defaultLanguage, TranslationSet translationSet, List<string> displayFoldersForTable) {

      List<string[]> rows = new List<string[]>();

      // always add row for caption
      List<string> captionRowValues = new List<string> {
        "Hierarchy",
        "Caption",
        $"{table.Name}[{hierarchy.Name}]",
        defaultLanguage.ObjectTranslations[hierarchy, TranslatedProperty.Caption]?.Value ?? hierarchy.Name
      };
      foreach (var language in translationSet.SecondaryLanguages) {
        captionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[hierarchy, TranslatedProperty.Caption]?.Value);
      }
      rows.Add(captionRowValues.ToArray());

      if (!string.IsNullOrEmpty(hierarchy.DisplayFolder) && !displayFoldersForTable.Contains(hierarchy.DisplayFolder)) {
        List<string> displayFolderRowValues = new List<string> {
          "Table",
          "DisplayFolder",
          $"{table.Name}[{hierarchy.DisplayFolder}]",
          defaultLanguage.ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder]?.Value ?? hierarchy.DisplayFolder
        };
        foreach (var language in translationSet.SecondaryLanguages) {
          displayFolderRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder]?.Value);
        }
        rows.Add(displayFolderRowValues.ToArray());
      }

      foreach (Level hierarchyLevel in hierarchy.Levels) {
        List<string> levelCaptionRowValues = new List<string> {
          "Level",
          "Caption",
          $"{table.Name}[{hierarchy.Name}]{hierarchyLevel.Name}",
          defaultLanguage.ObjectTranslations[hierarchyLevel, TranslatedProperty.Caption]?.Value ?? hierarchyLevel.Name
        };
        foreach (var language in translationSet.SecondaryLanguages) {
          levelCaptionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[hierarchyLevel, TranslatedProperty.Caption]?.Value);
        }
        rows.Add(levelCaptionRowValues.ToArray());
      }



      if (!string.IsNullOrEmpty(hierarchy.Description)) {
        List<string> descriptionRowValues = new List<string> { "Hierarchy", "Description", $"{table.Name}[{hierarchy.Name}]", defaultLanguage.ObjectTranslations[hierarchy, TranslatedProperty.Description]?.Value };
        foreach (var language in translationSet.SecondaryLanguages) {
          descriptionRowValues.Add(model.Cultures[language.LanguageId].ObjectTranslations[hierarchy, TranslatedProperty.Description]?.Value);
        }
        rows.Add(descriptionRowValues.ToArray());
      }

      return rows;
    }

    public static List<string> GetSecondaryLanguagesInDataModel() {
      List<string> secondaryCultures = new List<string>();
      foreach (var culture in model.Cultures) {
        if (!culture.Name.Equals(model.Culture)) {
          secondaryCultures.Add(culture.Name);
        }
      }
      return secondaryCultures;
    }

    public static List<string> GetSecondaryLanguageFullNamesInDataModel() {
      var secondaryCultures = new List<string>();
      foreach (var culture in model.Cultures) {
        if (!culture.Name.Equals(model.Culture)) {
          secondaryCultures.Add(SupportedLanguages.AllLanguages[culture.Name].FullName);
        }
      }
      return secondaryCultures;
    }

    public static List<string> GetSecondaryLanguagesAvailable() {
      var secondaryCultures = new List<string>();
      foreach (var language in SupportedLanguages.AllLanguages) {
        if (!language.Value.LanguageId.Equals(model.Culture)) {
          secondaryCultures.Add(language.Value.LanguageId);
        }
      }
      return secondaryCultures;
    }

    public static void PopulateCultureWithMachineTranslations(string CultureName, IStatusCalback StatusCalback = null) {

      // add culture to data model if it doesn't already exist
      if (!model.Cultures.ContainsName(CultureName)) {
        model.Cultures.Add(new Culture { Name = CultureName });
      }

      // load culture metadata object
      Culture culture = model.Cultures[CultureName];

      // enumerate through tables
      foreach (Table table in model.Tables) {

        string tableName = table.Name.ToLower();
        if ((!table.IsHidden && !tableName.Contains("translated") && !tableName.Contains("translations")) || table.Name.Equals(LocalizedLabelsTableName)) {

          // set Caption translation for table
          var defaultTranslation = GetDefaultTranslation(table.Name, "Table", "Caption");
          var translatedTableName = TranslateContent(defaultTranslation, CultureName);
          culture.ObjectTranslations.SetTranslation(table, TranslatedProperty.Caption, translatedTableName);
          UpdateStatus(StatusCalback, CultureName, table.Name, table.Name, translatedTableName);

          // set Description translation for table
          if (!string.IsNullOrEmpty(table.Description)) {
            var translatedTableDescription = TranslateContent(table.Description, CultureName);
            culture.ObjectTranslations.SetTranslation(table, TranslatedProperty.Description, translatedTableDescription);
            UpdateStatus(StatusCalback, CultureName, table.Name, table.Description, translatedTableDescription);
          }

          // enumerate through columns
          foreach (Column column in table.Columns) {
            if (!column.IsHidden && !column.Name.ToLower().Contains("translation") && !table.Name.Equals("Localized Labels")) {

              // set Caption translation for column
              var translatedColumnName = TranslateContent(column.Name, CultureName);
              culture.ObjectTranslations.SetTranslation(column, TranslatedProperty.Caption, translatedColumnName);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{column.Name}]", column.Name, translatedColumnName);

              // set DisplayFolder translation for column
              if (!string.IsNullOrEmpty(column.DisplayFolder)) {
                var translatedColumnDisplayFolder = TranslateContent(column.DisplayFolder, CultureName);
                culture.ObjectTranslations.SetTranslation(column, TranslatedProperty.DisplayFolder, translatedColumnDisplayFolder);
                UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{column.Name}]", column.DisplayFolder, translatedColumnDisplayFolder);
              }

              // set Description translation for column
              if (!string.IsNullOrEmpty(column.Description)) {
                var translatedColumnDescription = TranslateContent(column.Description, CultureName);
                culture.ObjectTranslations.SetTranslation(column, TranslatedProperty.Description, translatedColumnDescription);
                UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{column.Name}]", column.Description, translatedColumnDescription);
              }

            }
          };

          // enumerate through measures
          foreach (Measure measure in table.Measures) {

            // set Caption translation for measure
            var translatedMeasureName = TranslateContent(measure.Name, CultureName);
            culture.ObjectTranslations.SetTranslation(measure, TranslatedProperty.Caption, translatedMeasureName);
            UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{measure.Name}]", measure.Name, translatedMeasureName);

            // set DisplayFolder translation for measure
            if (!string.IsNullOrEmpty(measure.DisplayFolder)) {
              var translatedMeasureDisplayFolder = TranslateContent(measure.DisplayFolder, CultureName);
              culture.ObjectTranslations.SetTranslation(measure, TranslatedProperty.DisplayFolder, translatedMeasureDisplayFolder);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{measure.Name}]", measure.DisplayFolder, translatedMeasureDisplayFolder);
            }

            // set Description translation for measure
            if (!string.IsNullOrEmpty(measure.Description)) {
              var translatedMeasureDescription = TranslateContent(measure.Description, CultureName);
              culture.ObjectTranslations.SetTranslation(measure, TranslatedProperty.Description, translatedMeasureDescription);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{measure.Name}]", measure.Description, translatedMeasureDescription);
            }

          };

          // enumerate through hierarchies
          foreach (Hierarchy hierarchy in table.Hierarchies) {

            // set Caption translation for hierachy
            var translatedHierarchyName = TranslateContent(hierarchy.Name, CultureName);
            culture.ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.Caption, translatedHierarchyName);
            UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]", hierarchy.Name, translatedHierarchyName);

            // translate colum names for hierachy levels
            foreach (var level in hierarchy.Levels) {
              var translatedLevelName = TranslateContent(level.Name, CultureName);
              culture.ObjectTranslations.SetTranslation(level, TranslatedProperty.Caption, translatedLevelName);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]{translatedLevelName}", hierarchy.Name, translatedLevelName);
            }

            // set DisplayFolder translation for hierarchy
            if (!string.IsNullOrEmpty(hierarchy.DisplayFolder)) {
              var translatedHierarchyDisplayFolder = TranslateContent(hierarchy.DisplayFolder, CultureName);
              culture.ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.DisplayFolder, translatedHierarchyDisplayFolder);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]", hierarchy.DisplayFolder, translatedHierarchyDisplayFolder);
            }

            // set Description translation for measure
            if (!string.IsNullOrEmpty(hierarchy.Description)) {
              var translatedHierachyDescription = TranslateContent(hierarchy.Description, CultureName);
              culture.ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.Description, translatedHierachyDescription);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]", hierarchy.Description, translatedHierachyDescription);
            }

          };
        }
      }

      model.SaveChanges();
    }

    public static void PopulateDatasetObjectWithMachineTranslations(string ObjectType, string PropertyName, string ObjectName, IStatusCalback StatusCalback = null) {

      MetadataObject DatasetObject = GetMetadataObject(ObjectType, PropertyName, ObjectName);
      var DefaultTranslation = GetDefaultTranslation(ObjectName, ObjectType, PropertyName);

      SetDatasetObjectTranslation(ObjectType, PropertyName, ObjectName, model.Culture, DefaultTranslation);
      UpdateStatus(StatusCalback, model.Culture, ObjectName, DefaultTranslation, DefaultTranslation);


      List<string> secondaryLanguagesList = new List<string>();
      foreach (var currentCulture in model.Cultures) {
        if (currentCulture.Name != model.Culture) {
          secondaryLanguagesList.Add(currentCulture.Name);
        }
      }
      string[] secondaryLanguages = secondaryLanguagesList.ToArray();

      foreach (string language in secondaryLanguages) {
        var translation = TranslateContent(DefaultTranslation, language);
        Culture culture = model.Cultures[language];
        SetDatasetObjectTranslation(ObjectType, PropertyName, ObjectName, language, translation);
        UpdateStatus(StatusCalback, language, ObjectName, DefaultTranslation, translation);
      }

    }

    public static void PopulateEmptyTranslations(string CultureName, IStatusCalback StatusCalback = null) {

      // add culture to data model if it doesn't already exist
      if (!model.Cultures.ContainsName(CultureName)) {
        model.Cultures.Add(new Culture { Name = CultureName });
      }

      // load culture metadata object
      Culture culture = model.Cultures[CultureName];

      // enumerate through tables
      foreach (Table table in model.Tables) {

        string tableName = table.Name.ToLower();
        if ((!table.IsHidden && !tableName.Contains("translated") && !tableName.Contains("translations")) || table.Name.Equals(LocalizedLabelsTableName)) {

          if (culture.ObjectTranslations[table, TranslatedProperty.Caption]?.Value == null) {
            // set Caption translation for table
            var translatedTableName = TranslateContent(table.Name, CultureName);
            culture.ObjectTranslations.SetTranslation(table, TranslatedProperty.Caption, translatedTableName);
            UpdateStatus(StatusCalback, CultureName, table.Name, table.Name, translatedTableName);

          }

          // enumerate through columns
          foreach (Column column in table.Columns) {
            if (!column.IsHidden && !column.Name.ToLower().Contains("translation") && !table.Name.Equals("Localized Labels")) {

              if (culture.ObjectTranslations[column, TranslatedProperty.Caption]?.Value == null) {
                // set Caption translation for column
                var translatedColumnName = TranslateContent(column.Name, CultureName);
                culture.ObjectTranslations.SetTranslation(column, TranslatedProperty.Caption, translatedColumnName);
                UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{column.Name}]", column.Name, translatedColumnName);

              }

              // set DisplayFolder translation for column
              if (!string.IsNullOrEmpty(column.DisplayFolder)) {
                if (culture.ObjectTranslations[column, TranslatedProperty.DisplayFolder]?.Value == null) {
                  var translatedColumnDisplayFolder = TranslateContent(column.DisplayFolder, CultureName);
                  culture.ObjectTranslations.SetTranslation(column, TranslatedProperty.DisplayFolder, translatedColumnDisplayFolder);
                  UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{column.Name}]", column.DisplayFolder, translatedColumnDisplayFolder);
                }
              }

            }
          };

          // enumerate through measures
          foreach (Measure measure in table.Measures) {

            if (culture.ObjectTranslations[measure, TranslatedProperty.Caption]?.Value == null) {
              // set Caption translation for measure
              var translatedMeasureName = TranslateContent(measure.Name, CultureName);
              culture.ObjectTranslations.SetTranslation(measure, TranslatedProperty.Caption, translatedMeasureName);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{measure.Name}]", measure.Name, translatedMeasureName);
            }

            // set DisplayFolder translation for measure
            if (!string.IsNullOrEmpty(measure.DisplayFolder)) {
              if (culture.ObjectTranslations[measure, TranslatedProperty.DisplayFolder]?.Value == null) {
                var translatedMeasureDisplayFolder = TranslateContent(measure.DisplayFolder, CultureName);
                culture.ObjectTranslations.SetTranslation(measure, TranslatedProperty.DisplayFolder, translatedMeasureDisplayFolder);
                UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{measure.Name}]", measure.DisplayFolder, translatedMeasureDisplayFolder);
              }
            }

          };

          // enumerate through hierarchies
          foreach (Hierarchy hierarchy in table.Hierarchies) {

            if (culture.ObjectTranslations[hierarchy, TranslatedProperty.Caption]?.Value == null) {
              // set Caption translation for hierachy
              var translatedHierarchyName = TranslateContent(hierarchy.Name, CultureName);
              culture.ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.Caption, translatedHierarchyName);
              UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]", hierarchy.Name, translatedHierarchyName);
            }

            // translate colum names for hierachy levels
            foreach (var level in hierarchy.Levels) {
              if (culture.ObjectTranslations[level, TranslatedProperty.Caption]?.Value == null) {
                var translatedLevelName = TranslateContent(level.Name, CultureName);
                culture.ObjectTranslations.SetTranslation(level, TranslatedProperty.Caption, translatedLevelName);
                UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]{translatedLevelName}", hierarchy.Name, translatedLevelName);
              }
            }

            // set DisplayFolder translation for hierarchy
            if (!string.IsNullOrEmpty(hierarchy.DisplayFolder)) {
              if (culture.ObjectTranslations[hierarchy, TranslatedProperty.DisplayFolder]?.Value == null) {
                var translatedHierarchyDisplayFolder = TranslateContent(hierarchy.DisplayFolder, CultureName);
                culture.ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.DisplayFolder, translatedHierarchyDisplayFolder);
                UpdateStatus(StatusCalback, CultureName, $"{table.Name}[{hierarchy.Name}]", hierarchy.DisplayFolder, translatedHierarchyDisplayFolder);
              }
            }


          };
        }
      }

      model.SaveChanges();


    }

    static string TranslateContent(string Content, string ToCultureName) {
      return TranslatorService.TranslateContent(Content, ToCultureName);
    }

    private static void UpdateStatus(IStatusCalback StatusCalback, string CultureName, string ObjectName, string OriginalText, string TranslatedText) {
      if (StatusCalback != null) {
        string TranslationType = "[" + model.Culture + "] => [" + CultureName + "]";
        StatusCalback.updateLoadingStatus(TranslationType, ObjectName, OriginalText, TranslatedText);
      }
    }

    public static void ExportModelAsBim() {

      DirectoryInfo path = Directory.CreateDirectory(AppSettings.TranslationsOutboxFolderPath);

      string filePath = path + @"/" + DatasetName + ".model.bim";

      string fileContent = JsonSerializer.SerializeDatabase(database, new SerializeOptions {
        IgnoreTimestamps = true,
        IgnoreInferredProperties = true,
        IgnoreInferredObjects = true,
      });

      StreamWriter writer = new StreamWriter(File.Open(filePath, FileMode.Create), Encoding.UTF8);
      writer.Write(fileContent);
      writer.Flush();
      writer.Dispose();

      ProcessStartInfo startInfo = new ProcessStartInfo();
      startInfo.FileName = "Notepad.exe";
      startInfo.Arguments = filePath;
      Process.Start(startInfo);


    }

    public static void ExportTranslations(string targetLanguage = null, bool OpenInExcel = true) {

      try {
        var translationsTable = GetTranslationsTable(targetLanguage);

        DirectoryInfo path = Directory.CreateDirectory(AppSettings.TranslationsOutboxFolderPath);

        string filePath;
        if (targetLanguage == null) {
          filePath = path + @"/" + DatasetName + "-Translations-Master.csv";
        }
        else {
          string targetLanguageDisplayName = SupportedLanguages.AllLanguages[targetLanguage].DisplayName;
          filePath = path + @"/" + DatasetName + "-Translations-" + targetLanguageDisplayName + ".csv";
        }

        StreamWriter writer = new StreamWriter(File.Open(filePath, FileMode.Create), Encoding.UTF8);
        var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
        foreach (var headerField in translationsTable.Headers)
        {
          csvWriter.WriteField(headerField);
        }
        csvWriter.NextRecord();
        foreach (var row in translationsTable.Rows)
        {
          foreach (var rowField in row)
          {
            csvWriter.WriteField(rowField);
          }
          csvWriter.NextRecord();
        }
        csvWriter.Flush();
        csvWriter.Dispose();

        //StreamWriter writerJson = new StreamWriter(File.Open(filePath.Replace(".csv", ".json"), FileMode.Create), Encoding.UTF8);
        //writerJson.Write(System.Text.Json.JsonSerializer.Serialize(translationsTable));
        //writerJson.Flush();
        //writerJson.Dispose();

        if (OpenInExcel) {
          string excelFilePath = @"""" + filePath + @"""";
          ExcelUtilities.OpenCsvInExcel(excelFilePath);
        }

      }

      catch (Exception ex) {
        UserInteraction.PromptUserWithError(ex);
      }


    }

    public static void ExportAllTranslationSheets(bool OpenInExcel = false) {

      try {
        foreach (var culture in model.Cultures) {
          if (culture.Name != model.Culture) {

            string targetLanguage = culture.Name;
            var translationsTable = GetTranslationsTable(targetLanguage);

            DirectoryInfo path = Directory.CreateDirectory(AppSettings.TranslationsOutboxFolderPath);

            string targetLanguageDisplayName = SupportedLanguages.AllLanguages[targetLanguage].DisplayName;
            string filePath = path + @"/" + DatasetName + "-Translations-" + targetLanguageDisplayName + ".csv";

            StreamWriter writer = new StreamWriter(File.Open(filePath, FileMode.Create), Encoding.UTF8);

            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            foreach (var headerField in translationsTable.Headers)
            {
              csvWriter.WriteField(headerField);
            }
            csvWriter.NextRecord();
            foreach (var row in translationsTable.Rows)
            {
              foreach(var rowField in row)
              {
                csvWriter.WriteField(rowField);
              }
              csvWriter.NextRecord();
            }
            csvWriter.Flush();
            csvWriter.Dispose();

            if (OpenInExcel) {
              string excelFilePath = @"""" + filePath + @"""";
              ExcelUtilities.OpenCsvInExcel(excelFilePath);
            }

          }
        }

      }
      catch (Exception ex) {
        UserInteraction.PromptUserWithError(ex);
      }

    }

    private static string GetTableName(string ObjectName) {
      return ObjectName.Substring(0, ObjectName.IndexOf("["));
    }

    private static string GetChildName(string ObjectName) {
      return ObjectName.Substring(ObjectName.IndexOf("[") + 1, (ObjectName.IndexOf("]") - ObjectName.IndexOf("[") - 1));
    }

    private static string GetLevelName(string ObjectName) {
      return ObjectName.Substring(ObjectName.IndexOf("]") + 1, ObjectName.Length - ObjectName.IndexOf("]") - 1);
    }

    public static MetadataObject GetMetadataObject(string ObjectType, string PropertyName, string ObjectName) {

      // special handling for table folder for table which must be propagated to 
      // display folder translations for child columns, measures and hierchies
      if (ObjectType.Equals("Table") && PropertyName.Equals("DisplayFolder")) { return null; }

      if (ObjectType.Equals("Table") && PropertyName.Equals("Caption")) {
        return model.Tables[ObjectName];
      }
      else {
        string tableName = GetTableName(ObjectName);
        string childName = GetChildName(ObjectName);
        Table table = model.Tables[tableName];
        switch (ObjectType) {
          case "Column":
            return table.Columns[childName];
          case "Measure":
            return table.Measures[childName];
          case "Hierarchy":
            return table.Hierarchies[childName];
          case "Level":
            string levelName = GetLevelName(ObjectName);
            return table.Hierarchies[childName].Levels.Find(levelName);
          default:
            throw new ApplicationException("Unknown tabular object type - " + ObjectType);
        }
      }
    }

    public static bool EnsureMetadataObjectExists(string ObjectType, string ObjectName) {

      if (ObjectType.Equals("Table")) {
        return model.Tables.Contains(ObjectName);
      }
      else {
        string tableName = GetTableName(ObjectName);
        bool tableExists = model.Tables.Contains(tableName);

        // create Localized Labels table if it does not exist
        bool shouldCreateLocalizedLabelsTable = !tableExists && tableName.Equals(LocalizedLabelsTableName);
        if (shouldCreateLocalizedLabelsTable) {
          CreateLocalizedLabelsTable(false);
          tableExists = true;
        }


        if (tableExists) {
          string childName = GetChildName(ObjectName);
          Table table = model.Tables[tableName];
          switch (ObjectType) {
            case "Column":
              return table.Columns.Contains(childName);
            case "Measure":
              bool measureExists = table.Measures.Contains(childName);
              if (!measureExists && tableName.Equals(LocalizedLabelsTableName)) {
                AddLocalizedLabel(childName, false);
                measureExists = true;
              }
              return measureExists;
            case "Hierarchy":
              return table.Hierarchies.Contains(childName);
            case "Level":
              string levelName = GetLevelName(ObjectName);
              return (table.Hierarchies[childName].Levels.Find(levelName) != null);
            default:
              return false;
          }
        }
        else {
          return false;
        }

      }
    }

    public static void SetDatasetObjectTranslation(string ObjectType, string PropertyName, string ObjectName, string TargetLanguage, string TranslatedValue) {

      MetadataObject targetObject = TranslationsManager.GetMetadataObject(ObjectType, PropertyName, ObjectName);

      switch (PropertyName) {

        case "Caption":
          model.Cultures[TargetLanguage].ObjectTranslations.SetTranslation(targetObject, TranslatedProperty.Caption, TranslatedValue);
          break;

        case "DisplayFolder":
          if (ObjectType.Equals("Table")) {
            UpdateDisplayFolderForTable(ObjectName, TargetLanguage, TranslatedValue);
          }
          break;

        case "Description":
          model.Cultures[TargetLanguage].ObjectTranslations.SetTranslation(targetObject, TranslatedProperty.Description, TranslatedValue);
          break;

      }

      model.SaveChanges();

    }

    public static void UpdateDisplayFolderForTable(string ObjectName, string TargetLanguage, string TranslatedValue) {

      string tableName = GetTableName(ObjectName);
      string folderName = GetChildName(ObjectName);


      Table table = model.Tables[tableName];

      foreach (Column column in table.Columns) {
        if (column.DisplayFolder.Equals(folderName)) {
          model.Cultures[TargetLanguage].ObjectTranslations.SetTranslation(column, TranslatedProperty.DisplayFolder, TranslatedValue);
        }
      }

      foreach (Measure measure in table.Measures) {
        if (measure.DisplayFolder.Equals(folderName)) {
          model.Cultures[TargetLanguage].ObjectTranslations.SetTranslation(measure, TranslatedProperty.DisplayFolder, TranslatedValue);
        }
      }

      foreach (Hierarchy hierarchy in table.Hierarchies) {
        if (hierarchy.DisplayFolder.Equals(folderName)) {
          model.Cultures[TargetLanguage].ObjectTranslations.SetTranslation(hierarchy, TranslatedProperty.DisplayFolder, TranslatedValue);
        }
      }

    }

    public static string GetDefaultTranslation(string FullObjectName, string ObjectType, string PropertyName) {

      // first check to see if default translation has customized value 
      var targetObject = GetMetadataObject(ObjectType, PropertyName, FullObjectName);

      TranslatedProperty PropertyType = TranslatedProperty.Caption;
      switch (PropertyName) {
        case "Description":
          PropertyType = TranslatedProperty.Description;
          break;
        case "DisplayFolder":
          PropertyType = TranslatedProperty.DisplayFolder;
          break;
      };

      var customizedDefaultTranslation = model.Cultures[model.Culture].ObjectTranslations[targetObject, PropertyType]?.Value;

      if (!string.IsNullOrEmpty(customizedDefaultTranslation)) {
        return customizedDefaultTranslation;
      }
      else {
        return GetDefaultTranslationFromObjectName(FullObjectName, ObjectType, PropertyName);
      }
    }

    public static string GetDefaultTranslationFromObjectName(string FullObjectName, string ObjectType, string PropertyName) {

      // get default translation from object name
      // special handling for DisplayFolder
      if (ObjectType.Equals("Table") && PropertyName.Equals("DisplayFolder")) {
        int startPosition = FullObjectName.IndexOf("[");
        return FullObjectName.Substring(startPosition + 1).Replace("]", "");
      }

      switch (ObjectType) {
        case "Table":
          return FullObjectName;
        case "Column":
        case "Measure":
        case "Hierarchy":
          int startPositionHierarchy = FullObjectName.IndexOf("[");
          return FullObjectName.Substring(startPositionHierarchy + 1).Replace("]", "");
        case "Level":
        case "DisplayFolder":
          int startPositionDisplayFolder = FullObjectName.IndexOf("]");
          return FullObjectName.Substring(startPositionDisplayFolder + 1);
      }
      throw new ApplicationException("Something bad happened in TranslationManager.GetObjectNameFromFullObjectName");
    }

    public static void ImportTranslations(string filePath) {

      try {

        string linebreak = "\r\n";

        FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        StreamReader reader = new StreamReader(stream);
        var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

        csvReader.Read();
        csvReader.ReadHeader();
        var headers = csvReader.HeaderRecord;

        var cultureCount = headers.Length - 3;
        var culturesList = new List<string>();

        for (int columnNumber = 3; (columnNumber < headers.Length); columnNumber++) {
          var language = SupportedLanguages.GetLanguageFromFullName(headers[columnNumber]);
          culturesList.Add(language.LanguageId);
        }

        foreach (string cultureName in culturesList) {
          if (!model.Cultures.Contains(cultureName)) {
            model.Cultures.Add(new Culture { Name = cultureName });
          }
        }

        while (csvReader.Read())
        {
          string objectType = csvReader.GetField(0);
          string propertyName = csvReader.GetField(1);
          string objectName = csvReader.GetField(2);
          for (int columnNumber = 3; columnNumber < csvReader.ColumnCount; columnNumber++)
          {
            string targetLanguage = SupportedLanguages.GetLanguageFromFullName(headers[columnNumber]).LanguageId;
            string translatedValue = csvReader.GetField(columnNumber);
            if (!string.IsNullOrEmpty(translatedValue) && TranslationsManager.EnsureMetadataObjectExists(objectType, objectName))
            {
              TranslationsManager.SetDatasetObjectTranslation(objectType, propertyName, objectName, targetLanguage, translatedValue);
            }
          }
        }

        reader.Close();
        stream.Close();

      }
      catch (Exception ex) {
        UserInteraction.PromptUserWithError(ex);
      }

    }

    public static void ImportLocalizedLabels(string filePath) {

      string linebreak = "\r\n";

      FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
      StreamReader reader = new StreamReader(stream);
      string[] labels = reader.ReadToEnd().Trim().Split(linebreak);

      foreach (string label in labels) {
        AddLocalizedLabel(label, false);
      }

      // close file and release resources
      reader.Close();
      stream.Close();
    }

    public static Table CreateEmptyTable(string TableName) {

      if (model.Tables.Find(TableName) != null) {
        model.Tables.Remove(TableName);
        model.SaveChanges();
      }

      string EmptyTableDaxExpression = @"DATATABLE(""unused"", STRING, {{""This is a table automatically generated by Translations Builder""}})";

      Table table = new Table {
        Name = TableName,
        Partitions = {
          new Partition {
            Source = new CalculatedPartitionSource {
              Expression = EmptyTableDaxExpression
            }
          }
        },
        Columns = {
          new DataColumn() { Name = "unused", DataType = DataType.String, SourceColumn = "unused" }
        }
      };

      model.Tables.Add(table);
      model.RequestRefresh(RefreshType.Calculate);
      model.SaveChanges();

      model.Tables[TableName].Columns["unused"].DisplayFolder = "unused";
      model.Tables[TableName].Columns["unused"].IsHidden = true;

      model.SaveChanges();

      return model.Tables[TableName];

    }

    public static void GenerateTranslatedDatasetObjectsTable() {

      Table translatedDatasetObjectsTable = CreateEmptyTable(TranslatedDatasetObjectsTableName);

      string defaultLanguageId = model.Culture;
      Culture defaultLanguage = model.Cultures[model.Culture];

      List<string> secondaryLanguagesList = new List<string>();

      foreach (var language in model.Cultures) {
        if (language.Name != model.Culture) {
          secondaryLanguagesList.Add(language.Name);
        }
      }

      string[] secondaryLanguages = secondaryLanguagesList.ToArray();

      foreach (Table table in model.Tables) {

        string tableName = table.Name.ToLower();
        if (!table.IsHidden && !tableName.Contains("translated") && !tableName.Contains("translations")) {

          // create measure for table
          string defaultranslation = table.Name;
          List<(string language, string value)> secondaryLanguageTranslations = new List<(string, string)>();
          foreach (string language in secondaryLanguages) {
            Culture culture = model.Cultures[language];
            string translation = culture.ObjectTranslations[table, TranslatedProperty.Caption]?.Value;
            if (!string.IsNullOrEmpty(translation)) {
              secondaryLanguageTranslations.Add((language, translation));
            }
          }

          translatedDatasetObjectsTable.Measures.Add(new Measure {
            Name = table.Name + " Table Name",
            Expression = GetReportLabelStaticMeasureDax(defaultranslation, secondaryLanguageTranslations),
          });


          foreach (Column column in table.Columns) {
            if (!column.IsHidden && !column.Name.ToLower().Contains("translation")) {

              // create measure for column
              defaultranslation = column.Name;
              secondaryLanguageTranslations = new List<(string, string)>();
              foreach (string language in secondaryLanguages) {
                Culture culture = model.Cultures[language];
                string translation = culture.ObjectTranslations[column, TranslatedProperty.Caption]?.Value;
                if (!string.IsNullOrEmpty(translation)) {
                  secondaryLanguageTranslations.Add((language, translation));
                }
              }

              translatedDatasetObjectsTable.Measures.Add(new Measure {
                Name = table.Name + ":" + column.Name + " Column Name",
                Expression = GetReportLabelStaticMeasureDax(defaultranslation, secondaryLanguageTranslations),
              });

            }
          }

          foreach (Measure measure in table.Measures) {
            if (!measure.IsHidden) {

              // create measure for measure
              defaultranslation = measure.Name;
              secondaryLanguageTranslations = new List<(string, string)>();
              foreach (string language in secondaryLanguages) {
                Culture culture = model.Cultures[language];
                string translation = culture.ObjectTranslations[measure, TranslatedProperty.Caption]?.Value;
                if (!string.IsNullOrEmpty(translation)) {
                  secondaryLanguageTranslations.Add((language, translation));
                }
              }

              translatedDatasetObjectsTable.Measures.Add(new Measure {
                Name = table.Name + ":" + measure.Name + " Measure Name",
                Expression = GetReportLabelStaticMeasureDax(defaultranslation, secondaryLanguageTranslations),
              });
            }
          }

          foreach (Hierarchy hierarchy in table.Hierarchies) {

            // create measure for hierarchy
            defaultranslation = hierarchy.Name;
            secondaryLanguageTranslations = new List<(string, string)>();
            foreach (string language in secondaryLanguages) {
              Culture culture = model.Cultures[language];
              string translation = culture.ObjectTranslations[hierarchy, TranslatedProperty.Caption]?.Value;
              if (!string.IsNullOrEmpty(translation)) {
                secondaryLanguageTranslations.Add((language, translation));
              }
            }

            translatedDatasetObjectsTable.Measures.Add(new Measure {
              Name = table.Name + ":" + hierarchy.Name + " Hierarchy Name",
              Expression = GetReportLabelStaticMeasureDax(defaultranslation, secondaryLanguageTranslations),
            });

            foreach (Level level in hierarchy.Levels) {
              // create measure for hierarchy
              defaultranslation = level.Name;
              secondaryLanguageTranslations = new List<(string, string)>();
              foreach (string language in secondaryLanguages) {
                Culture culture = model.Cultures[language];
                string translation = culture.ObjectTranslations[level, TranslatedProperty.Caption]?.Value;
                if (!string.IsNullOrEmpty(translation)) {
                  secondaryLanguageTranslations.Add((language, translation));
                }
              }

              translatedDatasetObjectsTable.Measures.Add(new Measure {
                Name = table.Name + ":" + hierarchy.Name + ":" + level.Name + " Level Name",
                Expression = GetReportLabelStaticMeasureDax(defaultranslation, secondaryLanguageTranslations),
              });

            }

          }

        }
      }

      model.SaveChanges();

    }

    public static string GetReportLabelStaticMeasureDax(string defaultTranslation, List<(string language, string value)> secondaryTranslations) {

      if (secondaryTranslations.Count == 0) {
        return @"""" + defaultTranslation + @"""";
      }

      string lineBreak = "\r\n";
      string measureExpression = "SWITCH(USERCULTURE()," + lineBreak;

      foreach (var translation in secondaryTranslations) {
        measureExpression += @"   """ + translation.language + @""", """ + translation.value + @"""," + lineBreak;
      }

      measureExpression += @"   """ + defaultTranslation + @"""" + lineBreak;
      measureExpression += ")";
      return measureExpression;

    }

    public static string GetReportLabelTableMeasureDax(string tableName, string labelName) {

      string lineBreak = "\r\n";
      string measureExpression = "LOOKUPVALUE(" + lineBreak +
                                 "  '" + tableName + "'[Value]," + lineBreak +
                                 "  '" + tableName + "'[Language]," + lineBreak +
                                 "   LEFT(USERCULTURE(), 2)," + lineBreak +
                                 "  '" + tableName + "'[Label]," + lineBreak +
                                @"  ""{0}"", " + lineBreak +
                                 "  LOOKUPVALUE(" + lineBreak +
                                 "    '" + tableName + "'[Value]," + lineBreak +
                                 "    '" + tableName + "'[Language]," + lineBreak +
                                @"    ""en""," + lineBreak +
                                 "    '" + tableName + "'[Label]," + lineBreak +
                                @"    ""{0}""" + lineBreak +
                                 "  )" + lineBreak +
                                 ")";


      return string.Format(measureExpression, labelName);
    }

    public static bool DoesTableExistInModel(string TableName) {
      return model.Tables.Find(TableName) != null;
    }

    public static void CreateLocalizedLabelsTable(bool AddSampleLabels = true) {

      Table localizedLabelsTable = CreateEmptyTable(LocalizedLabelsTableName);

      localizedLabelsTable.IsHidden = true;
      model.SaveChanges();

      if (AddSampleLabels) {
        AddLocalizedLabel("My Report Title");
        AddLocalizedLabel("My Button Caption");
        AddLocalizedLabel("My Visual Title");
      }

    }

    public static void AddLocalizedLabel(string Label, bool NotifyOnDuplicateError = true) {

      Table localizedLabelsTable = model.Tables[LocalizedLabelsTableName];

      if (localizedLabelsTable.Measures.Contains(Label)) {
        if (NotifyOnDuplicateError) {
          UserInteraction.PromptUserWithError("The label '" + Label + "' is already in the Localized Label table.");
        }
        return;
      }

      model.Tables[LocalizedLabelsTableName].Measures.Add(new Measure {
        Name = Label,
        Expression = "0",
        IsHidden = true
      });

      model.SaveChanges();

      // set translation text for default culture
      Measure measure = model.Tables[LocalizedLabelsTableName].Measures[Label];
      model.Cultures[model.Culture].ObjectTranslations.SetTranslation(measure, TranslatedProperty.Caption, Label);
      model.SaveChanges();

    }

    public static void DeleteAllLocalizedLabels() {
      Table localizedLabelsTable = model.Tables[LocalizedLabelsTableName];
      foreach (var measure in localizedLabelsTable.Measures) {
        foreach (Culture culture in model.Cultures) {
          ObjectTranslationCollection defaultLanguageTranslations = culture.ObjectTranslations;
          var defaultLanguageTranslation = defaultLanguageTranslations[measure, TranslatedProperty.Caption];
          defaultLanguageTranslations.Remove(defaultLanguageTranslation);
        }
        localizedLabelsTable.Measures.Remove(measure);
        model.SaveChanges();
      }

    }

    public static void DeleteLocalizedLabel(string LabelName) {
      Table localizedLabelsTable = model.Tables[LocalizedLabelsTableName];
      Measure measure = localizedLabelsTable.Measures[LabelName];
      foreach (Culture culture in model.Cultures) {
        ObjectTranslationCollection defaultLanguageTranslations = culture.ObjectTranslations;
        var defaultLanguageTranslation = defaultLanguageTranslations[measure, TranslatedProperty.Caption];
        defaultLanguageTranslations.Remove(defaultLanguageTranslation);
      }
      localizedLabelsTable.Measures.Remove(measure);
      model.SaveChanges();
    }

    public static void UpdateLocalizedLabel(string CurrentLabelName, string NewLabelName) {

      Table localizedLabelsTable = model.Tables[LocalizedLabelsTableName];
      Measure measure = localizedLabelsTable.Measures[CurrentLabelName];
      measure.Name = NewLabelName;
      model.SaveChanges();

      // set translation text for default culture
      model.Cultures[model.Culture].ObjectTranslations.SetTranslation(measure, TranslatedProperty.Caption, NewLabelName);
      model.SaveChanges();

    }

    public static void GenerateTranslatedLocalizedLabelsTable() {

      Table translatedLocalizedLabelsTable = CreateEmptyTable(TranslatedLocalizedLabelsTableName);

      Culture defaultLanguage = model.Cultures[model.Culture];

      Table localizedLabelsTable = model.Tables[LocalizedLabelsTableName];

      List<string> secondaryLanguagesList = new List<string>();

      foreach (var currentCulture in model.Cultures) {
        if (currentCulture.Name != model.Culture) {
          secondaryLanguagesList.Add(currentCulture.Name);
        }
      }

      string[] secondaryLanguages = secondaryLanguagesList.ToArray();

      foreach (Measure measure in localizedLabelsTable.Measures) {

        string defaultranslation = defaultLanguage.ObjectTranslations[measure, TranslatedProperty.Caption]?.Value;

        List<(string language, string value)> secondaryLanguageTranslations = new List<(string, string)>();

        foreach (var language in secondaryLanguages) {
          var translation = model.Cultures[language].ObjectTranslations[measure, TranslatedProperty.Caption]?.Value;
          if (!string.IsNullOrEmpty(translation)) {
            secondaryLanguageTranslations.Add((language: language, translation));
          }
        }

        Measure newMeasure = new Measure {
          Name = measure.Name + " Label",
          Expression = GetReportLabelStaticMeasureDax(defaultranslation, secondaryLanguageTranslations)
        };
        translatedLocalizedLabelsTable.Measures.Add(newMeasure);

      }

      model.SaveChanges();

    }

    public static void DeleteSecondaryLanguage(string Language) {
      model.Cultures.Remove(Language);
      model.SaveChanges();
    }

    public static void DuplicateLanguage(Culture SourceLanguage, string TargetLanguageIdentifier) {
      Culture targetLanguage = SourceLanguage.Clone();
      targetLanguage.RequestRename(TargetLanguageIdentifier);
      model.Cultures.Add(targetLanguage);
      model.SaveChanges();
    }

    public static void CopyTranslationsToExisitngLanguage(string SourceLanguageId, string TargetLanguageId) {      
      Culture sourceLanguage = model.Cultures[SourceLanguageId];
      Culture targetLanguage = model.Cultures[TargetLanguageId];
      model.Cultures.Remove(targetLanguage);
      model.SaveChanges();
      DuplicateLanguage(sourceLanguage, TargetLanguageId);
    }
 
  }

}

