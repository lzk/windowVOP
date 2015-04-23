using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.IO;

namespace VOP
{
    class StatusXmlHelper
    {
        private static string root             = "DeviceStatus";
        private static string strPrinter       = "Printer";
        private static string strTonerCapacity = "TonerCapacity";
        private static string strStatus        = "status";
        private static string strMachineJob    = "machineJob";
        private static string strName          = "Name";

        private string xmlFileName;
        private XmlDocument xmlDoc = new XmlDocument();

        public StatusXmlHelper(string _xmlFileName = "DeviceStatus.xml")
        {
            xmlFileName = _xmlFileName;
        }


        ///<summary>
        /// Create a Xml Document.
        ///</summary>
        ///<param name="rootNodeName"></param>
        ///<param name="version">XML Version(must Be:"1.0")</param>
        ///<param name="encoding"></param>
        ///<param name="standalone">Must Be "yes" or "no"</param>
        ///<returns></returns>
        public bool CreateXmlDocument(string rootNodeName, string version, string encoding, string standalone)
        {
            bool isSuccess = false;

            if (!File.Exists(xmlFileName)) // Create xml File
            {
                try
                {
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration(version, encoding, standalone);
                    XmlNode root = xmlDoc.CreateElement(rootNodeName);
                    xmlDoc.AppendChild(xmlDeclaration);
                    xmlDoc.AppendChild(root);
                    xmlDoc.Save(xmlFileName);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else // Load xml File
            {
                xmlDoc.Load(xmlFileName);
                isSuccess = true;
            }
            return isSuccess;
        }

        public bool CreateOrUpdateXmlNodeByXPath(XmlNode xmlNode, string xmlNodeName, string innerText)
        {
            bool isSuccess = false;
            bool isExistsNode = false;

            try
            {
                if (xmlNode != null)
                {
                    // The traversal of all the child nodes under the xmlNode node
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (node.Name.ToLower() == xmlNodeName.ToLower())
                        {
                            node.InnerXml = innerText;
                            isExistsNode = true;
                            break;
                        }
                    }
                    if (!isExistsNode)
                    {
                        // If node is not exist, create node
                        XmlElement subElement = xmlDoc.CreateElement(xmlNodeName);
                        subElement.InnerXml = innerText;
                        xmlNode.AppendChild(subElement);
                    }
                }
                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isSuccess;
        }



        public bool SavePrinterInfo(string printerName, string deviceStatus, string machineJob, string tonerCapacity)
        {
            bool isSuccess = false;
            bool isExistsNode = false;

            try
            {
                XmlNode xmlNode = xmlDoc.SelectSingleNode(root);
                if (xmlNode != null)
                {
                    // The traversal of all the child nodes under the xmlNode node
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name == strName)
                            {
                                if (printerName == attribute.Value)
                                {
                                    CreateOrUpdateXmlNodeByXPath(node, strTonerCapacity, tonerCapacity);
                                    CreateOrUpdateXmlNodeByXPath(node, strStatus, deviceStatus);
                                    CreateOrUpdateXmlNodeByXPath(node, strMachineJob, machineJob);

                                    isExistsNode = true;
                                    break;
                                }
                            }
                        }

                        if (isExistsNode) break;
                    }

                    if (!isExistsNode)
                    {
                        XmlElement node = xmlDoc.CreateElement(strPrinter);
                        node.SetAttribute(strName, printerName);

                        CreateOrUpdateXmlNodeByXPath(node, strTonerCapacity, tonerCapacity);
                        CreateOrUpdateXmlNodeByXPath(node, strStatus, deviceStatus);
                        CreateOrUpdateXmlNodeByXPath(node, strMachineJob, machineJob);

                        xmlNode.AppendChild(node);
                    }
                }

                xmlDoc.Save(xmlFileName);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return true;
        }

        public static bool SavePrinterInfo(string printerName, string deviceStatus, string machineJob, string tonerCapacity, string xmlFileName)
        {
            StatusXmlHelper xml = new StatusXmlHelper(xmlFileName);

            if (!xml.CreateXmlDocument("DeviceStatus", "1.0", "gb2312", "yes"))
            {
                return false;
            }

            return xml.SavePrinterInfo(printerName, deviceStatus, machineJob, tonerCapacity);            
        }


