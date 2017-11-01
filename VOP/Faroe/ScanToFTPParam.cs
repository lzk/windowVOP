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
    public class ScanToFTPParam : ICloneable
    {
        private string m_serverAddress = "ftp://localhost";
        private string m_userName = "";
        private string m_password = "";
        private string m_targetPath = "/files";
                       
            
        public string ServerAddress
        {
            get
            {
                return this.m_serverAddress;
            }
            set
            {
                this.m_serverAddress = value;
            }
        }
        public string UserName
        {
            get
            {
                return this.m_userName;
            }
            set
            {
                this.m_userName = value;
            }
        }
        public string Password
        {
            get
            {
                return this.m_password;
            }
            set
            {
                this.m_password = value;
            }
        }
        public string TargetPath
        {
            get
            {
                return this.m_targetPath;
            }
            set
            {
                this.m_targetPath = value;
            }
        }       

        public ScanToFTPParam()
        {

        }

        public ScanToFTPParam(string address,string name,string password,string path)
        {
            this.m_serverAddress = address;
            this.m_userName = name;
            this.m_password = password;
            this.m_targetPath = path;
           
        }

        public object Clone()
        {
            return new ScanToFTPParam(this.m_serverAddress, this.m_userName, this.m_password, this.m_targetPath);
        }

    }

}
