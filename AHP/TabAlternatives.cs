using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHP
{
    public partial class Form1 : Form
    {
        public void TabAlternativesShow()
        {
            Label[] l = new Label[AHP.alternatives_q];
            t = new TextBox[AHP.alternatives_q];
            Button button = new Button();
            button.Text = "Далее";
            button.Name = "button3";
            button.Size = new Size(100, 50);
            button.Font = new Font("Microsoft Sans Serif", 12.0f);
            button.Click += new EventHandler(this.button3_Click);
            button.Anchor = AnchorStyles.Top;
            tabPage3.Controls.Add(button);
            tabPage3.AutoScroll = true;

            int start_y = 10;
            int ti = 0;
            for (int i = 0; i < AHP.alternatives_q; i++)
            {
                l[i] = new Label();
                l[i].Location = new Point(20, i * 45 + start_y);
                l[i].Size = new Size(170, 30);
                l[i].Text = String.Format("Название альтернативы №{0}:", i + 1);
                l[i].AutoSize = true;

                this.tabPage3.Controls.Add(l[i]);

                t[i] = new TextBox();
                t[i].Location = new Point(l[i].Width + 35, i * 45 + start_y);
                t[i].Size = new Size(tabPage3.Width - l[i].Width - 50, 30);
                t[i].TabIndex = ti;
                t[i].MaxLength = 30;
                ti++;
                t[i].Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;

                this.tabPage3.Controls.Add(t[i]);
                button.Location = new Point((tabPage3.Width - button.Width) / 2, (i + 1) * 45 + start_y);
            }
            button.TabIndex = ti;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.t.Any(x => x.Text == ""))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            AHP.alternatives = new string[AHP.alternatives_q];
            for (int i = 0; i < AHP.alternatives_q; i++)
                AHP.alternatives[i] = t[i].Text;
            TabHandle.MakeReadOnly(tabControl1.SelectedTab);
            TabCompCritShowFirst();
            tabControl1.SelectedTab = tabPage4;
            t = null;
            (sender as Button).Visible = false;
        }
    }
}
