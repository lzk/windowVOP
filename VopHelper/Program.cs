using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VopHelper
{
    class Program
    {
        /// <summary>
        /// Rotate bitmap file pathSrc and save to pathDst.
        /// </summary>
        private static void SaveRotatedImage( string pathSrc, string pathDst, string angle )
        {
            System.Drawing.Image i = System.Drawing.Image.FromFile( pathSrc );
            i.RotateFlip( System.Drawing.RotateFlipType.Rotate90FlipNone );
            i.Save( pathDst, System.Drawing.Imaging.ImageFormat.Bmp );
        }
        
        static void Main(string[] args)
        {
            SaveRotatedImage( args[0], args[1], args[2] );
        }
    }
}
