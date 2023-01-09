using System;
using System.Collections.Generic;
using System.Linq;

namespace lab1
{
    public static class Misc
    {
        public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180;
        public static int Factorial(int n)
        {
            if (n <= 1) return 1;

            return n * Factorial(n - 1);
        }

        public static IEnumerable<double> Arange(double start, int count)
        {
            return Enumerable.Range((int)start, count).Select(v => (double)v);
        }

        public static IEnumerable<double> LinSpace(double start, double stop, int num, bool endpoint = true)
        {
            var result = new List<double>();
            if (num <= 0)
            {
                return result;
            }

            if (endpoint)
            {
                if (num == 1)
                {
                    return new List<double>() { start };
                }

                var step = (stop - start) / ((double)num - 1.0d);
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }
            else
            {
                var step = (stop - start) / (double)num;
                result = Arange(0, num).Select(v => (v * step) + start).ToList();
            }

            return result;
        }
    }
}
