using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationsBuilder.Models;

namespace TranslationsBuilder {

  public partial class FormLoadingStatus : Form, IStatusCalback {

    public void updateLoadingStatus(string TranslationType, string ObjectName, string OriginalText, string TranslatedText) {
      lblTranslationType.Text = TranslationType;
      lblObjectName.Text = ObjectName;
      lblOriginalText.Text = OriginalText;
      lblTranslatedText.Text = TranslatedText;
      RedrawUI();
    }

    public FormLoadingStatus() {
      InitializeComponent();
      RedrawUI();
    }

    private void RedrawUI() {
      foreach (Control control in this.Controls) {
        control.Refresh();
      }
    }
  }
}
