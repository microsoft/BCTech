using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationsBuilder.Models;
using TranslationsBuilder.Services;

namespace TranslationsBuilder.Forms {
  public partial class FormConnect : Form {

    public string ConnectString;

    public FormConnect() {
      InitializeComponent();
      RefreshActiveSessionsListbox();
    }

    public void RefreshActiveSessionsListbox() {

      var sessions = PowerBiDesktopUtilities.GetActiveDatasetConnections();
      lstSessions.DataSource = sessions;
      lstSessions.DisplayMember= "DisplayName";

      if (TranslationsManager.IsConnected) {
        for (int index = 0; index < lstSessions.Items.Count; index++) {
          DatasetConnection session = lstSessions.Items[index] as DatasetConnection;
          if (session.ConnectString.Equals(TranslationsManager.ActiveConnection.ConnectString)) {
            lstSessions.SelectedIndex = index;
            break;
          }
        }

      }

    }

    private void btnConnect_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e) {

      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void lstSessions_SelectedIndexChanged(object sender, EventArgs e) {

      string selectedServerConnectString = (lstSessions.SelectedValue as DatasetConnection).ConnectString;
      ConnectString = selectedServerConnectString;

    }
  }
}
