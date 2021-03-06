﻿/*
 * Copyright (c) 2016 All Rights Reserved
 * Hugo Honda
 */

using System;
using System.IO;
using System.Windows.Forms;
using StroopTest.Models;
using StroopTest.Views;
using StroopTest.Views.SidebarControls;
using TestPlatform.Views.SidebarUserControls;
using System.Collections.Generic;
using TestPlatform.Views;

namespace StroopTest
{
    public partial class FormMain : Form
    {
        private FolderBrowserDialog folderBrowserDialog1;
        
        /*
         * Valores constantes do programa
         * */
        private static string stroopProgramPath = "StroopTestFiles/prg/";
        private static string reactionProgramPath = "ReactionTestFiles/prg/";
        private static string LSTFOLDERNAME = "/Lst/";
        private static string stroopResultsPath = "StroopTestFiles/data/";
        private static string reactionResultsPath = "ReactionTestFiles/data/";
        private static string BACKUPFOLDERNAME = "/backup/";
        private static string DEFAULTPGRNAME = "padrao";
        private static string DEFAULTUSERNAME = "padrao";
        private static string INSTRUCTIONSFILENAME = "editableInstructions.txt";
        private static string PGRCONFIGHELPFILENAME = "prgConfigHelp.txt";
        private static string INSTRUCTIONSTEXT = "O participante deve ser orientado para execução de forma clara e uniforme entre os experimentares e o grupo de participantes.<br><br>Para o Stroop clássico as instruções básicas praticadas são:<br>'Nesta tarefa você deve falar o nome da cor em que as palavras estão pintadas.'<br>ou<br>'Nesta tarefa você deve ler a palavra apresentada na tela.'";
        private static string TECHTEXT = HelpData.TechnicalInformations;
        private static string HELPTEXT = HelpData.VisualizeHelp;

        /* Variaveis
         */
        private StroopProgram programDefault = new StroopProgram();
        private Control currentPanelContent; //guarda o painel do segundo menu que esta renderizado no momento da execução
        private string testFilesPath;
        public string defaultPath = (Path.GetDirectoryName(Application.ExecutablePath));

