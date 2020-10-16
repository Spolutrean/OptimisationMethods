using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HW1
{
    public abstract class OptimizationMethod
    {
        protected readonly double Epsilon;
        protected OptimizationMethod(double epsilon)
        {
            Epsilon = epsilon;
        }

        public abstract (double X, double Y) FindMinimum(Func<double, double> f, double l, double r);

        protected void UpdateInterval(ref double l, ref double r, double x1, double x2, double y1, double y2)
        {
            if (y1 > y2)
            {
                l = x1;
            }
            else
            {
                r = x2;
            }
        }
    }

    public class DichotomyMethod : OptimizationMethod
    {
        public DichotomyMethod(double epsilon) : base(epsilon) { }
        
        public override (double X, double Y) FindMinimum(Func<double, double> f, double l, double r)
        {
            double delta = Epsilon / 4;
            while (Math.Abs(r - l) > Epsilon)
            {
                double middle = (r + l) / 2;
                double x1 = middle - delta, 
                    x2 = middle + delta;
                double y1 = f(x1), y2 = f(x2);

                UpdateInterval(ref l, ref r, x1, x2, y1, y2);
            }

            double answerX = (r + l) / 2;
            double answerY = f(answerX);
            return (answerX, answerY);
        }

        public override string ToString()
        {
            return "DichotomyMethod";
        }
    }

    public class GoldenSectionMethod : OptimizationMethod
    {
        private const double F = 1.618033988749894848;

        public GoldenSectionMethod(double epsilon) : base(epsilon) { }

        public override (double X, double Y) FindMinimum(Func<double, double> f, double l, double r)
        {
            Dictionary<double, double> cache = new Dictionary<double, double>();

            while (Math.Abs(r - l) > Epsilon)
            {
                double x1 = r - (r - l) / F, 
                    x2 = l + (r - l) / F;
                double y1 = Ask(x1), y2 = Ask(x2);

                UpdateInterval(ref l, ref r, x1, x2, y1, y2);
            }

            double answerX = (r + l) / 2;
            double answerY = f(answerX);
            return (answerX, answerY);

            double Ask(double x)
            {
                if (!cache.ContainsKey(x))
                {
                    cache[x] = f(x);
                }
                return cache[x];
            }
        }
        
        public override string ToString()
        {
            return "GoldenSectionMethod";
        }
    }

    public class FibonacciMethod : OptimizationMethod
    {
        public FibonacciMethod(double epsilon) : base(epsilon) { }

        public override (double X, double Y) FindMinimum(Func<double, double> f, double l, double r)
        {
            Dictionary<double, double> cache = new Dictionary<double, double>();

            double f1 = 0, f2 = 1, f3 = 1;
            while (f3 <= (r - l) / Epsilon)
            {
                f1 = f2;
                f2 = f3;
                f3 = f1 + f2;
            }

            double x1 = l + f1 / f3 * (r - l);
            double x2 = l + f2 / f3 * (r - l);
            double y1, y2;

            while (Math.Abs(r - l) > Epsilon)
            {
                y1 = Ask(x1);
                y2 = Ask(x2);
                UpdateInterval(ref l, ref r, ref x1, ref x2, y1, y2);
            }
            
            double answerX = (r + l) / 2;
            double answerY = f(answerX);
            return (answerX, answerY);
            
            double Ask(double x)
            {
                if (!cache.ContainsKey(x))
                {
                    cache[x] = f(x);
                }
                return cache[x];
            }
        }

        void UpdateInterval(ref double l, ref double r, ref double x1, ref double x2, double y1, double y2)
        {
            if (y1 > y2)
            {
                l = x1;
                x1 = l + (r - x2);
            }
            else
            {
                r = x2;
                x2 = r - (x1 - l);
            }

            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
            }
        }
        
        public override string ToString()
        {
            return "FibonacciMethod";
        }
    }
    
    public class Program
    {
        static void Main(string[] args)
        {
            (double l, double r, Func<double, double> f)[] queries =
            {
                (-0.5, 0.5,
                    x => -5 * Math.Pow(x, 5) + 4 * Math.Pow(x, 4) - 12 * Math.Pow(x, 3) + 11 * Math.Pow(x, 2) - 2 * x + 1),
                (6, 9.9,
                    x => Math.Pow(Math.Log10(x - 2), 2) + Math.Pow(Math.Log10(10 - x), 2) - Math.Pow(x, 0.2)),
                (0, Math.PI * 2,
                    x => -3 * x * Math.Sin(0.75 * x) + Math.Exp(x) - 2 * x),
                (0, 1,
                    x => Math.Exp(3 * x) + 5 * Math.Exp(x) - 2 * x),
                (0.5, 2.5, 
                    x => 0.2 * x * Math.Log10(x) + (x - 2.3) * (x - 2.3))
            };
            
            double epsilon = 1e-5;
            OptimizationMethod[] methods =
                {new DichotomyMethod(epsilon), new GoldenSectionMethod(epsilon), new FibonacciMethod(epsilon)};

            foreach (var query in queries)
            {
                foreach (var method in methods)
                {
                    var result = method.FindMinimum(query.f, query.l, query.r);
                    Console.WriteLine(method + ": X=" + result.X + " min Y=" + result.Y);
                }
                Console.WriteLine();
            }

        }
    }
}