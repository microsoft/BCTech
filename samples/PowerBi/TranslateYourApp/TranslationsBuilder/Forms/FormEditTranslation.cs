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
  public partial class FormEditTranslation : Form {

    private string ObjectType;
    private string PropertyName;
    private string DefaultTranslation;
    private string ObjectName;
    private string Language;

    public FormEditTranslation(string objectType, string propertyName, string objectName, string language, string translationValue) {
      InitializeComponent();
      ObjectType = objectType;
      PropertyName = propertyName;
      ObjectName = objectName;
      DefaultTranslation = TranslationsManager.GetDefaultTranslation(objectName, objectType, propertyName);
      Language = language;
      txtTranslation.Text = translationValue;
      lblObjectType.Text = objectType;
      lblPropertyName.Text = propertyName;
      lblObjectName.Text = objectName;
      lblDefaultTranslation.Text = DefaultTranslation;
      lblLanguage.Text = language;
      btnGetMachineTranslation.Enabled = TranslatorService.IsAvailable;
    }

    private void btnUpdate_Click(object sender, EventArgs e) {
      TranslationsManager.SetDatasetObjectTranslation(ObjectType, PropertyName, ObjectName, Language, txtTranslation.Text);
      this.DialogResult = DialogResult.OK;
      this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e) {
      this.DialogResult = DialogResult.Cancel;
      this.Close();

    }

    private void btnGetMachineTranslation_Click(object sender, EventArgs e) {
      txtTranslation.Text = TranslatorService.TranslateContent(DefaultTranslation, Language);
    }
  }
}
