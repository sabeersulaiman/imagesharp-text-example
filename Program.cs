using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace DrawText
{
    public class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var dosisResource = assembly.GetManifestResourceStream("DrawText.Fonts.Dosis-Regular.ttf");
            var ralewayResource = assembly.GetManifestResourceStream("DrawText.Fonts.Raleway-Bold.ttf");

            var fontCollection = new FontCollection();
            var maxFontSize = 230f;
            var dosis = new Font(fontCollection.Install(dosisResource), maxFontSize);
            var raleway = new Font(fontCollection.Install(ralewayResource), maxFontSize);

            // draw the first text image
            try
            {
                MakeTextImage("Nov 22", 293, 717, raleway, "raleway.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.InnerException.Message);
                Console.WriteLine(e.InnerException.StackTrace);
                Console.WriteLine(e.InnerException.InnerException.Message);
                Console.WriteLine(e.InnerException.InnerException.StackTrace);
            }

            try
            {
                MakeTextImage("Nov 22", 100, 500, new Font(raleway, 75f), "raleway2.png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine(e.InnerException);
            }
        }

        public static void MakeTextImage(string text, int imageHeight, int imageWidth, Font font, string outFileName)
        {
            var image = new Image<Rgba32>(imageWidth, imageHeight);
            var bounds = TextMeasurer.MeasureBounds(text, new RendererOptions(font, 72, 72));

            while (bounds.Height > imageHeight || bounds.Width > imageWidth)
            {
                var nSize = font.Size - 1;
                if (nSize < 1) throw new InvalidDataException("Image Text Scale became too low");
                font = new Font(font, nSize);
                bounds = TextMeasurer.MeasureBounds(text, new RendererOptions(font, 72, 72));
            }

            // vertical centering
            var posY = (imageHeight - bounds.Height) / 2;
            posY -= bounds.Top;

            // horizontal centering
            var posX = ((imageWidth - bounds.Width) / 2);
            posX -= bounds.Left;

            var point = new PointF(posX, posY);

            image.Mutate(
                x => x.Fill(NamedColors<Rgba32>.White)
                        .DrawText(
                            text,
                            font,
                            NamedColors<Rgba32>.BurlyWood,
                            point
                        )
            );

            image.Save(outFileName);
        }
    }
}
