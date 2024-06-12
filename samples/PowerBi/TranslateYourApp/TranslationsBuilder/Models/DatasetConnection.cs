using System;
using System.Collections.Generic;

namespace TranslationsBuilder.Models {

  enum ConnectionType {
    PowerBiDesktop,
    PowerBiService
  }

  class DatasetConnection {
    public string ConnectString { get; set; }
    public string DatasetId { get; set; }
    public string DatasetName { get; set; }
    public ConnectionType ConnectionType { get; set; }

    public string DisplayName {
      get {
        return DatasetName + " (" + ConnectString + ")";
      }
    }
  }

}
