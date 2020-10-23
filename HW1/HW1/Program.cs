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
            if (x1 > x2)
            {
                (x1, x2) = (x2, x1);
                (y1, y2) = (y2, y1);
            }
            
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

    public class ParabolasMethod : OptimizationMethod
    {
        public ParabolasMethod(double epsilon) : base(epsilon) { }

        public override (double X, double Y) FindMinimum(Func<double, double> f, double l, double r)
        {
            Dictionary<double, double> cache = new Dictionary<double, double>();

            while (Math.Abs(r - l) > Epsilon)
            {
                double x2 = (r + l) / 2;
                double y1 = Ask(l), y2 = Ask(x2), y3 = Ask(r);

                double numerator = (x2 - l) * (x2 - l) * (y2 - y3) - (x2 - r) * (x2 - r) * (y2 - y1);
                double denominator = 2 * ((x2 - l) * (y2 - y3) - (x2 - r) * (y2 - y1));
                double u = x2 - numerator / denominator;
                double yu = Ask(u);
                UpdateInterval(ref l, ref r, x2, u, y2, yu);
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
            return "ParabolasMethod";
        }
    }

    public class BrentsCombinedMethod : OptimizationMethod
    {
        public BrentsCombinedMethod(double epsilon) : base(epsilon) { }

        public override (double X, double Y) FindMinimum(Func<double, double> f, double l, double r)
        {
            double K = (3 - Math.Sqrt(5)) / 2;

            double x, w, v, fx, fw, fv;
            x = w = v = (r + l) / 2;
            fx = fw = fv = f(x);

            double d, e;
            d = e = r - l;

            while (Math.Abs(r - l - 2 * Epsilon) > Epsilon)
            {
                double g = e;
                e = d;

                double u;
                if (x != w && x != v && w != v && fx != fw && fx != fv && fw != fv)
                {
                    double[] X = { x, w, v };
                    double[] Y = { fx, fw, fv };

                    for (int i = 0; i < 3; ++i)
                    {
                        for (int j = 0; j < 2; ++j)
                        {
                            if (X[j] > X[j + 1])
                            {
                                (X[j], X[j + 1]) = (X[j + 1], X[j]);
                                (Y[j], Y[j + 1]) = (Y[j + 1], Y[j]);
                            }
                        }
                    }

                    double numerator = (X[1] - X[0]) * (X[1] - X[0]) * (Y[1] - Y[2]) - (X[1] - X[2]) * (X[1] - X[2]) * (Y[1] - Y[0]);
                    double denominator = 2 * ((X[1] - X[0]) * (Y[1] - Y[2]) - (X[1] - X[2]) * (Y[1] - Y[0]));
                    u = X[1] - numerator / denominator;

                    if (u >= l + Epsilon && u <= r - Epsilon && Math.Abs(u - x) < g / 2)
                    {
                        d = Math.Abs(u - x);
                    }
                    else
                    {
                        u = GoldenSectionNext();
                    }
                }
                else
                {
                    u = GoldenSectionNext();
                }

                if (Math.Abs(u - x) < Epsilon)
                {
                    u = x + Math.Sign(u - x) * Epsilon;
                }

                double fu = f(u);
                UpdateInterval(ref l, ref r, x, u, fx, fu);
                if (fu < fx)
                {
                    (v, w, x) = (w, x, u);
                    (fv, fw, fx) = (fw, fx, fu);
                }
                else
                {
                    if (fu <= fw || w == x)
                    {
                        (v, w) = (w, u);
                        (fv, fw) = (fw, fu);
                    }
                    else if (fu <= fv || v == x || v == w)
                    {
                        v = u;
                        fv = fu;
                    }
                    else
                    {
                        break;
                    }
                }

                double GoldenSectionNext()
                {
                    double result;
                    if (2 * x < r + l)
                    {
                        result = x + K * (r - x);
                        d = r - x;
                    }
                    else
                    {
                        result = x - K * (x - l);
                        d = x - l;
                    }

                    return result;
                }
            }

            double answerX = x;
            double answerY = f(answerX);
            return (answerX, answerY);
        }

        public override string ToString()
        {
            return "BrentsCombinedMethod";
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
                    x => -3 * x * Math.Sin(0.75 * x) + Math.Exp(-2 * x)),
                (0, 1,
                    x => Math.Exp(3 * x) + 5 * Math.Exp(-2 * x)),
                (0.5, 2.5, 
                    x => 0.2 * x * Math.Log10(x) + (x - 2.3) * (x - 2.3))
            };
            
            double epsilon = 1e-5;
            OptimizationMethod[] methods =
            {
                new DichotomyMethod(epsilon), 
                new GoldenSectionMethod(epsilon), 
                new FibonacciMethod(epsilon), 
                new ParabolasMethod(epsilon),
                new BrentsCombinedMethod(epsilon)
            };

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