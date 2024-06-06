using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationsBuilder.Models {
  public interface IStatusCalback {
    void updateLoadingStatus(string TranslationType, string ObjectName, string OriginalText, string TranslatedText);
  }
}
