using BezelTools.Options;
using System.ComponentModel;

namespace BezelTools.GUI
{
    /// <summary>
    /// The starting page form
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form"/>
    public partial class StartPage : Form
    {
        private ProgressForm? progressForm;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartPage"/> class.
        /// </summary>
        public StartPage()
        {
            InitializeComponent();

            SetupButtons();

            // default dialog parameters
            this.openFileDialog.Multiselect = false;
            this.openFileDialog.CheckFileExists = true;
            this.openFileDialog.CheckPathExists = true;
            this.folderBrowserDialog.ShowNewFolderButton = true;
            this.saveFileDialog.OverwritePrompt = true;

            // setup handlers
            Interaction.Log = (message) => { };
            Interaction.Ask = Ask;
            ThreadUtils.RunAsync = RunAsync;
        }

        private int Ask(string question, string[] answers)
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

        private void RunAsync(int threadsNb, IEnumerable<string> files, Action<string> action)
        {
            progressForm = new ProgressForm();

            // event handler for "done" status stackoverflow.com/a/1333948/6776
            var doneEvent = new AutoResetEvent(false);

            var worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            progressForm.InitProgress(files.Count());

            // handle process
            worker.DoWork += (_, e) =>
            {
                try
                {
                    for (int i = 0; i < files.Count(); i++)
                    {
                        var file = files.ElementAt(i);

                        worker.ReportProgress(i, file);

                        action(file);
                    }
                }
                finally
                {
                    doneEvent.Set();
                }
            };

            // handle progress
            worker.ProgressChanged += (_, e) =>
            {
                progressForm.SetProgress(e.ProgressPercentage);
                progressForm.SetLabel($"Processing {e.UserState}");
            };

            // handle completion
            worker.RunWorkerCompleted += (_, e) =>
            {
                if (e.Cancelled)
                {
                    MessageBox.Show("Process has been cancelled");
                }

                progressForm.SetNext();
            };

            // start process
            worker.RunWorkerAsync();

            // show progression
            progressForm.ShowDialog(this);

            // wait for the done event
            doneEvent.WaitOne();
        }

        private void SetupButtons()
        {
            buttonCheckPathOverlays.Click += (_, _) => { OpenFolder(textBoxCheckPathOverlays); };
            buttonCheckPathRoms.Click += (_, _) => { OpenFolder(textBoxCheckPathRoms); };
            buttonCheckPathTemplateOverlay.Click += (_, _) => { OpenFile(textBoxCheckPathTemplateOverlay); };
            buttonCheckPathTemplateRom.Click += (_, _) => { OpenFile(textBoxCheckPathTemplateRom); };
            buttonDebugBrowse.Click += (_, _) => { OpenFolder(textBoxDebugFiles); };
            buttonGenerateImages.Click += (_, _) => { OpenFolder(textBoxGenerateImages); };
            buttonGenerateOverlayTemplate.Click += (_, _) => { OpenFile(textBoxGenerateOverlayTemplate); };
            buttonGenerateRoms.Click += (_, _) => { OpenFolder(textBoxGenerateRoms); };
            buttonGenerateRomTemplate.Click += (_, _) => { OpenFile(textBoxGenerateRomTemplate); };
            buttonMtrOutOverlays.Click += (_, _) => { OpenFolder(textBoxMtrOutOverlays); };
            buttonMtrOutRoms.Click += (_, _) => { OpenFolder(textBoxMtrOutRoms); };
            buttonMtrSource.Click += (_, _) => { OpenFolder(textBoxMtrSource); };
            buttonMtrSourceConfig.Click += (_, _) => { OpenFolder(textBoxMtrSourceConfig); };
            buttonMtrTemplateOverlay.Click += (_, _) => { OpenFile(textBoxMtrTemplateOverlay); };
            buttonMtrTemplateRom.Click += (_, _) => { OpenFile(textBoxMtrTemplateRom); };
            buttonResolution1080p.Click += (_, _) => { textBoxTargetResolution.Text = "1920x1080"; };
            buttonResolution720p.Click += (_, _) => { textBoxTargetResolution.Text = "1280x720"; };
            buttonRtmOut.Click += (_, _) => { OpenFolder(textBoxRtmOut); };
            buttonRtmSourceOvl.Click += (_, _) => { OpenFolder(textBoxRtmSourceOvl); };
            buttonRtmSourceRoms.Click += (_, _) => { OpenFolder(textBoxRtmSourceRoms); };
            buttonRtmTemplate.Click += (_, _) => { OpenFile(textBoxRtmTemplate); };

            buttonErrorBrowse.Click += (_, _) =>
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
            };
        }

        private void StartCheck(object sender, EventArgs e)
        {
            if (!ValidateCheck())
            {
                MessageBox.Show(this, "Please fill all mandatory fields and try again.");
                return;
            }

            CheckOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = 1,

                // specific
                AutoFix = checkBoxCheckAutofix.Checked,
                ErrorMargin = (int)numericUpDownCheckMargin.Value,
                InputOverlayConfigPathInRomConfig = textBoxCheckPathInRom.Text,
                OverlaysConfigFolder = textBoxCheckPathOverlays.Text,
                RomsConfigFolder = textBoxCheckPathRoms.Text,
                TemplateOverlay = textBoxCheckPathTemplateOverlay.Text,
                TemplateRom = textBoxCheckPathTemplateRom.Text
            };

