using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Principal;

namespace Ex
{
    public partial class Form1 : Form
    {

        private volatile bool cancelSearch = false;
        private object searchLock = new object();

        private ComboBox comboBoxWords;
        private Label resultsLabel;
        private Button pauseButton;
        private Button exportButton;
        private Button openFolderButton;
        private Button saveReportButton;
        private string TextBoxMessege;
        private const string LogTXT = "Log.txt";

        public Form1()
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


            InitializeComponent();
            
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            TextBoxInit();
            OnForm();
            MenuStripInit();
        }
        /*-----------------------------------------------------------------*/
        //TextBox1 Init
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void TextBoxInit()
        {
            try
            {
                TextBoxMessege = "Enter your text here...";
                textBox1.Text = TextBoxMessege;
                textBox1.Enter += TextBox1_Enter1;
                textBox1.Leave += TextBox1_Leave;
                TextBox1_Leave(textBox1, null);
                TextBox1_Leave(textBox1, null);
            }
            catch (Exception error)
            {

                CatchExToLog(error);
            }
        }
        /*-----------------------------------------------------------------*/


        private void importMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exportMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
        }
        public void OnForm()
        {


            comboBoxWords = new ComboBox();
            comboBoxWords.Location = new Point(10, 120);
            comboBoxWords.Size = new Size(200, 30);
            this.Controls.Add(comboBoxWords);



            resultsLabel = new Label();
            resultsLabel.Location = new Point(10, 160);
            resultsLabel.Size = new Size(300, 100);
            resultsLabel.Text = "Results will be displayed here.";
            this.Controls.Add(resultsLabel);


        }
        /*-----------------------------------------------------------------*/
        //MenuStrip
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MenuStripInit()
        {
            try
            {
                MenuStrip menuStrip = new MenuStrip();

                ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("File");
                ToolStripMenuItem editMenuItem = new ToolStripMenuItem("View");
                ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("About");

                menuStrip.Items.Add(fileMenuItem);
                menuStrip.Items.Add(editMenuItem);
                menuStrip.Items.Add(helpMenuItem);

                this.MainMenuStrip = menuStrip;

                this.Controls.Add(menuStrip);
            }
            catch (Exception error)
            {

                CatchExToLog(error);
            }
        }
        /*-----------------------------------------------------------------*/


        /*-----------------------------------------------------------------*/
        //Search&CopyFiles
        private void SearchFilesAndDirectories(string directoryWhere, string wordsToSearch)
        {
            directoryWhere = "E:\\TEST";
            try
            {
                string[] files = Directory.GetFiles(directoryWhere);
                int totalFiles = files.Length;
                int processedFiles = 0;

                foreach (string file in files)
                {
                    listBox1.Items.Add(file);
                    if (cancelSearch)
                    {
                        return;
                    }

                    string fileContents = File.ReadAllText(file);
                    if (fileContents.Contains(wordsToSearch))
                    {
                        string path_to = Directory.GetCurrentDirectory() + '\\';
                        CopyAndModifyFile(directoryWhere, path_to, wordsToSearch);
                        MessageBox.Show("Dire bef in Search^ " + directoryWhere);
                    }

                    processedFiles++;
                    UpdateProgressBar(processedFiles, totalFiles);
                }

                string[] subDirectories = Directory.GetDirectories(directoryWhere);
                foreach (string subDirectory in subDirectories)
                {
                    SearchFilesAndDirectories(subDirectory, wordsToSearch);
                }
            }
            catch (Exception ex)
            {
                CatchExToLog(ex);
            }
        }
        private void CopyAndModifyFile(string directoryWhere, string path_to, string wordsToSearch)
        {
            try
            {

                MessageBox.Show("dir in copy show: " + directoryWhere);

                string fileName = Path.GetFileName(directoryWhere);
                // string patht = "E:\\TEST\\123.txt";
                string ResPath = Path.Combine(path_to, fileName);
                File.Copy(directoryWhere, ResPath, true);

                string fileContents = File.ReadAllText(ResPath);

                fileContents = fileContents.Replace(wordsToSearch, "*******");

                File.WriteAllText(ResPath, fileContents);

            }
            catch (Exception ex)
            {
                CatchExToLog(ex);
            }
        }
        /*-----------------------------------------------------------------*/


        /*-----------------------------------------------------------------*/
        //ProgeressBar
        private void UpdateProgressBar(int processedFiles, int totalFiles)
        {
            try
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action<int, int>(UpdateProgressBar), processedFiles, totalFiles);
                    return;
                }

                progressBar1.Value = (int)(((double)processedFiles / totalFiles) * 100);
                if (progressBar1.Value == progressBar1.Maximum)
                {
                    MessageBox.Show("Finished!");
                    progressBar1.Value = 0;
                }
            }
            catch (Exception error)
            {

                CatchExToLog(error);
            }
        }
        /*-----------------------------------------------------------------*/


        /*-----------------------------------------------------------------*/
        //Exception procesing
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CatchExToLog(Exception error)
        {
            try
            {

                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    log.WriteLine(error.ToString());
                }
            }
            catch (Exception serror)
            {
                using (StreamWriter log = new StreamWriter(LogTXT, true))
                {
                    log.WriteLine(serror.ToString());
                }
            }
        }
        /*-----------------------------------------------------------------*/

        /*-----------------------------------------------------------------*/

        private void exportButton_Click(object sender, EventArgs e)
        {

        }
        private void importButton_Click(object sender, EventArgs e)
        {

        }
        /*-----------------------------------------------------------------*/

        /*-----------------------------------------------------------------*/
        //exit button
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
        //start button
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                cancelSearch = false;
                string wSearch = textBox1.Text;
                DriveInfo[] allDrives = DriveInfo.GetDrives();
                Thread searchThread = new Thread(() =>
                {

                    foreach (DriveInfo drive in allDrives)
                    {
                        if (drive.IsReady)
                        {
                            string rootDirectory = drive.RootDirectory.FullName;
                            SearchFilesAndDirectories(rootDirectory, wSearch);
                            //MessageBox.Show(rootDirectory);
                        }
                    }
                });

                searchThread.Start();
            }
            catch (Exception error)
            {

                CatchExToLog(error);
            }
        }
        //Browse button
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
        /*-----------------------------------------------------------------*/

        /*-----------------------------------------------------------------*/
        //TextBox
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
        //TextBox
        private void TextBox1_Enter1(object? sender, EventArgs e)
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
        //StopSearchButton
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                cancelSearch = true;

            }
            catch (Exception error)
            {

                CatchExToLog(error);            }
        }
        /*-----------------------------------------------------------------*/

    }
}