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
        public void TabCriteriaLevelsShow(TabPage crL)
        {
            crL.AutoScroll = true;
            Button button = new Button();
            button.Text = "Далее";
            button.Name = "btnCrLevels";
            button.Size = new Size(100, 50);
            button.Font = new Font("Microsoft Sans Serif", 12.0f);
            button.Click += new EventHandler(this.btnLevels_Click);
            button.Anchor = AnchorStyles.Top;
            crL.Controls.Add(button);

            Label[] l = new Label[AHP.levels - 1];
            t = new TextBox[AHP.levels - 1];
            int start_y = 10;
            int ti = 0;
            for (int i = 0; i < AHP.levels - 1; i++)
            {
                l[i] = new Label();
                l[i].Location = new Point(20, i * 45 + start_y);
                l[i].Size = new Size(150, 30);
                l[i].Text = String.Format("Количество критериев на уровне №{0}:", i + 1);
                l[i].AutoSize = true;

                crL.Controls.Add(l[i]);

                t[i] = new TextBox();
                t[i].Location = new Point(l[i].Width + 35, i * 45 + start_y);
                t[i].Size = new Size(crL.Width - l[i].Width - 50, 30);
                t[i].MaxLength = 2;
                t[i].TabIndex = ti;
                ti++;
                t[i].Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
                
                crL.Controls.Add(t[i]);
                button.Location = new Point((crL.Width - button.Width) / 2, (i + 1) * 45 + start_y);
            }
            button.TabIndex = ti;
        }

        private void btnLevels_Click(object sender, EventArgs e)
        {
            if (this.t.Any(x => x.Text == ""))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }
            try
            {
                if (this.t.Any(x => Convert.ToInt32(x.Text) < 2 || Convert.ToInt32(x.Text) > 10))
                {
                    MessageBox.Show("Проверьте правильность заполнения полей!");
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Проверьте правильность заполнения полей!");
                return;
            }
            for (int i = 0; i < AHP.criterias_q.Length; i++)
                AHP.criterias_q[i] = Convert.ToInt32(t[i].Text);
            TabHandle.MakeReadOnly(tabControl1.SelectedTab);
            TabCriteriasShow();
            tabControl1.SelectedTab = tabPage2;
            (sender as Button).Visible = false;
        }
    }
}