            Initializer.InitCommon(o);
            if (!Initializer.InitCheck(o)) { return; }

            Checker.Check(o);

            MessageBox.Show(this, "All done !");
        }

        private void StartGenerate(object sender, EventArgs e)
        {
            if (!ValidateGenerate())
            {
                MessageBox.Show(this, "Please fill all mandatory fields and try again.");
                return;
            }

            GenerateOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = 1,

                // specific
                TemplateOverlay = textBoxGenerateOverlayTemplate.Text,
                TemplateRom = textBoxGenerateRomTemplate.Text,
                ImagesFolder = textBoxGenerateImages.Text,
                RomsFolder = textBoxGenerateRoms.Text,
                Overwrite = checkBoxGenerateOverwrite.Checked
            };

            Initializer.InitCommon(o);
            if (!Initializer.InitGenerate(o)) { return; }

            Generator.Generate(o);

            MessageBox.Show(this, "All done !");
        }

        private void StartMameToRetroarch(object sender, EventArgs e)
        {
            if (!ValidateMameToRetroarch())
            {
                MessageBox.Show(this, "Please fill all mandatory fields and try again.");
                return;
            }

            MameToRaOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = 1,

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

            Initializer.InitCommon(o);
            if (!Initializer.InitMameToRa(o)) { return; }

            Converter.ConvertMameToRetroarch(o);

            MessageBox.Show(this, "All done !");
        }

        private void StartRetroarchToMame(object sender, EventArgs e)
        {
            if (!ValidateRetroarchToMame())
            {
                MessageBox.Show(this, "Please fill all mandatory fields and try again.");
                return;
            }

            RaToMameOptions o = new()
            {
                // common
                ErrorFile = textBoxErrorFile.Text,
                Margin = (int)numericUpDownMargin.Value,
                OutputDebug = textBoxDebugFiles.Text,
                TargetResolution = textBoxTargetResolution.Text,
                Threads = 1,

                // specific
                Overwrite = checkBoxRtmOverwrite.Checked,
                ScanBezelForScreenCoordinates = checkBoxMtrScanBezel.Checked,
                SourceConfigs = textBoxMtrSourceConfig.Text,
                Output = textBoxRtmOut.Text,
                SourceRoms = textBoxRtmSourceRoms.Text,
                Template = textBoxRtmTemplate.Text,
                Zip = checkBoxRtmZip.Checked
            };

            Initializer.InitCommon(o);
            if (!Initializer.InitRaToMame(o)) { return; }

            Converter.ConvertRetroarchToMame(o);

            MessageBox.Show(this, "All done !");
        }

        private bool ValidateCheck()
        {
            var result = true;

            result = ValidateCommon() && result;

            result = ValidateIsEmpty(textBoxCheckPathOverlays) && result;
            result = ValidateIsEmpty(textBoxCheckPathRoms) && result;

            if (checkBoxCheckAutofix.Checked)
            {
                result = ValidateIsEmpty(textBoxCheckPathTemplateOverlay) && result;
                result = ValidateIsEmpty(textBoxCheckPathTemplateRom) && result;
            }
            else
            {
                errorProvider.SetError(textBoxCheckPathTemplateOverlay, string.Empty);
                errorProvider.SetError(textBoxCheckPathTemplateRom, string.Empty);
            }

            return result;
        }

        private bool ValidateCommon()
        {
            var result = true;

            result = ValidateIsEmpty(textBoxTargetResolution) && result;

            return result;
        }

        private bool ValidateGenerate()
        {
            var result = true;

            result = ValidateCommon() && result;

            result = ValidateIsEmpty(textBoxGenerateImages) && result;
            result = ValidateIsEmpty(textBoxGenerateRoms) && result;
            result = ValidateIsEmpty(textBoxGenerateOverlayTemplate) && result;
            result = ValidateIsEmpty(textBoxGenerateRomTemplate) && result;

            return result;
        }

        private bool ValidateIsEmpty(TextBox control)
        {
            if (string.IsNullOrEmpty(control.Text))
            {
                errorProvider.SetError(control, "Enter a value");
                errorProvider.SetIconPadding(control, -(errorProvider.Icon.Width + control.Bounds.Width - control.ClientRectangle.Width));
                return false;
            }
            else
            {
                errorProvider.SetError(control, string.Empty);
                return true;
            }
        }

        private bool ValidateMameToRetroarch()
        {
            var result = true;

            result = ValidateCommon() && result;

            result = ValidateIsEmpty(textBoxMtrSource) && result;
            result = ValidateIsEmpty(textBoxMtrOutOverlays) && result;
            result = ValidateIsEmpty(textBoxMtrOutRoms) && result;
            result = ValidateIsEmpty(textBoxMtrTemplateOverlay) && result;
            result = ValidateIsEmpty(textBoxMtrTemplateRom) && result;

            return result;
        }

        private bool ValidateRetroarchToMame()
        {
            var result = true;

            result = ValidateCommon() && result;

            result = ValidateIsEmpty(textBoxRtmSourceRoms) && result;
            result = ValidateIsEmpty(textBoxRtmSourceOvl) && result;
            result = ValidateIsEmpty(textBoxRtmOut) && result;
            result = ValidateIsEmpty(textBoxRtmTemplate) && result;

            return result;
        }
    }
}