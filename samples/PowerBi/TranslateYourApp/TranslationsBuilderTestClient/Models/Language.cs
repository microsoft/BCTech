
namespace TranslationsBuilderTestClient.Models {

  public class Language {
    public string LanguageTag { get; set; }
    public string TranslationId { get; set; }
    public string DisplayName { get; set; }
    public string NativeName { get; set; }
    public string FullName {
      get {
        return DisplayName + " [" + LanguageTag + "]";
      }
    }
    public string LocalizedName {
      get {
        return DisplayName + " (" + NativeName + ")";
      }
    }

    public override string ToString() {
      return DisplayName + " [" + LanguageTag + "]";
    }
  }
}
