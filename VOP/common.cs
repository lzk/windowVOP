using System;
using System.Windows;
using System.Printing;
using System.Windows.Media; // For ImageSource
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace VOP
{
    public class PrinterInfo
    {
        public string m_name;   // Name of the printer driver
        public bool m_isSFP;    // true for SFP printer
        public bool m_isWiFi;   // true for wifi printer 

        public PrinterInfo(
                string strDrvName,
                bool isSFP,
                bool isWiFi 
                )
        {
            m_name   = strDrvName; 
            m_isSFP  = isSFP;      
            m_isWiFi = isWiFi;     
        }
    }

    public class ErrorMsgManager
    {
        private string m_strError = "";
        private string m_strInfo  = "";
        private int m_infoTimeOut  = 30; // sec
        private System.Windows.Threading.DispatcherTimer m_infoTimer = new System.Windows.Threading.DispatcherTimer();

        public delegate void HandlerMsgUpdate( string msg );
        public event HandlerMsgUpdate eventMsgUpdate = null;

        private string _strShow;
        private string m_strShow
        {
            set
            {
                _strShow = value;

                if ( null != eventMsgUpdate )
                    eventMsgUpdate( value );
            }
        }

        public ErrorMsgManager()
        {
            m_infoTimer.Interval = new TimeSpan(0,0,m_infoTimeOut);
            m_infoTimer.Tick += new EventHandler( HandlerInfoTimer );
        }

        private void HandlerInfoTimer(object sender, EventArgs e)
        {
            m_infoTimer.Stop();
//          m_strInfo = "";
//          m_strShow = m_strError;
        }

        // use MessageHandler( "", EnumStatusType.error ) to clear message
        public void MessageHandler( string msg, EnumStatusType emerge )
        {
            if ( EnumStatusType.error == emerge )
            {
                if ((msg != m_strError && msg == "Device Offline"))
                {
                    m_infoTimer.Start();
                }
                else if (m_strInfo.Length == 0 || (msg != m_strError && msg != ""))
                {
                    m_infoTimer.Stop();
                }
                
                m_strInfo = msg;
                m_strShow = msg;
            }
            else
            {
                m_infoTimer.Start();
                m_strInfo = msg;
                m_strShow = msg;
            }
        }
    }

    public class AutoMachine
    {
        public delegate void HandlerStateUpdate( EnumState state );
        public event HandlerStateUpdate eventStateUpdate = null;
        private int CMD_WAIT_TIME = 3000; // ms 
        private int m_timeOutBegin = 0; 
        private EnumState _autoMachineState = EnumState.init;
        private EnumState m_autoMachineState 
        {
            get
            {
                return _autoMachineState;
            }
            set
            {
                _autoMachineState = value;
                if ( null != eventStateUpdate )
                    eventStateUpdate( value );
            }
        }

        private bool IsIdle( EnumMachineJob job )
        {
            return ( EnumMachineJob.PrintJob        != job
                    && EnumMachineJob.NormalCopyJob != job
                    && EnumMachineJob.ScanJob       != job
                    && EnumMachineJob.Nin1CopyJob   != job
                    && EnumMachineJob.IDCardCopyJob != job );
        }

        public AutoMachine()
        {
            ResetAutoMachine();
        }

        public void ResetAutoMachine()
        {
            m_autoMachineState = EnumState.init;
        }

        public void TranferState( EnumMachineJob job )
        {
            if ( IsIdle(job) )
            {
                if ( EnumState.waitCmdBegin == m_autoMachineState )
                {
                    if ( Environment.TickCount - m_timeOutBegin > CMD_WAIT_TIME )
                        m_autoMachineState = EnumState.init;
                }
                else
                {
                    m_autoMachineState = EnumState.init;
                }
            }
            else
            {
                m_autoMachineState = EnumState.doingJob;
            }
        }

        public void TranferState( EnumStatus status )
        {
            if ( EnumStatusType.error == common.GetStatusType( status ) ||
               EnumStatus.IDCardMode == status)
            {
                m_autoMachineState = EnumState.stopWorking;
            }
        }

        public void TranferToWaitCmdBegin()
        {
            m_autoMachineState = EnumState.waitCmdBegin;
            m_timeOutBegin = Environment.TickCount;
        }
    }

    public class common
    {
        private static PrinterInfo[] printerInfos = 
        {
            new PrinterInfo("Lenovo ABC M001"   , false , false ) ,
            new PrinterInfo("Lenovo ABC M001 w" , false , true  ) ,
            new PrinterInfo("Lenovo ABC P001"   , true  , false ) ,
            new PrinterInfo("Lenovo ABC P001 w" , true  , true  ) ,
        };

        public static BitmapSource GetOrigBitmapSource( ScanFiles obj  )
        {
            BitmapSource origSource = null;
            try
            {
                Uri myUri = new Uri(obj.m_pathOrig, UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                BitmapSource bitmapSource = decoder.Frames[0];

                origSource = common.RotateBitmap(bitmapSource, obj.m_rotate);
            }
            catch
            {
                origSource = null;
            }

            return origSource;
        }

        public static EnumStatusType GetStatusType( EnumStatus status )
        {
            EnumStatusType statusType = EnumStatusType.info;

            switch ( (EnumStatus)status )
            {
                case EnumStatus.Ready                      : // Ready  
                case EnumStatus.PowerSaving                : // Sleep  
                case EnumStatus.SCANUSBDisconnect          : // --     
                case EnumStatus.ScanNETDisconnect          : // --     
                case EnumStatus.ScanPCUnkownCommandNET     : // --     
                case EnumStatus.ScanPCUnkownCommandUSB     : // --     
                case EnumStatus.NetWirelessConnectFail     : // --     
                case EnumStatus.NetWirelessDisable         : // --     
                    statusType = EnumStatusType.info;
                    break;
                case EnumStatus.FWUpdate                   : // Warning
                case EnumStatus.OverHeat                   : // Warning
                case EnumStatus.TonerNearEnd               : // Warning
                case EnumStatus.TonerEnd1                  : // Warning  
                case EnumStatus.PrinterDataError           : // Warning  
                    statusType = EnumStatusType.warning;
                    break;
                case EnumStatus.CopyCanceling              : // Busy   
                case EnumStatus.IDCardMode                 : // Busy   
                case EnumStatus.CopyPrinting               : // Busy   
                case EnumStatus.CopyScanNextPage           : // Busy   
                case EnumStatus.CopyScanning               : // Busy   
                case EnumStatus.ManualFeedRequired         : // Busy   
                case EnumStatus.PrintCanceling             : // Busy   
                case EnumStatus.Printing                   : // Busy   
                case EnumStatus.Processing                 : // Busy   
                case EnumStatus.ScanCanceling              : // Busy   
                case EnumStatus.ScannerBusy                : // Busy   
                case EnumStatus.ScanScanning               : // Busy   
                case EnumStatus.ScanSending                : // Busy   
                case EnumStatus.WarmingUp                  : // Busy   
                    statusType = EnumStatusType.info;
                    break;
                case EnumStatus.BeamSynchronizeError       : // Error  
                case EnumStatus.BiasLeak                   : // Error  
                case EnumStatus.PlateActionError:// Error
                case EnumStatus.MainFanMotorEorror:// Error
                case EnumStatus.CTL_PRREQ_NSignalNoCome: // Error  
                case EnumStatus.CoverOpen                  : // Error  
                case EnumStatus.EEPROMCommunicationError   : // Error  
                case EnumStatus.Joiner3timesJamError        : // Error  
                case EnumStatus.JoinerFullHeaterError       : // Error  
                case EnumStatus.JoinerReloadError           : // Error  
                case EnumStatus.JoinerThermistorError       : // Error  
                case EnumStatus.HighTemperatureErrorHard   : // Error  
                case EnumStatus.HighTemperatureErrorSoft   : // Error  
                case EnumStatus.InitializeJam              : // Error  
                case EnumStatus.JamAtExitNotReach          : // Error  
                case EnumStatus.JamAtExitStayOn            : // Error  
                case EnumStatus.JamAtRegistStayOn          : // Error  
                case EnumStatus.LowVoltageJoinerReloadError : // Error  
                case EnumStatus.MainmotorError             : // Error  
                case EnumStatus.MotorThermistorError       : // Error  
                case EnumStatus.NoTonerCartridge           : // Error  
                case EnumStatus.NofeedJam                  : // Error  
                case EnumStatus.PolygomotorLockSignalError : // Error  
                case EnumStatus.PolygomotorOffTimeoutError : // Error  
                case EnumStatus.PolygomotorOnTimeoutError  : // Error  
                case EnumStatus.ScanMotorError             : // Error  
                case EnumStatus.TonerEnd2                  : // Error  
                case EnumStatus.WasteTonerFull             : // Error  
                case EnumStatus.NetWirelessDongleCfgFail   : // Error  
                case EnumStatus.FWUpdateError              : // Error  
                case EnumStatus.DSPError                   : // Error  
                case EnumStatus.CodecError                 : // Error  
                    statusType = EnumStatusType.error;
                    break;
                case EnumStatus.Unknown             : // Error  
                case EnumStatus.Offline             : // Error  
                case EnumStatus.PowerOff            :
                    statusType = EnumStatusType.error;
                    break;
            }

            return statusType;
       }

        public static void SelectItemByContext( ComboBox cbo, byte context )
        {
            if ( null != cbo )
            {
                int idx = -1;
                int counter = 0;
                foreach (ComboBoxItem item in cbo.Items)
                {
                    if ( null != item.DataContext )
                    {
                        if ( context == (byte)item.DataContext )
                        {
                            idx = counter;
                            break;
                        }
                    }
                    counter++;
                }

                if ( -1 != idx )
                {
                    cbo.SelectedIndex = idx;
                }
            }
        }
        public static void SelectItemByContext( ComboBox cbo, sbyte context )
        {
            if ( null != cbo )
            {
                int idx = -1;
                int counter = 0;
                foreach (ComboBoxItem item in cbo.Items)
                {
                    if ( null != item.DataContext )
                    {
                        if ( context == (sbyte)item.DataContext )
                        {
                            idx = counter;
                            break;
                        }
                    }
                    counter++;
                }

                if ( -1 != idx )
                {
                    cbo.SelectedIndex = idx;
                }
            }
        }

        public static void select_cbo_by_content( ComboBox cbo, int val )
        {
            if ( null != cbo )
            {
                int idx = -1;
                int counter = 0;
                foreach (ComboBoxItem item in cbo.Items)
                {
                    string str_content = item.Content as string; 

                    if ( null != str_content )
                    {
                        try
                        {
                            if ( val == Convert.ToInt32(str_content) )
                            {
                                idx = counter;
                                break;
                            }
                        }
                        catch 
                        {
                        }
                    }
                    counter++;
                }

                if ( -1 != idx )
                {
                    cbo.SelectedIndex = idx;
                }
            }
        }

        public static bool IsVailibleSSID(string text)
        {
            if (text == null || text.Length > 32)
                return false;

            foreach (char ch in text)
            {
                if ((int)ch < 0x20 || (int)ch > 0x7e)
                    return false;
            }

            return true;
        }

        public static BitmapSource RotateBitmap( BitmapSource bmpSrc, int angle )
        {
            BitmapSource ret = null;

            if ( 0 != angle )
            {
                CachedBitmap cache = new CachedBitmap(bmpSrc, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                ret = BitmapFrame.Create(new TransformedBitmap(cache, new RotateTransform(angle)));
            }
            else
            {
                ret = bmpSrc;
            }

            return ret;
        }

        public static string GetErrorMsg(EnumStatus status, EnumMachineJob job )
        {
            string errMsg = "";

            switch ( status )
            {
                case EnumStatus.Ready                      : errMsg = "   "                                                                                                     ; break;
                case EnumStatus.Printing                   : errMsg = "Printing"                                                                                                ; break;
                case EnumStatus.PowerSaving                : errMsg = "   "                                                                                                     ; break;
                case EnumStatus.WarmingUp                  : errMsg = "WarmingUp"                                                                                               ; break;
                case EnumStatus.PrintCanceling             : errMsg = "Print Cancelling"                                                                                         ; break;
                case EnumStatus.Processing                 : errMsg = "Processing"                                                                                              ; break;
                case EnumStatus.CopyScanning               : errMsg = "Copying"                                                                                                 ; break;
                case EnumStatus.CopyScanNextPage           :
                    if ( job == EnumMachineJob.IDCardCopyJob )
                        errMsg = "Turn card over to copy the reverse.";
                    else if ( job == EnumMachineJob.Nin1CopyJob )
                        errMsg = "Place Next Page";
                    else
                        errMsg = "";
                    break;
                case EnumStatus.CopyPrinting               : errMsg = "Copying"                                                                                                 ; break;
                case EnumStatus.CopyCanceling              : errMsg = "Copy Cancelling"                                                                                         ; break;
                case EnumStatus.ScanScanning               : errMsg = "Scanning"                                                                                                ; break;
                case EnumStatus.IDCardMode                 : errMsg = "ID card mode"                                                                                            ; break;
                case EnumStatus.ScanSending                : errMsg = "Scanning"                                                                                                ; break;
                case EnumStatus.ScanCanceling              : errMsg = "Scan Cancelling"																							; break;
                case EnumStatus.ScannerBusy                : errMsg = "Scanner Busy"                                                                                            ; break;
                case EnumStatus.TonerEnd1                  : errMsg = "Toner End"                                                                                               ; break;
                case EnumStatus.TonerEnd2                  : errMsg = "Toner End"                                                                                               ; break;
                case EnumStatus.TonerNearEnd               : errMsg = "Toner near end"                                                                                          ; break;
                case EnumStatus.ManualFeedRequired         : errMsg = "Waiting 2nd pages when print manual duplex job"                                                          ; break;
                case EnumStatus.InitializeJam              : errMsg = "Paper Jam: paper remained"                                                                               ; break;
                case EnumStatus.NofeedJam                  : errMsg = "Paper Jam: Nofeed"                                                                                       ; break;
                case EnumStatus.JamAtRegistStayOn          : errMsg = "Paper Jam: Regist"                                                                                       ; break;
                case EnumStatus.JamAtExitNotReach          : errMsg = "Paper Jam: Exit"                                                                                         ; break;
                case EnumStatus.JamAtExitStayOn            : errMsg = "Paper Jam: Exit"                                                                                         ; break;
                case EnumStatus.CoverOpen                  : errMsg = "Cover Open"                                                                                              ; break;
                case EnumStatus.NoTonerCartridge           : errMsg = "No toner cartridge"                                                                                      ; break;
                case EnumStatus.WasteTonerFull             : errMsg = "Please replace Toner"                                                                                    ; break;
                case EnumStatus.FWUpdate                   : errMsg = "FW updating"                                                                                             ; break;
                case EnumStatus.OverHeat                   : errMsg = "Over Heat"                                                                                               ; break;
                case EnumStatus.PolygomotorOnTimeoutError: errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC202"; break;
                case EnumStatus.PolygomotorOffTimeoutError : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC203" ; break;
                case EnumStatus.PolygomotorLockSignalError : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC204" ; break;
                case EnumStatus.BeamSynchronizeError       : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC220" ; break;
                case EnumStatus.BiasLeak                   : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC491" ; break;
                case EnumStatus.PlateActionError           : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC501" ; break;
                case EnumStatus.MainmotorError: errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC520"; break;
                case EnumStatus.MainFanMotorEorror: errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC530"; break;
                case EnumStatus.JoinerThermistorError: errMsg = "Please contact customer support: SC541"; break;
                case EnumStatus.JoinerReloadError           : errMsg = "Please contact customer support: SC542"                                                                  ; break;
                case EnumStatus.HighTemperatureErrorSoft   : errMsg = "Please contact customer support: SC543"                                                                  ; break;
                case EnumStatus.HighTemperatureErrorHard   : errMsg = "Please contact customer support: SC544"                                                                  ; break;
                case EnumStatus.JoinerFullHeaterError       : errMsg = "Please contact customer support: SC545"                                                                  ; break;
                case EnumStatus.Joiner3timesJamError        : errMsg = "Please contact customer support: SC559"                                                                  ; break;
                case EnumStatus.LowVoltageJoinerReloadError : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC560" ; break;
                case EnumStatus.MotorThermistorError       : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC587" ; break;
                case EnumStatus.EEPROMCommunicationError   : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC669" ; break;
                case EnumStatus.CTL_PRREQ_NSignalNoCome    : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC688" ; break;
                case EnumStatus.ScanPCUnkownCommandUSB     : errMsg = "   "                                                                                                     ; break;
                case EnumStatus.SCANUSBDisconnect          : errMsg = "   "                                                                                                     ; break;
                case EnumStatus.ScanPCUnkownCommandNET     : errMsg = "   "                                                                                                     ; break;
                case EnumStatus.ScanNETDisconnect          : errMsg = "   "                                                                                                     ; break;
                case EnumStatus.ScanMotorError             : errMsg = "Turn off the printer, and turn it on again. Contact customer support if this failure is repeated: SC800" ; break;
                case EnumStatus.NetWirelessConnectFail     : errMsg = "Wireless Connection Fail"                                                                                ; break;
                case EnumStatus.NetWirelessDisable         : errMsg = "Wireless Disabled"                                                                                       ; break;
                case EnumStatus.NetWirelessDongleCfgFail   : errMsg = "Wireless Dongle Config Fail"                                                                             ; break;
                case EnumStatus.FWUpdateError              : errMsg = "FW download Error;Turn off the printer, and turn it on again.Contact customer support if this failure is repeated"; break;
                case EnumStatus.DSPError                   : errMsg = "DSP Error;Turn off the printer, and turn it on again.Contact customer support if this failure is repeated"; break;
                case EnumStatus.CodecError                 : errMsg = "CODEC Error;Turn off the printer, and turn it on again.Contact customer support if this failure is repeated"; break;
                case EnumStatus.PrinterDataError           : errMsg = "Print Data Error;"; break;               
                case EnumStatus.Unknown                    : errMsg = "Status Unknown"; break;
                case EnumStatus.Offline                    : errMsg = "Device Offline"; break;
                case EnumStatus.PowerOff                   : errMsg = "Power Off"; break;

                default:
                                                             errMsg = "";
                                                             break;
            }

            return errMsg;
        }

        public static string GetStatusMsg( EnumStatus status )
        {
            string strStatus = "Unknown";
            string strDebug;

            switch ( status )
            {
                case EnumStatus.Ready                      : strStatus = "Ready   "; break;
                case EnumStatus.Printing                   : strStatus = "Busy    "; break;
                case EnumStatus.PowerSaving                : strStatus = "Sleep   "; break;
                case EnumStatus.WarmingUp                  : strStatus = "Busy    "; break;
                case EnumStatus.PrintCanceling             : strStatus = "Busy    "; break;
                case EnumStatus.Processing                 : strStatus = "Busy    "; break;
                case EnumStatus.CopyScanning               : strStatus = "Busy    "; break;
                case EnumStatus.CopyScanNextPage           : strStatus = "Busy    "; break;
                case EnumStatus.CopyPrinting               : strStatus = "Busy    "; break;
                case EnumStatus.CopyCanceling              : strStatus = "Busy    "; break;
                case EnumStatus.IDCardMode                 : strStatus = "Busy    "; break;
                case EnumStatus.ScanScanning               : strStatus = "Busy    "; break;
                case EnumStatus.ScanSending                : strStatus = "Busy    "; break;
                case EnumStatus.ScanCanceling              : strStatus = "Busy    "; break;
                case EnumStatus.ScannerBusy                : strStatus = "Busy    "; break;
                case EnumStatus.TonerEnd1                  : strStatus = "Warning "; break;
                case EnumStatus.TonerEnd2                  : strStatus = "Error   "; break;
                case EnumStatus.TonerNearEnd               : strStatus = "Warning" ; break;
                case EnumStatus.ManualFeedRequired         : strStatus = "Busy    "; break;
                case EnumStatus.InitializeJam              : strStatus = "Error   "; break;
                case EnumStatus.NofeedJam                  : strStatus = "Error   "; break;
                case EnumStatus.JamAtRegistStayOn          : strStatus = "Error   "; break;
                case EnumStatus.JamAtExitNotReach          : strStatus = "Error   "; break;
                case EnumStatus.JamAtExitStayOn            : strStatus = "Error   "; break;
                case EnumStatus.CoverOpen                  : strStatus = "Error   "; break;
                case EnumStatus.NoTonerCartridge           : strStatus = "Error   "; break;
                case EnumStatus.WasteTonerFull             : strStatus = "Error   "; break;
                case EnumStatus.FWUpdate                   : strStatus = "Warning "; break;
                case EnumStatus.OverHeat                   : strStatus = "Warning "; break;
                case EnumStatus.PolygomotorOnTimeoutError: strStatus = "Error   "; break;
                case EnumStatus.PolygomotorOffTimeoutError : strStatus = "Error   "; break;
                case EnumStatus.PolygomotorLockSignalError : strStatus = "Error   "; break;
                case EnumStatus.BeamSynchronizeError       : strStatus = "Error   "; break;
                case EnumStatus.BiasLeak                   : strStatus = "Error   "; break;
                case EnumStatus.PlateActionError           : strStatus = "Error   "; break;
                case EnumStatus.MainmotorError             : strStatus = "Error   "; break;
                case EnumStatus.MainFanMotorEorror         : strStatus = "Error   "; break;
                case EnumStatus.JoinerThermistorError       : strStatus = "Error   "; break;
                case EnumStatus.JoinerReloadError           : strStatus = "Error   "; break;
                case EnumStatus.HighTemperatureErrorSoft   : strStatus = "Error   "; break;
                case EnumStatus.HighTemperatureErrorHard   : strStatus = "Error   "; break;
                case EnumStatus.JoinerFullHeaterError       : strStatus = "Error   "; break;
                case EnumStatus.Joiner3timesJamError        : strStatus = "Error   "; break;
                case EnumStatus.LowVoltageJoinerReloadError : strStatus = "Error   "; break;
                case EnumStatus.MotorThermistorError       : strStatus = "Error   "; break;
                case EnumStatus.EEPROMCommunicationError   : strStatus = "Error   "; break;
                case EnumStatus.CTL_PRREQ_NSignalNoCome    : strStatus = "Error   "; break;
                case EnumStatus.ScanPCUnkownCommandUSB     : strStatus = "        "; break;
                case EnumStatus.SCANUSBDisconnect          : strStatus = "        "; break;
                case EnumStatus.ScanPCUnkownCommandNET     : strStatus = "        "; break;
                case EnumStatus.ScanNETDisconnect          : strStatus = "        "; break;
                case EnumStatus.ScanMotorError             : strStatus = "Error   "; break;
                case EnumStatus.NetWirelessConnectFail     : strStatus = "Ready   "; break;
                case EnumStatus.NetWirelessDisable         : strStatus = "Ready   "; break;
                case EnumStatus.NetWirelessDongleCfgFail   : strStatus = "Error   "; break;
                case EnumStatus.FWUpdateError              : strStatus = "Error   "; break;
                case EnumStatus.DSPError                   : strStatus = "Error   "; break;
                case EnumStatus.CodecError                 : strStatus = "Error   "; break;
                case EnumStatus.PrinterDataError           : strStatus = "Warning "; break;               
                case EnumStatus.Offline                    : strStatus = "Offline "; break;
                case EnumStatus.PowerOff                   : strStatus = "Power Off"; break;
                default:         
                    strDebug = "@@@ Vop: status = 0x" + status.ToString("x") + " strStatus == \"Unknown\" @@@";
                    dll.OutputDebugStringToFile_(strDebug);
                    strStatus = "Unknown"; 
                    break;       
            }

            return strStatus;
        }


        /// <summary> Get the support printer list.  </summary>
        /// <param name="listPrinters">Used to store the supported printers.</param>
        /// <returns> None. </returns>
        /// <remarks> If the port of printer isn't support, the printer will
        /// not store in the list. The printer in the list were sorted by
        /// name. The default printer will be the 1st element, if it is in the
        /// list.  </remarks>
        public static void GetSupportPrinters(List<string> listPrinters)
        {
            listPrinters.Clear();

            try
            {
                // If spooler was stopped, new PrintServer( null ) will throw
                // a exception.

                string strDefPrinter = "";

                PrinterSettings settings = new PrinterSettings();
                PrintServer myPrintServer = new PrintServer(null);
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    PrintDriver queuedrv = pq.QueueDriver;
                    if (IsSupportPrinter(queuedrv.Name) == true )
                    {
                        if ( EnumPortType.PT_UNKNOWN != (EnumPortType)dll.CheckPortAPI(pq.Name))
                            listPrinters.Add(pq.Name);
                    }
                    settings.PrinterName = pq.Name;
                    if (settings.IsDefaultPrinter)
                    {
                        strDefPrinter = pq.Name;
                    }
                }

                // Store the printer name list
                for ( int i=0; i<listPrinters.Count; i++ )
                {
                    for ( int j=i+1; j<listPrinters.Count; j++ )
                    {
                        if ( -1 == string.Compare( listPrinters[j] , listPrinters[i] ) )
                        {
                            // swrap the value 
                            string t = listPrinters[i];
                            listPrinters[i] = listPrinters[j];
                            listPrinters[j] = t;
                        }
                    }
                }

                // If the default printer is in the support list, make it the
                // 1st item.
                int index = listPrinters.FindIndex( 
                        delegate( string s )
                        {
                        return s == strDefPrinter;
                        }
                        );

                if ( index != -1 && index != 0 )
                {
                    listPrinters.RemoveAt( index );
                    listPrinters.Insert( 0, strDefPrinter );
                }
            }
            catch
            {
            }

        }

        public static bool IsSupportPrinter(
                string strDrvName       // Name of printer driver
                )
        {
            bool isSupport = false;

            foreach (PrinterInfo el in printerInfos)
            {
                if (el.m_name == strDrvName)
                {
                    isSupport = true;
                    break;
                }
            }

            return isSupport;
        }

        public static bool IsSFPPrinter(
                string strDrvName       // Name of printer driver
                )
        {
            bool bResult = false;

            foreach (PrinterInfo el in printerInfos)
            {
                if (el.m_name == strDrvName)
                {
                    bResult = el.m_isSFP;
                    break;
                }
            }

            return bResult;
        }

        public static bool IsSupportWifi(
        string strDrvName       // Name of printer driver
        )
        {
            bool bResult = false;

            foreach (PrinterInfo el in printerInfos)
            {
                if (el.m_name == strDrvName)
                {
                    bResult = el.m_isWiFi;
                    break;
                }
            }

            return bResult;
        }
    }


}
