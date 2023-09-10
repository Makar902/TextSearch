using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
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
        private bool haveAdminRights;

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

                //generateReport.Click += GenerateReport_Click;

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

        //private void GenerateReport_Click(object? sender, EventArgs e)
        //{
        //    try
        //    {
        //        using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
        //        {
        //            folderDialog.Description = "Select way...";
        //            folderDialog.SelectedPath = @"C:\";

        //            DialogResult result = folderDialog.ShowDialog();

        //            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
        //            {
        //                string selectedFolderPath = folderDialog.SelectedPath;
        //                try
        //                {
        //                    using (StreamWriter userReportTXT = new StreamWriter(selectedFolderPath + ".txt", true))
        //                    {
        //                        List<FileInfo> resultFromSearch = SearchAndModifyFiles(directoryWhere, wordsToSearch, true);
        //                        for (int i = 0; i < resultFromSearch.ToArray().Length; i++)
        //                        {
        //                            userReportTXT.WriteLine(resultFromSearch[i]);
        //                        }
        //                        userReportTXT.Close();
        //                    }
        //                }
        //                catch (Exception hereEr)
        //                {
        //                    CatchExToLog(hereEr, "Here with writer: ");
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception error)
        //    {
        //        CatchExToLog(error);
        //    }
        //}
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
                    haveAdminRights = true;
                }
                else
                {
                    MessageBox.Show("The program is not launched with administrator rights.");
                    haveAdminRights = false;
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }



        //Start Button
        private async void button3_Click(bool FromImport)
        {
            try
            {
                cancelSearch = false;
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                List<Task> tasks = new List<Task>();

                foreach (DriveInfo drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        directoryWhere = drive.RootDirectory.FullName;

                        tasks.Add(SearchAndModifyFilesAsync(directoryWhere, wordsToSearch));
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception error)
            {

                CatchExToLog(error);
            }
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                
                    cancelSearch = false;
                    wordsToSearch = textBox1.Text;

                    DriveInfo[] allDrives = DriveInfo.GetDrives();

                    List<Task> tasks = new List<Task>();

                    foreach (DriveInfo drive in allDrives)
                    {
                        if (drive.IsReady)
                        {
                            directoryWhere = drive.RootDirectory.FullName;

                            tasks.Add(SearchAndModifyFilesAsync(directoryWhere, wordsToSearch));
                        }
                    }

                    await Task.WhenAll(tasks);                             
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }
        //Import and start
        private void button1_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex >= 0)
                {
                    string selectedFilePath = listBox1.SelectedItem.ToString();

                    string fileContent = File.ReadAllText(selectedFilePath);

                    wordsToSearch = fileContent;
                    button3_Click(true);
                }
                else
                {
                    MessageBox.Show("Select file from list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    string[] selectedFilePaths = openFileDialog.FileNames;

                    foreach (string filePath in selectedFilePaths)
                    {
                        listBox1.Items.Add(filePath);
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

        private async Task SearchAndModifyFilesAsync(string directoryPath, string searchText)
        {
            directoryPath = "E:\\TEST";
            try
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(directoryPath))
                    {
                        int totalFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories).Length;
                        int processedFiles = 0;
                        foreach (string filePath in Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                string fileContent = File.ReadAllText(filePath);

                                if (fileContent.Contains(searchText))
                                {
                                    processedFiles++;
                                    int progressPercentage = (int)(((double)processedFiles / totalFiles) * 100);
                                    UpdateProgressBar(progressPercentage);

                                    string copyFilePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(filePath));

                                    try
                                    {
                                        using (FileStream fs = new FileStream(copyFilePath, FileMode.Open, FileAccess.Write, FileShare.None))
                                        {
                                            File.Copy(filePath, copyFilePath, true);
                                            fileContent = fileContent.Replace(searchText, "*******");
                                            File.WriteAllText(copyFilePath, fileContent);
                                        }
                                    }
                                    catch (IOException ex)
                                    {
                                        string processName = GetProcessNameByFilePath(copyFilePath);

                                        if (haveAdminRights==true)
                                        {
                                            if (!string.IsNullOrEmpty(processName))
                                            {
                                                Process[] processes = Process.GetProcessesByName(processName);
                                                foreach (Process process in processes)
                                                {
                                                    process.Kill();
                                                }
                                            }
                                            using (FileStream fs = new FileStream(copyFilePath, FileMode.Open, FileAccess.Write, FileShare.None))
                                            {
                                                File.Copy(filePath, copyFilePath, true);
                                                fileContent = fileContent.Replace(searchText, "*******");
                                                File.WriteAllText(copyFilePath, fileContent);
                                            }
                                        }                                        
                                    }
                                }
                            }
                            catch (UnauthorizedAccessException exr)
                            {
                                CatchExToLog(exr, "Trouble with access to file");
                            }
                            catch (Exception ex)
                            {
                                CatchExToLog(ex, "Trouble with access");
                            }
                        }
                    }
                    else
                    {
                        CatchExToLog($"The {directoryPath} directory does not exist.");
                    }
                });
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        private string GetProcessNameByFilePath(string filePath)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();
                Process[] processes = Process.GetProcesses();

                foreach (Process process in processes)
                {
                    try
                    {
                        if (process.MainModule != null)
                        {
                            string processFileName = Path.GetFileNameWithoutExtension(process.MainModule.FileName).ToLower();
                            if (processFileName == fileName)
                            {
                                return process.ProcessName;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        CatchExToLog("Have no rights in GetProcessNameByFilePath()");
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }


        private void UpdateProgressBar(int progressPercentage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateProgressBar), progressPercentage);
                return;
            }

            progressBar1.Value = progressPercentage;

            if (progressPercentage == 100)
            {
                MessageBox.Show("Finished", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CatchExToLog(string errorText)
        {
            using (StreamWriter log = new StreamWriter(LogTXT, true))
            {
                log.WriteLine(errorText);
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