        public static bool GetXmlNodeByXPath(XmlNode xmlNode, string xmlNodeName, out string innerText)
        {
            innerText = "";
            bool isSuccess = false;

            try
            {
                if (xmlNode != null)
                {
                    // The traversal of all the child nodes under the xmlNode node
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (node.Name.ToLower() == xmlNodeName.ToLower())
                        {
                            innerText = node.InnerXml;
                            break;
                        }
                    }
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="deviceStatus"></param>
        /// <param name="machineJob"></param>
        /// <param name="tonerCapacity"></param>
        /// <returns></returns>
        public static bool GetPrinterInfo(string printerName, out string deviceStatus, out string machineJob, out string tonerCapacity, string xmlFileName)
        {
            XmlDocument xmlDoc = new XmlDocument();


            bool isSuccess = false;
            bool isExistsNode = false;

            tonerCapacity = "";
            deviceStatus = "";
            machineJob = "";

            if (!File.Exists(xmlFileName))
            {
                return false;
            }
            else // Load xml File
            {
                xmlDoc.Load(xmlFileName);
            }


            try
            {
                XmlNode xmlNode = xmlDoc.SelectSingleNode(root);
                if (xmlNode != null)
                {
                    // The traversal of all the child nodes under the xmlNode node
                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        foreach (XmlAttribute attribute in node.Attributes)
                        {
                            if (attribute.Name == strName)
                            {
                                if (printerName == attribute.Value)
                                {
                                    GetXmlNodeByXPath(node, strStatus, out deviceStatus);
                                    GetXmlNodeByXPath(node, strMachineJob, out machineJob);
                                    GetXmlNodeByXPath(node, strTonerCapacity, out tonerCapacity);

                                    isExistsNode = true;
                                    isSuccess = true;
                                    break;
                                }
                            }
                        }

                        if (isExistsNode) break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return isSuccess;
        }




        public static EnumStatus GetStatusTypeFormString(string strSatus)
        {
            EnumStatus retStatus = EnumStatus.Offline;
            switch (strSatus)
            {
                case "Ready"                       : retStatus = EnumStatus.Ready;                       break;
                case "Printing"                    : retStatus = EnumStatus.Printing;                    break;
                case "PowerSaving"                 : retStatus = EnumStatus.PowerSaving;                 break;
                case "WarmingUp"                   : retStatus = EnumStatus.WarmingUp;                   break;
                case "PrintCanceling"              : retStatus = EnumStatus.PrintCanceling;              break;
                case "Processing"                  : retStatus = EnumStatus.Processing;                  break;
                case "CopyScanning"                : retStatus = EnumStatus.CopyScanning;                break;
                case "CopyScanNextPage"            : retStatus = EnumStatus.CopyScanNextPage;            break;
                case "CopyPrinting"                : retStatus = EnumStatus.CopyPrinting;                break;
                case "CopyCanceling"               : retStatus = EnumStatus.CopyCanceling;               break;
                case "IDCardMode"                  : retStatus = EnumStatus.IDCardMode;                  break;
                case "ScanScanning"                : retStatus = EnumStatus.ScanScanning;                break;
                case "ScanSending"                 : retStatus = EnumStatus.ScanSending;                 break;
                case "ScanCanceling"               : retStatus = EnumStatus.ScanCanceling;               break;
                case "ScannerBusy"                 : retStatus = EnumStatus.ScannerBusy;                 break;
                case "TonerEnd1"                   : retStatus = EnumStatus.TonerEnd1;                   break;
                case "TonerEnd2"                   : retStatus = EnumStatus.TonerEnd2;                   break;
                case "TonerNearEnd"                : retStatus = EnumStatus.TonerNearEnd;                break;
                case "ManualFeedRequired"          : retStatus = EnumStatus.ManualFeedRequired;          break;
                case "InitializeJam"               : retStatus = EnumStatus.InitializeJam;               break;
                case "NofeedJam"                   : retStatus = EnumStatus.NofeedJam;                   break;
                case "JamAtRegistStayOn"           : retStatus = EnumStatus.JamAtRegistStayOn;           break;
                case "JamAtExitNotReach"           : retStatus = EnumStatus.JamAtExitNotReach;           break;
                case "JamAtExitStayOn"             : retStatus = EnumStatus.JamAtExitStayOn;             break;
                case "CoverOpen"                   : retStatus = EnumStatus.CoverOpen;                   break;
                case "NoTonerCartridge"            : retStatus = EnumStatus.NoTonerCartridge;            break;
                case "WasteTonerFull"              : retStatus = EnumStatus.WasteTonerFull;              break;
                case "FWUpdate"                    : retStatus = EnumStatus.FWUpdate;                    break;
                case "OverHeat"                    : retStatus = EnumStatus.OverHeat;                    break;
                case "PolygomotorOnTimeoutError"   : retStatus = EnumStatus.PolygomotorOnTimeoutError;   break;
                case "PolygomotorOffTimeoutError"  : retStatus = EnumStatus.PolygomotorOffTimeoutError;  break;
                case "PolygomotorLockSignalError"  : retStatus = EnumStatus.PolygomotorLockSignalError;  break;
                case "BeamSynchronizeError"        : retStatus = EnumStatus.BeamSynchronizeError;        break;
                case "BiasLeak"                    : retStatus = EnumStatus.BiasLeak;                    break;
                case "PlateActionError"            : retStatus = EnumStatus.PlateActionError;            break;
                case "MainmotorError"              : retStatus = EnumStatus.MainmotorError;              break;
                case "MainFanMotorEorror"          : retStatus = EnumStatus.MainFanMotorEorror;          break;
                case "JoinerThermistorError"       : retStatus = EnumStatus.JoinerThermistorError;       break;
                case "JoinerReloadError"           : retStatus = EnumStatus.JoinerReloadError;           break;
                case "HighTemperatureErrorSoft"    : retStatus = EnumStatus.HighTemperatureErrorSoft;    break;
                case "HighTemperatureErrorHard"    : retStatus = EnumStatus.HighTemperatureErrorHard;    break;
                case "JoinerFullHeaterError"       : retStatus = EnumStatus.JoinerFullHeaterError;       break;
                case "Joiner3timesJamError"        : retStatus = EnumStatus.Joiner3timesJamError;        break;
                case "LowVoltageJoinerReloadError" : retStatus = EnumStatus.LowVoltageJoinerReloadError; break;
                case "MotorThermistorError"        : retStatus = EnumStatus.MotorThermistorError;        break;
                case "EEPROMCommunicationError"    : retStatus = EnumStatus.EEPROMCommunicationError;    break;
                case "CTL_PRREQ_NSignalNoCome"     : retStatus = EnumStatus.CTL_PRREQ_NSignalNoCome;     break;
                case "ScanMotorError"              : retStatus = EnumStatus.ScanMotorError;              break;
                case "NetWirelessDongleCfgFail"    : retStatus = EnumStatus.NetWirelessDongleCfgFail;    break;
                case "PrinterDataError"            : retStatus = EnumStatus.PrinterDataError;            break;
                case "Unknown"                     : retStatus = EnumStatus.Unknown;                     break;
                case "Offline"                     : retStatus = EnumStatus.Offline;                     break;
                case "PowerOff"                    : retStatus = EnumStatus.PowerOff;                    break;
                default                            :
                    retStatus = EnumStatus.Unknown;                  
                    break;
            }

            return retStatus;
        }
        public static EnumMachineJob GetJobTypeFormString(string strJob)
        {
            EnumMachineJob retJob = EnumMachineJob.UnknowJob;
            switch (strJob)
            {
                case "UnknowJob"     : retJob = EnumMachineJob.UnknowJob;     break;
                case "PrintJob"      : retJob = EnumMachineJob.PrintJob;      break;
                case "NormalCopyJob" : retJob = EnumMachineJob.NormalCopyJob; break;
                case "ScanJob"       : retJob = EnumMachineJob.ScanJob;       break;
                case "FaxJob"        : retJob = EnumMachineJob.FaxJob;        break;
                case "FaxJob2"       : retJob = EnumMachineJob.FaxJob2;       break;
                case "ReportJob"     : retJob = EnumMachineJob.ReportJob;     break;
                case "Nin1CopyJob"   : retJob = EnumMachineJob.Nin1CopyJob;   break;
                case "IDCardCopyJob" : retJob = EnumMachineJob.IDCardCopyJob; break;
                case "PreIDCardCopyJob" : retJob = EnumMachineJob.PreIDCardCopyJob; break;
                default              :
                    retJob = EnumMachineJob.UnknowJob;
                    break;
            }
            return retJob;
        }



    }
}
