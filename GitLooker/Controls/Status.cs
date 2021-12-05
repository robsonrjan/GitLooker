namespace GitLooker.Controls
{
    public partial class Status : Form
    {
        private Action forceWithRebase;

        public Status(string text, Action forceWithRebase, bool canReset)
        {
            InitializeComponent();
            textBox1.Text = text;
            this.forceWithRebase = forceWithRebase;
            button1.Visible = canReset;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            forceWithRebase?.Invoke();
            Close();
        }

    }
}
