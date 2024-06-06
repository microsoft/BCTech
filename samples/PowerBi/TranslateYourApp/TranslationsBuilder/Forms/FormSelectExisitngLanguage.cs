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
using TranslationsBuilder.Services;

namespace TranslationsBuilder.Forms {
  public partial class FormSelectExisitngLanguage : Form {

    public string SourceLanguageId { get; set; }

    public FormSelectExisitngLanguage(string sourceLanguageId) {
      InitializeComponent();
      this.SourceLanguageId = sourceLanguageId;
      PopulateListBox();
    }

    private void PopulateListBox() {

      listLanguages.Items.Clear();

      var model = TranslationsManager.model;

      var sourceLanguage = SupportedLanguages.AllLanguages[SourceLanguageId];
      var sourceLanguageGroup = sourceLanguage.LanguageGroup;

      foreach (var language in model.Cultures) {
        if (!language.Name.Equals(model.Culture) && !language.Name.Equals(SourceLanguageId)) {
          listLanguages.Items.Add(SupportedLanguages.AllLanguages[language.Name]);
        }
      }

      if (listLanguages.Items.Count > 0) {
        for (int i = 0; i < listLanguages.Items.Count; i++) {
          Language languageItem = listLanguages.Items[i] as Language;
          if (languageItem.LanguageGroup == sourceLanguageGroup) {
            listLanguages.SetSelected(i, true);
          }
        }
      }
    }

    public string[] getLanguages() {
      List<string> Languages = new List<string>();
      foreach (object item in listLanguages.SelectedItems) {
        Languages.Add(((Language)item).LanguageId);
      }
      return Languages.ToArray();
    }

    private void btnCopyTanslations_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
