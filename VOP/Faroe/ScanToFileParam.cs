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
    public class ScanToFileParam : ICloneable
    {

        private string m_fileSaveType = "PDF";
        private string m_fileName = "ScanPictures";
        private string m_filePath = App.PictureFolder;
       
        public string SaveType
        {
            get
            {
                return this.m_fileSaveType;
            }
            set
            {
                this.m_fileSaveType = value;
            }
        }
        public string FileName
        {
            get
            {
                return this.m_fileName;
            }
            set
            {
                this.m_fileName = value;
            }
        }
        public string FilePath
        {
            get
            {
                return this.m_filePath;
            }
            set
            {
                this.m_filePath = value;
            }
        }
        public ScanToFileParam()
        {

        }

        public ScanToFileParam(string type, string name, string path)
        {
            this.m_fileSaveType = type;
            this.FileName = name;
            this.m_filePath = path;

        }

        public object Clone()
        {
            return new ScanToFileParam(this.m_fileSaveType, this.FileName, this.m_filePath);
        }
    }

}
