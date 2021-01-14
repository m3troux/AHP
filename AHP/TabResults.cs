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
        public void TabResultsShow()
        {
            ClearTab();
            FillTab();
        }
        private void ClearTab()
        {
            for (int i = tabPage6.Controls.Count - 1; i >= 0; i--)
            {
                Label ll = tabPage6.Controls[i] as Label;
                ProgressBar pbb = tabPage6.Controls[i] as ProgressBar;
                TextBox tt = tabPage6.Controls[i] as TextBox;
                if (ll != null && ll.Name != "sort")
                {
                    tabPage6.Controls.RemoveAt(i);
                    ll.Dispose();
                }
                else if (pbb != null)
                {
                    tabPage6.Controls.RemoveAt(i);
                    pbb.Dispose();
                }
                else if (tt != null)
                {
                    tabPage6.Controls.RemoveAt(i);
                    tt.Dispose();
                }
            }
        }
        private void FillTab()
        {
            Label[] l = new Label[AHP.results.Count];
            ProgressBar[] pb = new ProgressBar[AHP.results.Count];
            t = new TextBox[AHP.results.Count];
            int start_y = 50;

            for (int i = 0; i < AHP.results.Count; i++)
            {
                l[i] = new Label();
                l[i].Location = new Point(20, i * 45 + start_y);
                l[i].Size = new Size(120, 30);
                l[i].Text = AHP.results[i].Key;
                l[i].Font = new Font("Times New Roman", 16.0f, FontStyle.Bold);
                l[i].AutoSize = true;
                l[i].Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;

                tabPage6.Controls.Add(l[i]);

                pb[i] = new ProgressBar();
                pb[i].Location = new Point(160, i * 45 + start_y);
                pb[i].Maximum = 10000;
                pb[i].Value = Convert.ToInt32(AHP.results[i].Value * pb[i].Maximum);
                pb[i].Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
                //if (i == AHP.results.Count - 1)
                    //pb[i].Value = pb[i].Maximum - (pb.Select(x => x.Value).Sum() - pb[pb.Length - 1].Value);
                    //pb[i].Value = pb[i].Maximum - Convert.ToInt32((AHP.results.Select(x => x.Value).Sum() - AHP.results[AHP.results.Count - 1].Value) * pb[i].Maximum);

                tabPage6.Controls.Add(pb[i]);

                t[i] = new TextBox();
                t[i].Size = new Size(70, 30);
                t[i].ReadOnly = true;
                t[i].Text = ((double)pb[i].Value / pb[i].Maximum).ToString(); //Math.Round(AHP.results[i].Value, Convert.ToInt32(Math.Log10(pb[i].Maximum))).ToString();
                t[i].Font = new Font("Microsoft Sans Serif", 12.0f, FontStyle.Bold);
                t[i].Anchor = AnchorStyles.Top | AnchorStyles.Right;

                tabPage6.Controls.Add(t[i]);
                t[i].Location = new Point(pb[i].Right + 10, i * 45 + start_y);
                pb[i].Size = new Size(tabPage6.Width - t[i].Left, 30);
                t[i].Location = new Point(pb[i].Right + 10, i * 45 + start_y);
            }
            int max = 0;
            foreach (Label lbl in l)
                if (lbl.Right > max)
                    max = lbl.Right;
            if (max <= 160) return;
            for (int i = 0; i < AHP.results.Count; i++)
            {
                l[i].Size = new Size(max, l[i].Height);
                pb[i].Location = new Point(max + 5, pb[i].Location.Y);
                pb[i].Size = new Size(t[i].Left - pb[i].Left - 10, 30);
                t[i].Location = new Point(pb[i].Right + 10, i * 45 + start_y);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                AHP.SortResults(true);
            else
                AHP.SortResults(false);
            ClearTab();
            FillTab();
        }
    }
}
