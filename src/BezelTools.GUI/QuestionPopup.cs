namespace BezelTools.GUI
{
    /// <summary>
    /// A question popup
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form"/>
    public partial class QuestionPopup : Form
    {
        private readonly List<RadioButton> radioAnswers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionPopup"/> class.
        /// </summary>
        public QuestionPopup()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The delegate for when an answer is picked
        /// </summary>
        /// <param name="index">The index.</param>
        public delegate void AnswerPickedDelegate(int index);

        /// <summary>
        /// Occurs when an answer is picked.
        /// </summary>
        public event AnswerPickedDelegate? AnswerPicked;

        /// <summary>
        /// Sets the question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="answers">The answers.</param>
        public void SetQuestion(string question, string[] answers)
        {
            this.labelQuestion.Text = question;
            bool ischecked = true;
            foreach (var a in answers)
            {
                var radio = new RadioButton
                {
                    Text = a,
                    Checked = ischecked
                };

                this.flowLayoutPanelAnswers.Controls.Add(radio);
                this.radioAnswers.Add(radio);

                ischecked = false;
            }
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            var chosen = radioAnswers.First(r => r.Checked);
            int idx = radioAnswers.IndexOf(chosen);

            AnswerPicked?.Invoke(idx);

            this.Close();
        }
    }
}