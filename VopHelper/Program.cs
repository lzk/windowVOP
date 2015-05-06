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
        /// <param name="angle" > value: "0", "90", "180", "270" </param>
        private static void SaveRotatedImage( string pathSrc, string pathDst, string angle )
        {
            int nAngle = 0;
            if ( int.TryParse( angle, out nAngle) && nAngle%90 == 0 )
            {
                System.Drawing.Image i = System.Drawing.Image.FromFile( pathSrc );

                switch ( nAngle%360 )
                {
                    case 90:
                        i.RotateFlip( System.Drawing.RotateFlipType.Rotate90FlipNone );
                        break;
                    case 180:
                        i.RotateFlip( System.Drawing.RotateFlipType.Rotate180FlipNone );
                        break;
                    case 270:
                        i.RotateFlip( System.Drawing.RotateFlipType.Rotate180FlipNone );
                        break;
                    default:
                        i.RotateFlip( System.Drawing.RotateFlipType.Rotate270FlipNone );
                        break;
                }

                i.Save( pathDst, System.Drawing.Imaging.ImageFormat.Bmp );
            }
        }
        
        static void Main(string[] args)
        {
            try
            {
                if ( args.Length == 3 )
                {
                    SaveRotatedImage( args[0], args[1], args[2] );
                }

                Environment.ExitCode = 0;
            }
            catch 
            {
                Environment.ExitCode = 1;
            }
        }
    }
}
