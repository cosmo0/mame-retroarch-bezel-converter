namespace BezelTools.GUI
{
    /// <summary>
    /// The log popup
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form"/>
    public partial class LogPopup : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogPopup"/> class.
        /// </summary>
        public LogPopup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Log(string message)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(Log), new object[] { message });
                return;
            }

            this.textBoxLog.Text += $"{message}{Environment.NewLine}";
        }

        /// <summary>
        /// Resets the log.
        /// </summary>
        public void Reset()
        {
            this.textBoxLog.Text = "";
        }

        private void LogPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }
    }
}