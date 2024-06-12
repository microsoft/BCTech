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

namespace TranslationsBuilder {
  public partial class FormAddLanguage : Form {

    private string DefaultLanguageGroup;

    public FormAddLanguage(string LanguageGroup = "") {
      InitializeComponent();
      DefaultLanguageGroup = LanguageGroup;
    }

    private void PopulateComboBox() {
      comboLanguageGroupFilter.Items.Clear();
      comboLanguageGroupFilter.Items.Add("Show all languages");
      comboLanguageGroupFilter.Items.AddRange(SupportedLanguages.GetLanguageGroups());
      if (DefaultLanguageGroup.Equals("")) {
        comboLanguageGroupFilter.SelectedIndex = 0;
      }
      else {
        comboLanguageGroupFilter.SelectedIndex = comboLanguageGroupFilter.Items.IndexOf(DefaultLanguageGroup);
        chkShowAllLanguages.Checked = true;
      }

    }

    private void PopulateListBox() {

      listLanguages.Items.Clear();

      var model = TranslationsManager.model;
      var cultures = model.Cultures;

      Dictionary<string, Language> languages;

      if (chkShowAllLanguages.Checked) {
        string languageGroupFilter = comboLanguageGroupFilter.SelectedItem as string ?? "";
        if (languageGroupFilter.Equals("Show all languages")) {
          languageGroupFilter = "";
        }
        languages = SupportedLanguages.GetLangauges(languageGroupFilter);
        if (comboLanguageGroupFilter.SelectedText.Equals("Show all languages")) {
          var xxx = languages.Where(lang => lang.Value.LanguageGroup.Equals(comboLanguageGroupFilter.Text));
        }
      }
      else {
        languages = SupportedLanguages.GetCommonLangauges();
      }

      foreach (var language in languages) {
        if (!cultures.ContainsName(language.Key)) {
          listLanguages.Items.Add(language.Value);
        }
      }

      if (listLanguages.Items.Count > 0) {
        listLanguages.SelectedIndex = 0;
      }
    }

    private void SetUI() {

      bool showAllLanguages = chkShowAllLanguages.Checked;
      lblFilterByLanguageGroup.Visible = showAllLanguages;
      comboLanguageGroupFilter.Visible = showAllLanguages;

      int listLanguagesYPosition = showAllLanguages ? 56 : 16;
      int listLanguagesHeight = showAllLanguages ? 480 : 504;

      listLanguages.Location = new Point(listLanguages.Location.X, listLanguagesYPosition);
      listLanguages.Size = new Size(listLanguages.Size.Width, listLanguagesHeight);

    }


    private void AddLanguageDialog_Load(object sender, EventArgs e) {
      PopulateComboBox();
      PopulateListBox();
      SetUI();

    }

    private void btnCancel_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }

    private void btnAddCulture_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.OK;
      this.Close();
    }


    private void chkShowAllLanguages_CheckedChanged(object sender, EventArgs e) {
      PopulateListBox();
      SetUI();
    }

    private void comboLanguageGroupFilter_SelectedIndexChanged(object sender, EventArgs e) {
      PopulateListBox();
    }

    public string[] GetSelectedLanguages() {
      List<string> Languages = new List<string>();
      foreach (object item in listLanguages.SelectedItems) {
        Languages.Add(((Language)item).LanguageId);
      }
      return Languages.ToArray();
    }

  }
}
