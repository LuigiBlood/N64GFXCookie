using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace N64GFXCookie
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        static Color[] palette;
        static byte[] graphics;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static bool LoadCI8File(Stream file)
        {
            if (file.Length < 512)
            {
                MessageBox.Show("File under 512 bytes, invalid CI8 file. Palette expected.");
                return false;
            }

            file.Seek(0, SeekOrigin.Begin);

            byte[] palettetemp = new byte[512];
            file.Read(palettetemp, 0, 512);

            // Palette processing RGBA 5551
            palette = new Color[256];
            ushort colortemp;
            Color colortemp2;
            byte R, G, B, A;
            for (int i = 0; i < 256; i++)
            {
                colortemp = (ushort)((palettetemp[i * 2] << 8) | (palettetemp[i * 2 + 1]));
                B = (byte)((colortemp & 0x3E) << 2);
                G = (byte)((colortemp & 0x7C0) >> 3);
                R = (byte)((colortemp & 0xF800) >> 8);
                A = (byte)(0xFF * ((colortemp) & 1));

                colortemp2 = Color.FromArgb(A, R, G, B);
                palette[i] = colortemp2;
            }

            if (file.Length > 512)
            {
                //If there's more than just the palette, then get the graphics too
                int GFXsize = (int)file.Length - 512;
                graphics = new byte[GFXsize];
                file.Read(graphics, 0, GFXsize);
            }
            file.Close();
            return true;
        }

        public static bool SaveCI8File(Stream file)
        {
            //Color save
            ushort colortemp;
            byte[] palettetemp = new byte[512];
            int R, G, B, A;
            for (int i = 0; i < 256; i++)
            {
                B = palette[i].B >> 2;
                G = palette[i].G << 3;
                R = palette[i].R << 8;
                A = palette[i].A;

                colortemp = (ushort)(R | G | B | (A & 1));
                palettetemp[i * 2] = (byte)(colortemp >> 8);
                palettetemp[i * 2 + 1] = (byte)(colortemp & 0xFF);
            }

            file.Write(palettetemp, 0, 512);
            //GFX save
            file.Write(graphics, 0, graphics.Length);

            file.Close();
            return true;
        }

        public static Bitmap RenderGFX(int width, int height, int zoom)
        {
            if (palette == null || graphics == null)
            {
                return new Bitmap(1, 1);
            }

            Bitmap img = new Bitmap(width * zoom, height * zoom);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if ((x + y * width) >= graphics.Length)
                        return img;

                    for (int i = 0; i < zoom; i++)
                    {
                        for (int j = 0; j < zoom; j++)
                        {
                            img.SetPixel((x * zoom) + j, (y * zoom) + i, palette[graphics[x + y * width]]);
                        }
                    }
                }
            }

            return img;
        }

        public static bool SavePNGRender(Stream file, int width, int height)
        {
            if (palette == null || graphics == null)
            {
                return false;
            }

            Bitmap img = RenderGFX(width, height, 1);
            img.Save(file, ImageFormat.Png);

            file.Close();
            return true;
        }

        public static bool ImportPNG(Stream file)
        {
            if (palette == null)
            {
                return false;
            }

            Bitmap img = new Bitmap(file);

            Color colortemp;
            graphics = new byte[img.Width * img.Height];

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    colortemp = img.GetPixel(x, y);
                    graphics[x + y * img.Width] = (byte)BitmapImport.closestColor2(new List<Color>(palette), colortemp);
                }
            }
            file.Close();
            return true;
        }
    }

    //   THIS CODE IS NOT MINE, modified it slightly.
    //   CANNOT SEEM TO FIND THE ORIGINAL SOURCE OF IT.
    static class BitmapImport
    {
        // closed match for hues only:
        static int closestColor1(List<Color> colors, Color target)
        {
            var hue1 = target.GetHue();
            var diffs = colors.Select(n => getHueDistance(n.GetHue(), hue1));
            var diffMin = diffs.Min(n => n);
            return diffs.ToList().FindIndex(n => n == diffMin);
        }

        // closed match in RGB space
        public static int closestColor2(List<Color> colors, Color target)
        {
            var colorDiffs = colors.Select(n => ColorDiff(n, target)).Min(n => n);
            return colors.FindIndex(n => ColorDiff(n, target) == colorDiffs);
        }

        // color brightness as perceived:
        static float getBrightness(Color c)
        { return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f; }

        // distance between two hues:
        static float getHueDistance(float hue1, float hue2)
        {
            float d = Math.Abs(hue1 - hue2); return d > 180 ? 360 - d : d;
        }

        // distance in RGB space
        static int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B)
                                   + (c1.A - c2.A) * (c1.A - c2.A));
        }
    }
}
