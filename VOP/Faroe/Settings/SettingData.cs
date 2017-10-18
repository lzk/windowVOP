using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace VOP
{
    [Serializable()]
    public class MatchListPair
    {
        public int Key { get; set; }
        public int Value { get; set; }
        public string ItemName { get; set; }

        public ScanParam m_ScanSettings = new ScanParam();
        public ScanToPrintParam m_PrintScanSettings = new ScanToPrintParam();
        public ScanToFileParam m_FileScanSettings = new ScanToFileParam();
        public ScanToAPParam m_APScanSettings = new ScanToAPParam();        
        public ScanToEmailParam m_EmailScanSettings = new ScanToEmailParam();
        public ScanToFTPParam m_FTPScanSettings = new ScanToFTPParam();
        public ScanToCloudParam m_CloudScanSettings = new ScanToCloudParam();

        public MatchListPair()
        {

        }

        public MatchListPair(int key, int value)
        {
            Key = key;
            Value = value;

        }
        public MatchListPair(int key, int value, string name)
        {
            Key = key;
            Value = value;
            ItemName = name;
            //switch (value)
            //{
            //    case 0:
            //        m_PrintScanSettings = 
            //        break;
            //    case 1:
                    
            //        break;
            //    case 2:

            //        break;
            //    case 3:

            //        break;
            //    case 4:

            //        break;
            //    case 5:

            //        break;
            //    default:
                    
            //        break;
            //}
        }        

    }   
    [Serializable()]
    public class SettingData
    {
        //Common
        public static int MaxShortCutNum = 6;
        public static string[] ScanToItems =
        {
            "Scan To Print",
            "Scan To File",
            "Scan To Application",
            "Scan To Email",
            "Scan To FTP",
            "Scan To Cloud",
        };
        public ScanParam m_commonScanSettings = new ScanParam();
        public List<MatchListPair> m_MatchList = new List<MatchListPair>();
        public bool m_QRcode_decode_hardest = false;
        public string m_QRcode_decode_hardest_level = "Standard";

        //Cloud
        public ScanParam m_cloudScanSettings = new ScanParam();
        public string m_dropBoxDefaultPath = "";

        //Ftp
        public ScanParam m_ftpScanSettings = new ScanParam();
        public string m_serverAddress = "ftp://localhost";
        public string m_userName = "vop";
        public string m_password = "";
        public string m_targetPath = "/files";

        public ScanParam m_quickScanFtpScanSettings = new ScanParam();
        public string m_quickScanServerAddress = "ftp://localhost";
        public string m_quickScanUserName = "vop";
        public string m_quickScanPassword = "";
        public string m_quickScanTargetPath = "/files";

        //Print
        public ScanParam m_printScanSettings = new ScanParam();
        public string m_printerName = "";

        //Email
        public ScanParam m_emailScanSettings = new ScanParam();
        public string m_attachmentType = "PDF";
        public string m_recipient = "Ally.qiu@liteon.com";
        public string m_subject = "Scan Pictures";

        //File
        public ScanParam m_fileScanSettings = new ScanParam();
        public string m_fileSaveType = "PDF";
        public string m_fileName = "ScanPictures";
        public string m_filePath = App.PictureFolder;

        //Application
        public ScanParam m_apScanSettings = new ScanParam();
        public string m_programType = "Paint";


        public string m_DeviceName = "";
        public bool m_isUsbConnect = false;
       

        public SettingData()
        {
         
        }

        public void InitSettingData()
        {
            m_MatchList.Clear();

            for (int i = 0; i < MaxShortCutNum; i++)
            {
                m_MatchList.Add(new MatchListPair(i, i, ScanToItems[i]));
            }
        }

        public static bool Serialize(SettingData value, String filename)
        {
            if (value == null)
            {
                return false;
            }
            try
            {
                XmlSerializer _xmlserializer = new XmlSerializer(typeof(SettingData));
                TextWriter stream = new StreamWriter(filename, false);
                _xmlserializer.Serialize(stream, value);
                stream.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static SettingData Deserialize(String filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                SettingData obj = new SettingData();
                obj.InitSettingData();
            }
            try
            {
                XmlSerializer _xmlSerializer = new XmlSerializer(typeof(SettingData));
                TextReader stream = new StreamReader(filename);
                var result = (SettingData)_xmlSerializer.Deserialize(stream);
                stream.Close();
                return result;
            }
            catch (Exception)
            {
                SettingData obj = new SettingData();
                obj.InitSettingData();
                return obj;
            }
        }
    }
}
