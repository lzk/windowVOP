using System;
using System.Windows;
using System.Printing;
using System.Windows.Media; // For ImageSource
using System.Drawing.Printing;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using System.IO;
using System.Text;

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

    public class Encrypt
    {
        //默认密钥向量
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEE };

        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }


        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
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

        /// <summary>
        /// Message of status show in bottom of status panel.
        /// </summary>
        public static string GetErrorMsg(EnumStatus status, EnumMachineJob job, object frameworkElement)
        {
            string errMsg = "";

            FrameworkElement _this = frameworkElement as FrameworkElement;

            if (null != _this)
            {
                switch (status)
                {
                    case EnumStatus.Ready: errMsg = ""; break;
                    case EnumStatus.Printing: errMsg = (string)_this.TryFindResource("ResStr_Printing"); break;
                    case EnumStatus.PowerSaving: errMsg = ""; break;
                    case EnumStatus.WarmingUp: errMsg = (string)_this.TryFindResource("ResStr_WarmingUp"); break;
                    case EnumStatus.PrintCanceling: errMsg = (string)_this.TryFindResource("ResStr_Print_Cancelling"); break;
                    case EnumStatus.Processing: errMsg = (string)_this.TryFindResource("ResStr_Processing"); break;
                    case EnumStatus.CopyScanning: errMsg = (string)_this.TryFindResource("ResStr_Copying"); break;
                    case EnumStatus.CopyScanNextPage:
                        if (job == EnumMachineJob.IDCardCopyJob)
                            errMsg = (string)_this.TryFindResource("ResStr_Turn_card_over_to_copy_the_reverse_");
                        else if (job == EnumMachineJob.Nin1CopyJob)
                            errMsg = (string)_this.TryFindResource("ResStr_Place_Next_Page");
                        else
                            errMsg = "";
                        break;
                    case EnumStatus.CopyPrinting: errMsg = (string)_this.TryFindResource("ResStr_Copying"); break;
                    case EnumStatus.CopyCanceling: errMsg = (string)_this.TryFindResource("ResStr_Copy_Cancelling"); break;
                    case EnumStatus.IDCardMode: errMsg = (string)_this.TryFindResource("ResStr_ID_Card_Mode"); break;
                    case EnumStatus.ScanScanning: errMsg = (string)_this.TryFindResource("ResStr_Scanning"); break;
                    case EnumStatus.ScanSending: errMsg = (string)_this.TryFindResource("ResStr_Scanning"); break;
                    case EnumStatus.ScanCanceling: errMsg = (string)_this.TryFindResource("ResStr_Scan_Cancelling"); break;
                    case EnumStatus.ScannerBusy: errMsg = (string)_this.TryFindResource("ResStr_Scanner_Busy"); break;
                    case EnumStatus.TonerEnd1: errMsg = (string)_this.TryFindResource("ResStr_Toner_End"); break;
                    case EnumStatus.TonerEnd2: errMsg = (string)_this.TryFindResource("ResStr_Toner_End"); break;
                    case EnumStatus.TonerNearEnd: errMsg = (string)_this.TryFindResource("ResStr_Toner_Near_End"); break;
                    case EnumStatus.ManualFeedRequired: errMsg = (string)_this.TryFindResource("ResStr_Waiting_2nd_pages_when_print_manual_duplex_job"); break;
                    case EnumStatus.InitializeJam: errMsg = (string)_this.TryFindResource("ResStr_Paper_Jam__Paper_Remained"); break;
                    case EnumStatus.NofeedJam: errMsg = (string)_this.TryFindResource("ResStr_Paper_Jam__Nofeed"); break;
                    case EnumStatus.JamAtRegistStayOn: errMsg = (string)_this.TryFindResource("ResStr_Paper_Jam__Regist"); break;
                    case EnumStatus.JamAtExitNotReach: errMsg = (string)_this.TryFindResource("ResStr_Paper_Jam__Exit_NotReach"); break;
                    case EnumStatus.JamAtExitStayOn: errMsg = (string)_this.TryFindResource("ResStr_Paper_Jam__Exit"); break;
                    case EnumStatus.CoverOpen: errMsg = (string)_this.TryFindResource("ResStr_Cover_Open"); break;
                    case EnumStatus.NoTonerCartridge: errMsg = (string)_this.TryFindResource("ResStr_No_Toner_Cartridge"); break;
                    case EnumStatus.WasteTonerFull: errMsg = (string)_this.TryFindResource("ResStr_Please_Replace_Toner"); break;
                    case EnumStatus.PDLMemoryOver: errMsg = (string)_this.TryFindResource("ResStr_PDL_Memory_Overflow"); break;
                    case EnumStatus.FWUpdate: errMsg = (string)_this.TryFindResource("ResStr_FW_Updating"); break;
                    case EnumStatus.OverHeat: errMsg = (string)_this.TryFindResource("ResStr_Overheat"); break;
                    case EnumStatus.PolygomotorOnTimeoutError:  errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "202"; break;
                    case EnumStatus.PolygomotorOffTimeoutError: errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "203"; break;
                    case EnumStatus.PolygomotorLockSignalError: errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "204"; break;
                    case EnumStatus.BeamSynchronizeError:       errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "220"; break;
                    case EnumStatus.BiasLeak:                   errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "491"; break;
                    case EnumStatus.PlateActionError:           errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "501"; break;
                    case EnumStatus.MainmotorError:             errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "520"; break;
                    case EnumStatus.MainFanMotorEorror:         errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "530"; break;
                    case EnumStatus.JoinerThermistorError:      errMsg = (string)_this.TryFindResource("ResStr_Please_contact_customer_support__SCxxx") + "541"; break;
                    case EnumStatus.JoinerReloadError:          errMsg = (string)_this.TryFindResource("ResStr_Please_contact_customer_support__SCxxx") + "542"; break;
                    case EnumStatus.HighTemperatureErrorSoft:   errMsg = (string)_this.TryFindResource("ResStr_Please_contact_customer_support__SCxxx") + "543"; break;
                    case EnumStatus.HighTemperatureErrorHard:   errMsg = (string)_this.TryFindResource("ResStr_Please_contact_customer_support__SCxxx") + "544"; break;
                    case EnumStatus.JoinerFullHeaterError:      errMsg = (string)_this.TryFindResource("ResStr_Please_contact_customer_support__SCxxx") + "545"; break;
                    case EnumStatus.Joiner3timesJamError:       errMsg = (string)_this.TryFindResource("ResStr_Please_contact_customer_support__SCxxx") + "559"; break;
                    case EnumStatus.LowVoltageJoinerReloadError:    errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "560"; break;
                    case EnumStatus.MotorThermistorError:           errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "497"; break;
                    case EnumStatus.EEPROMCommunicationError:       errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "669"; break;
                    case EnumStatus.CTL_PRREQ_NSignalNoCome:        errMsg = (string)_this.TryFindResource("ResStr_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "688"; break;

                    case EnumStatus.SCAN_USB_Disconnect:        errMsg = (string)_this.TryFindResource("ResStr_USB_write_failed_during_scan_job_cancelling"); break;
                    case EnumStatus.SCAN_NET_Disconnect:        errMsg = (string)_this.TryFindResource("ResStr_NET_write_failed_during_scan_job_cancelling"); break;

                    case EnumStatus.ScanMotorError: errMsg = (string)_this.TryFindResource("ResStr_anner_not_found_home_position_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SC1001"); break;
                    case EnumStatus.SCAN_DRV_CALIB_FAIL: errMsg = (string)_this.TryFindResource("ResStr_Scan_Calibration_Error_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SC1001"); break;
                    case EnumStatus.NetWirelessDongleCfgFail: errMsg = (string)_this.TryFindResource("ResStr_Wireless_Dongle_Config_Fail_Turn_off_the_printer__and_turn_it_on_again_Contact_customer_support_if_this_failure_is_repeated_SCxxx") + "1002"; break;
                    case EnumStatus.DMAError: errMsg = (string)_this.TryFindResource("ResStr_DMA_Error_SCxxx") + "1006"; break;
                    case EnumStatus.Offline:
                    case EnumStatus.PowerOff:
                    case EnumStatus.Unknown: errMsg = ""; break;
                    default:
                        errMsg = "";
                        break;
                }
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
                case EnumStatus.WasteTonerFull              : st = StatusDisplayType.Ready; break;
                case EnumStatus.PDLMemoryOver              : st = StatusDisplayType.Error; break;
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
                case EnumStatus.SCAN_USB_Disconnect         : st = StatusDisplayType.Error; break; 
                case EnumStatus.SCAN_NET_Disconnect         : st = StatusDisplayType.Error; break;
                case EnumStatus.ScanMotorError              : st = StatusDisplayType.Error; break;
                case EnumStatus.SCAN_DRV_CALIB_FAIL         : st = StatusDisplayType.Error; break;
                case EnumStatus.NetWirelessDongleCfgFail    : st = StatusDisplayType.Error; break;
                case EnumStatus.DMAError                    : st = StatusDisplayType.Error; break;

                case EnumStatus.Offline                     :
                case EnumStatus.PowerOff                    :
                case EnumStatus.Unknown                     : st = StatusDisplayType.Offline; break;

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
        public static bool GetPrinterDrvName( 
                string strPrinterName, ref string strDrvName
                )
        {
            strDrvName = "";
            bool bSuccess = false;

            try
            {
                PrintServer myPrintServer = new PrintServer( null );
                PrintQueueCollection myPrintQueues = myPrintServer.GetPrintQueues();
                foreach (PrintQueue pq in myPrintQueues)
                {
                    if ( strPrinterName == pq.Name )
                    {
                        strDrvName = pq.QueueDriver.Name;
                        bSuccess = true;
                        break;
                    }
                }
            }
            catch
            {
            }

            return bSuccess;
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
                    nWidth  = 8500;
                    nHeight = 11000;
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

        public static bool IsAsciiLetter(string strText)
        {
            bool bIsAsciiLetter = true;
            foreach (char ch in strText)
            {
                if (((UInt16)ch) > 128)
                {
                    bIsAsciiLetter = false;
                    break;
                }
            }
            return bIsAsciiLetter;
        }

        public static bool IsOffline( EnumStatus status )
        {
            return ( EnumStatus.Offline == status 
                    || EnumStatus.PowerOff == status 
                    || EnumStatus.Unknown == status );
        }

        public static bool IsError(EnumStatus status)
        {
            bool bError = false;
            switch (status)
            {
                case EnumStatus.InitializeJam:
                case EnumStatus.NofeedJam:
                case EnumStatus.JamAtRegistStayOn:
                case EnumStatus.JamAtExitNotReach:
                case EnumStatus.JamAtExitStayOn:
                case EnumStatus.CoverOpen:
                case EnumStatus.NoTonerCartridge:
                case EnumStatus.PolygomotorOnTimeoutError:
                case EnumStatus.PolygomotorOffTimeoutError:
                case EnumStatus.PolygomotorLockSignalError:
                case EnumStatus.BeamSynchronizeError:
                case EnumStatus.BiasLeak:
                case EnumStatus.PlateActionError:
                case EnumStatus.MainmotorError:
                case EnumStatus.MainFanMotorEorror:
                case EnumStatus.JoinerThermistorError:
                case EnumStatus.JoinerReloadError:
                case EnumStatus.HighTemperatureErrorSoft:
                case EnumStatus.HighTemperatureErrorHard:
                case EnumStatus.JoinerFullHeaterError:
                case EnumStatus.Joiner3timesJamError:
                case EnumStatus.LowVoltageJoinerReloadError:
                case EnumStatus.MotorThermistorError:
                case EnumStatus.EEPROMCommunicationError:
                case EnumStatus.CTL_PRREQ_NSignalNoCome:
                case EnumStatus.ScanMotorError:
                case EnumStatus.SCAN_DRV_CALIB_FAIL:
                case EnumStatus.NetWirelessDongleCfgFail:
                case EnumStatus.DMAError:
                case EnumStatus.PDLMemoryOver:
                case EnumStatus.SCAN_USB_Disconnect:
                case EnumStatus.SCAN_NET_Disconnect:
                    bError = true;
                break;
                default:
                    break;
            }

            return bError;
        }

        public static bool IsStatusNeedMaintain( EnumStatus status )
        {
            return ((status >= EnumStatus.PolygomotorOnTimeoutError && status <= EnumStatus.CTL_PRREQ_NSignalNoCome) 
                    || status == EnumStatus.ScanMotorError 
                    || status == EnumStatus.ScanDriverCalibrationFail 
                    || status == EnumStatus.NetWirelessDongleCfgFail);
        }

    }


}
