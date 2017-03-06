﻿using System.Windows.Controls;
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
        private EnumScanResln m_scanResln = EnumScanResln._300x300;
        private EnumPaperSizeScan m_paperSize = EnumPaperSizeScan._Auto;
        private EnumColorType m_color = EnumColorType.color_24bit;
        private int m_brightness = 50;
        private int m_contrast = 50;
        private bool m_ADFMode = true;
        private bool m_MultiFeed = true;
        private bool m_AutoCrop = true;
        private bool m_onepage = false;

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

        public ScanParam()
        {

        }

        public ScanParam(EnumScanResln res, EnumPaperSizeScan paperSize, EnumColorType color, bool ADFMode, bool MultiFeed, bool AutoCrop, int b, int c, bool onepage = false)
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
        }

        public object Clone()
        {
            return new ScanParam(this.m_scanResln, this.m_paperSize,
                                 this.m_color, this.m_ADFMode, this.m_MultiFeed, 
                                 this.m_AutoCrop, this.m_brightness, this.m_contrast, this.m_onepage);
        }

    }

}
