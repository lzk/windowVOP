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
    public class ScanToAPParam : ICloneable
    {        

        private string m_programType = "Paint";

        public string ProgramType
        {
            get
            {
                return this.m_programType;
            }
            set
            {
                this.m_programType = value;
            }
        }        

        public ScanToAPParam()
        {

        }

        public ScanToAPParam( string type)
        {
            this.m_programType = type;
           
        }

        public object Clone()
        {
            return new ScanToAPParam(this.m_programType);
        }

    }

}
