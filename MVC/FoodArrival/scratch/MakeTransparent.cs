using System;
using System.Drawing;
using System.Drawing.Imaging;

class Program
{
    static void Main()
    {
        string srcPath = @"D:\avi\C#\MVC\FoodArrival\Content\images\registration-hero.png";
        string outPath = @"D:\avi\C#\MVC\FoodArrival\Content\images\registration-hero-transparent.png";

        using (var bmp = new Bitmap(srcPath))
        using (var outBmp = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb))
        {
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    // If near black (the background of the render)
                    // Check if it's the solid dark background
                    if (c.R <= 35 && c.G <= 36 && c.B <= 36)
                    {
                        outBmp.SetPixel(x, y, Color.FromArgb(0, 0, 0, 0));
                    }
                    else
                    {
                        // Slight feathering for edge pixels
                        int maxVal = Math.Max(c.R, Math.Max(c.G, c.B));
                        if (maxVal < 60)
                        {
                            int alpha = (int)(((maxVal - 35) / 25.0) * 255);
                            alpha = Math.Max(0, Math.Min(255, alpha));
                            outBmp.SetPixel(x, y, Color.FromArgb(alpha, c.R, c.G, c.B));
                        }
                        else
                        {
                            outBmp.SetPixel(x, y, c);
                        }
                    }
                }
            }
            outBmp.Save(outPath, ImageFormat.Png);
        }
        Console.WriteLine("Done creating transparent hero image.");
    }
}
