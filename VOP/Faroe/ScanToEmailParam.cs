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
    public class ScanToEmailParam : ICloneable
    {
        private string m_attachmentType = "PDF";
        private string m_recipient = "";
        private string m_subject = "Scan Pictures";

        public string AttachmentType
        {
            get
            {
                return this.m_attachmentType;
            }
            set
            {
                this.m_attachmentType = value;
            }
        }
        public string Recipient
        {
            get
            {
                return this.m_recipient;
            }
            set
            {
                this.m_recipient = value;
            }
        }
        public string Subject
        {
            get
            {
                return this.m_subject;
            }
            set
            {
                this.m_subject = value;
            }
        }
        public ScanToEmailParam()
        {

        }

        public ScanToEmailParam(string attachmentType,string recipient,string subject)
        {
            this.m_attachmentType = attachmentType;
            this.m_recipient = recipient;
            this.m_subject = subject;
           
        }

        public object Clone()
        {
            return new ScanToEmailParam(this.m_attachmentType, this.m_recipient, this.m_subject);
        }

    }

}
