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
            var alfa = Math.Sqrt(1 / 2 * Math.Pow(10, -snr / 10));
            for (int i = 0; i < real.Count(); i++)
            {
                real[i] += rnd.NextDouble() * alfa;
                imag[i] += rnd.NextDouble() * alfa;
            }
        }
    }
}
