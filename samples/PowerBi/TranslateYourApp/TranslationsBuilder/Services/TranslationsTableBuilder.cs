using System;
using System.Collections.Generic;
using TranslationsBuilder.Models;

namespace TranslationsBuilder.Services {

  class TranslationsTableBuilder {

    const string lineBreak = "\r\n";

    const string TableHeader = @"DATATABLE (""Label"", STRING, ""Language"", STRING, ""Value"", STRING, {" + lineBreak;
    const string TableFooter = "})";

    private static List<TranslationsTableItem> Translations = new List<TranslationsTableItem>();

    public static void AddTranslations(string Label, string Language, string Value, bool ClearExistingItems = false) {
      if (ClearExistingItems) { Translations.Clear(); }
      Translations.Add(new TranslationsTableItem { Label = Label, Language = Language, Value = Value });
    }

    public static string GetTranslationItems() {
      string items = "";
      bool firstOne = true;
      foreach (TranslationsTableItem item in Translations) {
        items += (firstOne ? "" : ",\r\n") + item.ToString();
        firstOne = false;
      }
      return items;
    }

    public static string GetTranslationsTableDax() {
      return TableHeader + GetTranslationItems() + lineBreak + TableFooter;
    }
  }

}
