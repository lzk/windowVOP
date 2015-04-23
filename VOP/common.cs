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
                    && EnumMachineJob.IDCardCopyJob != job 
                    && EnumMachineJob.PreIDCardCopyJob != job );
        }

        public AutoMachine()
        {
            ResetAutoMachine();
        }

        public void ResetAutoMachine()
        {
            m_autoMachineState = EnumState.init;
        }

        /// <summary>
        /// Tranfer status using job 1st, then using status.
        /// </summary>
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
            StatusDisplayType dt = common.GetStatusTypeForUI( status );
            if ( StatusDisplayType.Error == dt || StatusDisplayType.Offline == dt )
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
            new PrinterInfo("Lenovo M7208"   , false , false) ,
            new PrinterInfo("Lenovo M7208W"  , false , true)  ,
            new PrinterInfo("Lenovo LJ2208"  , true  , false) ,
            new PrinterInfo("Lenovo LJ2208W" , true  , true)  ,
        };

        public static BitmapSource GetOrigBitmapSource( ScanFiles obj  )
        {
            BitmapSource origSource = null;
            try
            {
                Uri myUri = new Uri(obj.m_pathOrig, UriKind.RelativeOrAbsolute);
                BmpBitmapDecoder decoder = new BmpBitmapDecoder(myUri, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad );
                origSource = decoder.Frames[0];
            }
            catch
            {
                origSource = null;
            }

            return origSource;
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

        /// <summary>
        /// Message of status show in bottom of status panel.
        /// </summary>
        public static string GetErrorMsg(EnumStatus status, EnumMachineJob job )
        {
            string errMsg = "";

            switch ( status )
            {
                case EnumStatus.Ready           :  errMsg = ""; break;
                case EnumStatus.Printing        :  errMsg = "Printing"; break;
                case EnumStatus.PowerSaving     :  errMsg = ""; break;
                case EnumStatus.WarmingUp       :  errMsg = "WarmingUp"; break;
                case EnumStatus.PrintCanceling  :  errMsg = "Print Cancelling"; break;
                case EnumStatus.Processing      :  errMsg = "Processing"; break;
                case EnumStatus.CopyScanning    :  errMsg = "Copying"; break;
               case EnumStatus.CopyScanNextPage           :
                    if ( job == EnumMachineJob.IDCardCopyJob )
                        errMsg = "Turn card over to copy the reverse.";
                    else if ( job == EnumMachineJob.Nin1CopyJob )
                        errMsg = "Place Next Page";
                    else
                        errMsg = "";
                    break;
                case EnumStatus.CopyPrinting                 :  errMsg = "Copying"; break;
                case EnumStatus.CopyCanceling                :  errMsg = "Copy Cancelling"; break;
                case EnumStatus.IDCardMode                   :  errMsg = "ID Card Mode"; break;
                case EnumStatus.ScanScanning                 :  errMsg = "Scanning"; break;
                case EnumStatus.ScanSending                  :  errMsg = "Scanning"; break;
                case EnumStatus.ScanCanceling                :  errMsg = "Scan Cancelling"; break;
                case EnumStatus.ScannerBusy                  :  errMsg = "Scanner Busy"; break;
                case EnumStatus.TonerEnd1                    :  errMsg = "Toner End"; break;
                case EnumStatus.TonerEnd2                    :  errMsg = "Toner End"; break;
                case EnumStatus.TonerNearEnd                 :  errMsg = "Toner Near End"; break;
                case EnumStatus.ManualFeedRequired           :  errMsg = "Waiting 2nd pages when print manual duplex job"; break;
                case EnumStatus.InitializeJam                :  errMsg = "Paper Jam: Paper Remained"; break;
                case EnumStatus.NofeedJam                    :  errMsg = "Paper Jam: Nofeed"; break;
                case EnumStatus.JamAtRegistStayOn            :  errMsg = "Paper Jam: Regist"; break;
                case EnumStatus.JamAtExitNotReach            :  errMsg = "Paper Jam: Exit"; break;
                case EnumStatus.JamAtExitStayOn              :  errMsg = "Paper Jam: Exit"; break;
                case EnumStatus.CoverOpen                    :  errMsg = "Cover Open"; break;
                case EnumStatus.NoTonerCartridge             :  errMsg = "No Toner Cartridge"; break;
                case EnumStatus.WasteTonerFull               :  errMsg = "Please Replace Toner"; break;
                case EnumStatus.FWUpdate                     :  errMsg = "FW Updating"; break;
                case EnumStatus.OverHeat                     :  errMsg = "Overheat"; break;
                case EnumStatus.PolygomotorOnTimeoutError    :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC202"; break;
                case EnumStatus.PolygomotorOffTimeoutError   :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC203"; break;
                case EnumStatus.PolygomotorLockSignalError   :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC204"; break;
                case EnumStatus.BeamSynchronizeError         :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC220"; break;
                case EnumStatus.BiasLeak                     :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC491"; break;
                case EnumStatus.PlateActionError             :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC501"; break;
                case EnumStatus.MainmotorError               :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC520"; break;
                case EnumStatus.MainFanMotorEorror           :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC530"; break;
                case EnumStatus.JoinerThermistorError        :  errMsg = "Please contact customer support:\nSC541"; break;
                case EnumStatus.JoinerReloadError            :  errMsg = "Please contact customer support:\nSC542"; break;
                case EnumStatus.HighTemperatureErrorSoft     :  errMsg = "Please contact customer support:\nSC543"; break;
                case EnumStatus.HighTemperatureErrorHard     :  errMsg = "Please contact customer support:\nSC544"; break;
                case EnumStatus.JoinerFullHeaterError        :  errMsg = "Please contact customer support:\nSC545"; break;
                case EnumStatus.Joiner3timesJamError         :  errMsg = "Please contact customer support:\nSC559"; break;
                case EnumStatus.LowVoltageJoinerReloadError  :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC560"; break;
                case EnumStatus.MotorThermistorError         :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC587"; break;
                case EnumStatus.EEPROMCommunicationError     :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC669"; break;
                case EnumStatus.CTL_PRREQ_NSignalNoCome      :  errMsg = "Turn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC688"; break;
                case EnumStatus.ScanMotorError               :  errMsg = "Scanner not found home position;\nTurn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC1001"; break;
                case EnumStatus.SCAN_DRV_CALIB_FAIL          :  errMsg = "Scan Calibration Error;\nTurn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC1001"; break;
                case EnumStatus.NetWirelessDongleCfgFail     :  errMsg = "Wireless Dongle Config Fail;\nTurn off the printer, and turn it on again.\nContact customer support if this failure is repeated:\nSC1002"; break;
                case EnumStatus.PrinterDataError             :  errMsg = "Print Data Error:\nSC1006"; break;
                case EnumStatus.Offline                      :
                case EnumStatus.PowerOff                   :
                case EnumStatus.Unknown                    : errMsg = ""; break;
                default:
                                                             errMsg = "";
                                                             break;
            }

            return errMsg;
        }

        /// <summary>
        /// Status Type for UI control in the middle of status panel.
        /// </summary>
        public static StatusDisplayType GetStatusTypeForUI( EnumStatus status )
        {
            StatusDisplayType st = StatusDisplayType.Offline;
            switch ( status )
            {
                case EnumStatus.Ready                       : st = StatusDisplayType.Ready; break;
                case EnumStatus.Printing                    : st = StatusDisplayType.Busy ; break;
                case EnumStatus.PowerSaving                 : st = StatusDisplayType.Sleep; break;
                case EnumStatus.WarmingUp                   : st = StatusDisplayType.Busy ; break;
                case EnumStatus.PrintCanceling              : st = StatusDisplayType.Busy ; break;
                case EnumStatus.Processing                  : st = StatusDisplayType.Busy ; break;
                case EnumStatus.CopyScanning                : st = StatusDisplayType.Busy ; break;
                case EnumStatus.CopyScanNextPage            : st = StatusDisplayType.Busy ; break;
                case EnumStatus.CopyPrinting                : st = StatusDisplayType.Busy ; break;
                case EnumStatus.CopyCanceling               : st = StatusDisplayType.Busy ; break;
                case EnumStatus.IDCardMode                  : st = StatusDisplayType.Busy ; break;
                case EnumStatus.ScanScanning                : st = StatusDisplayType.Busy ; break;
                case EnumStatus.ScanSending                 : st = StatusDisplayType.Busy ; break;
                case EnumStatus.ScanCanceling               : st = StatusDisplayType.Busy ; break;
                case EnumStatus.ScannerBusy                 : st = StatusDisplayType.Busy ; break;
                case EnumStatus.TonerEnd1                   : st = StatusDisplayType.Ready; break;
                case EnumStatus.TonerEnd2                   : st = StatusDisplayType.Ready; break;
                case EnumStatus.TonerNearEnd                : st = StatusDisplayType.Ready; break;
                case EnumStatus.ManualFeedRequired          : st = StatusDisplayType.Busy ; break;
                case EnumStatus.InitializeJam               : st = StatusDisplayType.Error; break;
                case EnumStatus.NofeedJam                   : st = StatusDisplayType.Error; break;
                case EnumStatus.JamAtRegistStayOn           : st = StatusDisplayType.Error; break;
                case EnumStatus.JamAtExitNotReach           : st = StatusDisplayType.Error; break;
                case EnumStatus.JamAtExitStayOn             : st = StatusDisplayType.Error; break;
                case EnumStatus.CoverOpen                   : st = StatusDisplayType.Error; break;
                case EnumStatus.NoTonerCartridge            : st = StatusDisplayType.Error; break;
                case EnumStatus.WasteTonerFull              : st = StatusDisplayType.Error; break;
                case EnumStatus.FWUpdate                    : st = StatusDisplayType.Busy ; break;
                case EnumStatus.OverHeat                    : st = StatusDisplayType.Busy ; break;
                case EnumStatus.PolygomotorOnTimeoutError   : st = StatusDisplayType.Error; break;
                case EnumStatus.PolygomotorOffTimeoutError  : st = StatusDisplayType.Error; break;
                case EnumStatus.PolygomotorLockSignalError  : st = StatusDisplayType.Error; break;
                case EnumStatus.BeamSynchronizeError        : st = StatusDisplayType.Error; break;
                case EnumStatus.BiasLeak                    : st = StatusDisplayType.Error; break;
                case EnumStatus.PlateActionError            : st = StatusDisplayType.Error; break;
                case EnumStatus.MainmotorError              : st = StatusDisplayType.Error; break;
                case EnumStatus.MainFanMotorEorror          : st = StatusDisplayType.Error; break;
                case EnumStatus.JoinerThermistorError       : st = StatusDisplayType.Error; break;
                case EnumStatus.JoinerReloadError           : st = StatusDisplayType.Error; break;
                case EnumStatus.HighTemperatureErrorSoft    : st = StatusDisplayType.Error; break;
                case EnumStatus.HighTemperatureErrorHard    : st = StatusDisplayType.Error; break;
                case EnumStatus.JoinerFullHeaterError       : st = StatusDisplayType.Error; break;
                case EnumStatus.Joiner3timesJamError        : st = StatusDisplayType.Error; break;
                case EnumStatus.LowVoltageJoinerReloadError : st = StatusDisplayType.Error; break;
                case EnumStatus.MotorThermistorError        : st = StatusDisplayType.Error; break;
                case EnumStatus.EEPROMCommunicationError    : st = StatusDisplayType.Error; break;
                case EnumStatus.CTL_PRREQ_NSignalNoCome     : st = StatusDisplayType.Error; break;
                case EnumStatus.ScanMotorError              : st = StatusDisplayType.Error; break;
                case EnumStatus.SCAN_DRV_CALIB_FAIL         : st = StatusDisplayType.Error; break;
                case EnumStatus.NetWirelessDongleCfgFail    : st = StatusDisplayType.Error; break;
                case EnumStatus.PrinterDataError            : st = StatusDisplayType.Ready; break;

                case EnumStatus.Offline                     : 
                case EnumStatus.PowerOff                    : 
                case EnumStatus.Unknown                     : st = StatusDisplayType.Offline ; break ;

                default                                     :
                                                              st = StatusDisplayType.Offline;
                                                              break;       
            }

            return st;
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

        /// <summary>
        /// Get the model name of printer. 
        /// </summary>
        /// <returns> 
        /// If success, return the name of printer driver, otherwise, return empty string.
        /// </returns>
        public static string GetPrinterDrvName( 
                string strPrinterName
                )
        {
            string strDrvName = "";

            try
            {
                PrintServer myPrintServer = new PrintServer( null );
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    if ( strPrinterName == pq.Name )
                    {
                        strDrvName = pq.QueueDriver.Name;
                        break;
                    }
                }
            }
            catch
            {
            }

            return strDrvName;
        }

        /// <summary>
        /// Get the value in milli-inch of width and height from paper size.
        /// </summary>
        public static void GetPaperSize( EnumPaperSizeScan s, ref int nWidth, ref int nHeight )
        {
            switch ( s )
            { 
                case EnumPaperSizeScan._A4         :
                    nWidth  = 8268;
                    nHeight = 11693;
                    break;
                case EnumPaperSizeScan._A5         :
                    nWidth  = 5827;
                    nHeight = 8268;
                    break;
                case EnumPaperSizeScan._B5         :
                    nWidth  = 7165;
                    nHeight = 10118;
                    break;
                case EnumPaperSizeScan._Letter     :
                    nWidth  = 8504;
                    nHeight = 10984;
                    break;
                case EnumPaperSizeScan._4x6Inch :
                    nWidth  = 4000;
                    nHeight = 6000;
                    break;
                default:
                    nWidth  = 8268;
                    nHeight = 11693;
                    break;
            }
        }
    }


}
