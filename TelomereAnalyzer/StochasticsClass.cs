using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelomereAnalyzer
{
    public class StochasticsClass : IDisposable
    {
        public StochasticsClass()
        {
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~StochasticsClass()
        {
            Dispose();
        }

        public Double Median(Double[] x)
        {
            Double Medi = 0.0;
            int n = x.Length;
            Array.Sort(x);

            if (n % 2 == 0)
            {
                int o = n / 2;
                int u = o - 1;
                Medi = (x[u] + x[o]) / 2.0;
            }
            else
                Medi = x[((n + 1) / 2) - 1];

            return Medi;
        }

        public bool MeanValue(VesselClass[] vessels, ref Double meanValue)
        {
            if (vessels == null)
                return false;

            if (vessels.Length < 2)
                return false;

            Double[] values = new Double[vessels.Length];

            for (Int32 v = 0; v < vessels.Length; v++)
                values[v] = vessels[v]._area;

            meanValue = MeanValue(values);
            return true;
        }

        public bool StdevValue(VesselClass[] vessels, ref Double meanValue, ref Double stdDevValue)
        {
            if (vessels == null)
                return false;

            if (vessels.Length < 2)
                return false;

            Double[] values = new Double[vessels.Length];

            for (Int32 v = 0; v < vessels.Length; v++)
                values[v] = vessels[v]._area;

            meanValue = MeanValue(values);
            stdDevValue = StandardDeviation(values, meanValue);
            return true;
        }

        public Double MeanValue(Double[] x)
        {
            int N = x.Length;
            Double MWx = 0.0;
            Double NN = Convert.ToDouble(N);

            for (Int32 A = 0; A < N; A++)
            {
                MWx += x[A];
            }
            MWx /= NN;

            return MWx;
        }

        public Double StandardDeviation(Double[] x)
        {
            int N = x.Length;
            Double AbweichungX = 0.0;
            Double SummeAbweichungsQuadratX = 0.0;
            Double MWx = 0.0;
            Double NN = Convert.ToDouble(N);
            Double MaxValue = Math.Sqrt(Double.MaxValue);

            for (Int32 A = 0; A < N; A++)
            {
                MWx += x[A];
            }

            MWx /= (NN - 1.0);

            for (Int32 B = 0; B < N; B++)
            {
                AbweichungX = x[B] - MWx;
                SummeAbweichungsQuadratX += AbweichungX * AbweichungX;
            }

            if (SummeAbweichungsQuadratX == 0.0)
                return 0.0;

            Double varianz = SummeAbweichungsQuadratX / NN;

            return Math.Sqrt(varianz);
        }

        public Double StandardDeviation(Double[] x, Double meanValue)
        {
            int N = x.Length;
            Double AbweichungX = 0.0;
            Double SummeAbweichungsQuadratX = 0.0;
            Double NN = Convert.ToDouble(N);
            Double MaxValue = Math.Sqrt(Double.MaxValue);

            for (Int32 B = 0; B < N; B++)
            {
                AbweichungX = x[B] - meanValue;
                SummeAbweichungsQuadratX += AbweichungX * AbweichungX;
            }

            if (SummeAbweichungsQuadratX == 0.0)
                return 0.0;

            Double varianz = SummeAbweichungsQuadratX / NN;

            return Math.Sqrt(varianz);
        }

        public double IsoPerimetricQuotient(double Perimeter, double Area)
        {
            double d = Perimeter / Math.PI;
            double KreisVol = (d * d * Math.PI) / 4.0;
            return Area / KreisVol;
        }

        public double Circularity(double Perimeter, double Area)
        {
            double p = 0.95 * Perimeter;
            double a = 4.0 * Math.PI * Area;
            return a / (p * p);
        }

        public double widthHeightVerhältnis(int width, int height)
        {
            if (width == height)
                return 1;

            if (width < height)
                return (width * 1.0) / height;

            else
                return (height * 1.0) / width;
        }

        public double[] KurveGlätten(Double[] Vektorlängen, Int32 numMean)
        {
            Int32 Integr = Convert.ToInt32(numMean);

            if (Integr == 0)
                return Vektorlängen;

            if (Vektorlängen.Length - Integr <= 0)
            {
                //MessageBox.Show("Objekt ist zu klein zum Glätten");
                return Vektorlängen;
            }

            Double[] Shadow = new Double[Vektorlängen.Length - Integr];
            Double MW = 0.0;

            for (Int32 A = 0; A < Vektorlängen.Length - Integr; A++)
            {
                MW = 0.0;

                for (Int32 M = 0; M < Integr; M++)
                    MW += Vektorlängen[A + M];

                MW /= (Double)(Integr);
                Shadow[A] = MW;
            }
            return Shadow;
        }

        public double Mode(double[] x, ref int maxcount)
        {
            var groups = x.GroupBy(v => v);
            int maxCount = groups.Max(g => g.Count());
            double mode = groups.First(g => g.Count() == maxCount).Key;
            maxcount = maxCount;
            return mode;
        }
    }
}
