using System;
using System.Linq;
using System.Collections.Generic;

namespace TranslationsBuilderTestClient.Models {

  class SupportedLanguages {

    static public Language Afrikaans = new Language { LanguageTag = "af-ZA", TranslationId = "af", DisplayName = "Afrikaans", NativeName = "Afrikaans" };
    static public Language Arabic = new Language { LanguageTag = "ar-001", TranslationId = "ar", DisplayName = "Arabic", NativeName = "العربية" };
    static public Language Bulgarian = new Language { LanguageTag = "bg-BG", TranslationId = "bg", DisplayName = "Bulgarian", NativeName = "български" };
    static public Language Catalan = new Language { LanguageTag = "ca-ES", TranslationId = "ca", DisplayName = "Catalan", NativeName = "Català" };
    static public Language Chinese = new Language { LanguageTag = "zh-CN", TranslationId = "zh", DisplayName = "Chinese", NativeName = "中文" };
    static public Language Croatian = new Language { LanguageTag = "hr-HR", TranslationId = "hr", DisplayName = "Croatian", NativeName = "Hrvatski" };
    static public Language Czech = new Language { LanguageTag = "cs-CZ", TranslationId = "cs", DisplayName = "Czech", NativeName = "Ceština" };
    static public Language Danish = new Language { LanguageTag = "da-DK", TranslationId = "da", DisplayName = "Danish", NativeName = "Dansk" };
    static public Language Dutch = new Language { LanguageTag = "nl-NL", TranslationId = "nl", DisplayName = "Dutch", NativeName = "Nederlands" };
    static public Language English = new Language { LanguageTag = "en-US", TranslationId = "en", DisplayName = "English", NativeName = "English" };
    static public Language Estonian = new Language { LanguageTag = "et-EE", TranslationId = "et", DisplayName = "Estonian", NativeName = "Eesti" };
    static public Language Filipino = new Language { LanguageTag = "fil-PH", TranslationId = "fil", DisplayName = "Filipino", NativeName = "Filipino" };
    static public Language Finnish = new Language { LanguageTag = "fi-FI", TranslationId = "fi", DisplayName = "Finnish", NativeName = "Suomi" };
    static public Language French = new Language { LanguageTag = "fr-FR", TranslationId = "fr", DisplayName = "French", NativeName = "Français" };
    static public Language German = new Language { LanguageTag = "de-DE", TranslationId = "de", DisplayName = "German", NativeName = "Deutsch" };
    static public Language Greek = new Language { LanguageTag = "el-GR", TranslationId = "el", DisplayName = "Greek", NativeName = "Greece" };
    static public Language Hebrew = new Language { LanguageTag = "he-IL", TranslationId = "he", DisplayName = "Hebrew", NativeName = "ישראל" };
    static public Language Hindi = new Language { LanguageTag = "hi-IN", TranslationId = "hi", DisplayName = "Hindi", NativeName = "हिन्दी" };
    static public Language Hungarian = new Language { LanguageTag = "hu-HU", TranslationId = "hu", DisplayName = "Hungarian", NativeName = "Magyar" };
    static public Language Icelandic = new Language { LanguageTag = "is-IS", TranslationId = "is", DisplayName = "Icelandic", NativeName = "Islenska" };
    static public Language Indonesian = new Language { LanguageTag = "id-ID", TranslationId = "id", DisplayName = "Indonesian", NativeName = "Indonesia" };
    static public Language Irish = new Language { LanguageTag = "ga-IE", TranslationId = "ga", DisplayName = "Irish", NativeName = "Gaeilge" };
    static public Language Italian = new Language { LanguageTag = "it-IT", TranslationId = "it", DisplayName = "Italian", NativeName = "Italiano" };
    static public Language Japanese = new Language { LanguageTag = "ja-JP", TranslationId = "ja", DisplayName = "Japanese", NativeName = "日本語 (日本)" };
    static public Language Korean = new Language { LanguageTag = "ko-KR", TranslationId = "ko", DisplayName = "Korean", NativeName = "한국어(대한민국)" };
    static public Language Latvian = new Language { LanguageTag = "lv-LV", TranslationId = "lv", DisplayName = "Latvian", NativeName = "Latviešu" };
    static public Language Napali = new Language { LanguageTag = "ne-NP", TranslationId = "ne", DisplayName = "Napali", NativeName = "Napal" };
    static public Language Norwegian = new Language { LanguageTag = "nb-NO", TranslationId = "nb", DisplayName = "Norwegian", NativeName = "Norsk" };
    static public Language Persian = new Language { LanguageTag = "fa-IR", TranslationId = "fa", DisplayName = "Persian", NativeName = "فارسی (ایران)" };
    static public Language Polish = new Language { LanguageTag = "pl-PL", TranslationId = "pl", DisplayName = "Polish", NativeName = "Polski" };
    static public Language Portuguese = new Language { LanguageTag = "pt-PT", TranslationId = "pt", DisplayName = "Portuguese", NativeName = "Português" };
    static public Language Romanian = new Language { LanguageTag = "ro-RO", TranslationId = "ro", DisplayName = "Romanian", NativeName = "Română" };
    static public Language Russian = new Language { LanguageTag = "ru-RU", TranslationId = "ru", DisplayName = "Russian", NativeName = "Pусский" };
    static public Language Serbian = new Language { LanguageTag = "sr-Latn-BA", TranslationId = "sr-Latn", DisplayName = "Serbian", NativeName = "Srpski" };
    static public Language Slovak = new Language { LanguageTag = "sk-SK", TranslationId = "sk", DisplayName = "Slovak", NativeName = "Slovenčina" };
    static public Language Slovenian = new Language { LanguageTag = "sl-SI", TranslationId = "sl", DisplayName = "Slovenian", NativeName = "Slovenščina (Slovenija)" };
    static public Language Somalian = new Language { LanguageTag = "so-SO", TranslationId = "so", DisplayName = "Somalian", NativeName = "Somalia" };
    static public Language Spanish = new Language { LanguageTag = "es-ES", TranslationId = "es", DisplayName = "Spanish", NativeName = "Español" };
    static public Language Swedish = new Language { LanguageTag = "sv-SE", TranslationId = "sv", DisplayName = "Swedish", NativeName = "Svenska" };
    static public Language Thai = new Language { LanguageTag = "th-TH", TranslationId = "th", DisplayName = "Thai", NativeName = "ไทย" };
    static public Language Turkish = new Language { LanguageTag = "tr-TR", TranslationId = "tr", DisplayName = "Turkish", NativeName = "Türkçe" };
    static public Language Ukrainian = new Language { LanguageTag = "uk-UA", TranslationId = "uk", DisplayName = "Ukrainian", NativeName = "українська" };
    static public Language Vietnamese = new Language { LanguageTag = "vi-VN", TranslationId = "vi", DisplayName = "Vietnamese", NativeName = "Tiếng Việt" };

    static public Language[] AllLangauges = new Language[] { 
      English, Spanish, French, German, Dutch, Italian, Portuguese, Greek, Russian, Japanese, Chinese ,Hindi, Hebrew, Afrikaans    };

  }
}

