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
    public class ScanParam : ICloneable
    {
        private EnumScanResln m_scanResln = EnumScanResln._200x200;
        private EnumPaperSizeScan m_paperSize = EnumPaperSizeScan._Auto;
        private EnumColorType m_color = EnumColorType.color_24bit;
        private EnumScanMediaType m_type = EnumScanMediaType._Normal;
        private int m_brightness = 50;
        private int m_contrast = 50;
        private bool m_ADFMode = true;
        private bool m_MultiFeed = true;
        private bool m_AutoCrop = true;
        private bool m_onepage = false;
        private bool m_autocolordetect = false;
        private bool m_skipblankpage = false;
        private double m_gamma = 1.8;

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

        public EnumScanMediaType ScanMediaType
        {
            get
            {
                return this.m_type;
            }
            set
            {
                this.m_type = value;
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

        public bool MultiFeed
        {
            get
            {
                return this.m_MultiFeed;
            }
            set
            {
                this.m_MultiFeed = value;
            }
        }

        public bool AutoCrop
        {
            get
            {
                return this.m_AutoCrop;
            }
            set
            {
                this.m_AutoCrop = value;
            }
        }

        public bool OnePage
        {
            get
            {
                return this.m_onepage;
            }
            set
            {
                this.m_onepage = value;
            }
        }

        public bool AutoColorDetect
        {
            get
            {
                return this.m_autocolordetect;
            }
            set
            {
                this.m_autocolordetect = value;
            }
        }

        public bool SkipBlankPage
        {
            get
            {
                return this.m_skipblankpage;
            }
            set
            {
                this.m_skipblankpage = value;
            }
        }

        public double Gamma
        {
            get
            {
                return this.m_gamma;
            }
            set
            {
                this.m_gamma = value;
            }
        }

        public ScanParam()
        {

        }

        public ScanParam(EnumScanResln res, EnumScanMediaType type, EnumPaperSizeScan paperSize, EnumColorType color, bool ADFMode, bool MultiFeed, 
            bool AutoCrop, int b, int c, bool autocolor, bool skipblank, double gamma, bool onepage = false)
        {
            this.m_scanResln = res;
            this.m_paperSize = paperSize;
            this.m_color = color;
            this.m_ADFMode = ADFMode;
            this.m_MultiFeed = MultiFeed;
            this.m_AutoCrop = AutoCrop;
            this.m_brightness = b;
            this.m_contrast = c;
            this.m_onepage = onepage;
            this.m_type = type;
            this.m_gamma = gamma;
            this.m_autocolordetect = autocolor;
            this.m_skipblankpage = skipblank;
        }

        public object Clone()
        {
            return new ScanParam(this.m_scanResln, this.m_type, this.m_paperSize,
                                 this.m_color, this.m_ADFMode, this.m_MultiFeed, 
                                 this.m_AutoCrop, this.m_brightness, this.m_contrast,
                                 this.m_autocolordetect, this.m_skipblankpage, this.m_gamma,
                                 this.m_onepage
                                 );
        }

    }

    [Serializable()]
    public class ScanParamShort : ICloneable
    {
        private EnumScanResln m_scanResln = EnumScanResln._200x200;
        private EnumPaperSizeScan m_paperSize = EnumPaperSizeScan._Auto;
        private EnumColorType m_color = EnumColorType.color_24bit;
        private EnumFileFormat m_fileFormat = EnumFileFormat.JPEG;
        private bool m_ADFMode = true;

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

        public EnumFileFormat FileFormat
        {
            get
            {
                return this.m_fileFormat;
            }
            set
            {
                this.m_fileFormat = value;
            }
        }

        public ScanParamShort()
        {

        }

        public ScanParamShort(EnumScanResln res, EnumPaperSizeScan paperSize, EnumColorType color, EnumFileFormat format, bool ADFMode)
        {
            this.m_scanResln = res;
            this.m_paperSize = paperSize;
            this.m_color = color;
            this.m_ADFMode = ADFMode;
            this.m_fileFormat = format;
        }

        public object Clone()
        {
            return new ScanParamShort(this.m_scanResln,  this.m_paperSize,
                                 this.m_color, this.m_fileFormat, this.m_ADFMode);
        }

    }
    [Serializable()]
    public class ScanParamDecode : ICloneable
    {
        private EnumPaperSizeScan m_paperSize = EnumPaperSizeScan._Auto;
        private EnumScanMediaType m_mediaType = EnumScanMediaType._Normal;
        private bool m_MultiFeed = true;

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

        public EnumScanMediaType ScanMediaType
        {
            get
            {
                return this.m_mediaType;
            }
            set
            {
                this.m_mediaType = value;
            }
        }


        public bool MultiFeed
        {
            get
            {
                return this.m_MultiFeed;
            }
            set
            {
                this.m_MultiFeed = value;
            }
        }

  

        public ScanParamDecode()
        {

        }

        public ScanParamDecode(EnumPaperSizeScan paperSize, EnumScanMediaType type, bool multifeed)
        {
            this.m_paperSize = paperSize;
            this.m_mediaType = type;
            this.m_MultiFeed = multifeed;
        }

        public object Clone()
        {
            return new ScanParamDecode(this.m_paperSize,
                                 this.m_mediaType, this.m_MultiFeed);
        }

    }

}
