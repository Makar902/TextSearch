using Ex.Class;
using FluentCommand.Extensions;
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
#pragma warning disable CS0414
        internal volatile bool cancelSearch = false;
        internal volatile static bool stopSearch;
#pragma warning restore CS0414
        private string directoryWhere;
        private string wordsToSearch;
        private const string TextBoxMessege = "Enter your text here...";
        public string userChPath;
        private string userReportPath;
        private List<ItemInfo> itemInfos = new List<ItemInfo>();
        internal static List<ItemInfo> currentData = new List<ItemInfo>();
        internal static nuint diskCount;
        internal static bool iterationWas;



#pragma warning disable CS8618
        public Form1()
#pragma warning restore CS8618 
        {
            FileManager.AdminRightsInitiate();
            InitializeComponent();
            progressBar1.Style = ProgressBarStyle.Continuous;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            TextBoxInit();
            MenuStripInit();

        }

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
                ErrorHandling.CatchExToLog(error);
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
                ErrorHandling.CatchExToLog(error);
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
                ErrorHandling.CatchExToLog(error);
            }
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

                fileMenuItem.DropDownItems.Add(openLog);
                fileMenuItem.DropDownItems.Add(generateReport);
                fileMenuItem.DropDownItems.Add(changeWayToSave);

                openLog.Click += OpenLog_Click;
                changeWayToSave.Click += ChangeWayToSave_Click;
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
                ErrorHandling.CatchExToLog(error);
            }
        }

        private void GenerateReport_Click(object? sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog dialog = new FolderBrowserDialog())
                {
                    dialog.RootFolder = Environment.SpecialFolder.Desktop;
                    DialogResult = dialog.ShowDialog();
                    if (DialogResult == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                    {
                        userReportPath = dialog.SelectedPath;
                    }
                }
                if (!string.IsNullOrEmpty(userReportPath))
                {
                    using (StreamWriter writer = new StreamWriter(userReportPath, true))
                    {
                        for (int i = 0; i < itemInfos.Count; i++)
                        {
                            ItemInfo item = itemInfos[i];
                            writer.WriteLine($"File Path: {item.FilePath}");
                            writer.WriteLine($"File Size (Bytes): {item.FileSizeBytes}");
                            writer.WriteLine($"Creation Date: {item.CreationDate}");
                            writer.WriteLine($"Last Modified Date: {item.LastModifiedDate}");
                            writer.WriteLine($"File Extension: {item.FileExtension}");
                            writer.WriteLine($"MIME Type: {item.MimeType}");
                            writer.WriteLine();
                        }
                    }

                }
                else
                {
                    ErrorHandling.CatchExToLog("User report path was`t set");
                }
            }
            catch (Exception error)
            {
                ErrorHandling.CatchExToLog(error);
            }
        }

        private void ChangeWayToSave_Click(object? sender, EventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

                    DialogResult result = folderBrowserDialog.ShowDialog();

                    if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        MessageBox.Show("Selected directory: " + folderBrowserDialog.SelectedPath);
                        userChPath = folderBrowserDialog.SelectedPath;
                    }
                }
            }
            catch (Exception error)
            {

                ErrorHandling.CatchExToLog(error);
            }
        }

        private void OpenLog_Click(object? sender, EventArgs e)
        {
            try
            {
                string fileDir = Directory.GetCurrentDirectory() + "\\" + "Log.txt";
                Process.Start("notepad.exe", fileDir);
            }
            catch (Exception error)
            {

                ErrorHandling.CatchExToLog(error);
            }
        }





        //Start Button
        private async void FromImport()
        {
            try
            {
                iterationWas = false;
                diskCount = 0;
                cancelSearch = false;
                stopSearch = false;
                DriveInfo[] allDrives = DriveInfo.GetDrives();

                List<Task> tasks = new List<Task>();

                foreach (DriveInfo drive in allDrives)
                {
                    diskCount++;
                }
                foreach (DriveInfo drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        directoryWhere = drive.RootDirectory.FullName;
                        iterationWas = true;
                        tasks.Add(SearchAndModifyFilesAsync(directoryWhere, wordsToSearch));
                    }
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception error)
            {

                ErrorHandling.CatchExToLog(error);
            }
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                iterationWas = false;
                button3.Enabled = false;
                cancelSearch = false;
                wordsToSearch = textBox1.Text;
                diskCount = 0;

                DriveInfo[] allDrives = DriveInfo.GetDrives();

                List<Task> tasks = new List<Task>();

                foreach (DriveInfo drive in allDrives)
                {
                    diskCount++;
                }
                foreach (DriveInfo drive in allDrives)
                {
                    if (drive.IsReady)
                    {
                        directoryWhere = drive.RootDirectory.FullName;
                        iterationWas = true;
                        tasks.Add(SearchAndModifyFilesAsync(directoryWhere, wordsToSearch));
                    }
                }

                await Task.WhenAll(tasks);
                itemInfos.Clear();
                itemInfos.AddRange(currentData);
            }
            catch (Exception error)
            {
                ErrorHandling.CatchExToLog(error);
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
                ErrorHandling.CatchExToLog(error);
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
                ErrorHandling.CatchExToLog(error);
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
                ErrorHandling.CatchExToLog(error);
            }
        }


        // CancelSearchButton
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                cancelSearch = true;
            }
            catch (Exception error)
            {
                ErrorHandling.CatchExToLog(error);
            }
        }
        //Stop&ContinueSearch
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    stopSearch = true;
                    cancelSearch = true;
                }
                catch (Exception)
                {

                    ErrorHandling.CatchExToLog("Trouble with setting StopSearch&CancelSearch");
                }


            }
            catch (Exception error)
            {
                ErrorHandling.CatchExToLog(error);
            }
        }


        internal void UpdateProgressBar(int progressPercentage)
        {
            progressBar1.Value = progressPercentage;
            progressBar1.Text = progressPercentage + "%";
            if (InvokeRequired)
            {
                BeginInvoke(new Action<int>(UpdateProgressBar), progressPercentage);
                return;
            }
        }


        private async Task SearchAndModifyFilesAsync(string directoryPath, string searchText)
        {
            try
            {
                await Task.Run(async () =>
                {
                    string rootDir = directoryPath;
                    directoryPath = "E:\\TEST";

                    if (Directory.Exists(directoryPath))
                    {
                        string[] filePaths = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
                        int totalFiles = filePaths.Length;
                        int totalFilesW = 0;
                        int processedFiles = 0;

                        foreach (string filePath in filePaths)
                        {
                            if (cancelSearch == false)
                            {
                                try
                                {
                                    string fileContent = File.ReadAllText(filePath);

                                    if (fileContent.Contains(searchText))
                                    {
                                        string copyFilePath = Path.Combine(Environment.CurrentDirectory, Path.GetFileName(filePath));
                                        totalFilesW++;
                                        if (!(userChPath.IsNullOrEmpty()))
                                        {
                                            copyFilePath = Path.Combine(userChPath, Path.GetFileName(filePath));
                                        }
                                        ItemInfo infoToAdd = new ItemInfo()
                                        {
                                            FilePath = copyFilePath,
                                            FileSizeBytes = new FileInfo(filePath).Length,
                                            CreationDate = File.GetCreationTime(filePath),
                                            LastModifiedDate = File.GetLastWriteTime(filePath),
                                            FileExtension = Path.GetExtension(filePath),
                                            MimeType = ItemInfo.GetMimeType(filePath)
                                        };
                                        currentData.Add(infoToAdd);

                                        File.Copy(filePath, copyFilePath, true);
                                        fileContent = fileContent.Replace(searchText, "*******");
                                        File.WriteAllText(copyFilePath, fileContent);
                                    }
                                }
                                catch (UnauthorizedAccessException ex1)
                                {
                                    ErrorHandling.CatchExToLog(ex1);
                                }
                                catch (Exception ex)
                                {
                                    ErrorHandling.CatchExToLog(ex, "Trouble with access");
                                }
                                finally
                                {
                                    processedFiles++;
                                    int progressPercentage = (int)(((double)processedFiles / totalFiles) * 100);
                                    UpdateProgressBar(progressPercentage);
                                }
                            }
                            else if (cancelSearch == true)
                            {
                                if (stopSearch == true)
                                {
                                   await FileManager.Wait();
                                    processedFiles++;
                                    
                                }
                                else if (stopSearch == false)
                                {
                                    string resultBreak = $"Canceled work with{rootDir}";
                                    MessageBox.Show(resultBreak, "Abort", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    UpdateProgressBar(0);
                                    return;
                                }
                                
                            }


                            if (processedFiles == totalFiles)
                            {
                                UpdateProgressBar(100);
                                MessageBox.Show($"Finished work with drive: {rootDir}", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                UpdateProgressBar(0);
                            }
                        }
                    }
                    else
                    {
                        ErrorHandling.CatchExToLog($"Directory: {directoryPath} does not exist.");
                    }

                });
            }
            catch (Exception error)
            {
                ErrorHandling.CatchExToLog(error);
            }
        }
        //Continue button
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                stopSearch = false;
                cancelSearch = false;

            }
            catch (Exception error)
            {

                ErrorHandling.CatchExToLog(error);
            }
        }
    }
}
