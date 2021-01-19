using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHP
{
    public partial class Form1 : Form
    {
        public void TabCompAltShowFirst()
        {
            curCriteria = AHP.criterias[AHP.criterias.Count - 1][0]; // первый критерий последнего уровня
            TabPage tab = new TabPage();
            tabControl4.TabPages.Add(tab);
            TabCompAltShow(ref tab);
        }
        public void TabCompAltShow(ref TabPage tab)
        {
            tab.Name = curCriteria;
            tab.Leave += Tab_Leave;
            tab.AutoScroll = true;
            tab.Width = tab.Parent.Width;
            tab.Height = tab.Parent.Height;

            tboxes = new TextBox[AHP.alternatives_q, AHP.alternatives_q];
            
            tab.Text = String.Format("Критерий \"{0}\"", curCriteria);

            Label compCrit = new Label();
            compCrit.AutoSize = true;
            compCrit.Text = "Сравнение альтернатив по критерию:\r\n";
            compCrit.Font = new Font("Times New Roman", 14.0f, FontStyle.Bold);
            Label compCrit1 = new Label();
            compCrit1.Text = curCriteria;
            compCrit1.AutoSize = true;
            compCrit1.Font = new Font("Times New Roman", 16.0f, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
            compCrit1.Location = new Point((tab.Width - compCrit1.Width) / 2, 40);
            tab.Controls.Add(compCrit);
            compCrit.Location = new Point((tab.Width - compCrit.Width) / 2, 10);
            tab.Controls.Add(compCrit1);

            table = new TableLayoutPanel();
            table.Location = new Point(20, 90);
            table.CellPaint += table_CellPaint;
            table.ColumnCount = table.RowCount = AHP.alternatives_q + 1;
            table.AutoSize = true;
            for (int k = 0; k < AHP.alternatives_q; k++)
            {
                string text = AHP.alternatives[k];
                /*if (text.Contains(" "))
                {
                    string[] tmp = text.Split();
                    text = "";
                    foreach (string v in tmp)
                    {
                        if (v.Length == 1)
                            text += v[0].ToString().ToLower();
                        else
                            text += v[0].ToString().ToUpper();
                    }
                }*/
                table.Controls.Add(new Label { Text = text, Anchor = AnchorStyles.None, AutoSize = true, Font = new Font("Microsoft Sans Serif", 11.0f, FontStyle.Bold) }, 0, k + 1);
                table.Controls.Add(new Label { Text = text, Anchor = AnchorStyles.None, AutoSize = true, Font = new Font("Microsoft Sans Serif", 11.0f, FontStyle.Bold) }, k + 1, 0);
            }
            for (int i = 0; i < table.RowCount; i++)
            {
                table.RowStyles.Add(new RowStyle() { Height = 30, SizeType = SizeType.Percent });
                table.ColumnStyles.Add(new ColumnStyle() { SizeType = SizeType.Percent });
            }
            for (int i = 1; i < table.RowCount; i++)
                for (int j = 1; j < table.ColumnCount; j++)
                {
                    int tI = i - 1;
                    int tJ = j - 1;
                    tboxes[tJ, tI] = new TextBox();
                    tboxes[tJ, tI].TextAlign = HorizontalAlignment.Center;
                    tboxes[tJ, tI].MaxLength = 15;
                    tboxes[tJ, tI].Anchor = AnchorStyles.None;
                    tboxes[tJ, tI].Margin = new Padding(1, 1, 1, 1);
                    tboxes[tJ, tI].BorderStyle = BorderStyle.None;
                    tboxes[tJ, tI].Name = tJ.ToString() + "-" + tI.ToString();
                    tboxes[tJ, tI].Font = new Font("Microsoft Sans Serif", 12.0f);
                    if (tI == tJ)
                    {
                        tboxes[tJ, tI].Text = "1";
                        tboxes[tJ, tI].ReadOnly = true;
                        tboxes[tJ, tI].TabStop = false;
                        tboxes[tJ, tI].BorderStyle = BorderStyle.None;
                    }
                    tboxes[tJ, tI].TextChanged += Ont_text_changed;
                    table.Controls.Add(tboxes[tJ, tI], j, i);
                }
            int tWidth = 0;
            for (int i = 1; i < table.RowCount; i++)
            {
                int w = table.GetColumnWidths()[i];
                if (w > tWidth)
                    tWidth = w;
            }
            foreach (TextBox tb in tboxes)
            {
                tb.Width = tWidth;
            }
            tab.Controls.Add(table);
            if (table.Width > tab.Width)
                table.Location = new Point(20, table.Location.Y);
            else
                table.Location = new Point((tab.Width - table.Width) / 2, table.Location.Y);
            SetDoubleBuffered(table);
            foreach (Control c in table.Controls)
                SetDoubleBuffered(c);
            Button button = new Button();
            button.Text = "Далее";
            button.Size = new Size(100, 50);
            button.Location = new Point((tab.Width - button.Width) / 2, table.Location.Y + table.Height + 40);
            button.Font = new Font("Microsoft Sans Serif", 12.0f);
            button.Click += new System.EventHandler(this.btnCompAlt_Click);
            tab.Controls.Add(button);
        }
        private void btnCompAlt_Click(object sender, EventArgs e)
        {
            int q = AHP.alternatives.Length;
            double[,] matrix = new double[q, q];
            for (int j = 0; j < q; j++)
                matrix[j, j] = 1;
            for (int i = 0; i < q - 1; i++)
                for (int j = i + 1; j < q; j++)
                {
                    double value = ParseValue(tboxes[j, i].Text); ;
                    /*if (tboxes[j, i].Text.Length == 1)
                        value = Convert.ToDouble(tboxes[j, i].Text);
                    else
                        value = Convert.ToDouble(tboxes[j, i].Text.Substring(tboxes[j, i].Text.IndexOf("/") + 1).ToString()) / Convert.ToDouble(tboxes[j, i].Text.Substring(0, tboxes[j, i].Text.IndexOf("/")).ToString());*/
                    matrix[i, j] = value;
                    matrix[j, i] = 1.0 / value;
                    if (matrix[i, j] == 0 || Double.IsInfinity(matrix[i, j]))
                    {
                        MessageBox.Show("Проверьте правильность заполнения полей!");
                        return;
                    }
                }
            matrixes.Add(new PairedMatrix(curCriteria, AHP.alternatives, matrix));
            TabHandle.MakeReadOnly(tabControl1.SelectedTab);
            // если последнее сравнение
            if (Array.IndexOf(AHP.criterias[AHP.criterias.Count - 1], curCriteria) == AHP.criterias[AHP.criterias.Count - 1].Length - 1)
            {
                AHP.criteriasComparison.Add(new List<PairedMatrix>(matrixes));
                AHP.Calculate();
                matrixes.Clear(); // GC
                TabResultsShow();
                sort.Visible = true;
                radioButton1.Visible = true;
                radioButton2.Visible = true;
                tabControl1.SelectedTab = tabPage6;
                tboxes = null;
            }
            else
            {
                curCriteria = AHP.criterias[AHP.criterias.Count - 1][Array.IndexOf(AHP.criterias[AHP.criterias.Count - 1], curCriteria) + 1];
                TabPage tabp = new TabPage();
                tabControl4.TabPages.Add(tabp);
                TabCompAltShow(ref tabp);
                tabControl4.SelectedTab = tabp;
            }
            (sender as Button).Visible = false;
        }
    }
}
