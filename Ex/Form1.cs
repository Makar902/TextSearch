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
using Microsoft.WindowsAPICodePack.Dialogs;
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
            InitializeComponent();
            IntiAsyncF();
            progressBar1.Style = ProgressBarStyle.Continuous;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

        }
        private async Task IntiAsyncF()
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
        private async Task TextBoxInit()
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
                await ErrorHandling.CatchExToLog(error);
            }
        }

        private async void TextBox1_Enter(object? sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    if (textBox1.Text == TextBoxMessege)
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

        private async void TextBox1_Leave(object? sender, EventArgs? e)
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
                await ErrorHandling.CatchExToLog(error);
            }
        }



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

        private async void About_Click(object? sender, EventArgs e)
        {
            try
            {
                Microsoft.WindowsAPICodePack.Dialogs.TaskDialog taskDialog = new Microsoft.WindowsAPICodePack.Dialogs.TaskDialog();
                taskDialog.Caption = "About";
                taskDialog.InstructionText = "Welcome, User!\r\n\r\nThis is a program designed to search for text within all the files on your drives. You can enter the text you're interested in, and the program will scan all the files on your computer, searching for matches. When a match is found, the program will highlight it and replace the text with \"*******,\" leaving the original file untouched.\r\n\r\nKey Features of the Program:\r\n- Search for text in all files on your computer.\r\n- Option to replace the found text with \"*******.\"\r\n- Save the found files to a directory of your choice.\r\n- View the results in a convenient format.\r\n\r\nInstructions for Use:\r\n1. Enter the text you want to search for in the text box.\r\n2. Click the \"Search\" button to start the search process.\r\n3. You can cancel the search at any time by clicking the \"Cancel Search\" button.\r\n4. To stop and resume the search, use the \"Stop/Continue Search\" button.\r\n5. To change the directory where the found files will be saved, click the \"Change Way to Save\" button and select a new directory.\r\n6. To generate a report of the found files, click the \"Generate Report\" button and choose a location to save the report.\r\n\r\nWe provide this program to make it easier for you to work with files and find the information you need.\r\n\r\nThank you for using our program!\r\n\r\nSincerely,\r\n[Text searcher]\r\n";

                taskDialog.StandardButtons = TaskDialogStandardButtons.Close;

                TaskDialogResult result = taskDialog.Show();

                if (result == TaskDialogResult.Close)
                {
                    // Користувач закрив діалогове вікно.
                }
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

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
                        await ErrorHandling.CatchExToLog("User report path was`t set");
                    }
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }

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





        //Start Button
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
        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
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
        //Import and start
        private async void button1_ClickAsync(object sender, EventArgs e)
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
                    await FromImport();

                }
                else
                {
                    MessageBox.Show("Select file from list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }
        // exit button
        private async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    DialogResult result = MessageBox.Show("Exit program?", "Exit", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        Application.Exit();
                    }
                    else if (result == DialogResult.No)
                    {
                    }
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }
        // Browse button
        private async void button4_Click(object sender, EventArgs e)
        {
            try
            {
                await Task.Run(() =>
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
                });
            }
            catch (Exception error)
            {
                await ErrorHandling.CatchExToLog(error);
            }
        }


        // CancelSearchButton
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
        //Stop&ContinueSearch
        private async void button6_Click(object sender, EventArgs e)
        {
            try
            {
                button5.Enabled = true;
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
                button5.Enabled = false;
            }
        }


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
        //Continue button
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
    }
}
