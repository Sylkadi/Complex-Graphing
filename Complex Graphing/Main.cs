using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Accord.Extensions.Imaging;

using C = Complex_Graphing.ComplexComputation.Complex;



namespace Complex_Graphing
{
    public partial class Main : Form
    {

        private Bitmap bitMap;

        public static double n = -5, // The changing value
                             YBottom = -3.0, //  bottom of graph
                             YTop = 3.0;   // top of graph (left and right are scaled proportionally to fit on the screen properly)
        private C equation(C Input) // Equation used
        {
            C Output = (Input ^ n) + 1; // This is the equation
            return Output;
        }

        public Main() // Start
        {
            InitializeComponent();
            // Setup

            int Width = ClientSize.Width, Height = ClientSize.Height;

            // What i used to create the video, but this does have problems on its own that you may encounter
            //BitmapComputation.CreateAvi(Environment.CurrentDirectory + @"\animation", 400, 400, 2400, 60, -3.0, 3.0, equation);
            bitMap = BitmapComputation.ComputeBitMap(equation, Width, Height, YBottom, YTop);

        }

        private void Graphics(object sender, PaintEventArgs e) // Draw
        {
            e.Graphics.DrawImage(bitMap, 0, 0); // The image

            // Real Axis and Imaginary Axis
            e.Graphics.DrawLine(Pens.Black, new Point(ClientSize.Width / 2, 0), new Point(ClientSize.Width / 2, ClientSize.Height));
            e.Graphics.DrawLine(Pens.Black, new Point(0, ClientSize.Height / 2), new Point(ClientSize.Width, ClientSize.Height / 2));

        }

        private void Update(object sender, EventArgs e) // Updater
        {

            Invalidate();
        }

        private void Resized(object sender, EventArgs e) // When user resizes the application, make the bitmap fit
        {
            bitMap = BitmapComputation.ComputeBitMap(equation, ClientSize.Width, ClientSize.Height, YBottom, YTop);
        }
    }
}
