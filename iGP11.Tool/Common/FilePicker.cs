using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using iGP11.Library;

namespace iGP11.Tool.Common
{
    public class FilePicker : IFilePicker
    {
        public void OpenDirectory(string filePath)
        {
            if ((filePath == null) || !File.Exists(filePath))
            {
                return;
            }

            Process.Start("explorer.exe", $"/select, \"{filePath}\"");
        }

        public void OpenFile(string filePath)
        {
            if ((filePath == null) || !File.Exists(filePath))
            {
                return;
            }

            Process.Start(filePath);
        }

        public string Pick(string initialFilePath)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = !initialFilePath.IsNullOrEmpty()
                                              ? Path.GetDirectoryName(initialFilePath)
                                              : null;

                dialog.Filter = @"application file (*.exe)|*.exe";
                dialog.Multiselect = false;

                return dialog.ShowDialog() == DialogResult.OK
                           ? dialog.FileName
                           : null;
            }
        }
    }
}