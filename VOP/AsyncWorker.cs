using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System.Net;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VOP.Controls;

namespace VOP
{
    public delegate int DoWorkDelegate();
    public delegate int ScanDelegate(string printerName, string szOrig, string szView, string szThumb,
                                        int scanMode, int resolution, int width, int height,
                                        int contrast, int brightness, int docuType, uint uMsg );
    public delegate int PrintFileDelegate(string printerName, string fileName, bool needFitToPage, int duplexType, bool IsPortrait, int copies, int scalingValue);
    public delegate bool RotateScannedFilesDelegate( ScanFiles objSrc, ScanFiles objDst, int nAngle );
    public delegate int CopyDelegate(string printerName,
                                    byte Density,
                                    byte copyNum,
                                    byte scanMode,
                                    byte orgSize,
                                    byte paperSize,
                                    byte nUp,
                                    byte dpi,
                                    ushort scale,
                                    byte mediaType);

   
    
    class AsyncWorker
    {
        private Window owner = null;
        private ProgressBarWindow pbw = null;
//        private ScanProgressBarWindow scanPbw = null;
        private ProgressBarWindow scanPbw = null;
        private ManualResetEvent asyncEvent = new ManualResetEvent(false);
        private bool isNeededProgress = false;

        public AsyncWorker()
        {
        }

        public AsyncWorker(Window owner)
        {
            this.owner = owner;
        }

        private void pbw_Loaded(object sender, RoutedEventArgs e)
        {
            asyncEvent.Set();
        }

        void CallbackMethod(IAsyncResult ar)
        {
            dll.OutputDebugStringToFile_("CopyPage CallbackMethod()\r\n");
            if (ar != null)
                ar.AsyncWaitHandle.WaitOne();

            dll.OutputDebugStringToFile_("CopyPage AsyncWaitHandle.WaitOne() end\r\n");


            if (isNeededProgress)
            {
                asyncEvent.WaitOne();

                if (pbw != null)
                {
                    pbw.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                    new Action(
                    delegate()
                    {
                        dll.OutputDebugStringToFile_("CopyPage  pbw.Close()\r\n");

                        pbw.Close();

                    }
                    ));
                }
            } 
           
        }

