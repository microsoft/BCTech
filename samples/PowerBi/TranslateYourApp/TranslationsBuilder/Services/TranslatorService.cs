using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace TranslationsBuilder.Services {

  class TranslatorService {

    private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";
    private static readonly string fromLanguage = TranslationsManager.model.Culture.Substring(0, 2);

    public static bool IsAvailable {
      get {
        return 
          !string.IsNullOrEmpty(AppSettings.AzureTranslatorServiceKey) && 
          !string.IsNullOrEmpty(AppSettings.AzureTranslatorServiceLocation);
      }
    }

    private class TranslatedText {
      public string text { get; set; }
      public string to { get; set; }
    }

    private class TranslatedTextResult {
      public List<TranslatedText> translations { get; set; }
    }

    static private List<TranslatedText> GetMachineTranslations(string textToTranslate, string[] languages) {

      string targetLanguages = "";
      foreach (string language in languages) {
        targetLanguages += "&to=" + language;
      }

      string route = "/translate?api-version=3.0&from=" + fromLanguage + targetLanguages;
      object[] body = new object[] { new { Text = textToTranslate } };
      var requestBody = JsonConvert.SerializeObject(body);

      using (var client = new HttpClient())
      using (var request = new HttpRequestMessage()) {

        // prepare HTTP request
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(endpoint + route);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", AppSettings.AzureTranslatorServiceKey);
        request.Headers.Add("Ocp-Apim-Subscription-Region", AppSettings.AzureTranslatorServiceLocation);

        // transmit HTTP request
        HttpResponseMessage response = client.Send(request);

        // extract translated content from HTTP response body
        string result = response.Content.ReadAsStringAsync().Result;
        List<TranslatedTextResult> convertedResult = JsonConvert.DeserializeObject<List<TranslatedTextResult>>(result);
        return convertedResult[0].translations;
      }

    }

    public static string TranslateContent(string textToTranslate, string language) {
      string[] languages = { language };
      var translationsResult = GetMachineTranslations(textToTranslate, languages);
      CultureInfo cultureInfo = new CultureInfo(language);
      return cultureInfo.TextInfo.ToTitleCase(translationsResult[0].text);
    }

  }
}
