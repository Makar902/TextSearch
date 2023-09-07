using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex
{
    public partial class Form1 : Form
    {
        private volatile bool cancelSearch = false;
        private const string LogTXT = "Log.txt";
        private string directoryWhere;
        private string wordsToSearch;
        private const string TextBoxMessege = "Enter your text here...";
        private ulong fileCount = 0;


        public Form1()
        {
            AdminRightsInitiate();
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            TextBoxInit();
            OnForm();
            MenuStripInit();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TextBoxInit()
        {
            try
            {
                textBox1.Text = TextBoxMessege;
                textBox1.Enter += TextBox1_Enter;
                textBox1.Leave += TextBox1_Leave;
                TextBox1_Leave(textBox1, null);
                TextBox1_Leave(textBox1, null);
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        private void TextBox1_Enter(object? sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == TextBoxMessege)
                {
                    textBox1.Text = "";
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        private void TextBox1_Leave(object? sender, EventArgs? e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    textBox1.Text = TextBoxMessege;
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        private void OnForm()
        {
            //comboBoxWords = new ComboBox();
            //comboBoxWords.Location = new Point(10, 120);
            //comboBoxWords.Size = new Size(200, 30);
            //this.Controls.Add(comboBoxWords);

            //resultsLabel = new Label();
            //resultsLabel.Location = new Point(10, 160);
            //resultsLabel.Size = new Size(300, 100);
            //resultsLabel.Text = "Results will be displayed here.";
            //this.Controls.Add(resultsLabel);
        }

        private void MenuStripInit()
        {
            try
            {
                MenuStrip menuStrip = new MenuStrip();

                ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("File");
                ToolStripMenuItem generateReport = new ToolStripMenuItem("Generate report");
                ToolStripMenuItem changeWayToSave = new ToolStripMenuItem("Change way to save");

                fileMenuItem.DropDownItems.Add(generateReport);
                fileMenuItem.DropDownItems.Add(changeWayToSave);

                generateReport.Click += GenerateReport_Click;

                ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("Help");
                ToolStripMenuItem helpWithBrowse = new ToolStripMenuItem("Help with browsing file");
                ToolStripMenuItem helpWithChangeWayToSave = new ToolStripMenuItem("Help with changing way to save");

                helpMenuItem.DropDownItems.Add(helpWithBrowse);
                helpMenuItem.DropDownItems.Add(helpWithChangeWayToSave);

                menuStrip.Items.Add(fileMenuItem);
                menuStrip.Items.Add(helpMenuItem);

                this.MainMenuStrip = menuStrip;
                this.Controls.Add(menuStrip);
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        private void GenerateReport_Click(object? sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = "Select way...";
                    folderDialog.SelectedPath = @"C:\";

                    DialogResult result = folderDialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                    {
                        string selectedFolderPath = folderDialog.SelectedPath;
                        try
                        {
                            using (StreamWriter userReportTXT = new StreamWriter(selectedFolderPath + ".txt", true))
                            {
                                List<FileInfo> resultFromSearch = SearchAndModifyFiles(directoryWhere, wordsToSearch, true);
                                for (int i = 0; i < resultFromSearch.ToArray().Length; i++)
                                {
                                    userReportTXT.WriteLine(resultFromSearch[i]);
                                }
                                userReportTXT.Close();
                            }
                        }
                        catch (Exception hereEr)
                        {
                            CatchExToLog(hereEr, "Here with writer: ");
                        }
                    }
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AdminRightsInitiate()
        {
            try
            {
                WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                WindowsPrincipal currentPrincipal = new WindowsPrincipal(currentIdentity);

                if (currentPrincipal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    MessageBox.Show("The program was launched with the rights of an administrator.");
                }
                else
                {
                    MessageBox.Show("The program is not launched with administrator rights.");
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FindClose(IntPtr hFindFile);
        [DllImport("SearchAndCopy", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern List<FileInfo> SearchAndModifyFiles(string directoryWhere, string wordsToSearch);
        [DllImport("SearchAndCopy", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern List<FileInfo> SearchAndModifyFiles(string directoryWhere, string wordsToSearch, bool userReport);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct WIN32_FIND_DATA
        {
            public uint dwFileAttributes;
            public long ftCreationTime;
            public long ftLastAccessTime;
            public long ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            public string cAlternateFileName;
        }
        //Start Button
        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                cancelSearch = false;
                wordsToSearch = textBox1.Text;
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                foreach (DriveInfo drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        directoryWhere = drive.RootDirectory.FullName;
                        // await Task.Run(() => SearchAndModifyFiles("E:\\", wordsToSearch));
                        SearchAndModifyFiles("E:\\", wordsToSearch);
                    }
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }
        // exit button
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Exit program?", "Exit", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    Application.Exit();
                }
                else if (result == DialogResult.No)
                {
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }
        // Browse button
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = true;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string[] UserFilePath = openFileDialog.FileNames;
                    for (int i = 0; i < UserFilePath.Length; i++)
                    {
                        listBox1.Items.Add(UserFilePath[i]);
                    }
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }
        // StopSearchButton
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                cancelSearch = true;
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        //private async Task SearchFilesAndDirectories(string directoryWhere, string wordsToSearch)
        //{
        //    try
        //    {
        //        string[] files = Directory.GetFiles(directoryWhere);
        //        ulong totalFiles = (ulong)files.Length;
        //        ulong processedFiles = 0;
        //        fileCount = totalFiles;

        //        foreach (string file in files)
        //        {
        //            if (cancelSearch)
        //            {
        //                return;
        //            }

        //            string fileContents = File.ReadAllText(file);
        //            if (fileContents.Contains(wordsToSearch))
        //            {
        //                string path_to = Directory.GetCurrentDirectory() + '\\';
        //                CopyAndModifyFile(directoryWhere, path_to, wordsToSearch);
        //                //MessageBox.Show("Dire bef in Search^ " + directoryWhere);
        //            }

        //            processedFiles++;
        //            UpdateProgressBar(processedFiles, totalFiles);
        //        }

        //        string[] subDirectories = Directory.GetDirectories(directoryWhere);
        //        foreach (string subDirectory in subDirectories)
        //        {
        //            await SearchFilesAndDirectories(subDirectory, wordsToSearch);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        CatchExToLog(ex);
        //    }
        //}
        //private dynamic SearchFilesAndDirectories(string directoryWhere, string wordsToSearch, bool userReport = false)
        //{
        //    ulong filesContainingText = 0;
        //    string res;

        //    List<string> resultList = new List<string>(); 

        //    WIN32_FIND_DATA findData;
        //    IntPtr findHandle = FindFirstFile(Path.Combine(directoryWhere, "*.*"), out findData);

        //    try
        //    {
        //        if (findHandle != IntPtr.Zero)
        //        {
        //            do
        //            {
        //                string fileName = findData.cFileName;
        //                string filePath = Path.Combine(directoryWhere, fileName);

        //                if (((FileAttributes)findData.dwFileAttributes & FileAttributes.Directory) == 0)
        //                {
        //                    StringBuilder fileContent = new StringBuilder(4096);

        //                    using (StreamReader reader = new StreamReader(filePath))
        //                    {
        //                        while (!reader.EndOfStream)
        //                        {
        //                            fileContent.Append(reader.ReadLine());
        //                        }
        //                    }

        //                    if (fileContent.ToString().Contains(wordsToSearch))
        //                    {
        //                        filesContainingText++;
        //                        res = "File name: " + findData.cFileName + " file path: " + filePath + " file attributes: " + findData.dwFileAttributes.ToString() + " file creation time: " + findData.ftCreationTime.ToString() + " file last access time: " + findData.ftLastAccessTime.ToString() + " file last written time: " + findData.ftLastWriteTime.ToString() + " file size: " + (((ulong)findData.nFileSizeHigh << 32) | findData.nFileSizeLow).ToString();
        //                        resultList.Add(res);
        //                    }
        //                }
        //            } while (FindNextFile(findHandle, out findData));

        //            FindClose(findHandle);
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        CatchExToLog(error);
        //    }      
        //        return resultList.ToArray();          
        //}


        //private void CopyAndModifyFile(string directoryWhere, string path_to, string wordsToSearch)
        //{
        //    try
        //    {
        //        //MessageBox.Show("dir in copy show: " + directoryWhere);

        //        string fileName = Path.GetFileName(directoryWhere);
        //        string ResPath = Path.Combine(path_to, fileName);
        //        File.Copy(directoryWhere, ResPath, true);

        //        string fileContents = File.ReadAllText(ResPath);

        //        fileContents = fileContents.Replace(wordsToSearch, "*******");

        //        File.WriteAllText(ResPath, fileContents);
        //    }
        //    catch (Exception ex)
        //    {
        //        CatchExToLog(ex);
        //    }
        //}

        private void UpdateProgressBar(ulong processedFiles, ulong totalFiles)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<ulong, ulong>(UpdateProgressBar), processedFiles, totalFiles);
                    return;
                }

                progressBar1.Value = (int)(((double)processedFiles / totalFiles) * 100);
                if (progressBar1.Value == progressBar1.Maximum)
                {
                   // MessageBox.Show("Finished!");
                    progressBar1.Value = 0;
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CatchExToLog(Exception error)
        {
            CatchExToLog(error, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CatchExToLog(Exception error, string? text)
        {
            try
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    if (text != null)
                    {
                        log.WriteLine(text += error.ToString());
                    }
                    else
                    {
                        log.WriteLine(error.ToString());
                    }
                }
            }
            catch (Exception serror)
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    if (text != null)
                    {
                        log.WriteLine(text += serror.ToString());
                    }
                    else
                    {
                        log.WriteLine(serror.ToString());
                    }
                }
            }
        }
    }
}
