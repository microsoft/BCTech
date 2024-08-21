using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationsBuilder.Services;

namespace TranslationsBuilder {

  public partial class FormEditLocalizedLabel : Form {

    private string OriginalLabelName;

    public FormEditLocalizedLabel(string LabelName) {
      InitializeComponent();
      OriginalLabelName = LabelName;
      txtLabelName.Text = LabelName;    
    }

    private void btnUpdate_Click(object sender, EventArgs e) {
      TranslationsManager.UpdateLocalizedLabel(OriginalLabelName, txtLabelName.Text);
      this.DialogResult= DialogResult.OK;
      this.Close();
    }

    private void btnDelete_Click(object sender, EventArgs e) {
      TranslationsManager.DeleteLocalizedLabel(OriginalLabelName);
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e) {

    }
  }
}
