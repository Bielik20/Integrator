using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integrator.Models
{
    static class RollEngine
    {
        static Random rnd = new Random();

        public static void Roll(int[] tab, int maxVal)
        {
            for (int i = 0; i < tab.Count(); i++)
            {
                tab[i] = rnd.Next(0, maxVal);
            }
        }

        public static void RollNoise(double[] real, double[] imag, int snr)
        {
            double alfa = Math.Sqrt(0.5 * Math.Pow(10, -snr / 10));

            for (int i = 0; i < real.Count(); i++)
            {
                double x1, x2, w, y1, y2;
                do
                {
                    x1 = 2.0 * rnd.NextDouble() - 1.0;
                    x2 = 2.0 * rnd.NextDouble() - 1.0;
                    w = x1 * x1 + x2 * x2;
                } while (w >= 1.0);

                w = Math.Sqrt((-2.0 * Math.Log(w)) / w);
                y1 = x1 * w;
                y2 = x2 * w;

                real[i] += y1 * alfa;
                imag[i] += y2 * alfa;
            }
        }
    }
}
