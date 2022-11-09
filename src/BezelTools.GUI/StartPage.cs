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

        private void buttonCheckPathOverlays_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxCheckPathOverlays.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonCheckPathRoms_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxCheckPathRoms.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void buttonCheckPathTemplateOverlay_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxCheckPathTemplateOverlay.Text = openFileDialog.FileName;
            }
        }

        private void buttonCheckPathTemplateRom_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxCheckPathTemplateRom.Text = openFileDialog.FileName;
            }
        }

        private void buttonDebugBrowse_Click(object sender, EventArgs e)
        {
            var result = folderBrowserDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                textBoxDebugFiles.Text = folderBrowserDialog.SelectedPath;
            }
        }

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

        private void buttonResolution1080p_Click(object sender, EventArgs e) => textBoxTargetResolution.Text = "1920x1080";

        private void buttonResolution720p_Click(object sender, EventArgs e) => textBoxTargetResolution.Text = "1280x720";

        private void buttonStartCheck_Click(object sender, EventArgs e)
        {
            CheckOptions options = new()
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

            Initializer.InitCommon(options);
            Initializer.InitCheck(options);

            Checker.Check(options);
        }
    }
}