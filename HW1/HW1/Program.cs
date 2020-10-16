using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HW1
{
    public abstract class OptimisationMethod
    {
        protected readonly double Epsilon;
        protected OptimisationMethod(double epsilon)
        {
            Epsilon = epsilon;
        }

        public abstract (double, double) FindMinimum(Func<double, double> f, double l, double r);

        protected void UpdateInterval(ref double l, ref double r, double x1, double x2, double y1, double y2)
        {
            if (x1 > x2)
            {
                throw new ArgumentOutOfRangeException(nameof(x1) + ", " + nameof(x2), "x1 should be less than x2");
            }
            
            if (Math.Abs(y1 - y2) < Epsilon)
            {
                l = x1;
                r = x2;
            }
            else if (y1 > y2)
            {
                l = x1;
            }
            else
            {
                r = x2;
            }
        }
    }

    public class DichotomyMethod : OptimisationMethod
    {
        public DichotomyMethod(double epsilon) : base(epsilon) { }
        
        public override (double, double) FindMinimum(Func<double, double> f, double l, double r)
        {
            if (l > r)
            {
                throw new ArgumentOutOfRangeException(nameof(l) + ", " + nameof(r), "l should be less than r");
            }

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
        
    }

    public class GoldenSectionMethod : OptimisationMethod
    {
        private const double F = 1.618033988749894848;

        public GoldenSectionMethod(double epsilon) : base(epsilon) { }

        public override (double, double) FindMinimum(Func<double, double> f, double l, double r)
        {
            Dictionary<double, double> cache = new Dictionary<double, double>();
            if (l > r)
            {
                throw new ArgumentOutOfRangeException(nameof(l) + ", " + nameof(r), "l should be less than r");
            }
            
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
    }
    
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}