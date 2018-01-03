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

        private string m_dropBoxDefaultPath = "/";

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

        private string m_onedriveDefaultPath = "/";

        public string DefaultOneDrivePath
        {
            get
            {
                return this.m_onedriveDefaultPath;
            }
            set
            {
                this.m_onedriveDefaultPath = value;
            }
        }

        private string m_evernotetile = "";
        private string m_evernotecontent = "";
        private bool m_bNeedReset = false;

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

        public bool NeedReset
        {
            get
            {
                return this.m_bNeedReset;
            }
            set
            {
                this.m_bNeedReset = value;
            }
        }

        private string m_googledriveDefaultPath = "/";
        private string m_folderID = "";
        public string DefaultGoogleDrivePath
        {
            get
            {
                return this.m_googledriveDefaultPath;
            }
            set
            {
                this.m_googledriveDefaultPath = value;
            }
        }

        public string GoogleDriveFolderID
        {
            get
            {
                return this.m_folderID;
            }
            set
            {
                this.m_folderID = value;
            }
        }

        public ScanToCloudParam()
        {

        }

        public ScanToCloudParam(string saveType, string Path, string onedriverpath, string title, string content, bool needReset)
        {
            this.m_cloudSaveType = saveType;
            this.m_dropBoxDefaultPath = Path;
            this.m_onedriveDefaultPath = onedriverpath;
            this.m_evernotetile = title;
            this.m_evernotecontent = content;
            this.m_bNeedReset = needReset;    
        }

        public object Clone()
        {
            return new ScanToCloudParam(this.m_cloudSaveType, this.m_dropBoxDefaultPath, this.m_onedriveDefaultPath,
                this.m_evernotetile, this.m_evernotecontent, this.m_bNeedReset);
        }

    }

}