        /**
         * Metodo construtor do formulario, cria os diretorios necessarios para o programa caso nao existam
         * */
        public FormMain()
        {
            testFilesPath = defaultPath + "/TestFiles/";
            string stroopTestFilesPath = testFilesPath + "/StroopTestFiles/";
            string reactionTestFilesPath = testFilesPath + "/ReactionTestFiles/";
            string listsPath = testFilesPath + "/Lst/";
            string stroopData = stroopTestFilesPath + "/data/";
            string reactionData = reactionTestFilesPath + "/data/";

            if (!Directory.Exists(testFilesPath))
                Directory.CreateDirectory(testFilesPath);

            // atualizando local de diretório da nova versão da plataforma e exclui a versão antiga do programa
            if (File.Exists(defaultPath + "/StroopTest.exe")) 
            {

                DirectoryInfo directoryOldLst = new DirectoryInfo(defaultPath + "/StroopTestFiles/lst");
                directoryOldLst.MoveTo(listsPath);

                DirectoryInfo directoryOldStroop = new DirectoryInfo(defaultPath + "/StroopTestFiles/");
                directoryOldStroop.MoveTo(stroopTestFilesPath);                

                DirectoryInfo directoryOldData = new DirectoryInfo(defaultPath + "/data");
                directoryOldData.MoveTo(testFilesPath + stroopResultsPath);

                try
                {
                    File.Delete(defaultPath + "/StroopTest.exe");
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }

                }



            if (!Directory.Exists(stroopTestFilesPath))
                Directory.CreateDirectory(stroopTestFilesPath);

            if (!Directory.Exists(reactionTestFilesPath))
                Directory.CreateDirectory(reactionTestFilesPath);

            if (!Directory.Exists(stroopTestFilesPath + stroopProgramPath))
                Directory.CreateDirectory(stroopTestFilesPath + stroopProgramPath);

            if (!Directory.Exists(reactionTestFilesPath + stroopProgramPath))
                Directory.CreateDirectory(reactionTestFilesPath + stroopProgramPath);

            if (!Directory.Exists(listsPath))
                Directory.CreateDirectory(listsPath);

            if (!Directory.Exists(testFilesPath + stroopResultsPath))
                Directory.CreateDirectory(testFilesPath + stroopResultsPath);

            if (!Directory.Exists(testFilesPath + reactionResultsPath))
                Directory.CreateDirectory(testFilesPath + reactionResultsPath);

            if (!Directory.Exists(defaultPath + BACKUPFOLDERNAME))
                Directory.CreateDirectory(defaultPath + BACKUPFOLDERNAME);
            if (!File.Exists(testFilesPath + INSTRUCTIONSFILENAME))
                File.Create(testFilesPath + "editableInstructions.txt").Dispose();
            if (!File.Exists(testFilesPath + PGRCONFIGHELPFILENAME))
                File.Create(testFilesPath + PGRCONFIGHELPFILENAME).Dispose();



            initializeDefaultProgram(); // inicializa programa padrão (cria arquivo programa padrão e listas de palavras e cores padrão)
            InitializeComponent();

            dirPathSL.Text = testFilesPath;
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R) // Ctrl+R - roda teste
            {
                beginStroopTest(executingNameLabel.Text);
            }
            if (e.Control && e.KeyCode == Keys.D) // Ctrl+D - define programa
            {
                defineTest();
            }
            if (e.Control && e.KeyCode == Keys.N) // Ctrl+N - novo programa
            {
                newProgram();
            }
            if (e.Control && e.KeyCode == Keys.H) // Ctrl+H - intruções / ajuda
            {
                showInstructions();
            }
        }


        
        private void newTextColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormWordColorConfig configureList = new FormWordColorConfig(testFilesPath + LSTFOLDERNAME, false);
            try
            {
                this.Controls.Add(configureList);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox aboutWindow = new AboutBox();
            try { aboutWindow.Show(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void instructionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showInstructions();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void defineProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            defineTest();
        }
        
        private void dirPathSL_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                dirPathSL.Text = folderBrowserDialog1.SelectedPath;
            }
            testFilesPath = dirPathSL.Text;
        }

