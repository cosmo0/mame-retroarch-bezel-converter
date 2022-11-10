using BezelTools.Options;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace BezelTools.GUI
{
    public partial class StartPage : Form
    {
        private LogPopup logPopup = new();

        public StartPage()
        {
            InitializeComponent();

            this.openFileDialog.Multiselect = false;
            this.openFileDialog.CheckFileExists = true;
            this.openFileDialog.CheckPathExists = true;

            this.folderBrowserDialog.ShowNewFolderButton = true;

            this.saveFileDialog.OverwritePrompt = true;

            Interaction.Log = logPopup.Log;
            Interaction.Ask = (question, answers) =>
            {
                int chosen = 0;
                var qp = new QuestionPopup();
                qp.SetQuestion(question, answers);
                qp.AnswerPicked += (index) =>
                {
                    chosen = index;
                };
                qp.ShowDialog();

                return chosen;
            };
        }

        private void buttonCheckPathOverlays_Click(object sender, EventArgs e) => OpenFolder(textBoxCheckPathOverlays);

        private void buttonCheckPathRoms_Click(object sender, EventArgs e) => OpenFolder(textBoxCheckPathRoms);

        private void buttonCheckPathTemplateOverlay_Click(object sender, EventArgs e) => OpenFile(textBoxCheckPathTemplateOverlay);

        private void buttonCheckPathTemplateRom_Click(object sender, EventArgs e) => OpenFile(textBoxCheckPathTemplateRom);

        private void buttonDebugBrowse_Click(object sender, EventArgs e) => OpenFolder(textBoxDebugFiles);

        private void buttonErrorBrowse_Click(object sender, EventArgs e)
        {
            saveFileDialog.Reset();
            saveFileDialog.Filter = "CSV files|*.csv";
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.AddExtension = true;

            var result = saveFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxErrorFile.Text = saveFileDialog.FileName;
            }
        }

        private void buttonGenerateImages_Click(object sender, EventArgs e) => OpenFolder(textBoxGenerateImages);

        private void buttonGenerateOverlayTemplate_Click(object sender, EventArgs e) => OpenFile(textBoxGenerateOverlayTemplate);

        private void buttonGenerateRoms_Click(object sender, EventArgs e) => OpenFolder(textBoxGenerateRoms);

        private void buttonGenerateRomTemplate_Click(object sender, EventArgs e) => OpenFile(textBoxGenerateRomTemplate);

        private void buttonMtrOutOverlays_Click(object sender, EventArgs e) => OpenFolder(textBoxMtrOutOverlays);

        private void buttonMtrOutRoms_Click(object sender, EventArgs e) => OpenFolder(textBoxMtrOutRoms);

        private void buttonMtrSource_Click(object sender, EventArgs e) => OpenFolder(textBoxMtrSource);

        private void buttonMtrSourceConfig_Click(object sender, EventArgs e) => OpenFolder(textBoxMtrSourceConfig);

        private void buttonMtrTemplateOverlay_Click(object sender, EventArgs e) => OpenFile(textBoxMtrTemplateOverlay);

        private void buttonMtrTemplateRom_Click(object sender, EventArgs e) => OpenFile(textBoxMtrTemplateRom);

        private void buttonResolution1080p_Click(object sender, EventArgs e) => textBoxTargetResolution.Text = "1920x1080";

        private void buttonResolution720p_Click(object sender, EventArgs e) => textBoxTargetResolution.Text = "1280x720";

        private void buttonRtmOut_Click(object sender, EventArgs e) => OpenFolder(textBoxRtmOut);

        private void buttonRtmSourceOvl_Click(object sender, EventArgs e) => OpenFolder(textBoxRtmSourceOvl);

        private void buttonRtmSourceRoms_Click(object sender, EventArgs e) => OpenFolder(textBoxRtmSourceRoms);

        private void buttonRtmTemplate_Click(object sender, EventArgs e) => OpenFile(textBoxRtmTemplate);

        private void buttonStartCheck_Click(object sender, EventArgs e)
        {
            CheckOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = (int)numericUpDownThreads.Value,

                // specific
                AutoFix = checkBoxCheckAutofix.Checked,
                ErrorMargin = (int)numericUpDownCheckMargin.Value,
                InputOverlayConfigPathInRomConfig = textBoxCheckPathInRom.Text,
                OverlaysConfigFolder = textBoxCheckPathOverlays.Text,
                RomsConfigFolder = textBoxCheckPathRoms.Text,
                TemplateOverlay = textBoxCheckPathTemplateOverlay.Text,
                TemplateRom = textBoxCheckPathTemplateRom.Text
            };

            logPopup.Reset();
            logPopup.Show(this);

            Initializer.InitCommon(o);
            if (!Initializer.InitCheck(o)) { return; }

            Checker.Check(o);
        }

        private void buttonStartGenerate_Click(object sender, EventArgs e)
        {
            GenerateOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = (int)numericUpDownThreads.Value,

                // specific
                TemplateOverlay = textBoxGenerateOverlayTemplate.Text,
                TemplateRom = textBoxGenerateRomTemplate.Text,
                ImagesFolder = textBoxGenerateImages.Text,
                RomsFolder = textBoxGenerateRoms.Text,
                Overwrite = checkBoxGenerateOverwrite.Checked
            };

            logPopup.Reset();
            logPopup.Show(this);

            Initializer.InitCommon(o);
            if (!Initializer.InitGenerate(o)) { return; }

            Generator.Generate(o);
        }

        private void buttonStartMtr_Click(object sender, EventArgs e)
        {
            MameToRaOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = (int)numericUpDownThreads.Value,

                // specific
                Overwrite = checkBoxMtrOverwrite.Checked,
                OutputOverlays = textBoxMtrOutOverlays.Text,
                OutputRoms = textBoxMtrOutRoms.Text,
                ScanBezelForScreenCoordinates = checkBoxMtrScanBezel.Checked,
                Source = textBoxMtrSource.Text,
                SourceConfigs = textBoxMtrSourceConfig.Text,
                TemplateGameCfg = textBoxMtrTemplateRom.Text,
                TemplateOverlayCfg = textBoxMtrTemplateOverlay.Text,
                UseFirstView = checkBoxMtrUseFirstView.Checked
            };

            logPopup.Reset();
            logPopup.Show(this);

            Initializer.InitCommon(o);
            if (!Initializer.InitMameToRa(o)) { return; }

            Converter.ConvertMameToRetroarch(o);
        }

        private void buttonStartRtm_Click(object sender, EventArgs e)
        {
            RaToMameOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = (int)numericUpDownThreads.Value,

                // specific
                Overwrite = checkBoxRtmOverwrite.Checked,
                ScanBezelForScreenCoordinates = checkBoxMtrScanBezel.Checked,
                SourceConfigs = textBoxMtrSourceConfig.Text,
                Output = textBoxRtmOut.Text,
                SourceRoms = textBoxRtmSourceRoms.Text,
                Template = textBoxRtmTemplate.Text,
                Zip = checkBoxRtmZip.Checked
            };

            logPopup.Reset();
            logPopup.Show(this);

            Initializer.InitCommon(o);
            if (!Initializer.InitRaToMame(o)) { return; }

            Converter.ConvertRetroarchToMame(o);
        }

        private void OpenFile(TextBox target)
        {
            var result = openFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                target.Text = openFileDialog.FileName;
            }
        }

        private void OpenFolder(TextBox target)
        {
            var result = folderBrowserDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                target.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}