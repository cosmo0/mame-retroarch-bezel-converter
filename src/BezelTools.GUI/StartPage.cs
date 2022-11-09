using BezelTools.Options;

namespace BezelTools.GUI
{
    public partial class StartPage : Form
    {
        public StartPage()
        {
            InitializeComponent();

            this.openFileDialog.Multiselect = false;
            this.openFileDialog.CheckFileExists = true;
            this.openFileDialog.CheckPathExists = true;

            this.folderBrowserDialog.ShowNewFolderButton = true;

            this.saveFileDialog.OverwritePrompt = true;
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

        private void buttonStartCheck_Click(object sender, EventArgs e)
        {
            CheckOptions o = new()
            {
                AutoFix = checkBoxCheckAutofix.Checked,
                ErrorFile = textBoxErrorFile.Text,
                ErrorMargin = (int)numericUpDownCheckMargin.Value,
                InputOverlayConfigPathInRomConfig = textBoxCheckPathInRom.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                OverlaysConfigFolder = textBoxCheckPathOverlays.Text,
                RomsConfigFolder = textBoxCheckPathRoms.Text,
                TargetResolution = textBoxTargetResolution.Text,
                TemplateOverlay = textBoxCheckPathTemplateOverlay.Text,
                TemplateRom = textBoxCheckPathTemplateRom.Text,
                Threads = (int)numericUpDownThreads.Value
            };

            Initializer.InitCommon(o);
            Initializer.InitCheck(o);

            Checker.Check(o);
        }

        private void buttonStartGenerate_Click(object sender, EventArgs e)
        {
            GenerateOptions o = new()
            {
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                TemplateOverlay = textBoxCheckPathTemplateOverlay.Text,
                TemplateRom = textBoxCheckPathTemplateRom.Text,
                Threads = (int)numericUpDownThreads.Value,
                ImagesFolder = textBoxGenerateImages.Text,
                RomsFolder = textBoxGenerateRoms.Text,
                Overwrite = checkBoxGenerateOverwrite.Checked
            };

            Initializer.InitCommon(o);
            Initializer.InitGenerate(o);

            Generator.Generate(o);
        }

        private void buttonStartMtr_Click(object sender, EventArgs e)
        {
            MameToRaOptions o = new()
            {
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = (int)numericUpDownThreads.Value,
                Overwrite = checkBoxGenerateOverwrite.Checked,
                OutputOverlays = textBoxMtrOutOverlays.Text,
                OutputRoms = textBoxMtrOutRoms.Text,
                ScanBezelForScreenCoordinates = checkBoxMtrScanBezel.Checked,
                Source = textBoxMtrSource.Text,
                SourceConfigs = textBoxMtrSourceConfig.Text,
                TemplateGameCfg = textBoxMtrTemplateRom.Text,
                TemplateOverlayCfg = textBoxMtrTemplateOverlay.Text,
                UseFirstView = checkBoxMtrUseFirstView.Checked
            };

            Initializer.InitCommon(o);
            Initializer.InitMameToRa(o);

            Converter.ConvertMameToRetroarch(o);
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