using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationsBuilder.Models {

  class TranslationsTableItem {
    public string Label { get; set; }
    public string Language { get; set; }
    public string Value { get; set; }
    public override string ToString() {
      return @"  { """ + Label + @""", """ + Language + @""", """ + Value + @""" }";
    }
  }

}
