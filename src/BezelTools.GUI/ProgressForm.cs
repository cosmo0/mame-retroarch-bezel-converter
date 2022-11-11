namespace BezelTools.GUI
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes the progress bar.
        /// </summary>
        /// <param name="max">The maximum value.</param>
        public void InitProgress(int max)
        {
            this.progressBar.Minimum = 0;
            this.progressBar.Maximum = max;
            this.progressBar.Value = 0;
        }

        /// <summary>
        /// Sets the label.
        /// </summary>
        /// <param name="label">The label.</param>
        public void SetLabel(string label)
        {
            this.labelTitle.Text = label;
        }

        /// <summary>
        /// Sets the form in a "next step" state.
        /// </summary>
        public void SetNext()
        {
            this.progressBar.Value = this.progressBar.Maximum;
            this.buttonNext.Enabled = true;
        }

        /// <summary>
        /// Sets the progress bar value.
        /// </summary>
        /// <param name="progress">The progress value.</param>
        public void SetProgress(int progress)
        {
            this.progressBar.Value = progress;
        }

        private void buttonFinish_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}