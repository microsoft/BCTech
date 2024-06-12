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

namespace TranslationsBuilder.Forms {

  public partial class FormEditDefaultTranslation : Form {

    private string ObjectType;
    private string PropertyName;
    private string DefaultTranslationFromObjectName;
    private string ObjectName;
    private string Language;

    public FormEditDefaultTranslation(string objectType, string propertyName, string objectName, string language, string translationValue) {
      InitializeComponent();
      ObjectType = objectType;
      PropertyName = propertyName;
      ObjectName = objectName;
      DefaultTranslationFromObjectName = TranslationsManager.GetDefaultTranslationFromObjectName(objectName, objectType, propertyName);
      Language = language;
      txtTranslation.Text = translationValue;
      lblObjectType.Text = objectType;
      lblPropertyName.Text = propertyName;
      lblObjectName.Text = objectName;
      lblDefaultTranslation.Text = DefaultTranslationFromObjectName;
      lblLanguage.Text = language;
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

    private void btnRevertToDefault_Click(object sender, EventArgs e) {
      txtTranslation.Text = DefaultTranslationFromObjectName;
      TranslationsManager.SetDatasetObjectTranslation(ObjectType, PropertyName, ObjectName, Language, DefaultTranslationFromObjectName);

    }
  }
}
