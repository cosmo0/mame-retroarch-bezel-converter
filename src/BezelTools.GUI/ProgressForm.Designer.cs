namespace BezelTools.GUI
{
    partial class ProgressForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.labelTitle = new System.Windows.Forms.Label();
			this.buttonNext = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Location = new System.Drawing.Point(12, 46);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(776, 23);
			this.progressBar.TabIndex = 0;
			// 
			// labelTitle
			// 
			this.labelTitle.AutoSize = true;
			this.labelTitle.Location = new System.Drawing.Point(12, 9);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Size = new System.Drawing.Size(61, 15);
			this.labelTitle.TabIndex = 2;
			this.labelTitle.Text = "Working...";
			// 
			// buttonNext
			// 
			this.buttonNext.Enabled = false;
			this.buttonNext.Location = new System.Drawing.Point(310, 75);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(181, 38);
			this.buttonNext.TabIndex = 3;
			this.buttonNext.Text = "Next step";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.buttonFinish_Click);
			// 
			// ProgressForm
			// 
			this.AcceptButton = this.buttonNext;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 125);
			this.ControlBox = false;
			this.Controls.Add(this.buttonNext);
			this.Controls.Add(this.labelTitle);
			this.Controls.Add(this.progressBar);
			this.Name = "ProgressForm";
			this.ShowIcon = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Working...";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ProgressBar progressBar;
        private Label labelTitle;
        private Button buttonNext;
    }
}