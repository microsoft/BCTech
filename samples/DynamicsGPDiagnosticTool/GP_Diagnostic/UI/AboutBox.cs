namespace Microsoft.GP.MigrationDiagnostic.UI;

using System;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

partial class AboutBox : Form
{
    public AboutBox()
    {
        InitializeComponent();
        this.Text = string.Format("About {0}", this.AssemblyTitle);
        this.labelProductName.Text = this.AssemblyProduct;
        this.labelVersion.Text = string.Format("Tool Version: {0}", this.AssemblyVersion);
        this.labelCopyright.Text = this.AssemblyCopyright;
        this.textBoxDescription.Text = this.AssemblyDescription;
    }

    #region Assembly Attribute Accessors

    public string AssemblyTitle
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);

            if (attributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];

                if (titleAttribute.Title != string.Empty)
                {
                    return titleAttribute.Title;
                }
            }

            return Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        }
    }

    public string AssemblyVersion
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyVersionAttribute), false);

            if (attributes.Length == 0)
            {
                return string.Empty;
            }

            return ((AssemblyVersionAttribute)attributes[0]).Version;
        }
    }

    public string AssemblyDescription
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);

            if (attributes.Length == 0)
            {
                return string.Empty;
            }

            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        }
    }

    public string AssemblyProduct
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);

            if (attributes.Length == 0)
            {
                return string.Empty;
            }

            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }

    public string AssemblyCopyright
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);

            if (attributes.Length == 0)
            {
                return string.Empty;
            }

            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    #endregion

    private void okButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
