namespace GitLooker.Controls
{
    public class EntryControl : Button
    {
        public EntryControl(string value) : this()
        {
            Text = value;
        }

        public EntryControl()
        {
            AutoSize = false;
            Dock = DockStyle.Top;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(238)));
            Image = Properties.Resources.line;
            ImageAlign = ContentAlignment.BottomCenter;
            Location = new Point(0, 0);
            Name = "repo";
            Size = new Size(664, 30);
            TabIndex = 0;
            Text = "repo";
            TextAlign = ContentAlignment.BottomLeft;

            GotFocus += onFocus;
            LostFocus += EntryControl_LostFocus;
        }

        private void EntryControl_LostFocus(object sender, EventArgs e)
        {
            BackColor = Parent.BackColor;
        }

        private void onFocus(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(247, 247, 247);
        }
    }
}
