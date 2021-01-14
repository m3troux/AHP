using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace AHP
{
    class DataHandle
    {
        public string problemName { get; set; }
        public int levels { get; set; }
        public int[] criterias_q { get; set; }
        public int alternatives_q { get; set; }
        public string[] alternatives { get; set; }
        public List<string[]> criterias { get; set; }
        public List<List<KeyValuePair<string, string[,]>>> criteriasComparison = new List<List<KeyValuePair<string, string[,]>>>();
        public List<KeyValuePair<string, double>> results = new List<KeyValuePair<string, double>>();
        public List<KeyValuePair<string, string[,]>> matrixs = new List<KeyValuePair<string, string[,]>>();

        public DataHandle() { }
        public void CollectData(TabControl tabControl3, TabControl tabControl4)
        {
            problemName = AHP.problemName;
            levels = AHP.levels;
            criterias_q = AHP.criterias_q;
            alternatives_q = AHP.alternatives_q;
            if (AHP.alternatives != null)
                alternatives = AHP.alternatives;
            if (AHP.criterias != null)
                criterias = new List<string[]>(AHP.criterias);
            if (AHP.criteriasComparison != null && AHP.criteriasComparison.Count != 0)
            {
                criteriasComparison.Clear();
                matrixs.Clear();
                TextBox[,] tboxes = GetTextBoxMatrix(tabControl3.TabPages[0]);
                List<KeyValuePair<string, string[,]>> ttmp = new List<KeyValuePair<string, string[,]>>();
                ttmp.Add(new KeyValuePair<string, string[,]>(AHP.criteriasComparison[0][0].mainCriteria, GetStringMatrix(tboxes)));
                criteriasComparison.Add(new List<KeyValuePair<string, string[,]>>(ttmp));

                for (int i = 1; i < AHP.criteriasComparison.Count; i++)
                {
                    for (int j = 0; j < AHP.criteriasComparison[i].Count; j++)
                        foreach (TabPage tab in tabControl3.TabPages)
                            if (tab.Name == AHP.criteriasComparison[i][j].mainCriteria)
                            {
                                tboxes = GetTextBoxMatrix(tab);
                                matrixs.Add(new KeyValuePair<string, string[,]>(AHP.criteriasComparison[i][j].mainCriteria, GetStringMatrix(tboxes)));
                            }
                    if (matrixs.Count != 0)
                        criteriasComparison.Add(new List<KeyValuePair<string, string[,]>>(matrixs));
                    matrixs.Clear();
                }
                for (int i = 0; i < AHP.criteriasComparison[AHP.criteriasComparison.Count - 1].Count; i++)
                {
                    foreach (TabPage tab in tabControl4.TabPages)
                        if (tab.Name == AHP.criteriasComparison[AHP.criteriasComparison.Count - 1][i].mainCriteria)
                        {
                            tboxes = GetTextBoxMatrix(tab);
                            matrixs.Add(new KeyValuePair<string, string[,]>(AHP.criteriasComparison[AHP.criteriasComparison.Count - 1][i].mainCriteria, GetStringMatrix(tboxes)));
                        }
                }
                if (matrixs.Count != 0)
                    criteriasComparison.Add(new List<KeyValuePair<string, string[,]>>(matrixs));
                matrixs.Clear();

                foreach (TabPage tab in tabControl3.TabPages)
                {
                    for (int i = 0; i < AHP.criterias.Count; i++)
                    {
                        for (int j = 0; j < AHP.criterias[i].Length; j++)
                            if (tab.Name == AHP.criterias[i][j])
                            {
                                tboxes = GetTextBoxMatrix(tab);
                                foreach (TextBox t in tboxes)
                                    if (t.Text == "")
                                        return;
                                matrixs.Add(new KeyValuePair<string, string[,]>(AHP.criterias[i][j], GetStringMatrix(tboxes)));
                            }
                        if (matrixs.Count == AHP.criterias[i].Length)
                            matrixs.Clear();
                    }
                }

                if (tabControl4.TabPages.Count != 0)
                {
                    foreach (TabPage tab in tabControl4.TabPages)
                    {
                        tboxes = GetTextBoxMatrix(tab);
                        foreach (TextBox t in tboxes)
                            if (t.Text == "")
                                return;
                        matrixs.Add(new KeyValuePair<string, string[,]>(tab.Name, GetStringMatrix(tboxes)));
                    }
                    if (matrixs.Count == AHP.criterias[AHP.criterias.Count - 1].Length)
                        matrixs.Clear();
                }

                if (AHP.results != null && AHP.results.Count != 0)
                {
                    results = new List<KeyValuePair<string, double>>(AHP.results);
                }
            }
        }
        private static TextBox[,] GetTextBoxMatrix(TabPage tabp)
        {
            TextBox[] tmp = TabHandle.FindTextBoxes(tabp);
            TextBox[,] tboxes = new TextBox[(int)Math.Sqrt(tmp.Length), (int)Math.Sqrt(tmp.Length)];
            int ind = 0;
            for (int i = 0; i < tboxes.GetLength(0); i++)
            {
                for (int j = 0; j < tboxes.GetLength(1); j++)
                {
                    tboxes[j, i] = tmp[ind];
                    ind++;
                }
            }
            return tboxes;
        }
        public static string[,] GetStringMatrix(TextBox[,] tboxes)
        {
            string[,] matrix = new string[tboxes.GetLength(0), tboxes.GetLength(1)];
            for (int i = 0; i < tboxes.GetLength(0); i++)
                for (int j = 0; j < tboxes.GetLength(1); j++)
                    matrix[i, j] = tboxes[j, i].Text;
            return matrix;
        }
    }
}
