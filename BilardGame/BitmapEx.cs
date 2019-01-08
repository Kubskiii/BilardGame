using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BilardGame
{
    static class BitmapEx
    {
        public static void SetPixel(this WriteableBitmap bmp, int x, int y, Color c)
        {
            unsafe
            {
                IntPtr buffer = bmp.BackBuffer;
                buffer += y * bmp.BackBufferStride + x * 4;
                *((UInt32*)buffer) = ConvertColor(c);
            }
        }
        public static UInt32 ConvertColor(Color c)
        {
            return (UInt32)((255 << 24) + (c.R << 16) + (c.G << 8) + (c.B << 0));
        }
        static public void FlushBuffer(this WriteableBitmap bmp)
        {
            bmp.Lock();
            bmp.AddDirtyRect(new System.Windows.Int32Rect(0, 0, bmp.PixelWidth, bmp.PixelHeight));
            bmp.Unlock();
        }

        static public void FillBitmap(this WriteableBitmap bmp, UInt32[,] colors)
        {
            unsafe
            {
                IntPtr buffer = bmp.BackBuffer;
                for(int y = 0; y < bmp.PixelHeight; y++)
                {
                    for(int x = 0; x < bmp.PixelWidth; x++)
                    {
                        *(UInt32*)buffer = colors[x, y];
                        buffer += 4;
                    }
                }
            }
            bmp.FlushBuffer();
        }
    }
}
