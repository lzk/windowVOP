using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using System.Collections.Generic;
using System.Windows.Interop; // for HwndSource
using System.Threading;
using Microsoft.Win32; // for SaveFileDialog
using PdfEncoderClient;
using VOP.Controls;
using System.ComponentModel;

namespace VOP
{
    [Serializable()]
    public class ScanToCloudParam : ICloneable
    {
        private string m_dropBoxDefaultPath = "";

        public string DefaultPath
        {
            get
            {
                return this.m_dropBoxDefaultPath;
            }
            set
            {
                this.m_dropBoxDefaultPath = value;
            }
        }

        public ScanToCloudParam()
        {

        }

        public ScanToCloudParam(string Path)
        {
            this.m_dropBoxDefaultPath = Path;           
        }

        public object Clone()
        {
            return new ScanToCloudParam(this.m_dropBoxDefaultPath);
        }

    }

}
