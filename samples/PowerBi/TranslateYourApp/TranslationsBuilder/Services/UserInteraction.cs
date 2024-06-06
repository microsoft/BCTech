using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranslationsBuilder.Services {

  class UserInteraction {

    public static void PromptUserWithInformation(string Message) {
      MessageBox.Show(Message, GlobalConstants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static void PromptOnLocalizedLabelsTableCreate() {
   
      string userMessage = "The Localized Label table has been created. \r\n" + 
                           "You can now use the Add Label command to add report labels.\r\n\r\n" + 
                           "Click YES if you want to read about the Localized Labels strategy.";
      
      DialogResult response  = MessageBox.Show(userMessage, GlobalConstants.ApplicationName, MessageBoxButtons.YesNo, MessageBoxIcon.Information,MessageBoxDefaultButton.Button2);
      
      if(response == DialogResult.Yes) {
        string helpUrl = "https://github.com/PowerBiDevCamp/TranslationsBuilder/blob/main/Docs/Building%20Multi-language%20Reports%20in%20Power%20BI.md#understanding-the-localized-labels-table";
        Process.Start(new ProcessStartInfo() { FileName = helpUrl, UseShellExecute = true });
      }
    }

    public static void PromptUserWithWarning(string Message) {
      MessageBox.Show(Message, GlobalConstants.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static void PromptUserWithError(Exception ex) {
      string errorType = ex.GetType().ToString();
      string errorMessage = ex.Message;
      string messageToUser = errorMessage + "\r\n" + "\r\n" + "Error: " + errorType;
      MessageBox.Show(messageToUser, GlobalConstants.ApplicationName + " Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void PromptUserWithError(string ErrorMessage) {
      MessageBox.Show(ErrorMessage, GlobalConstants.ApplicationName + " Error Occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static bool PromptUserToConfirmOperation(string Message, string Operation) {
      var dialogResult = MessageBox.Show(Message, "Confirm " + Operation, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
      return dialogResult == DialogResult.OK;
    }
  }
}
