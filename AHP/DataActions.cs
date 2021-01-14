using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AHP
{
    public partial class Form1 : Form
    {
        TextBox[,] ed_tboxes;
        public event EventHandler ed_t_text_changed;
        public virtual void On_ed_t_text_changed(object sender, EventArgs e)
        {
            ed_t_text_changed?.Invoke(this, e);
            TextBox tbox = sender as TextBox;
            int tJ = Convert.ToInt32(tbox.Name.Substring(0, tbox.Name.IndexOf("-")));
            int tI = Convert.ToInt32(tbox.Name.Substring(tbox.Name.IndexOf("-") + 1));
            ed_tboxes[tI, tJ].TextChanged -= On_ed_t_text_changed;
            try
            {
                if (!tbox.Text.Contains("/"))
                {
                    if (tbox.Text == "1")
                        ed_tboxes[tI, tJ].Text = "1";
                    else
                        ed_tboxes[tI, tJ].Text = "1/" + tbox.Text;
                }
                else if (tbox.Text.Length == 0)
                    ed_tboxes[tI, tJ].Text = "";
                else if (tbox.Text.Substring(0, tbox.Text.IndexOf("/")) == "1")
                    ed_tboxes[tI, tJ].Text = tbox.Text.Substring(tbox.Name.IndexOf("-") + 1);
                else
                    ed_tboxes[tI, tJ].Text = tbox.Text.Substring(tbox.Text.IndexOf("/") + 1).ToString() + "/" + tbox.Text.Substring(0, tbox.Text.IndexOf("/")).ToString();
            }
            catch { }
            ed_tboxes[tI, tJ].TextChanged += On_ed_t_text_changed;
        }
        private void EditTableData(TabPage tab)
        {
            foreach (Control c in tab.Controls)
                if ((c as TabControl) != null)
                    foreach (Control ct in (c as TabControl).SelectedTab.Controls)
                        if ((ct as TableLayoutPanel) != null)
                        {
                            TableLayoutPanel tabl = ct as TableLayoutPanel;
                            ed_tboxes = new TextBox[tabl.RowCount - 1, tabl.ColumnCount - 1];
                            for (int i = 1; i < tabl.RowCount; i++)
                                for (int j = 1; j < tabl.ColumnCount; j++)
                                {
                                    int tI = i - 1;
                                    int tJ = j - 1;
                                    ed_tboxes[tJ, tI] = (tabl.GetControlFromPosition(i, j) as TextBox);
                                    ed_tboxes[tJ, tI].ReadOnly = false;
                                    if (tI == tJ)
                                        ed_tboxes[tJ, tI].ReadOnly = true;
                                    ed_tboxes[tJ, tI].Name = tJ.ToString() + "-" + tI.ToString();
                                    ed_tboxes[tJ, tI].TextChanged -= Ont_text_changed;
                                    ed_tboxes[tJ, tI].TextChanged += On_ed_t_text_changed;
                                }
                        }
        }
        public double ParseValue(string s)
        {
            double res = 0.0;
            if (s.Contains("/"))
            {
                double numer = Convert.ToDouble(s.Substring(0, s.IndexOf("/")));
                double denom = Convert.ToDouble(s.Substring(s.IndexOf("/") + 1));
                res = numer / denom;
            }
            else
                res = Convert.ToDouble(s);
            return res;
        }
        private void editProblemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditProblemForm form = new EditProblemForm(this.tb_problemName);
            form.Show();
        }
        private void editDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EditTableData(tabControl1.SelectedTab);
            editing = true;
            editDataToolStripMenuItem.Enabled = false;
            saveDataToolStripMenuItem.Enabled = true;
        }
        private void saveDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tab = new TabPage();
            foreach (Control ctrl in tabControl1.SelectedTab.Controls)
                if ((ctrl as TabControl) != null)
                    tab = (ctrl as TabControl).SelectedTab;

            int q = ed_tboxes.GetLength(0);
            double[,] matrix = new double[q, q];
            for (int j = 0; j < q; j++)
                matrix[j, j] = 1;
            int tIndex = 0;
            for (int i = 0; i < q - 1; i++)
                for (int j = i + 1; j < q; j++)
                {
                    double value = ParseValue(ed_tboxes[i, j].Text);
                    /*if (ed_tboxes[i, j].Text.Length == 1)
                        value = Convert.ToDouble(ed_tboxes[i, j].Text);
                    else
                        value = 1.0 / Convert.ToDouble(ed_tboxes[i, j].Text[2].ToString());*/
                    matrix[i, j] = value;
                    matrix[j, i] = 1.0 / value;
                    tIndex++;
                }
            for (int i = 0; i < matrixes.Count; i++)
                if (tab.Name == matrixes[i].mainCriteria)
                    matrixes[i] = PairedMatrix.EditPairedMatrix(matrixes[i], matrix);

            for (int i = 0; i < AHP.criteriasComparison.Count; i++)
                for (int j = 0; j < AHP.criteriasComparison[i].Count; j++)
                    if (tab.Name == AHP.criteriasComparison[i][j].mainCriteria)
                        AHP.criteriasComparison[i][j] = PairedMatrix.EditPairedMatrix(AHP.criteriasComparison[i][j], matrix);

            if (AHP.results != null)
            {
                AHP.Calculate();
                TabResultsShow();
                t = null;
                tboxes = null;
            }
            TabHandle.MakeReadOnly(tabControl1.SelectedTab);
            editing = false;
            editDataToolStripMenuItem.Enabled = true;
            saveDataToolStripMenuItem.Enabled = false;
            ed_tboxes = null;
        }

        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Data File|*.dat";
            sfd.Title = "Save state";
            sfd.FileName = "AHP_State.dat";
            if (AHP.problemName != null)
                sfd.FileName = AHP.problemName + ".dat";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                DataHandle dh = new DataHandle();
                dh.CollectData(tabControl3, tabControl4);
                string data = JsonConvert.SerializeObject(dh);
                string filename = sfd.FileName;
                System.IO.File.WriteAllText(filename, data);
                MessageBox.Show("Состояние сохранено!");
            }
        }
        private void loadStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Data File|*.dat";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (AHP.problemName != null)
                {
                    DialogResult confirm = MessageBox.Show("Сохранить состояние?", "Загрузка", MessageBoxButtons.YesNoCancel);
                    if (confirm == DialogResult.Yes)
                        saveStateToolStripMenuItem.PerformClick();
                    else if (confirm == DialogResult.Cancel)
                        return;
                }
                string filename = ofd.FileName;
                string data = System.IO.File.ReadAllText(filename);
                AHP.Reset();
                DataHandle dh = JsonConvert.DeserializeObject<DataHandle>(data);
                this.Hide();
                this.Controls.Clear();
                Form1 f = new Form1();
                f.NormalizeSize();
                f.Size = this.Size;
                f.Show();
                f.Location = this.Location;
                f.LoadData(dh);
            }
        }
        private void LoadData(DataHandle dh)
        {
            AHP.problemName = dh.problemName;
            AHP.levels = dh.levels;
            AHP.criterias_q = dh.criterias_q;
            AHP.alternatives_q = dh.alternatives_q;
            if (dh.alternatives != null)
                AHP.alternatives = dh.alternatives;
            if (dh.criterias != null)
                AHP.criterias = new List<string[]>(dh.criterias);
            if (dh.results != null)
            {
                AHP.results = new List<KeyValuePair<string, double>>(dh.results);
                AHP.SortResults(false);
            }
            matrixes = new List<PairedMatrix>();

            for (int i = 0; i < dh.matrixs.Count; i++)
            {
                string[,] matrix = dh.matrixs[i].Value;
                double[,] res = new double[matrix.GetLength(0), matrix.GetLength(1)];
                for (int k = 0; k < matrix.GetLength(0); k++)
                    for (int s = 0; s < matrix.GetLength(1); s++)
                    {
                        string v = matrix[k, s];
                        if (v.Contains("/"))
                            res[k, s] = Convert.ToDouble(matrix[k, s].Substring(0, matrix[k, s].IndexOf("/"))) / Convert.ToDouble(matrix[k, s].Substring(matrix[k, s].IndexOf("/") + 1));
                        else
                            res[k, s] = Convert.ToDouble(matrix[k, s]);
                    }
                matrixes.Add(new PairedMatrix(dh.matrixs[i].Key, res));
            }

            for (int i = 0; i < dh.criteriasComparison.Count; i++)
            {
                List<PairedMatrix> lpm = new List<PairedMatrix>();
                for (int j = 0; j < dh.criteriasComparison[i].Count; j++)
                {
                    string[,] matrix = dh.criteriasComparison[i][j].Value;
                    double[,] res = new double[matrix.GetLength(0), matrix.GetLength(1)];
                    for (int k = 0; k < matrix.GetLength(0); k++)
                        for (int s = 0; s < matrix.GetLength(1); s++)
                        {
                            string v = matrix[k, s];
                            if (v.Contains("/"))
                                res[k, s] = Convert.ToDouble(matrix[k, s].Substring(0, matrix[k, s].IndexOf("/"))) / Convert.ToDouble(matrix[k, s].Substring(matrix[k, s].IndexOf("/") + 1));
                            else
                                res[k, s] = Convert.ToDouble(matrix[k, s]);
                        }
                    lpm.Add(new PairedMatrix(dh.criteriasComparison[i][j].Key, res));
                }
                AHP.criteriasComparison.Add(new List<PairedMatrix>(lpm));
            }

            // Начало
            tb_problemName.Text = dh.problemName;
            tb_levels.Text = (dh.levels + 1).ToString();
            textBox1.Text = dh.alternatives_q.ToString();
            if (dh.levels == 2)
                textBox2.Text = dh.criterias_q[0].ToString();
            TabHandle.MakeReadOnly(tabPage1);
            TabHandle.FindButton(tabPage1).Visible = false;
            if (dh.levels >= 3 && dh.criterias_q != null)
            {
                TabPage crL = new TabPage("Уровни критериев");
                crL.Name = "tabCrL";
                tabControl2.TabPages.Add(crL);
                TabCriteriaLevelsShow(crL);
                for (int i = 0; i < t.Length; i++)
                    t[i].Text = dh.criterias_q[i].ToString();
                TabHandle.MakeReadOnly(crL);
                TabHandle.FindButton(crL).Visible = false;
                tabControl2.SelectedTab = tabControl2.TabPages[1];
            }
            TabCriteriasShow();

            // Критерии
            if (dh.criterias == null || dh.criterias.Count == 0)
            {
                if (dh.levels >= 3)
                    tabControl2.SelectedTab = tabControl2.TabPages[1];
                tabControl1.SelectedTab = tabPage2;
                return;
            }
            int ind = 0;
            for (int i = 0; i < dh.criterias.Count; i++)
                for (int j = 0; j < dh.criterias[i].Length; j++)
                {
                    t[ind].Text = dh.criterias[i][j];
                    ind++;
                }
            TabHandle.MakeReadOnly(tabPage2);
            TabHandle.FindButton(tabPage2).Visible = false;
            TabAlternativesShow();

            // Альтернативы
            if (dh.alternatives == null)
            {
                tabControl1.SelectedTab = tabPage3;
                return;
            }
            for (int i = 0; i < dh.alternatives.Length; i++)
                t[i].Text = dh.alternatives[i];
            TabHandle.MakeReadOnly(tabPage3);
            TabHandle.FindButton(tabPage3).Visible = false;
            
            TabCompCritShowFirst();

            // Сравнение критериев, tabPage4
            if (dh.criteriasComparison.Count == 0)
            {
                tabControl1.SelectedTab = tabPage4;
                return;
            }
            int alt_q = 0;
            //if (dh.criteriasComparison[dh.criteriasComparison.Count - 1][0].Key == dh.criterias[dh.criterias.Count - 1][0])
            if (dh.criteriasComparison.Count == AHP.levels)
                alt_q = 1;

            TabPage tabp = new TabPage();
            for (int i = 0; i < dh.criteriasComparison.Count - alt_q; i++)
            {
                for (int j = 0; j < dh.criteriasComparison[i].Count; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        curCriteria = dh.criteriasComparison[i][j].Key;
                        tabp = new TabPage();
                        tabControl3.TabPages.Add(tabp);
                        TabCompCritShow(ref tabp);
                    }
                    string[,] matrix = dh.criteriasComparison[i][j].Value;
                    for (int k = 0; k < matrix.GetLength(0) - 1; k++)
                        for (int s = k + 1; s < matrix.GetLength(1); s++)
                        {
                            tboxes[s, k].Text = matrix[k, s];
                        }
                    if (i == 0 && j == 0)
                    {
                        TabHandle.FindButton(tabControl3.TabPages[0]).Visible = false;
                        TabHandle.MakeReadOnly(tabControl3.TabPages[0]);
                    }
                    else
                    {
                        TabHandle.FindButton(tabp).Visible = false;
                        TabHandle.MakeReadOnly(tabp);
                    }
                }
                lvl++;
            }
            if (alt_q == 0 && dh.criteriasComparison.Count != (AHP.levels - 1))
            {
                bool done = false;
                for (int i = 0; i < dh.criterias.Count; i++)
                    for (int j = 0; j < dh.criterias[i].Length; j++)
                        if (dh.criterias[i][j] == tabControl3.TabPages[tabControl3.TabPages.Count - 1].Name)
                        {
                            curCriteria = dh.criterias[i][j];
                            tabp = new TabPage();
                            tabControl3.TabPages.Add(tabp);
                            TabCompCritShow(ref tabp);
                            done = true;
                        }
                if (!done)
                {
                    curCriteria = dh.criterias[0][0];
                    tabp = new TabPage();
                    tabControl3.TabPages.Add(tabp);
                    TabCompCritShow(ref tabp);
                }
            }

            if (dh.matrixs != null && dh.matrixs.Count != 0)
                foreach (KeyValuePair<string, string[,]> m in dh.matrixs)
                    for (int l = 0; l < dh.criterias.Count - 1; l++)
                        foreach (string c in dh.criterias[l])
                            if (m.Key == c)
                            {
                                for (int i = 0; i < dh.matrixs.Count; i++)
                                {
                                    string[,] matrix = dh.matrixs[i].Value;
                                    for (int k = 0; k < matrix.GetLength(0) - 1; k++)
                                        for (int s = k + 1; s < matrix.GetLength(1); s++)
                                        {
                                            tboxes[s, k].Text = matrix[k, s];
                                        }
                                    TabHandle.FindButton(tabp).Visible = false;
                                    TabHandle.MakeReadOnly(tabp);
                                }
                                for (int i = 0; i < dh.criterias.Count - 1; i++)
                                {
                                    bool done = false;
                                    for (int j = 0; j < dh.criterias[i].Length; j++)
                                        if (dh.criterias[i][j] == tabControl3.TabPages[tabControl3.TabPages.Count - 1].Name && !done && dh.criteriasComparison.Count != (AHP.levels - 1))
                                        {
                                            curCriteria = dh.criterias[i][j + 1];
                                            tabp = new TabPage();
                                            tabControl3.TabPages.Add(tabp);
                                            TabCompCritShow(ref tabp);
                                            done = true;
                                        }
                                }
                            }

            // Сравнение альтернатив, tabPage5
            if ((dh.matrixs != null && dh.matrixs.Any(x => !dh.criterias[dh.criterias.Count - 1].Contains(x.Key))) || tabControl3.TabPages.Count != (AHP.criterias_q.Sum() - AHP.criterias_q[AHP.criterias_q.Length - 1] + 1))
            {
                tabControl1.SelectedTab = tabPage4;
                tabControl3.SelectedTab = tabControl3.TabPages[tabControl3.TabPages.Count - 1];
                return;
            }
            else if ((dh.matrixs == null || dh.matrixs.Count == 0) && dh.criteriasComparison[dh.criteriasComparison.Count - 1][0].Key != dh.criterias[dh.criterias.Count - 1][0] && dh.criteriasComparison.Count == (AHP.levels - 1))
            {
                TabCompAltShowFirst();
                tabControl1.SelectedTab = tabPage5;
                tabControl3.SelectedTab = tabControl4.TabPages[0];
                return;
            }

            if (tabControl4.TabPages.Count == 0 && tabControl3.TabPages.Count == (AHP.criterias_q.Sum() - AHP.criterias_q[AHP.criterias_q.Length - 1] + 1))
                TabCompAltShowFirst();
            int count = (dh.matrixs == null || dh.matrixs.Count == 0) ? dh.criteriasComparison[dh.criteriasComparison.Count - 1].Count : dh.matrixs.Count;
            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    curCriteria = dh.criterias[dh.criterias.Count - 1][i];
                    tabp = new TabPage();
                    tabControl4.TabPages.Add(tabp);
                    TabCompAltShow(ref tabp);
                }
                string[,] matrix = (dh.matrixs == null || dh.matrixs.Count == 0) ? dh.criteriasComparison[dh.criteriasComparison.Count - 1][i].Value : dh.matrixs[i].Value;
                for (int k = 0; k < matrix.GetLength(0) - 1; k++)
                    for (int s = k + 1; s < matrix.GetLength(1); s++)
                    {
                        tboxes[s, k].Text = matrix[k, s];
                    }
                if (i == 0)
                {
                    TabHandle.FindButton(tabControl4.TabPages[0]).Visible = false;
                    TabHandle.MakeReadOnly(tabControl4.TabPages[0]);
                }
                else
                {
                    TabHandle.FindButton(tabp).Visible = false;
                    TabHandle.MakeReadOnly(tabp);
                }
            }
            if (dh.matrixs != null && dh.matrixs.Count != 0)
            {
                bool done = false;
                for (int i = 0; i < dh.criterias[dh.criterias.Count - 1].Length; i++)
                    if (tabControl4.TabPages[tabControl4.TabPages.Count - 1].Name == dh.criterias[dh.criterias.Count - 1][i] && !done)
                    {
                        curCriteria = dh.criterias[dh.criterias.Count - 1][i + 1];
                        tabp = new TabPage();
                        tabControl4.TabPages.Add(tabp);
                        TabCompAltShow(ref tabp);
                        done = true;
                    }
            }

            if (dh.results == null || dh.results.Count == 0)
            {
                tabControl1.SelectedTab = tabPage5;
                tabControl4.SelectedTab = tabControl4.TabPages[tabControl4.TabPages.Count - 1];
                return;
            }
            TabResultsShow();
            sort.Visible = radioButton1.Visible = radioButton2.Visible = true;
            tabControl1.SelectedTab = tabPage1;
            tabControl2.SelectedTab = tabPage7;
        }

        // загрузка макросом, на случай, если та выше работает некорректно
        /*private void LoadData(DataHandle dh)
        {
            // Начало
            tb_problemName.Text = dh.problemName;
            tb_levels.Text = (dh.levels + 1).ToString();
            textBox1.Text = dh.alternatives_q.ToString();
            if (dh.levels == 2)
                textBox2.Text = dh.criterias_q[0].ToString();
            button1.PerformClick();
            if (dh.levels >= 3 && dh.criterias_q != null)
            {
                for (int i = 0; i < t.Length; i++)
                    t[i].Text = dh.criterias_q[i].ToString();
                TabHandle.FindButton(tabControl2.SelectedTab).PerformClick();
            }

            // Критерии
            if (dh.criterias == null) return;
            int ind = 0;
            for (int i = 0; i < dh.criterias.Count; i++)
                for (int j = 0; j < dh.criterias[i].Length; j++)
                {
                    t[ind].Text = dh.criterias[i][j];
                    ind++;
                }
            TabHandle.FindButton(tabControl1.SelectedTab).PerformClick();

            // Альтернативы
            if (dh.alternatives == null) return;
            for (int i = 0; i < dh.alternatives.Length; i++)
                t[i].Text = dh.alternatives[i];
            TabHandle.FindButton(tabControl1.SelectedTab).PerformClick();

            // Сравнение критериев, tabPage4
            if (dh.criteriasComparison.Count == 0) return;
            int alt_q = 0;
            if (dh.criteriasComparison[dh.criteriasComparison.Count - 1][0].Key == dh.criterias[dh.criterias.Count - 1][0])
                alt_q = 1;
            for (int i = 0; i < dh.criteriasComparison.Count - alt_q; i++)
                for (int j = 0; j < dh.criteriasComparison[i].Count; j++)
                {
                    string[,] matrix = dh.criteriasComparison[i][j].Value;
                    for (int k = 0; k < matrix.GetLength(0) - 1; k++)
                        for (int s = k + 1; s < matrix.GetLength(1); s++)
                        {
                            tboxes[s, k].Text = matrix[k, s];
                        }
                    TabHandle.FindButton(tabControl1.SelectedTab).PerformClick();
                }

            if (dh.matrixs != null)
                foreach (KeyValuePair<string, string[,]> m in dh.matrixs)
                    for (int l = 0; l < dh.criterias.Count - 1; l++)
                        foreach (string c in dh.criterias[l])
                            if (m.Key == c)
                                for (int i = 0; i < dh.matrixs.Count; i++)
                                {
                                    string[,] matrix = dh.matrixs[i].Value;
                                    for (int k = 0; k < matrix.GetLength(0) - 1; k++)
                                        for (int s = k + 1; s < matrix.GetLength(1); s++)
                                        {
                                            tboxes[s, k].Text = matrix[k, s];
                                        }
                                    TabHandle.FindButton(tabControl1.SelectedTab).PerformClick();
                                }

            // Сравнение альтернатив, tabPage5
            if (dh.matrixs != null && dh.matrixs.Any(x => !dh.criterias[dh.criterias.Count - 1].Contains(x.Key)))
                return;
            else if ((dh.matrixs == null || dh.matrixs.Count == 0) && dh.criteriasComparison[dh.criteriasComparison.Count - 1][0].Key != dh.criterias[dh.criterias.Count - 1][0])
                return;
            int count = (dh.matrixs == null || dh.matrixs.Count == 0) ? dh.criteriasComparison[dh.criteriasComparison.Count - 1].Count : dh.matrixs.Count;
            for (int i = 0; i < count; i++)
            {
                string[,] matrix = (dh.matrixs == null || dh.matrixs.Count == 0) ? dh.criteriasComparison[dh.criteriasComparison.Count - 1][i].Value : dh.matrixs[i].Value;
                for (int k = 0; k < matrix.GetLength(0) - 1; k++)
                    for (int s = k + 1; s < matrix.GetLength(1); s++)
                    {
                        double value = ParseValue(matrix[k, s]);
                        if (value < 1)
                        {
                            value = Math.Round(1.0 / value);
                            tboxes[s, k].Text = "1/" + value.ToString();
                        }
                        else
                            tboxes[s, k].Text = value.ToString();
                    }
                TabHandle.FindButton(tabControl1.SelectedTab).PerformClick();
            }
        }*/
    }
}
