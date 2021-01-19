using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP
{
    public static class AHP
    {
        public static string problemName { get; set; }
        public static int levels { get; set; }
        public static int[] criterias_q { get; set; }
        public static int alternatives_q { get; set; }
        public static string[] alternatives { get; set; }
        public static List<string[]> criterias = new List<string[]>();
        public static List<List<PairedMatrix>> criteriasComparison = new List<List<PairedMatrix>>();
        public static List<KeyValuePair<string, double>> results { get; set; }

        public static void Calculate()
        {
            List<double[,]> matrixes = new List<double[,]>();
            foreach (List<PairedMatrix> l in criteriasComparison)
            {
                double[,] matrix = MakeNPVMatrix(l);
                matrixes.Add(matrix);
            }
            matrixes.Reverse(); // перемножение с нижнего уровня
            double[,] tmp = MultiplyMatrix(matrixes.ToArray());
            // transpose results
            List<KeyValuePair<string, double>> res = new List<KeyValuePair<string, double>>();
            for (int i = 0; i < alternatives_q; i++)
                res.Add(new KeyValuePair<string, double>(alternatives[i], tmp[i, 0]));
            results = res;
        }
        public static void SortResults(bool desc)
        {
            double FindValue(string key)
            {
                foreach (KeyValuePair<string, double> x in results)
                    if (x.Key == key)
                        return x.Value;
                return 0;
            }
            if (desc)
                results.Sort((x, y) => y.Value.CompareTo(x.Value));
            else
            {
                List<KeyValuePair<string, double>> res = new List<KeyValuePair<string, double>>();
                for (int i = 0; i < alternatives.Length; i++)
                {
                    double value = FindValue(alternatives[i]);
                    res.Add(new KeyValuePair<string, double>(alternatives[i], value));
                }
                results = res;
            }
        }
        private static double[,] MakeNPVMatrix(List<PairedMatrix> list)
        {
            double[,] res = new double[list[0].GetNPVRow().Length, list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                //if (i == 0)
                    //res = (double[,])ResizeArray(res, new int[] { list[i].GetNPVRow().Length, list.Count });
                for (int j = 0; j < res.GetLength(0); j++)
                    res[j, i] = list[i].GetNPVRow()[j];
            }
            return res;
        }
        /*private static Array ResizeArray(Array arr, int[] newSizes)
        {
            if (newSizes.Length != arr.Rank)
                throw new ArgumentException("arr must have the same number of dimensions " +
                                            "as there are elements in newSizes", "newSizes");

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }*/
        private static double[,] MultiplyMatrix(params double[][,] matrixes)
        {
            double[,] res = matrixes[0];
            for (int m = 1; m < matrixes.Length; m++)
            {
                double[,] c = new double[res.GetLength(0), matrixes[m].GetLength(1)];
                for (int i = 0; i < c.GetLength(0); i++)
                    for (int j = 0; j < c.GetLength(1); j++)
                    {
                        c[i, j] = 0;
                        for (int k = 0; k < res.GetLength(1); k++)
                            c[i, j] = c[i, j] + res[i, k] * matrixes[m][k, j];
                    }
                res = c;
            }
            return res;
        }
        public static void Reset()
        {
            problemName = null;
            levels = 0;
            criterias_q = null;
            alternatives_q = 0;
            alternatives = null;
            criterias = new List<string[]>();
            criteriasComparison = new List<List<PairedMatrix>>();
            results = null;
        }
    }
}
