using System;

namespace TranslationsBuilder.Models {

  class Language {
    public string LanguageId { get; set; }
    public string LanguageGroupId { get; set; }
    public string LanguageGroup { get; set; }
    public string DisplayName { get; set; }
    public string NativeName { get; set; }
    public bool ShownOnlyOnExtendedLanguageList { get; set; }
    public string FullName { get { 
      return LanguageGroup + " [" + LanguageId + "]";
      }
    }

    public string SelectionName {
      get {
        return DisplayName + " [" + LanguageId + "]";
      }
    }

    public override string ToString() {
      return DisplayName + " - [" + LanguageId + "]";
    }
  }

}
