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
    //[Serializable()]
    public class ScanParam
    {
        private EnumScanDocType m_docutype = EnumScanDocType.Photo;
        private EnumScanResln m_scanResln = EnumScanResln._300x300;
        private EnumPaperSizeScan m_paperSize = EnumPaperSizeScan._A4;
        private EnumColorType m_color = EnumColorType.grayscale_8bit;
        private int m_brightness = 50;
        private int m_contrast = 50;
        private bool m_ADFMode = false;


        public EnumScanResln ScanResolution
        {
            get
            {
                return this.m_scanResln;
            }
            set
            {
                this.m_scanResln = value;
            }
        }

        public EnumPaperSizeScan PaperSize
        {
            get
            {
                return this.m_paperSize;
            }
            set
            {
                this.m_paperSize = value;
            }
        }

        public EnumColorType ColorType
        {
            get
            {
                return this.m_color;
            }
            set
            {
                this.m_color = value;
            }
        }

        public int Brightness
        {
            get
            {
                return this.m_brightness;
            }
            set
            {
                this.m_brightness = value;
            }
        }

        public int Contrast
        {
            get
            {
                return this.m_contrast;
            }
            set
            {
                this.m_contrast = value;
            }
        }

        public bool ADFMode
        {
            get
            {
                return this.m_ADFMode;
            }
            set
            {
                this.m_ADFMode = value;
            }
        }

        public ScanParam()
        {

        }

        public ScanParam(EnumScanResln res, EnumPaperSizeScan paperSize, EnumColorType color, bool ADFMode, int b, int c)
        {
            this.m_scanResln = res;
            this.m_paperSize = paperSize;
            this.m_color = color;
            this.m_ADFMode = ADFMode;
            this.m_brightness = b;
            this.m_contrast = c;
        }
    }

}
