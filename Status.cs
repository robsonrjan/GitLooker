using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitLooker
{
    public partial class Status : Form
    {
        private Action forceWithRebase;

        public Status(string text, Action forceWithRebase,bool canReset)
        {
            InitializeComponent();
            this.textBox1.Text = text;
            this.forceWithRebase = forceWithRebase;
            button1.Visible = canReset;
        }

        private void Status_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            forceWithRebase?.Invoke();
            this.Close();
        }
        
    }
}
