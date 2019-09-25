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
        public Status(string text)
        {
            InitializeComponent();
            this.textBox1.Text = text;
        }

        private void Status_Load(object sender, EventArgs e)
        {

        }
    }
}
