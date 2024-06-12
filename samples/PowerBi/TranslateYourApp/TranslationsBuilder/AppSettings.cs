using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationsBuilder.Properties;
using TranslationsBuilder.Services;

namespace TranslationsBuilder {

  class AppSettings {

    private static Settings settings = new Settings();

    public static string Server {
      get { return settings.Server; }
      set {
        settings.Server = value;
        settings.Save();
      }
    }

    public static string Database {
      get { return settings.Database; }
      set {
        settings.Database = value;
        settings.Save();
      }
    }
  
    public static string TranslationsOutboxFolderPath {
      get {
        if (string.IsNullOrEmpty(settings.TranslationsOutboxFolderPath)) {
          return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        else {
          return settings.TranslationsOutboxFolderPath;
        }
      }
      set {
        settings.TranslationsOutboxFolderPath = value;
        settings.Save();
      }
    }

    public static string TranslationsInboxFolderPath {
      get {

        if (string.IsNullOrEmpty(settings.TranslationsInboxFolderPath)) {
          return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }
        else {
          return settings.TranslationsInboxFolderPath;
        }
      }
      set {
        settings.TranslationsInboxFolderPath = value;
        settings.Save();
      }
    }

    public static string AzureTranslatorServiceKey {
      get { return settings.AzureTranslatorServiceKey; }
      set {
        settings.AzureTranslatorServiceKey = value;
        settings.Save();
      }
    }

    public static string AzureTranslatorServiceLocation {
      get { return settings.AzureTranslatorServiceLocation; }
      set {
        settings.AzureTranslatorServiceLocation = value;
        settings.Save();
      }
    }

    public static void CheckForUpgrade() {
      if (settings.UpgradeSettings) {
        settings.Upgrade();
        settings.UpgradeSettings = false;
      }
    }

    public static void processStartupParameters(string[] args) {

      CheckForUpgrade();

      if (args.Length > 0) {
        if (args[0].ToLower().Contains("powerbi:")) {
          UserInteraction.PromptUserWithWarning("PBIX projects with Live Mode Datasets Not Supported *YET*.");
        }
        else {
          Server = args[0];
        }
        
      }
      if (args.Length > 1) {
        Database = args[1];
      }

      if (args.Length == 0 && string.IsNullOrEmpty(settings.Server)) {
        // start up without loading data model
      }

    }

  }
}

