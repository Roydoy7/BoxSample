using System.Collections.Generic;
using System.Windows.Forms;

namespace BoxSampleAutoCAD.Components
{
    internal class FileBrowser
    {
        public static IEnumerable<string> BrowseFile(string fileTypeName, string[] fileExtensions, bool multiFile = false)
        {
            var fileExtentsionsInternal = new List<string>();
            foreach (var fileEx in fileExtensions)
                fileExtentsionsInternal.Add("*." + fileEx);
            string fileExtentionsWithSpace = string.Join(" ", fileExtentsionsInternal.ToArray());
            string fileExtensionsWithSeparator = string.Join(";", fileExtentsionsInternal.ToArray());
            string fileFilter = string.Format("{0} ({1})|{2}|All files (*.*)|*.*", fileTypeName, fileExtentionsWithSpace, fileExtensionsWithSeparator);

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = fileFilter;//"Excel Files (*.xls *.xlsx *.xlsm)|*.xls;*.xlsx;*.xlsm|All files (*.*)|*.*";
                openFileDialog.Multiselect = multiFile;
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in openFileDialog.FileNames)
                        yield return fileName;
                }
            }

        }

        public static string BrowseFolder()
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            folderDlg.ShowNewFolderButton = true;
            // Show the FolderBrowserDialog.  
            DialogResult result = folderDlg.ShowDialog();
            var path = "";
            if (result == DialogResult.OK)
            {
                path = folderDlg.SelectedPath;
            }
            return path;
        }
    }
}
