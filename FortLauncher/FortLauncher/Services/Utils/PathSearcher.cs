using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortLauncher.Services.Utils
{
    public class PathSearcher
    {
        public static string Open()
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
            commonOpenFileDialog.IsFolderPicker = true;
            commonOpenFileDialog.Title = "Select A Fortnite Build";
            commonOpenFileDialog.Multiselect = false;
            CommonFileDialogResult commonFileDialogResult = commonOpenFileDialog.ShowDialog();


            bool flag = commonFileDialogResult == CommonFileDialogResult.Ok;
            if (flag)
            {
                if (string.IsNullOrEmpty(commonOpenFileDialog.FileName))
                {
                    return "";
                }
                if (File.Exists(Path.Join(commonOpenFileDialog.FileName, "FortniteGame\\Binaries\\Win64\\FortniteClient-Win64-Shipping.exe")))
                {
                    return commonOpenFileDialog.FileName;
                }
                else
                {
                    return "NotFound";
                }
            }

            return "";
        }
    }
}
