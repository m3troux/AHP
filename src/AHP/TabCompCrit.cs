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
        public int lvl;
        public string curCriteria;
        List<PairedMatrix> matrixes = new List<PairedMatrix>();
        TextBox[,] tboxes;
        TableLayoutPanel table;
        public void TabCompCritShowFirst()
        {
            lvl = 0;
            curCriteria = AHP.problemName;
            TabPage tab = new TabPage();
            tabControl3.TabPages.Add(tab);
            TabCompCritShow(ref tab);
        }

        public event EventHandler t_text_changed;
        public virtual void Ont_text_changed(object sender, EventArgs e)
        {
            t_text_changed?.Invoke(this, e);
            TextBox tbox = sender as TextBox;
            int tJ = Convert.ToInt32(tbox.Name.Substring(0, tbox.Name.IndexOf("-")));
            int tI = Convert.ToInt32(tbox.Name.Substring(tbox.Name.IndexOf("-") + 1));
            tboxes[tI, tJ].TextChanged -= Ont_text_changed;
            try
            {
                if (!tbox.Text.Contains("/"))
                {
                    if (tbox.Text == "1")
                        tboxes[tI, tJ].Text = "1";
                    else
                        tboxes[tI, tJ].Text = "1/" + tbox.Text;
                }
                else if (tbox.Text.Length == 0)
                    tboxes[tI, tJ].Text = "";
                else if (tbox.Text.Substring(0, tbox.Text.IndexOf("/")) == "1")
                    tboxes[tI, tJ].Text = tbox.Text.Substring(tbox.Name.IndexOf("-") + 1);
                else
                    tboxes[tI, tJ].Text = tbox.Text.Substring(tbox.Text.IndexOf("/") + 1).ToString() + "/" + tbox.Text.Substring(0, tbox.Text.IndexOf("/")).ToString();
            }
            catch { }
            tboxes[tI, tJ].TextChanged += Ont_text_changed;
        }
        private void table_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            TableLayoutPanel table = sender as TableLayoutPanel;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            var rectangle = e.CellBounds;
            using (var pen = new Pen(Color.Black, 1))
            {
                pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                if (e.Row == (table.RowCount - 1))
                    rectangle.Height -= 1;

                if (e.Column == (table.ColumnCount - 1))
                    rectangle.Width -= 1;
                e.Graphics.DrawRectangle(pen, rectangle);
            }
        }
        public static void SetDoubleBuffered(Control c) // remove flickering
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;
            System.Reflection.PropertyInfo aProp = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }
        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }*/
        public void TabCompCritShow(ref TabPage tab)
        {
            tab.Name = curCriteria;
            tab.Leave += Tab_Leave;
            tab.AutoScroll = true;
            tab.Width = tab.Parent.Width;
            tab.Height = tab.Parent.Height;

            tboxes = new TextBox[AHP.criterias[lvl].Length, AHP.criterias[lvl].Length];
            if (lvl == 0)
                tab.Text = "Основная цель";
            else
                tab.Text = String.Format("Критерий \"{0}\"", curCriteria);
            Label compCrit = new Label();
            compCrit.Font = new Font("Times New Roman", 14.0f, FontStyle.Bold);
            compCrit.Text = "Сравнение критериев";
            compCrit.AutoSize = true;

            table = new TableLayoutPanel();
            table.Location = new Point((tab.Width - table.Width) / 2, 50);
            table.CellPaint += table_CellPaint;
            table.ColumnCount = table.RowCount = AHP.criterias[lvl].Length + 1;
            table.AutoSize = true;
            if (AHP.levels >= 3)
            {
                Label compCrit1 = new Label();
                compCrit.Text += String.Format(" {0}-го уровня", lvl + 1);
                if (lvl > 0)
                {
                    compCrit.Text += String.Format(" по критерию:\r\n");
                    compCrit1.Text = curCriteria;
                }
                compCrit1.Font = new Font("Times New Roman", 16.0f, FontStyle.Bold | FontStyle.Italic | FontStyle.Underline);
                compCrit1.AutoSize = true;
                compCrit1.Location = new Point((tab.Width - compCrit1.Width) / 2, 40);
                tab.Controls.Add(compCrit1);
                table.Location = new Point(table.Location.X, table.Location.Y + 40);
            }
            tab.Controls.Add(compCrit);
            compCrit.Location = new Point((tab.Width - compCrit.Width) / 2, 10);
            for (int k = 0; k < AHP.criterias[lvl].Length; k++)
            {
                string text = AHP.criterias[lvl][k];
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
            button.Click += new EventHandler(this.btnCompCrit_Click);
            tab.Controls.Add(button);
        }
        private void btnCompCrit_Click(object sender, EventArgs e)
        {
            int q = AHP.criterias[lvl].Length;
            double[,] matrix = new double[q, q];
            for (int j = 0; j < q; j++)
                matrix[j, j] = 1;
            for (int i = 0; i < q - 1; i++)
                for (int j = i + 1; j < q; j++)
                {
                    double value = ParseValue(tboxes[j, i].Text);
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
            matrixes.Add(new PairedMatrix(curCriteria, AHP.criterias[lvl], matrix));
            TabHandle.MakeReadOnly(tabControl1.SelectedTab);

            // если главный критерий на текущем уровне - последний
            if (curCriteria == AHP.problemName || Array.IndexOf(AHP.criterias[lvl - 1], curCriteria) == AHP.criterias[lvl - 1].Length - 1)
            {
                AHP.criteriasComparison.Add(new List<PairedMatrix>(matrixes));
                // перейти к след. уровню критериев или на след. форму
                if (lvl == AHP.levels - 2)
                {
                    matrixes.Clear();
                    TabCompAltShowFirst();
                    tabControl1.SelectedTab = tabPage5;
                }
                else
                {
                    lvl++;
                    curCriteria = AHP.criterias[lvl - 1][0];
                    TabPage tabp = new TabPage();
                    tabControl3.TabPages.Add(tabp);
                    matrixes.Clear();
                    TabCompCritShow(ref tabp);
                    tabControl3.SelectedTab = tabp;
                }
            }
            else
            {
                curCriteria = AHP.criterias[lvl - 1][Array.IndexOf(AHP.criterias[lvl - 1], curCriteria) + 1];
                TabPage tabp = new TabPage();
                tabControl3.TabPages.Add(tabp);
                TabCompCritShow(ref tabp);
                tabControl3.SelectedTab = tabp;
            }
            (sender as Button).Visible = false;
        }
    }
}
