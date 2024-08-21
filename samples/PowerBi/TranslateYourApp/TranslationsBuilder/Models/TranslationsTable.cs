using System;
using System.Collections.Generic;

namespace TranslationsBuilder.Models {

  class TranslationsTable {
    public string[] Headers { get; set; }
    public List<string[]> Rows { get; set; }
  }

}