using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHP
{
    public partial class EditProblemForm : Form
    {
        TextBox t;
        public EditProblemForm(TextBox tb)
        {
            InitializeComponent();
            this.AcceptButton = button1;
            this.textBox1.Text = tb.Text;
            t = tb;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string pname = this.textBox1.Text;
            t.Text = pname;
            AHP.problemName = pname;
            if (AHP.criteriasComparison.Count > 0)
                AHP.criteriasComparison[0][0].mainCriteria = pname;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
