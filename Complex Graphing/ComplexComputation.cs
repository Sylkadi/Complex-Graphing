
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Complex_Graphing
{
    internal class ComplexComputation
    {

        public struct Complex
        {
            public double a; // Real Part
            public double b; // Imaginary Part

            public Complex(double A, double B)
            {
                a = A;
                b = B;
            }


            public override string ToString()
            {
                if (b >= 0) return $"{a}+{b}i";
                return $"{a}{b}i";
            }

            #region Basic Arithmetic

            // Additon

            public static Complex operator +(Complex A, Complex B)
            {
                return new Complex(A.a + B.a, A.b + B.b);
            }

            public static Complex operator +(Complex A, double B)
            {
                return new Complex(A.a + B, A.b);
            }

            public static Complex operator +(double A, Complex B)
            {
                return new Complex(A + B.a, B.b);
            }

            // Subtraction

            public static Complex operator -(Complex A, Complex B)
            {
                return new Complex(A.a - B.a, A.b - B.b);
            }

            public static Complex operator -(Complex A, double B)
            {
                return new Complex(A.a - B, A.b);
            }

            public static Complex operator -(double A, Complex B)
            {
                return new Complex(A - B.a, B.b * -1);
            }

            // Multiplication

            public static Complex operator *(Complex A, Complex B)
            {
                return new Complex((A.a * B.a) - (A.b * B.b), (A.a * B.b) + (A.b * B.a));
            }

            public static Complex operator *(Complex A, double B)
            {
                return new Complex(A.a * B, A.b * B);
            }

            public static Complex operator *(double A, Complex B)
            {
                return new Complex(A * B.a, A * B.b);
            }

            // Division

            public static Complex operator /(Complex A, Complex B)
            {
                double Divisor = 1 / ((B.a * B.a) + (B.b * B.b));
                return new Complex(((A.a * B.a) + (A.b * B.b)) * Divisor, ((A.b * B.a) - (A.a * B.b)) * Divisor);
            }

            public static Complex operator /(Complex A, double B)
            {
                return new Complex(A.a / B, A.b / B);
            }

            public static Complex operator /(double A, Complex B)
            {
                double Divisor = 1 / ((B.a * B.a) + (B.b * B.b));
                return new Complex((B.a * A) * Divisor, (B.b * A) * Divisor * -1);
            }

            #endregion

            // Powers

            public static Complex operator ^(Complex A, Complex B)
            {
                return Exp(B * ln(A));
            }

            public static Complex operator ^(Complex A, double B)
            {
                return Exp(B * ln(A));
            }

            public static Complex operator ^(double A, Complex B)
            {
                return Exp(B * ln(new Complex(A, 0)));
            }

            public static Complex Exp(Complex A) // e^(a+bi) where e approx 2.718... euler's constant
            {
                double Exponent = Math.Exp(A.a);
                return new Complex(Exponent * Math.Cos(A.b), Exponent * Math.Sin(A.b));
            }

            public static double Absolute(Complex A) // Absolute value of a complex value
            {
                return Math.Sqrt((A.a * A.a) + (A.b * A.b));
            }

            public static Complex ln(Complex A) // Inverse of Complex.Exp the natural log, ln(a+bi)
            {
                return new Complex(Math.Log(Absolute(A)), Math.Atan2(A.b, A.a));
            }

            public static Complex Cos(Complex A) // Cosine
            {
                return new Complex(Math.Cos(A.a) * Math.Cosh(A.b), -1 * Math.Sin(A.a) * Math.Sinh(A.b));
            }

            public static Complex Sin(Complex A) // Sine
            {
                return new Complex(Math.Sin(A.a) * Math.Cosh(A.b), Math.Cos(A.a) * Math.Sinh(A.b));
            }

            public static Complex Tan(Complex A) // Tangent
            {
                return Sin(A) / Cos(A);
            }



        }

    }
}
