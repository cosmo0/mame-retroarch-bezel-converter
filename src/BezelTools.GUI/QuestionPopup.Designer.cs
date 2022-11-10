namespace BezelTools.GUI
{
    partial class QuestionPopup
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
			this.labelQuestion = new System.Windows.Forms.Label();
			this.buttonChoose = new System.Windows.Forms.Button();
			this.flowLayoutPanelAnswers = new System.Windows.Forms.FlowLayoutPanel();
			this.SuspendLayout();
			// 
			// labelQuestion
			// 
			this.labelQuestion.AutoSize = true;
			this.labelQuestion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.labelQuestion.Location = new System.Drawing.Point(23, 19);
			this.labelQuestion.Name = "labelQuestion";
			this.labelQuestion.Size = new System.Drawing.Size(67, 15);
			this.labelQuestion.TabIndex = 0;
			this.labelQuestion.Text = "QUESTION";
			// 
			// buttonChoose
			// 
			this.buttonChoose.Location = new System.Drawing.Point(443, 259);
			this.buttonChoose.Name = "buttonChoose";
			this.buttonChoose.Size = new System.Drawing.Size(130, 23);
			this.buttonChoose.TabIndex = 2;
			this.buttonChoose.Text = "Choose selected";
			this.buttonChoose.UseVisualStyleBackColor = true;
			this.buttonChoose.Click += new System.EventHandler(this.buttonChoose_Click);
			// 
			// flowLayoutPanelAnswers
			// 
			this.flowLayoutPanelAnswers.AutoScroll = true;
			this.flowLayoutPanelAnswers.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanelAnswers.Location = new System.Drawing.Point(23, 37);
			this.flowLayoutPanelAnswers.Name = "flowLayoutPanelAnswers";
			this.flowLayoutPanelAnswers.Size = new System.Drawing.Size(550, 216);
			this.flowLayoutPanelAnswers.TabIndex = 3;
			this.flowLayoutPanelAnswers.WrapContents = false;
			// 
			// QuestionPopup
			// 
			this.AcceptButton = this.buttonChoose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(604, 303);
			this.ControlBox = false;
			this.Controls.Add(this.flowLayoutPanelAnswers);
			this.Controls.Add(this.buttonChoose);
			this.Controls.Add(this.labelQuestion);
			this.Name = "QuestionPopup";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Question";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private Label labelQuestion;
        private Button buttonChoose;
        private FlowLayoutPanel flowLayoutPanelAnswers;
    }
}