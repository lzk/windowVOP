using System;
using System.Windows;
using System.Printing;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using Microsoft.Win32;

namespace VOP
{
    public class SelfCloseRegistry
    {

        static RegistryKey LocalKey = Registry.LocalMachine;
        static RegistryKey rootKey = null;
        static string openKeyString = @"Software\Lenovo\Printer SSW\Version";


        public static bool Open()
        {
            try
            {
                rootKey = LocalKey.OpenSubKey(openKeyString, false);

                if (rootKey == null)
                    return false;
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                return false;
            }

            return true;
        }

        public static void Close()
        {
            rootKey.Close();
            LocalKey.Close();
        }

        public static string GetEXIT()
        {
            string str = "";
            try
            {
                str = rootKey.GetValue("VOP").ToString();
            }
            catch (Exception)
            {

            }

            return str;
        }

        public static bool DeleteEXIT()
        {
            try
            {
                rootKey.DeleteValue("VOP", false);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static string GetAgreement()
        {
            string str = "";
            try
            {
                str = rootKey.GetValue("CrmStatus").ToString();
            }
            catch (Exception)
            {

            }

            return str;
        }

        public static bool SetAgreement(bool agree)
        {
            try
            {
                rootKey.SetValue("CrmStatus", agree.ToString(), RegistryValueKind.String);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static bool DeleteAgreement()
        {
            try
            {
                rootKey.DeleteValue("CrmStatus", false);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

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

    public class common
    {
        private static PrinterInfo[] printerInfos = 
        {
            new PrinterInfo("Lenovo M7208"   , false , false) ,
            new PrinterInfo("Lenovo M7208W"  , false , true)  ,
            new PrinterInfo("Lenovo LJ2208"  , true  , false) ,
            new PrinterInfo("Lenovo LJ2208W" , true  , true)  ,
        };

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

    }

}