        private void newImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImgConfig configureImagesList = new FormImgConfig(testFilesPath + LSTFOLDERNAME, "false");
            try
            {
                this.Controls.Add(configureImagesList);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void initializeDefaultProgram() // inicializa programDefault padrão
        {
            programDefault.UserName = DEFAULTPGRNAME;
            programDefault.ProgramName = DEFAULTUSERNAME;
            try
            {
                programDefault.writeDefaultProgramFile(testFilesPath + stroopProgramPath + programDefault.ProgramName + ".prg"); // ao inicializar formulario escreve arquivo programa padrao
                StrList.writeDefaultWordsList(testFilesPath + LSTFOLDERNAME); // escreve lista de palavras padrão
                StrList.writeDefaultColorsList(testFilesPath + LSTFOLDERNAME); // escreve lista de cores padrão 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private void editProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editTextColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormWordColorConfig configureList = new FormWordColorConfig(testFilesPath + LSTFOLDERNAME, true);
            try
            {
                this.Controls.Add(configureList);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void deleteDataFile_ToolStrip_Click(object sender, EventArgs e)
        {
            moveFileToBackup(testFilesPath + stroopResultsPath, defaultPath + BACKUPFOLDERNAME, "txt");
        }

        private void deleteProgramFile_ToolStrip_Click(object sender, EventArgs e)
        {
            moveFileToBackup(testFilesPath + stroopProgramPath, defaultPath + BACKUPFOLDERNAME, "prg");
        }

        private void deleteListFile_ToolStrip_Click(object sender, EventArgs e)
        {
            moveFileToBackup(testFilesPath + LSTFOLDERNAME, defaultPath + BACKUPFOLDERNAME, "lst");
        }

        private void moveFileToBackup (string originPath, string backupPath, string fileType)
        {
            try
            {
                FormDefine defineFilePath = new FormDefine("Excluir: ", originPath, fileType, "_image_words_color_audio", true);
                var result = defineFilePath.ShowDialog();
                if (result == DialogResult.OK)
                {
                    DialogResult dialogResult = MessageBox.Show("Deseja realmente excluir " + defineFilePath.ReturnValue + "?", "", MessageBoxButtons.YesNo); // pergunta se deseja repetir o programa
                    if (dialogResult == DialogResult.Yes)
                    {
                        File.Move(originPath + defineFilePath.ReturnValue + "." + fileType, backupPath + "backup_" + defineFilePath.ReturnValue + "." + fileType);
                        MessageBox.Show(defineFilePath.ReturnValue + ".lst excluída com sucesso!");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void startTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            beginStroopTest(executingNameLabel.Text);
        }

        private void beginStroopTest(string programName)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                int showOnMonitor = 0;
                if (sc.Length > 1)
                {
                    if (sc[0].Bounds == Screen.FromControl(this).Bounds) { showOnMonitor = 1; }
                    if (sc[1].Bounds == Screen.FromControl(this).Bounds) { showOnMonitor = 0; }
                }
                FormExposition f = new FormExposition(programName, participantTextBox.Text, testFilesPath);
                f.StartPosition = FormStartPosition.Manual;
                f.Location = Screen.AllScreens[showOnMonitor].WorkingArea.Location;
                SendKeys.SendWait("i");
                f.Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void beginReactionTest(string programName)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                int showOnMonitor = 0;
                if (sc.Length > 1)
                {
                    if (sc[0].Bounds == Screen.FromControl(this).Bounds) { showOnMonitor = 1; }
                    if (sc[1].Bounds == Screen.FromControl(this).Bounds) { showOnMonitor = 0; }
                }
              //  FormReactExposition f = new FormReactExposition(programName, participantTextBox.Text, testFilesPath);
              //  f.StartPosition = FormStartPosition.Manual;
              // f.Location = Screen.AllScreens[showOnMonitor].WorkingArea.Location;
              // SendKeys.SendWait("i");
              // f.Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        
        private void defineTest()
        {
            FormDefineTest defineTest = new FormDefineTest(testFilesPath, stroopProgramPath, reactionProgramPath);
            try
            {
                var result = defineTest.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string progName = defineTest.returnValues[1];
                    executingNameLabel.Text = progName;
                    executingTypeLabel.Text = defineTest.returnValues[0];
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void newProgram()
        {

            FormPrgConfig configureProgram = new FormPrgConfig();
            configureProgram.Path = testFilesPath;
            this.Controls.Add(configureProgram);
        }

        private void dataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormShowData showData;
            try
            {
                showData = new FormShowData(testFilesPath + stroopResultsPath);
                this.Controls.Add(showData);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        
        private void showInstructions()
        {
            FormInstructions infoBox = new FormInstructions(INSTRUCTIONSTEXT, (testFilesPath + INSTRUCTIONSFILENAME));
            try { infoBox.Show(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void editImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormImgConfig configureImagesList = new FormImgConfig(testFilesPath + "/Lst/", "");
            try { this.Controls.Add(configureImagesList); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void audioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAudioConfig configureAudioList = new FormAudioConfig(testFilesPath + "/Lst/", false);
            try
            {
                this.Controls.Add(configureAudioList);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void techInfoButto_ToolStrip_Click(object sender, EventArgs e)
        {
            FormInstructions infoBox = new FormInstructions(TECHTEXT);
            try { infoBox.Show(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormInstructions infoBox = new FormInstructions(HELPTEXT);
            try { infoBox.Show(); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }
        
        private void editAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAudioConfig configureAudioList = new FormAudioConfig(testFilesPath + "/Lst/", true);
            try
            {
                this.Controls.Add(configureAudioList);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void displayAudiosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormShowAudio showAudio;
            try
            {
                showAudio = new FormShowAudio(testFilesPath + stroopResultsPath);
                showAudio.ShowDialog();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void newAudioToolStripMenu_Click(object sender, EventArgs e)
        {
            FormNewAudio newAudio;
            try
            {
                newAudio = new FormNewAudio();
                this.Controls.Add(newAudio);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void experimentButton_CheckedChanged(object sender, EventArgs e)
        {
            if (experimentButton.Checked)
            {
                if(currentPanelContent != null)
                {
                    Controls.Remove(currentPanelContent);
                }

                ExperimentControl experimentControl = new ExperimentControl();
                this.Controls.Add(experimentControl);
                currentPanelContent = experimentControl;
            }
        }

        private void buttonStroop_CheckedChanged(object sender, EventArgs e)
        {
            
            if (buttonStroop.Checked)
            {
                if (currentPanelContent != null)
                {
                    Controls.Remove(currentPanelContent);
                }
                StroopControl stroopControl = new StroopControl();
                stroopControl.TestFilesPath = testFilesPath;
                this.Controls.Add(stroopControl);
                currentPanelContent = stroopControl;
            }
        }

        private void buttonReaction_Click(object sender, EventArgs e)
        {
            if (buttonReaction.Checked)
            {
                if (currentPanelContent != null)
                {
                    Controls.Remove(currentPanelContent);
                }
                ReactionControl reactControl = new ReactionControl();
                reactControl.TestFilesPath = testFilesPath;
                this.Controls.Add(reactControl);
                currentPanelContent = reactControl;
            }

        }

        private void buttonList_CheckedChanged(object sender, EventArgs e)
        {
            if (buttonList.Checked)
            {
                if (currentPanelContent != null)
                {
                    Controls.Remove(currentPanelContent);
                }
                ListUserControl listControl = new ListUserControl();
                listControl.TestFilesPath = testFilesPath;
                this.Controls.Add(listControl);
                currentPanelContent = listControl;
            }
        }

        private void resultButton_CheckedChanged(object sender, EventArgs e)
        {
            if (resultButton.Checked)
            {
                if (currentPanelContent != null)
                {
                    Controls.Remove(currentPanelContent);
                }
                FormShowData showData;
                try
                {
                    showData = new FormShowData(testFilesPath + stroopResultsPath);
                    this.Controls.Add(showData);
                    currentPanelContent = showData;
                    resultButton.Checked = false;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }


        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            List<Control> controls = new List<Control>();

            if (e.Control.GetType().BaseType == typeof(UserControl))
            {
                int count = 0;
                foreach (Control c in Controls)
                {
                    if (!(c.Equals(currentPanelContent)) && c is UserControl)
                    {
                        controls.Add(c);
                        count++;
                    }
                }
                if (count > 1)
                {
                    Controls.Remove(controls[0]);

                }
            }
        }

        private void stroopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormPrgConfig configureProgram = new FormPrgConfig();
            configureProgram.Path = testFilesPath;
            this.Controls.Add(configureProgram);
        }

        private void reactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTRConfig configureProgram = new FormTRConfig();
            configureProgram.Path = testFilesPath;
            this.Controls.Add(configureProgram);
        }

        private void stroopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FormDefine defineProgram;
            DialogResult result;
            string editProgramName = "error";

            try
            {
                defineProgram = new FormDefine("Editar Programa: ", testFilesPath + "/prg/", "prg", "program", false);
                result = defineProgram.ShowDialog();
                if (result == DialogResult.OK)
                {
                    editProgramName = defineProgram.ReturnValue;
                    FormPrgConfig configureProgram = new FormPrgConfig();
                    configureProgram.Path = testFilesPath;
                    configureProgram.PrgName = editProgramName;
                    this.Controls.Add(configureProgram);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void reactionToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void executeButton_Click(object sender, EventArgs e)
        {
            if (executingTypeLabel.Text.Equals("StroopTest"))
            {
                beginStroopTest(executingNameLabel.Text);
            }

            else if (executingTypeLabel.Text.Equals("ReactionTest"))
            {
                beginReactionTest(executingNameLabel.Text);
            }
            
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
                defineTest();            
        }
    }
}
