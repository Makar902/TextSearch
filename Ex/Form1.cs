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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ex
{
    public partial class Form1 : Form
    {
#pragma warning disable CS0414
        private volatile bool cancelSearch = false;
#pragma warning restore CS0414
        private const string LogTXT = "Log.txt";
        private string directoryWhere;
        private string wordsToSearch;
        private const string TextBoxMessege = "Enter your text here...";
        private object logLock = new object();
        private bool haveAdminRights;

#pragma warning disable CS8618
        public Form1()
#pragma warning restore CS8618 
        {
            AdminRightsInitiate();
            InitializeComponent();
            progressBar1.Style = ProgressBarStyle.Continuous;
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
                ToolStripMenuItem openLog = new ToolStripMenuItem("Open Log.txt");
                ToolStripMenuItem generateReport = new ToolStripMenuItem("Generate report");
                ToolStripMenuItem changeWayToSave = new ToolStripMenuItem("Change way to save");

                fileMenuItem.DropDownItems.Add(generateReport);
                fileMenuItem.DropDownItems.Add(changeWayToSave);
                fileMenuItem.DropDownItems.Add(openLog);

                openLog.Click += OpenLog_Click;                //generateReport.Click += GenerateReport_Click;

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

        private void OpenLog_Click(object? sender, EventArgs e)
        {
            try
            {
                string currentDirectory = Directory.GetCurrentDirectory() + "\\" + LogTXT;
                MessageBox.Show(currentDirectory);
                Process.Start(currentDirectory);
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
                    MessageBox.Show("The program is not launched with administrator rights.May be some trouble with acces");
                    haveAdminRights = false;
                }
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }



        //Start Button
        private async void FromImport()
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
                button3.Enabled = false;
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
            finally
            {
                button3.Enabled = true;
            }
        }
        //Import and start
        private void button1_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedIndex >= 0)
                {
#pragma warning disable CS8602
#pragma warning disable CS8600 
                    string selectedFilePath = listBox1.SelectedItem.ToString();
#pragma warning restore CS8600
#pragma warning restore CS8602

#pragma warning disable CS8604 
                    string fileContent = File.ReadAllText(selectedFilePath);
#pragma warning restore CS8604 

                    wordsToSearch = fileContent;
                    FromImport();
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
            string rootDir = directoryPath;
            directoryPath = "E:\\TEST";
            try
            {
                await Task.Run(() =>
                {
                    if (Directory.Exists(directoryPath))
                    {
                        string[] filePaths = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                        int totalFiles = filePaths.Length;
                        int processedFiles = 0;

                        foreach (string filePath in filePaths)
                        {
                            try
                            {
                                string fileContent = File.ReadAllText(filePath);

                                if (fileContent.Contains(searchText))
                                {
                                    string copyFilePath = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(filePath));

                                    File.Copy(filePath, copyFilePath, true);
                                    fileContent = fileContent.Replace(searchText, "*******");
                                    File.WriteAllText(copyFilePath, fileContent);
                                }
                            }
                            catch (UnauthorizedAccessException ex1)
                            {
                                CatchExToLog(ex1);
                            }
                            catch (Exception ex)
                            {
                                CatchExToLog(ex, "Trouble with access");
                            }
                            finally
                            {
                                processedFiles++;
                                int progressPercentage = (int)(((double)processedFiles / totalFiles) * 100);
                                UpdateProgressBar(progressPercentage);
                            }
                        }

                        
                            if (processedFiles == totalFiles)
                            {
                                UpdateProgressBar(100);
                                MessageBox.Show($"Finished work with drive: {rootDir}", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                UpdateProgressBar(0);
                            }
                           
                        
                    }
                    else
                    {
                        CatchExToLog($"Directory: {directoryPath} does not exist.");
                    }
                });
            }
            catch (Exception error)
            {
                CatchExToLog(error);
            }
        }

        private void UpdateProgressBar(int progressPercentage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateProgressBar), progressPercentage);
                return;
            }

            progressBar1.Value = progressPercentage;
            progressBar1.Text = progressPercentage + "%";
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CatchExToLog(string errorText)
        {
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {errorText}";

            lock (logLock)
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    log.WriteLine(logMessage);
                }
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
            string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {(text != null ? text : "")}{error}";

            lock (logLock)
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    log.WriteLine(logMessage);
                }
            }
        }
    }
}
