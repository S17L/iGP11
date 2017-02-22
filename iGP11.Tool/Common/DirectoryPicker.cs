using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

using iGP11.Library;

namespace iGP11.Tool.Common
{
    public class DirectoryPicker : IDirectoryPicker
    {
        public void Open(string directory)
        {
            if (directory.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!directory.EndsWith("\\"))
            {
                directory = string.Concat(directory, "\\");
            }

            Directory.CreateDirectory(directory);
            Process.Start(directory);
        }

        public string Pick(string initialDirectory)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = initialDirectory;
                dialog.ShowNewFolderButton = true;

                return dialog.ShowDialog() == DialogResult.OK
                           ? dialog.SelectedPath
                           : null;
            }
        }
    }
}