using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;
using System.IO;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.ComponentModel;

namespace PLOS.Gui.Core.Forms
{
    public class OpenFolderDialog
    {
        public OpenFolderDialog()
        {
            this.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        [DefaultValue("")]
        public string RootFolder { set; get; }

        [DefaultValue("")]
        public string SelectedPath { set; get; }

        public DialogResult ShowDialog()
        {
            CommonOpenFileDialog dlg = new CommonOpenFileDialog("フォルダー選択");
            dlg.AllowNonFileSystemItems = false;
            dlg.Multiselect = false;
            dlg.NavigateToShortcut = true;
            dlg.InitialDirectory = this.SelectedPath;
            dlg.IsFolderPicker = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.SelectedPath = dlg.FileName;

                return DialogResult.OK;
            }

            return DialogResult.Cancel;
        }
    }
}
