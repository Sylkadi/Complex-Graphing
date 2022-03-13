using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using C = Complex_Graphing.ComplexComputation.Complex;
using System.IO;
using Accord.Extensions.Imaging;

namespace Complex_Graphing
{
    internal class BitmapComputation
    {

        public static Bitmap ComputeBitMap(Func<C, C> Equation, int ImageWidth, int ImageHeight, double YBottom, double YTop)
        {
            Bitmap bitmap = new Bitmap(ImageWidth, ImageHeight);

            // Screen Fitting Calculations
            double Ratio = (double)ImageWidth / ImageHeight;
            double XLeft = YBottom * Ratio, XRight = YTop * Ratio; // Scale Left and right with proportion to top and bottom
            double TopBottom = Math.Abs(YTop - YBottom) / ImageHeight, LeftRight = Math.Abs(XLeft - XRight) / ImageWidth;

            // Lockbits and get buffer
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] buffer = new byte[data.Width * data.Height * (Image.GetPixelFormatSize(data.PixelFormat) / 8)];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);

            // For Mulithreading
            void ComputePixelColor(int YStart, int YEnd)
            {
                for (int x = 0; x != ImageWidth; x++)
                {
                    double X = (LeftRight * x) + XLeft;
                    for (int y = YStart; y != YEnd; y++)
                    {
                        double Y = (TopBottom * y) + YBottom;
                        C c = Equation(new C(X, Y)); // Compute Complex function

                        // Change C.a and C.b to X and Y to show the domain coloring or else this will show the output
                        int offset = ((y * ImageWidth) + x) * 4;
                        buffer[offset] = Fix(Red(c.a, c.b));       // Red
                        buffer[offset + 1] = Fix(Green(c.a, c.b)); // Green
                        buffer[offset + 2] = Fix(Blue(c.a, c.b));  // Blue
                        buffer[offset + 3] = 255;                  // Alpha
                    }
                }
            }

            // Multithreading
            int NumberOfThreads = Environment.ProcessorCount;

            // Make list of splices for each thread to take
            List<Point> StartEndList = new List<Point>();
            int Slice = ImageHeight / NumberOfThreads;
            for (int i = 0; i != NumberOfThreads - 1; i++)
            {
                StartEndList.Add(new Point(Slice * i, (Slice * i) + Slice));
            }
            StartEndList.Add(new Point(Slice * (NumberOfThreads - 1), ImageHeight));


            Parallel.ForEach(StartEndList, point => // Initiate the multithreading
            {
                ComputePixelColor(point.X, point.Y);
            });

            // Undo lockbits and copy buffer back to bitmap
            Marshal.Copy(buffer, 0, data.Scan0, buffer.Length);
            bitmap.UnlockBits(data);

            return bitmap;
        }


        // Domain Colouring
        public static double Red(double x, double y)
        {
            return ((x * x) + (y * y)) * 200.0;
        }

        public static double Green(double x, double y)
        {
            return ((x * x) + (y * y)) * 200.0;
        }

        public static double Blue(double x, double y)
        {
            return ((x * x) + (y * y)) * 200.0;
        }

        public static byte Fix(double Input)
        {
            int NewColor = (int)Math.Abs(162.3380419537332424842614386399646 * Math.Atan(Input * 0.003921568627450980392156862745098)); // First is 510/pi and second constant is 1/255
            if (NewColor == int.MinValue) return 255; // fix problems with ln(0) and division by zero
            return (byte)NewColor;
        }

        // Video (has its own problems you many encounter)
        public static void CreateAvi(string OutputDirectoryWithName, int Width, int Height, int FrameCount, int Fps, double YBottom, double YTop, Func<C, C> Equation)
        {
            string ImagesFolder = Environment.CurrentDirectory + @"\bitmapimages";
            Bitmap bm = new Bitmap(Width, Height);

            if (!Directory.Exists(ImagesFolder))
            {
                Directory.CreateDirectory(ImagesFolder);
            } else {
                DirectoryInfo di = new DirectoryInfo(ImagesFolder);
                foreach(FileInfo File in di.GetFiles())
                {
                    File.Delete();
                }
            }

            bm = ComputeBitMap(Equation, Width, Height, YBottom, YTop); // Pre Calculate for first image

            for (int i = 0; i != FrameCount; i++)
            {
                bm.Save(i + ".png", ImageFormat.Png);
                File.Copy(Environment.CurrentDirectory + @"\" + i + ".png", ImagesFolder + @"\" + i + ".png");
                File.Delete(Environment.CurrentDirectory + @"\" + i + ".png");
                Console.WriteLine("Created Image: " + i + "/" + FrameCount);
                bm.Dispose();
                bm = ComputeBitMap(Equation, Width, Height, YBottom, YTop);

                Main.n += 1.0 / (Fps * 4); // Heres where n changes
            }

            VideoWriter w = new VideoWriter(OutputDirectoryWithName + ".avi", new Accord.Extensions.Size(Width, Height), Fps, true);
            ImageDirectoryReader ir = new ImageDirectoryReader(ImagesFolder, "*.png");
            while (ir.Position < ir.Length)
            {
                IImage i = ir.Read();
                w.Write(i);
            }

            w.Close();
        }


    }
}
