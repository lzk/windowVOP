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
        private string m_cloudSaveType = "DropBox";

        public string SaveType
        {
            get
            {
                return this.m_cloudSaveType;
            }
            set
            {
                this.m_cloudSaveType = value;
            }
        }

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

        private string m_evernotetile = "";
        private string m_evernotecontent = "";

        public string EverNoteTitle
        {
            get
            {
                return this.m_evernotetile;
            }
            set
            {
                this.m_evernotetile = value;
            }
        }

        public string EverNoteContent
        {
            get
            {
                return this.m_evernotecontent;
            }
            set
            {
                this.m_evernotecontent = value;
            }
        }
        public ScanToCloudParam()
        {

        }

        public ScanToCloudParam(string saveType, string Path, string title, string content)
        {
            this.m_cloudSaveType = saveType;
            this.m_dropBoxDefaultPath = Path;
            this.m_evernotetile = title;
            this.m_evernotecontent = content;      
        }

        public object Clone()
        {
            return new ScanToCloudParam(this.m_cloudSaveType, this.m_dropBoxDefaultPath,
                this.m_evernotetile, this.m_evernotecontent);
        }

    }

}
