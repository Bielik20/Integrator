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

        public static void Roll_4(int[] tab)
        {
            for (int i = 0; i < tab.Count(); i++)
            {
                tab[i] = rnd.Next(0, 3);
            }
        }
        public static void Roll_16(int[] tab)
        {
            for (int i = 0; i < tab.Count(); i++)
            {
                tab[i] = rnd.Next(0, 15);
            }
        }
        public static void Roll_64(int[] tab)
        {
            {
                for (int i = 0; i < tab.Count(); i++)
                {
                    tab[i] = rnd.Next(0, 63);
                }
            }
        }
        public static void RollNoise(double[] real, double[] imag, int snr)
        {
            var linear_snr = Math.Pow(10, snr);
            for (int i = 0; i < real.Count(); i++)
            {
                real[i] += rnd.NextDouble() * linear_snr;
                imag[i] += rnd.NextDouble() * linear_snr;
            }
        }
    }
}
