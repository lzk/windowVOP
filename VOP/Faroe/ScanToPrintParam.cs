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
    public class ScanToPrintParam : ICloneable
    {
        private string m_printerName = "";

        public string PrinterName
        {
            get
            {
                return this.m_printerName;
            }
            set
            {
                this.m_printerName = value;
            }
        }

        public ScanToPrintParam()
        {

        }

        public ScanToPrintParam(string name)
        {
            this.m_printerName = name;            
        }

        public object Clone()
        {
            return new ScanToPrintParam(this.m_printerName);
        }

    }

}
