using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing.Imaging;
using System.IO;

namespace PdfEncoderClient
{
    public class PdfHelper : IDisposable
    {
        [System.Runtime.InteropServices.DllImport("OLE32.DLL", EntryPoint = "CreateStreamOnHGlobal")] // Create a COM stream from a pointer in unmanaged memory
        public extern static int CreateStreamOnHGlobal(IntPtr ptr, bool delete, out System.Runtime.InteropServices.ComTypes.IStream pOutStm);
        [System.Runtime.InteropServices.DllImport("OLE32.DLL", EntryPoint = "GetHGlobalFromStream")]
        public extern static void GetHGlobalFromStream(IStream stm, ref IntPtr hGlobal);

        private PdfEncoderLib.IPdfEncoderClass obj = new PdfEncoderLib.PdfEncoderClass();
        bool disposed = false;

        public PdfHelper()
        {
            obj.GdiStartUp();
        }

        ~PdfHelper()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here. 
                //
                obj.GdiShutDown();
                obj = null;
            }

            // Free any unmanaged objects here. 
            //
            disposed = true;
        }

        public void Open(string fileName)
        {
            obj.Open(fileName); 
        }

        public void Close()
        {
            obj.Close();
        }

        public void AddImage(BitmapSource bs, int rotation)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bs));

            using (MemoryStream ms = new MemoryStream())
            {
                switch (rotation)
                {
                    case 0:
                        encoder.Rotation = Rotation.Rotate0;
                        break;
                    case 90:
                        encoder.Rotation = Rotation.Rotate90;
                        break;
                    case 180:
                        encoder.Rotation = Rotation.Rotate180;
                        break;
                    case 270:
                        encoder.Rotation = Rotation.Rotate270;
                        break;
                }

                encoder.Save(ms);
                data = ms.ToArray();
            }

            IStream iis;
            IntPtr hGlobal = IntPtr.Zero;

            if (CreateStreamOnHGlobal((IntPtr)null, false, out iis) == 0)
            {
                GetHGlobalFromStream(iis, ref hGlobal);
                iis.Write(data, data.GetLength(0), IntPtr.Zero);
                obj.AddImage(0, (PdfEncoderLib.IStream)iis);
                iis.Commit(0);
                Marshal.FreeHGlobal(hGlobal);
            }
        }
    }
}