        void ScanCallbackMethod(IAsyncResult ar)
        {
            if (scanPbw != null)
            {
                scanPbw.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal,
                 new Action(
                 delegate()
                 {
                     scanPbw.Close();
                 }
                 ));
            }

        }

        public int InvokeScanMethod(ScanDelegate method, string printerName, string szOrig, string szView, string szThumb,
                                                                      int scanMode, int resolution, int width, int height,
                                                                      int contrast, int brightness, int docuType, uint uMsg )
        {

            if (method != null)
            {
                ScanDelegate caller = method;


                IAsyncResult result = caller.BeginInvoke(printerName, szOrig, szView, szThumb, scanMode, resolution, width, height,contrast, brightness, docuType, uMsg,
                                                         new AsyncCallback(ScanCallbackMethod), null);

                if (!result.AsyncWaitHandle.WaitOne(100, false))
                {
//                    scanPbw = new ScanProgressBarWindow();
                    scanPbw.Owner = this.owner;
                    scanPbw.ShowDialog();
                }
                            
                if (result.AsyncWaitHandle.WaitOne(100, false))
                {
                    return caller.EndInvoke(result);
                }

            }

            return 1;
        }


        public bool InvokeRotateScannedFiles(RotateScannedFilesDelegate method, ScanFiles objSrc, ScanFiles objDst, int nAngle )
        {
            bool bSuccess = false;

            if (method != null)
            {
                RotateScannedFilesDelegate caller = method;

                IAsyncResult result = caller.BeginInvoke( objSrc, objDst, nAngle, new AsyncCallback(CallbackMethod), null);

                if (!result.AsyncWaitHandle.WaitOne(100, false))
                {
                    pbw = new ProgressBarWindow();
                    pbw.Owner = this.owner;
                    pbw.ShowDialog();
                }

                if ( result.AsyncWaitHandle.WaitOne(100, false) )
                    bSuccess = caller.EndInvoke( result );
            }

            return bSuccess;
        }

        public PrintError InvokePrintFileMethod(PrintFileDelegate method, string printerName, string fileName, bool needFitToPage, int duplexType, bool IsPortrait, int copies, int scalingValue)
        {

            if (method != null)
            {
                PrintFileDelegate caller = method;

                isNeededProgress = false;
                asyncEvent.Reset();

                IAsyncResult result = caller.BeginInvoke(printerName, fileName, needFitToPage, duplexType, IsPortrait, copies, scalingValue, new AsyncCallback(CallbackMethod), null);

                if (!result.AsyncWaitHandle.WaitOne(100, false))
                {
                    isNeededProgress = true;

                    pbw = new ProgressBarWindow();
                    pbw.Owner = this.owner;
                    pbw.Loaded += pbw_Loaded;
                    pbw.ShowDialog();
                }

                if (result.AsyncWaitHandle.WaitOne(100, false))
                {
                    return (PrintError)caller.EndInvoke(result);
                }

            }

            return PrintError.Print_File_Not_Support;
        }



        public int InvokeCopyMethod(CopyDelegate method,
                                       string printerName,
                                       byte Density,
                                       byte copyNum,
                                       byte scanMode,
                                       byte orgSize,
                                       byte paperSize,
                                       byte nUp,
                                       byte dpi,
                                       ushort scale,
                                       byte mediaType)
        {

            if (method != null)
            {
                CopyDelegate caller = method;

                isNeededProgress = false;
                asyncEvent.Reset();

                IAsyncResult result = caller.BeginInvoke(printerName, Density, copyNum, scanMode, orgSize, paperSize, nUp, dpi, scale, mediaType, new AsyncCallback(CallbackMethod), null);

                if (!result.AsyncWaitHandle.WaitOne(100, false))
                {
                    isNeededProgress = true;

                    dll.OutputDebugStringToFile_("CopyPage new ProgressBarWindow()\r\n");
                    pbw = new ProgressBarWindow();
                    pbw.Owner = this.owner;
                    pbw.Loaded += pbw_Loaded;
                   
                    pbw.ShowDialog();
                    dll.OutputDebugStringToFile_("CopyPage end ShowDialog()\r\n");

                }

                if (result.AsyncWaitHandle.WaitOne(100, false))
                {
                    dll.OutputDebugStringToFile_("CopyPage caller.EndInvoke(result)\r\n");
                    return caller.EndInvoke(result);
                }

            }

            return 1;
        }

        public int InvokeDoWorkMethod(DoWorkDelegate method)
        {

            if (method != null)
            {
                DoWorkDelegate caller = method;

                isNeededProgress = false;
                asyncEvent.Reset();

                IAsyncResult result = caller.BeginInvoke(new AsyncCallback(CallbackMethod), null);

                if (!result.AsyncWaitHandle.WaitOne(100, false))
                {
                    isNeededProgress = true;

                    pbw = new ProgressBarWindow();
                    pbw.Owner = this.owner;
                    pbw.Loaded += pbw_Loaded;
                    pbw.ShowDialog();
                }

                if (result.AsyncWaitHandle.WaitOne(100, true))
                {
                    return caller.EndInvoke(result);
                }

            }

            return 1;
        }

        public bool InvokeMethod<T>(string printerName, ref T rec,  DllMethodType methodType, object frameworkElement) where T : class
        {
            T record = rec;

            switch (methodType)
            {
                case DllMethodType.SetIpInfo:
                case DllMethodType.SetPowerSaveTime:
                case DllMethodType.SetSoftAp:
                case DllMethodType.SetWiFiInfo:
                case DllMethodType.SetUserConfig:
                case DllMethodType.SetPassword:
                case DllMethodType.SetFusingResetCmd:
                    string strDrvName = "";
                    if (false == common.GetPrinterDrvName(printerName, ref strDrvName))
                    {
                        FrameworkElement _this = frameworkElement as FrameworkElement;
                        if(null != _this)
                        {
                            MessageBoxEx_Simple messageBox =
                                new MessageBoxEx_Simple((string)_this.TryFindResource("ResStr_can_not_be_carried_out_due_to_software_has_error__please_try__again_after_reinstall_the_Driver_and_Virtual_Operation_Panel_"), (string)_this.FindResource("ResStr_Error"));
                            messageBox.Owner = App.Current.MainWindow;
                            messageBox.ShowDialog();
                        }
                        return false;
                    }

                    if (!((MainWindow)App.Current.MainWindow).PasswordCorrect())
                    {
                        PasswordWindow pw = new PasswordWindow();
                        pw.Owner = App.Current.MainWindow;
                        Nullable<bool> dialogResult = pw.ShowDialog();

                        if (dialogResult != true)
                        {
                            return false;
                        }
                    }
                    break;
            }

            Thread thread = new Thread(() =>
            {
                switch (methodType)
                {
                    case DllMethodType.GetIpInfo:
                           record = (T)(dynamic)GetIpInfo(printerName);
                           break;
                    case DllMethodType.SetIpInfo:
                           record = (T)(dynamic)SetIpInfo((IpInfoRecord)(dynamic)record);
                           break;
                    case DllMethodType.GetPowerSaveTime:
                           record = (T)(dynamic)GetPowerSaveTime(printerName);
                           break;
                    case DllMethodType.SetPowerSaveTime:
                           record = (T)(dynamic)SetPowerSaveTime((PowerSaveTimeRecord)(dynamic)record);
                           break;
                    case DllMethodType.GetSoftAp:
                           record = (T)(dynamic)GetSoftAp(printerName);
                           break;
                    case DllMethodType.SetSoftAp:
                           record = (T)(dynamic)SetSoftAp((SoftApRecord)(dynamic)record);
                           break;
                    case DllMethodType.GetApList:
                           record = (T)(dynamic)GetApList(printerName);
                           break;
                    case DllMethodType.GetWiFiInfo:
                           record = (T)(dynamic)GetWiFiInfo(printerName);
                           break;
                    case DllMethodType.SetWiFiInfo:
                           record = (T)(dynamic)SetWiFiInfo((WiFiInfoRecord)(dynamic)record);
                           break;
                    case DllMethodType.GetUserConfig:
                           record = (T)(dynamic)GetUserCfg(printerName);
                           break;
                    case DllMethodType.SetUserConfig:
                           record = (T)(dynamic)SetUserCfg((UserCfgRecord)(dynamic)record);
                           break;
                    case DllMethodType.SetFusingResetCmd:
                           record = (T)(dynamic)SetFusingReset((FusingResetRecord)(dynamic)record);
                           break;
                    case DllMethodType.SetPassword:
                           record = (T)(dynamic)SetPassword((PasswordRecord)(dynamic)record);
                           break;
                    case DllMethodType.ConfirmPassword:
                           record = (T)(dynamic)ConfirmPassword((PasswordRecord)(dynamic)record);
                           break;
                    default: break;
                }

                CallbackMethod(null);
            });

            thread.Start();

            if (!thread.Join(100))
            {
#if (DEBUG)
                pbw = new ProgressBarWindow(6);
#else
                pbw = new ProgressBarWindow(30);
#endif
                pbw.Owner = this.owner;
                pbw.ShowDialog();
            }

            if (thread.Join(100))
            {
                rec = record;
                return true;
            }
            else
            {
                return false;
            }
        }

        public UserCfgRecord GetUserCfg(string printerName)
        {
            UserCfgRecord rec = new UserCfgRecord();

            sbyte leadingEdge = 1;
            sbyte sideToSide = 1;
            sbyte imageDensity = 0;
            sbyte lowHumidityMode = 0;
            sbyte platecontrolmode = 2;
            sbyte primarycoolingmode = 0;

            int result = dll.GetUserCfg(printerName, ref leadingEdge, ref sideToSide, ref imageDensity, ref lowHumidityMode, ref platecontrolmode, ref primarycoolingmode);

            rec.PrinterName = printerName;

            rec.LeadingEdge = leadingEdge;
            rec.SideToSide = sideToSide;
            rec.ImageDensity = imageDensity;
            rec.LowHumidityMode = lowHumidityMode;
            rec.PlateControlMode = platecontrolmode;
            rec.PrimaryCoolingMode = primarycoolingmode;

            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

        public UserCfgRecord SetUserCfg(UserCfgRecord rec)
        {
            string printerName = "";
            sbyte leadingEdge = 1;
            sbyte sideToSide = 1;
            sbyte imageDensity = 0;
            sbyte lowHumidityMode = 0;
            sbyte platecontrolmode = 2;
            sbyte primarycoolingmode = 0;

            if (rec != null)
            {
                printerName = rec.PrinterName;

                leadingEdge = rec.LeadingEdge;
                sideToSide = rec.SideToSide;
                imageDensity = rec.ImageDensity;
                lowHumidityMode = rec.LowHumidityMode;
                platecontrolmode = rec.PlateControlMode;
                primarycoolingmode = rec.PrimaryCoolingMode;

                int result = dll.SetUserCfg(printerName, leadingEdge, sideToSide, imageDensity, lowHumidityMode, platecontrolmode, primarycoolingmode);

                rec.CmdResult = (EnumCmdResult)result;

            }

            return rec;
        }

        public FusingResetRecord SetFusingReset(FusingResetRecord rec)
        {
            string printerName = "";

            if (rec != null)
            {
                printerName = rec.PrinterName;

                int result = dll.SetFusingSCReset(printerName);

                rec.CmdResult = (EnumCmdResult)result;

            }

            return rec;
        }

        public PasswordRecord SetPassword(PasswordRecord rec)
        {
            string printerName = "";
            string pwd = "";

            if (rec != null)
            {
                printerName = rec.PrinterName;
                pwd = rec.PWD;
                int result = dll.SetPassword(printerName, pwd);

                rec.CmdResult = (EnumCmdResult)result;

            }

            return rec;
        }

        public PasswordRecord ConfirmPassword(PasswordRecord rec)
        {
            string printerName = "";
            string pwd = "";

            if (rec != null)
            {
                printerName = rec.PrinterName;
                pwd = rec.PWD;
                int result = dll.ConfirmPassword(printerName, pwd);

                rec.CmdResult = (EnumCmdResult)result;

            }

            return rec;
        }

        public WiFiInfoRecord GetWiFiInfo(string printerName)
        {
            WiFiInfoRecord rec = new WiFiInfoRecord();
       
            StringBuilder ssid = new StringBuilder(128);
            StringBuilder pwd = new StringBuilder(128);
            byte encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
            byte wepKeyId = 1;
            byte wifiEnabel = 0;

            int result = dll.GetWiFiInfo(printerName, ref wifiEnabel, ssid, pwd, ref encryption, ref wepKeyId);

            rec.PrinterName = printerName;
            rec.WifiEnable = wifiEnabel;
            rec.SSID = ssid.ToString();
            rec.PWD = pwd.ToString();
            rec.Encryption = (EnumEncryptType)encryption;
            rec.WepKeyId = wepKeyId;

            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

        public WiFiInfoRecord SetWiFiInfo(WiFiInfoRecord rec)
        {
            string printerName = "";
            string ssid = "";
            string pwd = "";
            byte encryption = (byte)EnumEncryptType.WPA2_PSK_AES;
            byte wepKeyId = 1;
            byte wifiEnabel = 0;
            byte wifiChangeFlag = 0;

            if (rec != null)
            {
                printerName = rec.PrinterName;
                ssid = rec.SSID;
                pwd = rec.PWD;
                encryption = (byte)rec.Encryption;
                wepKeyId = rec.WepKeyId;
                wifiEnabel = rec.WifiEnable;
                wifiChangeFlag = rec.WifiChangeFlag;

                int result = dll.SetWiFiInfo(printerName, wifiEnabel, wifiChangeFlag, ssid, pwd, encryption, wepKeyId);

                rec.CmdResult = (EnumCmdResult)result;

            }
            return rec;
        }

        public ApListRecord GetApList(string printerName)
        {
            ApListRecord rec = new ApListRecord();

            StringBuilder ssid0 = new StringBuilder(128);
            StringBuilder ssid1 = new StringBuilder(128);
            StringBuilder ssid2 = new StringBuilder(128);
            StringBuilder ssid3 = new StringBuilder(128);
            StringBuilder ssid4 = new StringBuilder(128);
            StringBuilder ssid5 = new StringBuilder(128);
            StringBuilder ssid6 = new StringBuilder(128);
            StringBuilder ssid7 = new StringBuilder(128);
            StringBuilder ssid8 = new StringBuilder(128);
            StringBuilder ssid9 = new StringBuilder(128);

            byte encryption0 = (byte)EnumEncryptType.NoSecurity;
            byte encryption1 = (byte)EnumEncryptType.NoSecurity;
            byte encryption2 = (byte)EnumEncryptType.NoSecurity;
            byte encryption3 = (byte)EnumEncryptType.NoSecurity;
            byte encryption4 = (byte)EnumEncryptType.NoSecurity;
            byte encryption5 = (byte)EnumEncryptType.NoSecurity;
            byte encryption6 = (byte)EnumEncryptType.NoSecurity;
            byte encryption7 = (byte)EnumEncryptType.NoSecurity;
            byte encryption8 = (byte)EnumEncryptType.NoSecurity;
            byte encryption9 = (byte)EnumEncryptType.NoSecurity;

            byte connected0 = 0;
            byte connected1 = 0;
            byte connected2 = 0;
            byte connected3 = 0;
            byte connected4 = 0;
            byte connected5 = 0;
            byte connected6 = 0;
            byte connected7 = 0;
            byte connected8 = 0;
            byte connected9 = 0;

            int result = dll.GetApList(printerName,
                                        ssid0, ref encryption0, ref connected0,
                                        ssid1, ref encryption1, ref connected1,
                                        ssid2, ref encryption2, ref connected2,
                                        ssid3, ref encryption3, ref connected3,
                                        ssid4, ref encryption4, ref connected4,
                                        ssid5, ref encryption5, ref connected5,
                                        ssid6, ref encryption6, ref connected6,
                                        ssid7, ref encryption7, ref connected7,
                                        ssid8, ref encryption8, ref connected8,
                                        ssid9, ref encryption9, ref connected9);

            rec.PrinterName = printerName;

            rec.SsidList.Add(ssid0.ToString());
            rec.SsidList.Add(ssid1.ToString());
            rec.SsidList.Add(ssid2.ToString());
            rec.SsidList.Add(ssid3.ToString());
            rec.SsidList.Add(ssid4.ToString());
            rec.SsidList.Add(ssid5.ToString());
            rec.SsidList.Add(ssid6.ToString());
            rec.SsidList.Add(ssid7.ToString());
            rec.SsidList.Add(ssid8.ToString());
            rec.SsidList.Add(ssid9.ToString());

            rec.EncryptionList.Add(encryption0);
            rec.EncryptionList.Add(encryption1);
            rec.EncryptionList.Add(encryption2);
            rec.EncryptionList.Add(encryption3);
            rec.EncryptionList.Add(encryption4);
            rec.EncryptionList.Add(encryption5);
            rec.EncryptionList.Add(encryption6);
            rec.EncryptionList.Add(encryption7);
            rec.EncryptionList.Add(encryption8);
            rec.EncryptionList.Add(encryption9);

            rec.ConnectedStatusList.Add(connected0 == 0x80);
            rec.ConnectedStatusList.Add(connected1 == 0x80);
            rec.ConnectedStatusList.Add(connected2 == 0x80);
            rec.ConnectedStatusList.Add(connected3 == 0x80);
            rec.ConnectedStatusList.Add(connected4 == 0x80);
            rec.ConnectedStatusList.Add(connected5 == 0x80);
            rec.ConnectedStatusList.Add(connected6 == 0x80);
            rec.ConnectedStatusList.Add(connected7 == 0x80);
            rec.ConnectedStatusList.Add(connected8 == 0x80);
            rec.ConnectedStatusList.Add(connected9 == 0x80);
          
            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

        public SoftApRecord GetSoftAp(string printerName)
        {
            SoftApRecord rec = new SoftApRecord();
            byte isEnableSoftAp = 0;
            StringBuilder ssid = new StringBuilder(128);
            StringBuilder pwd = new StringBuilder(128);

            int result = dll.GetSoftAp(printerName, ssid, pwd, ref isEnableSoftAp);

            rec.PrinterName = printerName;
            rec.SSID = ssid.ToString();
            rec.PWD = pwd.ToString();
            rec.WifiEnable = (isEnableSoftAp != 0);

            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

        public SoftApRecord SetSoftAp(SoftApRecord rec)
        {
            string printerName = "";
            string ssid = "";
            string pwd = "";
            bool isEnableSoftAp = false;

            if (rec != null)
            {
                printerName = rec.PrinterName;
                ssid = rec.SSID;
                pwd = rec.PWD;
                isEnableSoftAp = rec.WifiEnable;

                int result = dll.SetSoftAp(printerName, ssid, pwd, isEnableSoftAp);

                rec.CmdResult = (EnumCmdResult)result;

            }
            return rec;
        }

        public PowerSaveTimeRecord GetPowerSaveTime(string printerName)
        {
            PowerSaveTimeRecord rec = new PowerSaveTimeRecord();
            byte time = 0;

            int result = dll.GetPowerSaveTime(printerName, ref time);
 
            rec.PrinterName = printerName;
            rec.Time = time;
    
            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

        public PowerSaveTimeRecord SetPowerSaveTime(PowerSaveTimeRecord rec)
        {
            string printerName = "";
            byte time = 0;

            if (rec != null)
            {
                printerName = rec.PrinterName;
                time = rec.Time;

                int result = dll.SetPowerSaveTime(printerName, time);

                rec.CmdResult = (EnumCmdResult)result;

            }
            return rec;
        }

        public IpInfoRecord GetIpInfo(string printerName)
        {
            IpInfoRecord rec = new IpInfoRecord();
            byte mode_ipversion = 0, mode_ipaddress = 0, ip0 = 0, ip1 = 0, ip2 = 0, ip3 = 0, mask0 = 0, mask1 = 0, mask2 = 0, mask3 = 0, gate0 = 0, gate1 = 0, gate2 = 0, gate3 = 0;

            int result = dll.GetIPInfo(printerName, ref mode_ipversion, ref mode_ipaddress, ref ip0, ref ip1, ref ip2, ref ip3,
                                    ref mask0, ref mask1, ref mask2, ref mask3, ref gate0, ref gate1, ref gate2, ref gate3);
         
            byte[] arr = new byte[4];
               
            rec.PrinterName = printerName;
            rec.IpVersion = mode_ipversion;
            rec.IpAddressMode = (EnumIPType)mode_ipaddress;

            arr[0] = ip0;
            arr[1] = ip1;
            arr[2] = ip2;
            arr[3] = ip3;
            rec.Ip = new IPAddress(arr);

            arr[0] = mask0;
            arr[1] = mask1;
            arr[2] = mask2;
            arr[3] = mask3;
            rec.Mask = new IPAddress(arr);

            arr[0] = gate0;
            arr[1] = gate1;
            arr[2] = gate2;
            arr[3] = gate3;
            rec.Gate = new IPAddress(arr);

            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }

        public IpInfoRecord SetIpInfo(IpInfoRecord rec)
        {
            string printerName = "";
            byte mode_ipversion = 0, mode_ipaddress = 0, ip0 = 0, ip1 = 0, ip2 = 0, ip3 = 0, mask0 = 0, mask1 = 0, mask2 = 0, mask3 = 0, gate0 = 0, gate1 = 0, gate2 = 0, gate3 = 0;

            if (rec != null)
            {
                printerName = rec.PrinterName;
                mode_ipversion = rec.IpVersion;
                mode_ipaddress = (byte)rec.IpAddressMode;

                if (rec.Ip != null)
                {
                    byte[] arr = rec.Ip.GetAddressBytes();
                    ip0 = arr[0];
                    ip1 = arr[1];
                    ip2 = arr[2];
                    ip3 = arr[3];
                }

                if (rec.Mask != null)
                {
                    byte[] arr = rec.Mask.GetAddressBytes();
                    mask0 = arr[0];
                    mask1 = arr[1];
                    mask2 = arr[2];
                    mask3 = arr[3];
                }

                if (rec.Gate != null)
                {
                    byte[] arr = rec.Gate.GetAddressBytes();
                    gate0 = arr[0];
                    gate1 = arr[1];
                    gate2 = arr[2];
                    gate3 = arr[3];
                }

                int result = dll.SetIPInfo(printerName, ref mode_ipversion, ref mode_ipaddress, ref ip0, ref ip1, ref ip2, ref ip3,
                                 ref mask0, ref mask1, ref mask2, ref mask3, ref gate0, ref gate1, ref gate2, ref gate3);


                rec.CmdResult = (EnumCmdResult)result;
          
            }
            return rec;
        }

        public UserCenterInfoRecord GetUserCenterInfo(string printerName)
        {
            UserCenterInfoRecord rec = new UserCenterInfoRecord();

            StringBuilder _2ndSerialNO = new StringBuilder(128);
            StringBuilder _serialNO4AIO = new StringBuilder(128);
            uint totalCount = 0;

            int result = dll.GetUserCenterInfo(printerName, _2ndSerialNO, ref totalCount, _serialNO4AIO);

            rec.PrinterName = printerName;
            rec.TotalCounter = totalCount;
            rec.SecondSerialNO = _2ndSerialNO.ToString();
            rec.SerialNO4AIO = _serialNO4AIO.ToString();

            rec.CmdResult = (EnumCmdResult)result;

            return rec;
        }
    }

    public class BaseRecord : INotifyPropertyChanged
    {
        protected string printerName;
        protected EnumCmdResult cmdResult;

        public string PrinterName
        {
            get { return this.printerName; }
            set
            {
                this.printerName = value;
                OnPropertyChanged("PrinterName");
            }
        }

        public EnumCmdResult CmdResult
        {
            get { return this.cmdResult; }
            set
            {
                this.cmdResult = value;
                OnPropertyChanged("CmdResult");
            }
        }

        public BaseRecord()
        {
            printerName = "";
        }

        public BaseRecord(string printerName)
        {
            this.printerName = printerName;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class PowerSaveTimeRecord : BaseRecord
    {
        private byte time;
        private bool isPowerOff;

        public byte Time
        {
            get { return this.time; }
            set
            {
                this.time = value;
                OnPropertyChanged("Time");
            }
        }

        public bool IsPowerOff
        {
            get { return isPowerOff; }
            set
            {
                isPowerOff = value;
                OnPropertyChanged("IsPowerOff");
            }
        }

        public PowerSaveTimeRecord()
        {
            printerName = "";
            time = 1;
            isPowerOff = true;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public PowerSaveTimeRecord(string printerName, byte time, bool isPowerOff = true)
        {
            this.printerName = printerName;
            this.time = time;
            this.isPowerOff = isPowerOff;
            cmdResult = EnumCmdResult._CMD_invalid;
        }
    }

    public class SoftApRecord : BaseRecord
    {
        private string ssid;
        private string pwd;
        private bool wifiEnable;

        public string SSID
        {
            get { return this.ssid; }
            set
            {
                this.ssid = value;
                OnPropertyChanged("SSID");
            }
        }

        public string PWD
        {
            get { return this.pwd; }
            set
            {
                this.pwd = value;
                OnPropertyChanged("PWD");
            }
        }

        public bool WifiEnable
        {
            get { return this.wifiEnable; }
            set
            {
                this.wifiEnable = value;
                OnPropertyChanged("WifiEnable");
            }
        }

        public SoftApRecord()
        {
            printerName = "";
            ssid = "";
            pwd = "";
            wifiEnable = false;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public SoftApRecord(string printerName, string ssid, string pwd, bool wifiEnable)
        {
            this.printerName = printerName;
            this.ssid = ssid;
            this.pwd = pwd;
            this.wifiEnable = wifiEnable;
            cmdResult = EnumCmdResult._CMD_invalid;
        }
    }

    public class ApListRecord : BaseRecord
    {
        private List<string> ssidList;
        private List<byte> encryptionList;
        private List<bool> connectedStatusList;

        public List<string> SsidList
        {
            get { return this.ssidList; }
            set
            {
                this.ssidList = value;
                OnPropertyChanged("SsidList");
            }
        }

        public List<byte> EncryptionList
        {
            get { return this.encryptionList; }
            set
            {
                this.encryptionList = value;
                OnPropertyChanged("EncryptionList");
            }
        }

        public List<bool> ConnectedStatusList
        {
            get { return this.connectedStatusList; }
            set
            {
                this.connectedStatusList = value;
                OnPropertyChanged("ConnectedStatusList");
            }
        }

        public ApListRecord()
        {
            printerName = "";
            ssidList = new List<string>();
            encryptionList = new List<byte>();
            connectedStatusList = new List<bool>();
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public ApListRecord(string printerName, List<string> ssidList, List<byte> encryptionList, List<bool> connectedStatusList)
        {
            this.printerName = printerName;
            this.ssidList = ssidList;
            this.encryptionList = encryptionList;
            this.connectedStatusList = connectedStatusList;
            cmdResult = EnumCmdResult._CMD_invalid;
        }
    }

    public class WiFiInfoRecord : BaseRecord
    {
        private byte wifiEnable;    // bit0: Wi-Fi Enable, bit1: GO Enable, bit2: P2P Enable
        private byte wifiChangeFlag; // For fw request enable or disable wifi set 1
        private string ssid;
        private string pwd;
        private EnumEncryptType encryption;
        private byte wepKeyId;

        public byte WifiEnable
        {
            get { return this.wifiEnable; }
            set
            {
                this.wifiEnable = value;
                OnPropertyChanged("WifiEnable");
            }
        }

        public byte WifiChangeFlag
        {
            get { return this.wifiChangeFlag; }
            set
            {
                this.wifiChangeFlag = value;
                OnPropertyChanged("WifiChangeFlag");
            }
        }

        public string SSID
        {
            get { return this.ssid; }
            set
            {
                this.ssid = value;
                OnPropertyChanged("SSID");
            }
        }

        public string PWD
        {
            get { return this.pwd; }
            set
            {
                this.pwd = value;
                OnPropertyChanged("PWD");
            }
        }

        public EnumEncryptType Encryption
        {
            get { return this.encryption; }
            set
            {
                this.encryption = value;
                OnPropertyChanged("Encryption");
            }
        }

        public byte WepKeyId
        {
            get { return this.wepKeyId; }
            set
            {
                this.wepKeyId = value;
                OnPropertyChanged("WepKeyId");
            }
        }

        public WiFiInfoRecord()
        {
            printerName = "";
            ssid = "";
            pwd = "";
            encryption = EnumEncryptType.WPA2_PSK_AES;
            wepKeyId = 1;
            wifiEnable = 1;
            wifiChangeFlag = 0;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public WiFiInfoRecord(string printerName, byte wifiEnable, byte wifiEnableFlag, string ssid, string pwd, EnumEncryptType encryption, byte wepKeyId)
        {
            this.wifiEnable = wifiEnable;
            this.printerName = printerName;
            this.ssid = ssid;
            this.pwd = pwd;
            this.encryption = encryption;
            this.wepKeyId = wepKeyId;
            this.wifiChangeFlag = wifiEnableFlag;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

    }

    public class PasswordRecord : BaseRecord
    {
        private string pwd;

        public string PWD
        {
            get { return this.pwd; }
            set
            {
                this.pwd = value;
                //OnPropertyChanged("PWD");
            }
        }
        public PasswordRecord()
        {
            printerName = "";
            pwd = "";
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public PasswordRecord(string printerName, string pwd)
        {
            this.printerName = printerName;
            this.pwd = pwd;
            cmdResult = EnumCmdResult._CMD_invalid;
        }
    }

    public class FusingResetRecord : BaseRecord
    {

        public FusingResetRecord()
        {
            printerName = "";
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public FusingResetRecord(string printerName)
        {
            this.printerName = printerName;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

    }

    public class UserCfgRecord : BaseRecord
    {
        private sbyte leadingEdge;
        private sbyte sideToSide;
        private sbyte imageDensity;
        private sbyte lowHumidityMode;
        private sbyte platecontrolmode;
        private sbyte primarycoolingmode;

        public sbyte LeadingEdge
        {
            get { return this.leadingEdge; }
            set
            {
                this.leadingEdge = value;
                OnPropertyChanged("LeadingEdge");
            }
        }

        public sbyte SideToSide
        {
            get { return this.sideToSide; }
            set
            {
                this.sideToSide = value;
                OnPropertyChanged("SideToSide");
            }
        }

        public sbyte ImageDensity
        {
            get { return this.imageDensity; }
            set
            {
                this.imageDensity = value;
                OnPropertyChanged("ImageDensity");
            }
        }

        public sbyte LowHumidityMode
        {
            get { return this.lowHumidityMode; }
            set
            {
                this.lowHumidityMode = value;
                OnPropertyChanged("LowHumidityMode");
            }
        }

        public sbyte PlateControlMode
        {
            get { return this.platecontrolmode; }
            set
            {
                this.platecontrolmode = value;
                OnPropertyChanged("PlateControlMode");
            }
        }

        public sbyte PrimaryCoolingMode
        {
            get { return this.primarycoolingmode; }
            set
            {
                this.primarycoolingmode = value;
                OnPropertyChanged("PrimaryCoolingMode");
            }
        }

        public UserCfgRecord()
        {
            printerName = "";
            leadingEdge = 1;
            sideToSide = 1;
            imageDensity = 0;
            lowHumidityMode = 0;
            platecontrolmode = 2;
            primarycoolingmode = 0;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public UserCfgRecord(string printerName,
                            sbyte leadingEdge,
                            sbyte sideToSide,
                            sbyte imageDensity,
                            sbyte lowHumidityMode,
                            sbyte platecontrolmode,
                            sbyte primarycoolingmode)
        {
            this.printerName = printerName;
            this.leadingEdge = leadingEdge;
            this.sideToSide = sideToSide;
            this.imageDensity = imageDensity;
            this.lowHumidityMode = lowHumidityMode;
            this.platecontrolmode = platecontrolmode;
            this.primarycoolingmode = primarycoolingmode;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

    }

    public class CopyCmdRecord : BaseRecord
    {
        private byte density;
        private byte copyNum;
        private byte scanMode;
        private byte orgSize;
        private byte paperSize;
        private byte nUp;
        private byte dpi;
        private ushort scale;
        private byte startAtOnceFlag;

        public byte Density
        {
            get { return this.density; }
            set
            {
                this.density = value;
                OnPropertyChanged("Density");
            }
        }

        public byte CopyNum
        {
            get { return this.copyNum; }
            set
            {
                this.copyNum = value;
                OnPropertyChanged("CopyNum");
            }
        }

        public byte ScanMode
        {
            get { return this.scanMode; }
            set
            {
                this.scanMode = value;
                OnPropertyChanged("ScanMode");
            }
        }

        public byte OrgSize
        {
            get { return this.orgSize; }
            set
            {
                this.orgSize = value;
                OnPropertyChanged("OrgSize");
            }
        }

        public byte PaperSize
        {
            get { return this.paperSize; }
            set
            {
                this.paperSize = value;
                OnPropertyChanged("PaperSize");
            }
        }

        public byte NUp
        {
            get { return this.nUp; }
            set
            {
                this.nUp = value;
                OnPropertyChanged("NUp");
            }
        }

        public byte Dpi
        {
            get { return this.dpi; }
            set
            {
                this.dpi = value;
                OnPropertyChanged("Dpi");
            }
        }

        public ushort Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = value;
                OnPropertyChanged("Scale");
            }
        }

        public byte StartAtOnceFlag
        {
            get { return this.startAtOnceFlag; }
            set
            {
                this.startAtOnceFlag = value;
                OnPropertyChanged("StartAtOnceFlag");
            }
        }

        public CopyCmdRecord()
        {
            printerName = "";
            density = 3;
            copyNum = 0;
            scanMode = 0;
            orgSize = 0;
            paperSize = 0;
            nUp = 0;
            dpi = 0;
            scale = 0;
            startAtOnceFlag = 0;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public CopyCmdRecord(string printerName, byte density, byte copyNum, byte scanMode, byte orgSize, byte paperSize, byte nUp, byte dpi, ushort scale, byte startAtOnceFlag)
        {
            this.printerName = printerName;
            this.density = density;
            this.copyNum = copyNum;
            this.scanMode = scanMode;
            this.orgSize = orgSize;
            this.paperSize = paperSize;
            this.nUp = nUp;
            this.dpi = dpi;
            this.scale = scale;
            this.startAtOnceFlag = startAtOnceFlag;
            cmdResult = EnumCmdResult._CMD_invalid;
        }

    }

    public class IpInfoRecord : BaseRecord
    {
        private byte ipVersion;
        private EnumIPType ipAddressMode;
        private IPAddress ip;
        private IPAddress mask;
        private IPAddress gate;

        public byte IpVersion
        {
            get { return this.ipVersion; }
            set
            {
                this.ipVersion = value;
                OnPropertyChanged("IpVersion");
            }
        }

        public EnumIPType IpAddressMode
        {
            get { return this.ipAddressMode; }
            set
            {
                this.ipAddressMode = value;
                OnPropertyChanged("IpAddressMode");
            }
        }

        public IPAddress Ip
        {
            get { return this.ip; }
            set
            {
                this.ip = value;
                OnPropertyChanged("Ip");
            }
        }

        public IPAddress Mask
        {
            get { return this.mask; }
            set
            {
                this.mask = value;
                OnPropertyChanged("Mask");
            }
        }

        public IPAddress Gate
        {
            get { return this.gate; }
            set
            {
                this.gate = value;
                OnPropertyChanged("Gate");
            }
        }

        public IpInfoRecord()
        {
            printerName = "";
            ipVersion = 0;
            ipAddressMode = EnumIPType.DHCP;
            ip = new IPAddress(new byte[] { 0, 0, 0, 0 });
            mask = new IPAddress(new byte[] { 0, 0, 0, 0 });
            gate = new IPAddress(new byte[] { 0, 0, 0, 0 });
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public IpInfoRecord(string printerName, byte ipVersion, EnumIPType ipAddressMode, IPAddress ip, IPAddress mask, IPAddress gate)
        {
            this.printerName = printerName;
            this.ipVersion = ipVersion;
            this.ipAddressMode = ipAddressMode;
            this.ip = ip;
            this.mask = mask;
            this.gate = gate;
            this.cmdResult = EnumCmdResult._CMD_invalid;
        }
    }

    public class UserCenterInfoRecord : BaseRecord
    {
        private uint    _totalCounter;
        private string  _2ndSerialNO;
        private string  _serialNO4AIO;

        public uint TotalCounter
        {
            get { return this._totalCounter; }
            set
            {
                this._totalCounter = value;
                OnPropertyChanged("TotalCounter");
            }
        }

        public string SecondSerialNO
        {
            get { return this._2ndSerialNO; }
            set
            {
                this._2ndSerialNO = value;
                OnPropertyChanged("SecondSerialNO");
            }
        }

        public string SerialNO4AIO
        {
            get { return this._serialNO4AIO; }
            set
            {
                this._serialNO4AIO = value;
                OnPropertyChanged("SerialNO4AIO");
            }
        }

        public UserCenterInfoRecord()
        {
            printerName = "";
            _totalCounter = 0;
            _2ndSerialNO = "";
            _serialNO4AIO = "";
       
            cmdResult = EnumCmdResult._CMD_invalid;
        }

        public UserCenterInfoRecord(string printerName, uint _totalCounter, string _2ndSerialNO, string _serialNO4AIO)
        {
            this.printerName = printerName;
            this._totalCounter = _totalCounter;
            this._2ndSerialNO = _2ndSerialNO;
            this._serialNO4AIO = _serialNO4AIO;

            cmdResult = EnumCmdResult._CMD_invalid;
        }

    }
}
