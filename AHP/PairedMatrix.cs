using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHP
{
    public class PairedMatrix
    {
        private static Dictionary<int, double> Pn = new Dictionary<int, double>()
        {
            { 1, 0 }, { 2, 0 }, { 3, 0.58 }, { 4, 0.9 }, { 5, 1.12 },
            { 6, 1.24 }, { 7, 1.32}, { 8, 1.41}, { 9, 1.45 }, { 10, 1.49}, { 11, 1.51 }
        };
        public string mainCriteria;
        string[] comparableCriterias;
        public double[,] matrix;
        int size;
        double[] GeomMeanRow;
        double[] NPVRow;
        double Lmax;
        double CI;
        double CR;

        public PairedMatrix() { }
        public PairedMatrix(string mainCrit, string[] compCrit, double[,] m)
        {
            mainCriteria = mainCrit;
            comparableCriterias = compCrit;
            matrix = m;
            size = matrix.GetLength(0);
            GeomMeanRow = GetGeomMeanRow(matrix);
            NPVRow = CalcNPVRow(GeomMeanRow);
            Lmax = GetLmax(matrix);
            CI = (Lmax - size) / (double)(size - 1);
            CR = CI / Pn[size];
        }
        public PairedMatrix(string mainCrit, double[,] m)
        {
            mainCriteria = mainCrit;
            matrix = m;
            size = matrix.GetLength(0);
            GeomMeanRow = GetGeomMeanRow(matrix);
            NPVRow = CalcNPVRow(GeomMeanRow);
            Lmax = GetLmax(matrix);
            CI = (Lmax - size) / (double)(size - 1);
            CR = CI / Pn[size];
        }
        public static PairedMatrix EditPairedMatrix(PairedMatrix pm, double[,] m)
        {
            return new PairedMatrix(pm.mainCriteria, pm.comparableCriterias, m);
        }
        private double[] GetGeomMeanRow(double[,] matrix)
        {
            double[] res = new double[size];
            for (int i = 0; i < size; i++)
            {
                double geomMean = 0.0;
                double product = 1.0;
                for (int j = 0; j < size; j++)
                {
                    product *= matrix[i, j];
                    geomMean = Math.Pow(product, 1.0 / size);
                }
                res[i] = geomMean;
            }
            return res;
        }
        public double[] GetNPVRow()
        {
            return this.NPVRow;
        }
        private double[] CalcNPVRow(double[] geomMean)
        {
            double[] res = new double[geomMean.Length];
            double geomSum = GeomMeanRow.Sum();
            for (int i = 0; i < geomMean.Length; i++)
                res[i] = geomMean[i] / geomSum;
            return res;
        }
        private double GetLmax(double[,] matrix)
        {
            double res = 0.0;
            for (int i = 0; i < size; i++)
            {
                double sum = 0.0;
                for (int j = 0; j < size; j++)
                    sum += matrix[j, i];
                res += sum * NPVRow[i];
            }
            return res;
        }
    }
}
