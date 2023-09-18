using Ex.Class;
using FluentCommand.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TaskDialogButton = Microsoft.WindowsAPICodePack.Dialogs.TaskDialogButton;

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
        private const string TextBoxMessage = "Enter your text here...";
        public string userChPath;
        private string userReportPath;
        private List<ItemInfo> itemInfos = new List<ItemInfo>();
        internal static List<ItemInfo> currentData = new List<ItemInfo>();
        internal static nuint diskCount;
        internal static bool iterationWas;

        public delegate void ProgressUpdatedEventHandler(object sender, ItemInfo e, string destinationPath);
        public event ProgressUpdatedEventHandler ProgressUpdated;

#pragma warning disable CS8618
        public Form1()
        {
            InitializeComponent();
            InitAsyncF();

            ProgressUpdated += UpdateListBox2;
            listBox2.HorizontalScrollbar = true;

            progressBar1.Style = ProgressBarStyle.Continuous;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

       
        internal async void UpdateListBox2(object sender, ItemInfo e, string destinationPath)
        {
            try
            {
                string logEntry = $"File: {e.FilePath} was copied to: {destinationPath}, Time: {DateTime.Now}";
                listBox2.Items.Add(logEntry);
                listBox2.TopIndex = listBox2.Items.Count - 1;
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Initializes the application.
        private async Task InitAsyncF()
        {
            try
            {
                await FileManager.AdminRightsInitiate();
                await TextBoxInit();
                await MenuStripInit();
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Initializes the text box.
        private async Task TextBoxInit()
        {
            try
            {
                textBox1.Text = TextBoxMessage;
                textBox1.Enter += TextBox1_Enter;
                textBox1.Leave += TextBox1_Leave;
                TextBox1_Leave(textBox1, null);
                TextBox1_Leave(textBox1, null);
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Handles the event when entering the text box.
        private async void TextBox1_Enter(object? sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (textBox1.Text == TextBoxMessage)
                    {
                        textBox1.Text = "";
                    }
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Handles the event when leaving the text box.
        private async void TextBox1_Leave(object? sender, EventArgs? e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(textBox1.Text))
                {
                    textBox1.Text = TextBoxMessage;
                }
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }



        // Comment: Initializes the menu strip.
        private async Task MenuStripInit()
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
                ToolStripMenuItem About = new ToolStripMenuItem("About...");

                helpMenuItem.DropDownItems.Add(About);
                About.Click += About_Click;

                menuStrip.Items.Add(fileMenuItem);
                menuStrip.Items.Add(helpMenuItem);

                this.MainMenuStrip = menuStrip;
                this.Controls.Add(menuStrip);
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Handles the event when clicking "About" in the menu.
        private async void About_Click(object? sender, EventArgs e)
        {
            try
            {
                Microsoft.WindowsAPICodePack.Dialogs.TaskDialog taskDialog = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialog();
                taskDialog.Caption = "About";
                taskDialog.InstructionText = @"Welcome, User!

This program is designed to search for text within all the files on your drives and provides a powerful yet easy-to-use file management tool. Below are instructions on how to use the program:

Key Features of the Program:
- Search for Text: You can enter the text you're interested in, and the program will scan all the files on your computer, searching for matches.
- Replace Text: The program allows you to replace the found text with ""*******"" while leaving the original file untouched.
- Save Found Files: You can choose a directory where the found files will be saved.
- View Results: The program displays the results in a convenient format, making it easy to access and manage the information you need.

Instructions for Use:
1. Enter Search Text: Start by entering the text you want to search for in the text box at the top of the window.
2. Initiate Search: Click the ""Start"" button to begin the search process. The program will check all drives on your computer for the specified text in files.
3. Cancel Search: You can cancel the search at any time by clicking the ""Cancel Search"" button.
4. Stop and Resume: To temporarily stop and resume the search, use the ""Stop/Continue Search"" button.
5. Change Save Location: To change the directory where the found files will be saved, click the ""Change Way to Save"" button and select a new directory.
6. Generate Report: If you want to create a report of the found files, click the ""Generate Report"" button and choose a location to save the report.
7. Clear Results: To clear the displayed information in List box, click the ""Clear"" button.

We provide this program to simplify your file management tasks and help you find the information you need efficiently.

Thank you for using our program!

Sincerely,
[Text Searcher]";

                taskDialog.StandardButtons = TaskDialogStandardButtons.Close;

                TaskDialogResult result = taskDialog.Show();

                if (result == TaskDialogResult.Close)
                {
                    // Handle close action if needed.
                }
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Handles the event when clicking "Generate report" in the menu.
        private async void GenerateReport_Click(object? sender, EventArgs e)
        {
            try
            {
                await Task.Run(async () =>
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
                        await ErrorHandling.CatchExToLog("User report path wasn't set");
                    }
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Handles the event when clicking "Change way to save" in the menu.
        private async void ChangeWayToSave_Click(object? sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
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
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Handles the event when clicking "Open Log.txt" in the menu.
        private async void OpenLog_Click(object? sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    string fileDir = Directory.GetCurrentDirectory() + "\\" + "Log.txt";
                    Process.Start("notepad.exe", fileDir);
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }




        //Comment: Start from Browsing button
        private async Task FromImport()
        {
            try
            {
                await Task.Run(async () =>
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
                });
            }
            catch (Exception error)
            {

                await ErrorHandling.CatchExToLog(error);
            }
        }
        // Comment: Initiates the asynchronous search process when the "Start" button is clicked.
        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                await Task.Run(async () =>
                {
                    iterationWas = false;
                    button3.Enabled = false;
                    button1.Enabled = false;
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
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
            finally
            {
                button3.Enabled = true;
                button1.Enabled = true;
            }
        }

        // Comment: Initiates the asynchronous search process when the "Import and start" button is clicked.
        private async void button1_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                button5.Enabled = true;
                button6.Enabled = true;
                button7.Enabled = true;
                if (listBox1.SelectedIndex >= 0)
                {
                    string selectedFilePath = listBox1.SelectedItem.ToString();
                    string fileContent = File.ReadAllText(selectedFilePath);
                    wordsToSearch = fileContent;
                    await FromImport();
                }
                else
                {
                    MessageBox.Show("Select file from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        // Comment: Exits the program when the "Exit" button is clicked.
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    DialogResult result = MessageBox.Show("Exit the program?", "Exit", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else if (result == DialogResult.No)
                    {
                        // Continue with program execution.
                    }
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        //Comment:  Browse button
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = true;
                
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
                await ErrorHandling.CatchExToLog(error);
            }
        }


        //Comment:  Cancel Search Button
        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                cancelSearch = true;
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }
        //Comment: Stop button
        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                button5.Enabled = false;
                try
                {

                    stopSearch = true;
                    cancelSearch = true;

                }
                catch (Exception)
                {

                    await ErrorHandling.CatchExToLog("Trouble with setting StopSearch&CancelSearch");
                }



            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
            finally
            {
                //button5.Enabled = true;
            }
        }

        //Comment: Update progress bar func
        internal async void UpdateProgressBar(int progressPercentage)
        {
            try
            {

                progressBar1.Value = progressPercentage;
                progressBar1.Text = progressPercentage + "%";
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<int>(UpdateProgressBar), progressPercentage);
                    return;
                }

            }
            catch (Exception error)
            {

                await ErrorHandling.CatchExToLog(error);
            }
        }

        //Comment:  Serch and Copy  func
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
                                        await Task.Run(async () =>
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
                                                MimeType = await ItemInfo.GetMimeType(filePath)
                                            };
                                            currentData.Add(infoToAdd);

                                            File.Copy(filePath, copyFilePath, true);
                                            fileContent = fileContent.Replace(searchText, "*******");
                                            File.WriteAllText(copyFilePath, fileContent);

                                            ProgressUpdated?.Invoke(this, infoToAdd, userReportPath);
                                        });
                                    }
                                }
                                catch (UnauthorizedAccessException ex1)
                                {
                                    await ErrorHandling.CatchExToLog(ex1);
                                }
                                catch (Exception ex)
                                {
                                    await ErrorHandling.CatchExToLog(ex, "Trouble with access");
                                }
                                finally
                                {
                                    await Task.Run(() =>
                                    {
                                        processedFiles++;
                                        int progressPercentage = (int)(((double)processedFiles / totalFiles) * 100);
                                        UpdateProgressBar(progressPercentage);

                                    });

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
                                await Task.Run(() =>
                                {
                                    UpdateProgressBar(100);

                                    MessageBox.Show($"Finished work with drive: {rootDir}", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    UpdateProgressBar(0);
                                    button5.Enabled = true;
                                });
                            }
                        }
                    }
                    else
                    {
                        await ErrorHandling.CatchExToLog($"Directory: {directoryPath} does not exist.");
                    }

                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

        //Comment: Continue serch button
        private async void button7_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    stopSearch = false;
                    cancelSearch = false;
                });

            }
            catch (Exception error)
            {

                await ErrorHandling.CatchExToLog(error);
            }
        }
        //Comment: Cleart listBox2 button
        private void button8_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }
    }
}
