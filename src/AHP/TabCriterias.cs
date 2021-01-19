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
        public void TabCriteriasShow()
        {
            Label[] l = new Label[AHP.criterias_q.Sum()];
            t = new TextBox[AHP.criterias_q.Sum()];
            Button button = new Button();
            button.Text = "Далее";
            button.Name = "button2";
            button.Size = new Size(100, 50);
            button.Font = new Font("Microsoft Sans Serif", 12.0f);
            button.Click += new EventHandler(this.button2_Click);
            button.Anchor = AnchorStyles.Top;
            tabPage2.Controls.Add(button);
            tabPage2.AutoScroll = true;
            int start_y = 10;
            int i = 0;
            int ti = 0;
            for (int j = 0; j < AHP.criterias_q.Length; j++)
            {
                for (int k = 0; k < AHP.criterias_q[j]; k++)
                {
                    l[i] = new Label();
                    l[i].Location = new Point(20, i * 45 + start_y);
                    l[i].Size = new Size(170, 30);
                    l[i].Font = new Font("Microsoft Sans Serif", 8);
                    l[i].AutoSize = true;
                    if (AHP.levels == 2)
                        l[i].Text = String.Format("Название критерия №{0}:", k + 1);
                    else
                        l[i].Text = String.Format("Критерий {0}-го уровня №{1}:", j + 1, k + 1);
                    tabPage2.Controls.Add(l[i]);

                    t[i] = new TextBox();
                    t[i].Location = new Point(l[i].Width + 35, i * 45 + start_y);
                    t[i].Size = new Size(tabPage2.Width - l[i].Width - 50, 30);
                    t[i].TabIndex = ti;
                    t[i].MaxLength = 30;
                    ti++;
                    t[i].Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
                    tabPage2.Controls.Add(t[i]);
                    i++;
                    button.Location = new Point((tabPage2.Width - button.Width) / 2, i * 45 + start_y);
                }
            }
            button.TabIndex = ti;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.t.Any(x => x.Text == ""))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            AHP.criterias.Clear();
            int ind = 0;
            for (int i = 0; i < AHP.criterias_q.Length; i++)
            {
                string[] crits = new string[AHP.criterias_q[i]];
                for (int j = 0; j < crits.Length; j++)
                {
                    crits[j] = t[ind].Text;
                    ind++;
                }
                AHP.criterias.Add(crits);
            }
            /*int l = 0;
            for (int i = 0; i < AHP.criterias_q.Length - 1; i++)
                l += AHP.criterias_q[i];*/
            TabHandle.MakeReadOnly(tabControl1.SelectedTab);
            TabAlternativesShow();
            tabControl1.SelectedTab = tabPage3;
            (sender as Button).Visible = false;
        }
    }
}
