using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;

namespace TestApp.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class SinusoidalController : ControllerBase
    {
        #region Utilities
        private double[] GenerateSinusoid(double A, double Fd, double Fs, int N)
        {
            int numSamples = (int)(Fs * N);
            double[] signal = new double[numSamples];
            double dt = 1.0 / Fs;

            for (int i = 0; i < numSamples; i++)
            {
                double t = i * dt;
                signal[i] = A * Math.Sin(2 * Math.PI * Fd * t);
            }

            return signal;
        }

        private Bitmap CreateImage(double[] signal)
        {
            int width = 800;
            int height = 400;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);

            float scaleX = (float)width / signal.Length;
            float scaleY = (float)height / 2;

            PointF[] points = new PointF[signal.Length];
            for (int i = 0; i < signal.Length; i++)
            {
                points[i] = new PointF(i * scaleX, (float)(height / 2 - signal[i] * scaleY));
            }

            Pen pen = new Pen(Color.Blue);
            g.DrawLines(pen, points);

            return image;
        }

        #endregion

        #region Methods

        [HttpPost]
        public IActionResult Post([FromBody] SinusoidParameters parameters)
        {
            if (parameters == null)
            {
                return BadRequest();
            }

            var signal = GenerateSinusoid(parameters.A, parameters.Fd, parameters.Fs, parameters.N);

            var image = CreateImage(signal);

            var stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);

            stream.Seek(0, SeekOrigin.Begin);
            return File(stream, "image/png");
        }


        #endregion

        #region Nested classes
        public class SinusoidParameters
        {
            public double A { get; set; }
            public double Fd { get; set; }
            public double Fs { get; set; }
            public int N { get; set; }
        }
        #endregion
    }
}