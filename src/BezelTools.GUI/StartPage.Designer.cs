namespace BezelTools.GUI
{
    partial class StartPage
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartPage));
			this.panelCommon = new System.Windows.Forms.Panel();
			this.buttonDebugBrowse = new System.Windows.Forms.Button();
			this.buttonErrorBrowse = new System.Windows.Forms.Button();
			this.numericUpDownThreads = new System.Windows.Forms.NumericUpDown();
			this.numericUpDownMargin = new System.Windows.Forms.NumericUpDown();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.labelMargin = new System.Windows.Forms.Label();
			this.labelDebug = new System.Windows.Forms.Label();
			this.labelErrorFile = new System.Windows.Forms.Label();
			this.buttonExecute = new System.Windows.Forms.Button();
			this.tabControlActions = new System.Windows.Forms.TabControl();
			this.tabPageCheck = new System.Windows.Forms.TabPage();
			this.pictureBoxCheckOutputDebug = new System.Windows.Forms.PictureBox();
			this.pictureBoxCheckTemplateRom = new System.Windows.Forms.PictureBox();
			this.pictureBoxCheckTemplateOverlay = new System.Windows.Forms.PictureBox();
			this.pictureBoxCheckExpectedPath = new System.Windows.Forms.PictureBox();
			this.pictureBoxCheckPathRom = new System.Windows.Forms.PictureBox();
			this.pictureBoxCheckPathOverlay = new System.Windows.Forms.PictureBox();
			this.buttonCheckPathDebugOutput = new System.Windows.Forms.Button();
			this.buttonCheckPathTemplateRom = new System.Windows.Forms.Button();
			this.buttonCheckPathTemplateOverlay = new System.Windows.Forms.Button();
			this.buttonCheckPathRoms = new System.Windows.Forms.Button();
			this.buttonCheckPathOverlays = new System.Windows.Forms.Button();
			this.textBoxCheckPathRoms = new System.Windows.Forms.TextBox();
			this.textBoxCheckPathOverlays = new System.Windows.Forms.TextBox();
			this.labelCheckFilesRoms = new System.Windows.Forms.Label();
			this.labelCheckFilesOverlays = new System.Windows.Forms.Label();
			this.textBoxCheckDebugOutput = new System.Windows.Forms.TextBox();
			this.labelCheckOutputDebug = new System.Windows.Forms.Label();
			this.textBoxCheckPathInRom = new System.Windows.Forms.TextBox();
			this.textBoxCheckPathTemplateRom = new System.Windows.Forms.TextBox();
			this.textBoxCheckPathTemplateOverlay = new System.Windows.Forms.TextBox();
			this.labelCheckTemplateRom = new System.Windows.Forms.Label();
			this.labelCheckTemplateOverlay = new System.Windows.Forms.Label();
			this.labelCheckInputOverlay = new System.Windows.Forms.Label();
			this.radioButtonCheckAndFix = new System.Windows.Forms.RadioButton();
			this.radioButtonCheckOnly = new System.Windows.Forms.RadioButton();
			this.tabPageGenerate = new System.Windows.Forms.TabPage();
			this.tabPageConvertMAMEtoRA = new System.Windows.Forms.TabPage();
			this.tabPageConvertRAtoMAME = new System.Windows.Forms.TabPage();
			this.toolTipInfo = new System.Windows.Forms.ToolTip(this.components);
			this.panelCommon.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreads)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMargin)).BeginInit();
			this.tabControlActions.SuspendLayout();
			this.tabPageCheck.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckOutputDebug)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckTemplateRom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckTemplateOverlay)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckExpectedPath)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckPathRom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckPathOverlay)).BeginInit();
			this.SuspendLayout();
			// 
			// panelCommon
			// 
			this.panelCommon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panelCommon.Controls.Add(this.buttonDebugBrowse);
			this.panelCommon.Controls.Add(this.buttonErrorBrowse);
			this.panelCommon.Controls.Add(this.numericUpDownThreads);
			this.panelCommon.Controls.Add(this.numericUpDownMargin);
			this.panelCommon.Controls.Add(this.textBox4);
			this.panelCommon.Controls.Add(this.textBox3);
			this.panelCommon.Controls.Add(this.label1);
			this.panelCommon.Controls.Add(this.labelMargin);
			this.panelCommon.Controls.Add(this.labelDebug);
			this.panelCommon.Controls.Add(this.labelErrorFile);
			this.panelCommon.Controls.Add(this.buttonExecute);
			this.panelCommon.Location = new System.Drawing.Point(12, 465);
			this.panelCommon.Name = "panelCommon";
			this.panelCommon.Size = new System.Drawing.Size(793, 174);
			this.panelCommon.TabIndex = 1;
			// 
			// buttonDebugBrowse
			// 
			this.buttonDebugBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonDebugBrowse.Location = new System.Drawing.Point(708, 101);
			this.buttonDebugBrowse.Name = "buttonDebugBrowse";
			this.buttonDebugBrowse.Size = new System.Drawing.Size(75, 23);
			this.buttonDebugBrowse.TabIndex = 9;
			this.buttonDebugBrowse.Text = "...";
			this.buttonDebugBrowse.UseVisualStyleBackColor = true;
			// 
			// buttonErrorBrowse
			// 
			this.buttonErrorBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonErrorBrowse.Location = new System.Drawing.Point(708, 70);
			this.buttonErrorBrowse.Name = "buttonErrorBrowse";
			this.buttonErrorBrowse.Size = new System.Drawing.Size(75, 23);
			this.buttonErrorBrowse.TabIndex = 6;
			this.buttonErrorBrowse.Text = "...";
			this.buttonErrorBrowse.UseVisualStyleBackColor = true;
			// 
			// numericUpDownThreads
			// 
			this.numericUpDownThreads.AllowDrop = true;
			this.numericUpDownThreads.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericUpDownThreads.Location = new System.Drawing.Point(265, 43);
			this.numericUpDownThreads.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.numericUpDownThreads.Name = "numericUpDownThreads";
			this.numericUpDownThreads.Size = new System.Drawing.Size(55, 23);
			this.numericUpDownThreads.TabIndex = 3;
			this.numericUpDownThreads.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
			// 
			// numericUpDownMargin
			// 
			this.numericUpDownMargin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.numericUpDownMargin.Location = new System.Drawing.Point(265, 14);
			this.numericUpDownMargin.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
			this.numericUpDownMargin.Name = "numericUpDownMargin";
			this.numericUpDownMargin.Size = new System.Drawing.Size(55, 23);
			this.numericUpDownMargin.TabIndex = 1;
			// 
			// textBox4
			// 
			this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox4.Location = new System.Drawing.Point(265, 100);
			this.textBox4.Name = "textBox4";
			this.textBox4.Size = new System.Drawing.Size(437, 23);
			this.textBox4.TabIndex = 8;
			// 
			// textBox3
			// 
			this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox3.Location = new System.Drawing.Point(265, 71);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(437, 23);
			this.textBox3.TabIndex = 5;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(152, 45);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(107, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "Number of threads";
			// 
			// labelMargin
			// 
			this.labelMargin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelMargin.AutoSize = true;
			this.labelMargin.Location = new System.Drawing.Point(36, 16);
			this.labelMargin.Name = "labelMargin";
			this.labelMargin.Size = new System.Drawing.Size(223, 15);
			this.labelMargin.TabIndex = 0;
			this.labelMargin.Text = "Margin to add to the screen position (px)";
			// 
			// labelDebug
			// 
			this.labelDebug.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelDebug.AutoSize = true;
			this.labelDebug.Location = new System.Drawing.Point(198, 103);
			this.labelDebug.Name = "labelDebug";
			this.labelDebug.Size = new System.Drawing.Size(61, 15);
			this.labelDebug.TabIndex = 7;
			this.labelDebug.Text = "Debug file";
			// 
			// labelErrorFile
			// 
			this.labelErrorFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelErrorFile.AutoSize = true;
			this.labelErrorFile.Location = new System.Drawing.Point(208, 74);
			this.labelErrorFile.Name = "labelErrorFile";
			this.labelErrorFile.Size = new System.Drawing.Size(51, 15);
			this.labelErrorFile.TabIndex = 4;
			this.labelErrorFile.Text = "Error file";
			// 
			// buttonExecute
			// 
			this.buttonExecute.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonExecute.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.buttonExecute.Location = new System.Drawing.Point(4, 142);
			this.buttonExecute.Name = "buttonExecute";
			this.buttonExecute.Size = new System.Drawing.Size(779, 29);
			this.buttonExecute.TabIndex = 10;
			this.buttonExecute.Text = "Start";
			this.buttonExecute.UseVisualStyleBackColor = true;
			// 
			// tabControlActions
			// 
			this.tabControlActions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlActions.Controls.Add(this.tabPageCheck);
			this.tabControlActions.Controls.Add(this.tabPageGenerate);
			this.tabControlActions.Controls.Add(this.tabPageConvertMAMEtoRA);
			this.tabControlActions.Controls.Add(this.tabPageConvertRAtoMAME);
			this.tabControlActions.Location = new System.Drawing.Point(12, 12);
			this.tabControlActions.Name = "tabControlActions";
			this.tabControlActions.SelectedIndex = 0;
			this.tabControlActions.Size = new System.Drawing.Size(793, 447);
			this.tabControlActions.TabIndex = 0;
			// 
			// tabPageCheck
			// 
			this.tabPageCheck.Controls.Add(this.pictureBoxCheckOutputDebug);
			this.tabPageCheck.Controls.Add(this.pictureBoxCheckTemplateRom);
			this.tabPageCheck.Controls.Add(this.pictureBoxCheckTemplateOverlay);
			this.tabPageCheck.Controls.Add(this.pictureBoxCheckExpectedPath);
			this.tabPageCheck.Controls.Add(this.pictureBoxCheckPathRom);
			this.tabPageCheck.Controls.Add(this.pictureBoxCheckPathOverlay);
			this.tabPageCheck.Controls.Add(this.buttonCheckPathDebugOutput);
			this.tabPageCheck.Controls.Add(this.buttonCheckPathTemplateRom);
			this.tabPageCheck.Controls.Add(this.buttonCheckPathTemplateOverlay);
			this.tabPageCheck.Controls.Add(this.buttonCheckPathRoms);
			this.tabPageCheck.Controls.Add(this.buttonCheckPathOverlays);
			this.tabPageCheck.Controls.Add(this.textBoxCheckPathRoms);
			this.tabPageCheck.Controls.Add(this.textBoxCheckPathOverlays);
			this.tabPageCheck.Controls.Add(this.labelCheckFilesRoms);
			this.tabPageCheck.Controls.Add(this.labelCheckFilesOverlays);
			this.tabPageCheck.Controls.Add(this.textBoxCheckDebugOutput);
			this.tabPageCheck.Controls.Add(this.labelCheckOutputDebug);
			this.tabPageCheck.Controls.Add(this.textBoxCheckPathInRom);
			this.tabPageCheck.Controls.Add(this.textBoxCheckPathTemplateRom);
			this.tabPageCheck.Controls.Add(this.textBoxCheckPathTemplateOverlay);
			this.tabPageCheck.Controls.Add(this.labelCheckTemplateRom);
			this.tabPageCheck.Controls.Add(this.labelCheckTemplateOverlay);
			this.tabPageCheck.Controls.Add(this.labelCheckInputOverlay);
			this.tabPageCheck.Controls.Add(this.radioButtonCheckAndFix);
			this.tabPageCheck.Controls.Add(this.radioButtonCheckOnly);
			this.tabPageCheck.Location = new System.Drawing.Point(4, 24);
			this.tabPageCheck.Name = "tabPageCheck";
			this.tabPageCheck.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageCheck.Size = new System.Drawing.Size(785, 419);
			this.tabPageCheck.TabIndex = 0;
			this.tabPageCheck.Text = "Check overlays";
			this.tabPageCheck.UseVisualStyleBackColor = true;
			// 
			// pictureBoxCheckOutputDebug
			// 
			this.pictureBoxCheckOutputDebug.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCheckOutputDebug.Image")));
			this.pictureBoxCheckOutputDebug.Location = new System.Drawing.Point(717, 232);
			this.pictureBoxCheckOutputDebug.Name = "pictureBoxCheckOutputDebug";
			this.pictureBoxCheckOutputDebug.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxCheckOutputDebug.TabIndex = 24;
			this.pictureBoxCheckOutputDebug.TabStop = false;
			// 
			// pictureBoxCheckTemplateRom
			// 
			this.pictureBoxCheckTemplateRom.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCheckTemplateRom.Image")));
			this.pictureBoxCheckTemplateRom.Location = new System.Drawing.Point(717, 203);
			this.pictureBoxCheckTemplateRom.Name = "pictureBoxCheckTemplateRom";
			this.pictureBoxCheckTemplateRom.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxCheckTemplateRom.TabIndex = 23;
			this.pictureBoxCheckTemplateRom.TabStop = false;
			// 
			// pictureBoxCheckTemplateOverlay
			// 
			this.pictureBoxCheckTemplateOverlay.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCheckTemplateOverlay.Image")));
			this.pictureBoxCheckTemplateOverlay.Location = new System.Drawing.Point(717, 174);
			this.pictureBoxCheckTemplateOverlay.Name = "pictureBoxCheckTemplateOverlay";
			this.pictureBoxCheckTemplateOverlay.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxCheckTemplateOverlay.TabIndex = 22;
			this.pictureBoxCheckTemplateOverlay.TabStop = false;
			// 
			// pictureBoxCheckExpectedPath
			// 
			this.pictureBoxCheckExpectedPath.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCheckExpectedPath.Image")));
			this.pictureBoxCheckExpectedPath.Location = new System.Drawing.Point(717, 145);
			this.pictureBoxCheckExpectedPath.Name = "pictureBoxCheckExpectedPath";
			this.pictureBoxCheckExpectedPath.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxCheckExpectedPath.TabIndex = 21;
			this.pictureBoxCheckExpectedPath.TabStop = false;
			// 
			// pictureBoxCheckPathRom
			// 
			this.pictureBoxCheckPathRom.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCheckPathRom.Image")));
			this.pictureBoxCheckPathRom.Location = new System.Drawing.Point(717, 116);
			this.pictureBoxCheckPathRom.Name = "pictureBoxCheckPathRom";
			this.pictureBoxCheckPathRom.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxCheckPathRom.TabIndex = 20;
			this.pictureBoxCheckPathRom.TabStop = false;
			this.toolTipInfo.SetToolTip(this.pictureBoxCheckPathRom, "The folder where your rom config files are located; usually the same folder as th" +
        "e rom files, but it may vary.");
			// 
			// pictureBoxCheckPathOverlay
			// 
			this.pictureBoxCheckPathOverlay.Image = ((System.Drawing.Image)(resources.GetObject("pictureBoxCheckPathOverlay.Image")));
			this.pictureBoxCheckPathOverlay.Location = new System.Drawing.Point(717, 87);
			this.pictureBoxCheckPathOverlay.Name = "pictureBoxCheckPathOverlay";
			this.pictureBoxCheckPathOverlay.Size = new System.Drawing.Size(16, 16);
			this.pictureBoxCheckPathOverlay.TabIndex = 19;
			this.pictureBoxCheckPathOverlay.TabStop = false;
			this.toolTipInfo.SetToolTip(this.pictureBoxCheckPathOverlay, "The folder where your overlays (images and associated .cfg files) are located.");
			// 
			// buttonCheckPathDebugOutput
			// 
			this.buttonCheckPathDebugOutput.Enabled = false;
			this.buttonCheckPathDebugOutput.Location = new System.Drawing.Point(636, 229);
			this.buttonCheckPathDebugOutput.Name = "buttonCheckPathDebugOutput";
			this.buttonCheckPathDebugOutput.Size = new System.Drawing.Size(75, 23);
			this.buttonCheckPathDebugOutput.TabIndex = 18;
			this.buttonCheckPathDebugOutput.Text = "...";
			this.buttonCheckPathDebugOutput.UseVisualStyleBackColor = true;
			// 
			// buttonCheckPathTemplateRom
			// 
			this.buttonCheckPathTemplateRom.Location = new System.Drawing.Point(636, 200);
			this.buttonCheckPathTemplateRom.Name = "buttonCheckPathTemplateRom";
			this.buttonCheckPathTemplateRom.Size = new System.Drawing.Size(75, 23);
			this.buttonCheckPathTemplateRom.TabIndex = 15;
			this.buttonCheckPathTemplateRom.Text = "...";
			this.buttonCheckPathTemplateRom.UseVisualStyleBackColor = true;
			// 
			// buttonCheckPathTemplateOverlay
			// 
			this.buttonCheckPathTemplateOverlay.Location = new System.Drawing.Point(636, 170);
			this.buttonCheckPathTemplateOverlay.Name = "buttonCheckPathTemplateOverlay";
			this.buttonCheckPathTemplateOverlay.Size = new System.Drawing.Size(75, 23);
			this.buttonCheckPathTemplateOverlay.TabIndex = 12;
			this.buttonCheckPathTemplateOverlay.Text = "...";
			this.buttonCheckPathTemplateOverlay.UseVisualStyleBackColor = true;
			// 
			// buttonCheckPathRoms
			// 
			this.buttonCheckPathRoms.Location = new System.Drawing.Point(636, 113);
			this.buttonCheckPathRoms.Name = "buttonCheckPathRoms";
			this.buttonCheckPathRoms.Size = new System.Drawing.Size(75, 23);
			this.buttonCheckPathRoms.TabIndex = 7;
			this.buttonCheckPathRoms.Text = "...";
			this.buttonCheckPathRoms.UseVisualStyleBackColor = true;
			// 
			// buttonCheckPathOverlays
			// 
			this.buttonCheckPathOverlays.Location = new System.Drawing.Point(636, 84);
			this.buttonCheckPathOverlays.Name = "buttonCheckPathOverlays";
			this.buttonCheckPathOverlays.Size = new System.Drawing.Size(75, 23);
			this.buttonCheckPathOverlays.TabIndex = 4;
			this.buttonCheckPathOverlays.Text = "...";
			this.buttonCheckPathOverlays.UseVisualStyleBackColor = true;
			// 
			// textBoxCheckPathRoms
			// 
			this.textBoxCheckPathRoms.Location = new System.Drawing.Point(261, 113);
			this.textBoxCheckPathRoms.Name = "textBoxCheckPathRoms";
			this.textBoxCheckPathRoms.Size = new System.Drawing.Size(369, 23);
			this.textBoxCheckPathRoms.TabIndex = 6;
			// 
			// textBoxCheckPathOverlays
			// 
			this.textBoxCheckPathOverlays.Location = new System.Drawing.Point(261, 84);
			this.textBoxCheckPathOverlays.Name = "textBoxCheckPathOverlays";
			this.textBoxCheckPathOverlays.Size = new System.Drawing.Size(369, 23);
			this.textBoxCheckPathOverlays.TabIndex = 3;
			// 
			// labelCheckFilesRoms
			// 
			this.labelCheckFilesRoms.AutoSize = true;
			this.labelCheckFilesRoms.Location = new System.Drawing.Point(118, 116);
			this.labelCheckFilesRoms.Name = "labelCheckFilesRoms";
			this.labelCheckFilesRoms.Size = new System.Drawing.Size(137, 15);
			this.labelCheckFilesRoms.TabIndex = 5;
			this.labelCheckFilesRoms.Text = "Path to the ROM configs";
			// 
			// labelCheckFilesOverlays
			// 
			this.labelCheckFilesOverlays.AutoSize = true;
			this.labelCheckFilesOverlays.Location = new System.Drawing.Point(96, 87);
			this.labelCheckFilesOverlays.Name = "labelCheckFilesOverlays";
			this.labelCheckFilesOverlays.Size = new System.Drawing.Size(159, 15);
			this.labelCheckFilesOverlays.TabIndex = 2;
			this.labelCheckFilesOverlays.Text = "Path to the overlays to check";
			// 
			// textBoxCheckDebugOutput
			// 
			this.textBoxCheckDebugOutput.Enabled = false;
			this.textBoxCheckDebugOutput.Location = new System.Drawing.Point(261, 229);
			this.textBoxCheckDebugOutput.Name = "textBoxCheckDebugOutput";
			this.textBoxCheckDebugOutput.Size = new System.Drawing.Size(369, 23);
			this.textBoxCheckDebugOutput.TabIndex = 17;
			// 
			// labelCheckOutputDebug
			// 
			this.labelCheckOutputDebug.AutoSize = true;
			this.labelCheckOutputDebug.Enabled = false;
			this.labelCheckOutputDebug.Location = new System.Drawing.Point(80, 232);
			this.labelCheckOutputDebug.Name = "labelCheckOutputDebug";
			this.labelCheckOutputDebug.Size = new System.Drawing.Size(175, 15);
			this.labelCheckOutputDebug.TabIndex = 16;
			this.labelCheckOutputDebug.Text = "Path to the output debug folder";
			// 
			// textBoxCheckPathInRom
			// 
			this.textBoxCheckPathInRom.Location = new System.Drawing.Point(261, 142);
			this.textBoxCheckPathInRom.Name = "textBoxCheckPathInRom";
			this.textBoxCheckPathInRom.Size = new System.Drawing.Size(450, 23);
			this.textBoxCheckPathInRom.TabIndex = 9;
			// 
			// textBoxCheckPathTemplateRom
			// 
			this.textBoxCheckPathTemplateRom.Location = new System.Drawing.Point(261, 200);
			this.textBoxCheckPathTemplateRom.Name = "textBoxCheckPathTemplateRom";
			this.textBoxCheckPathTemplateRom.Size = new System.Drawing.Size(369, 23);
			this.textBoxCheckPathTemplateRom.TabIndex = 14;
			// 
			// textBoxCheckPathTemplateOverlay
			// 
			this.textBoxCheckPathTemplateOverlay.Location = new System.Drawing.Point(261, 171);
			this.textBoxCheckPathTemplateOverlay.Name = "textBoxCheckPathTemplateOverlay";
			this.textBoxCheckPathTemplateOverlay.Size = new System.Drawing.Size(369, 23);
			this.textBoxCheckPathTemplateOverlay.TabIndex = 11;
			// 
			// labelCheckTemplateRom
			// 
			this.labelCheckTemplateRom.AutoSize = true;
			this.labelCheckTemplateRom.Location = new System.Drawing.Point(95, 203);
			this.labelCheckTemplateRom.Name = "labelCheckTemplateRom";
			this.labelCheckTemplateRom.Size = new System.Drawing.Size(160, 15);
			this.labelCheckTemplateRom.TabIndex = 13;
			this.labelCheckTemplateRom.Text = "Template for the ROM config";
			// 
			// labelCheckTemplateOverlay
			// 
			this.labelCheckTemplateOverlay.AutoSize = true;
			this.labelCheckTemplateOverlay.Location = new System.Drawing.Point(121, 174);
			this.labelCheckTemplateOverlay.Name = "labelCheckTemplateOverlay";
			this.labelCheckTemplateOverlay.Size = new System.Drawing.Size(134, 15);
			this.labelCheckTemplateOverlay.TabIndex = 10;
			this.labelCheckTemplateOverlay.Text = "Template for the overlay";
			// 
			// labelCheckInputOverlay
			// 
			this.labelCheckInputOverlay.AutoSize = true;
			this.labelCheckInputOverlay.Location = new System.Drawing.Point(29, 145);
			this.labelCheckInputOverlay.Name = "labelCheckInputOverlay";
			this.labelCheckInputOverlay.Size = new System.Drawing.Size(226, 15);
			this.labelCheckInputOverlay.TabIndex = 8;
			this.labelCheckInputOverlay.Text = "Expected path in the overlay\'s rom config";
			// 
			// radioButtonCheckAndFix
			// 
			this.radioButtonCheckAndFix.AutoSize = true;
			this.radioButtonCheckAndFix.Checked = true;
			this.radioButtonCheckAndFix.Location = new System.Drawing.Point(261, 23);
			this.radioButtonCheckAndFix.Name = "radioButtonCheckAndFix";
			this.radioButtonCheckAndFix.Size = new System.Drawing.Size(163, 19);
			this.radioButtonCheckAndFix.TabIndex = 0;
			this.radioButtonCheckAndFix.TabStop = true;
			this.radioButtonCheckAndFix.Text = "Check and fix the overlays";
			this.radioButtonCheckAndFix.UseVisualStyleBackColor = true;
			// 
			// radioButtonCheckOnly
			// 
			this.radioButtonCheckOnly.AutoSize = true;
			this.radioButtonCheckOnly.Location = new System.Drawing.Point(261, 48);
			this.radioButtonCheckOnly.Name = "radioButtonCheckOnly";
			this.radioButtonCheckOnly.Size = new System.Drawing.Size(267, 19);
			this.radioButtonCheckOnly.TabIndex = 1;
			this.radioButtonCheckOnly.TabStop = true;
			this.radioButtonCheckOnly.Text = "Just check the overlays, and output the results";
			this.radioButtonCheckOnly.UseVisualStyleBackColor = true;
			// 
			// tabPageGenerate
			// 
			this.tabPageGenerate.Location = new System.Drawing.Point(4, 24);
			this.tabPageGenerate.Name = "tabPageGenerate";
			this.tabPageGenerate.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageGenerate.Size = new System.Drawing.Size(785, 419);
			this.tabPageGenerate.TabIndex = 1;
			this.tabPageGenerate.Text = "Generate from images";
			this.tabPageGenerate.UseVisualStyleBackColor = true;
			// 
			// tabPageConvertMAMEtoRA
			// 
			this.tabPageConvertMAMEtoRA.Location = new System.Drawing.Point(4, 24);
			this.tabPageConvertMAMEtoRA.Name = "tabPageConvertMAMEtoRA";
			this.tabPageConvertMAMEtoRA.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageConvertMAMEtoRA.Size = new System.Drawing.Size(785, 419);
			this.tabPageConvertMAMEtoRA.TabIndex = 2;
			this.tabPageConvertMAMEtoRA.Text = "Convert from MAME to RA";
			this.tabPageConvertMAMEtoRA.UseVisualStyleBackColor = true;
			// 
			// tabPageConvertRAtoMAME
			// 
			this.tabPageConvertRAtoMAME.Location = new System.Drawing.Point(4, 24);
			this.tabPageConvertRAtoMAME.Name = "tabPageConvertRAtoMAME";
			this.tabPageConvertRAtoMAME.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageConvertRAtoMAME.Size = new System.Drawing.Size(785, 419);
			this.tabPageConvertRAtoMAME.TabIndex = 3;
			this.tabPageConvertRAtoMAME.Text = "Convert from RA to MAME";
			this.tabPageConvertRAtoMAME.UseVisualStyleBackColor = true;
			// 
			// toolTipInfo
			// 
			this.toolTipInfo.AutoPopDelay = 0;
			this.toolTipInfo.InitialDelay = 250;
			this.toolTipInfo.ReshowDelay = 100;
			// 
			// StartPage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(817, 651);
			this.Controls.Add(this.tabControlActions);
			this.Controls.Add(this.panelCommon);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "StartPage";
			this.Text = "Bezel Tools";
			this.panelCommon.ResumeLayout(false);
			this.panelCommon.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownThreads)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownMargin)).EndInit();
			this.tabControlActions.ResumeLayout(false);
			this.tabPageCheck.ResumeLayout(false);
			this.tabPageCheck.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckOutputDebug)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckTemplateRom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckTemplateOverlay)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckExpectedPath)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckPathRom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxCheckPathOverlay)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private Panel panelCommon;
        private TabControl tabControlActions;
        private TabPage tabPageCheck;
        private TabPage tabPageGenerate;
		private Button buttonExecute;
		private TabPage tabPageConvertMAMEtoRA;
		private TabPage tabPageConvertRAtoMAME;
		private TextBox textBox4;
		private TextBox textBox3;
		private Label label1;
		private Label labelMargin;
		private Label labelDebug;
		private Label labelErrorFile;
		private NumericUpDown numericUpDownThreads;
		private NumericUpDown numericUpDownMargin;
		private Button buttonDebugBrowse;
		private Button buttonErrorBrowse;
		private TextBox textBoxCheckDebugOutput;
		private Label labelCheckOutputDebug;
		private TextBox textBoxCheckPathInRom;
		private TextBox textBoxCheckPathTemplateRom;
		private TextBox textBoxCheckPathTemplateOverlay;
		private Label labelCheckTemplateRom;
		private Label labelCheckTemplateOverlay;
		private Label labelCheckInputOverlay;
		private RadioButton radioButtonCheckAndFix;
		private RadioButton radioButtonCheckOnly;
		private TextBox textBoxCheckPathRoms;
		private TextBox textBoxCheckPathOverlays;
		private Label labelCheckFilesRoms;
		private Label labelCheckFilesOverlays;
		private Button buttonCheckPathDebugOutput;
		private Button buttonCheckPathTemplateRom;
		private Button buttonCheckPathTemplateOverlay;
		private Button buttonCheckPathRoms;
		private Button buttonCheckPathOverlays;
		private PictureBox pictureBoxCheckPathOverlay;
		private PictureBox pictureBoxCheckOutputDebug;
		private PictureBox pictureBoxCheckTemplateRom;
		private PictureBox pictureBoxCheckTemplateOverlay;
		private PictureBox pictureBoxCheckExpectedPath;
		private PictureBox pictureBoxCheckPathRom;
		private ToolTip toolTipInfo;
	}
}