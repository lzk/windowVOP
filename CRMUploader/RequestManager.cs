using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Timers;
using System.Management;
using System.Configuration;
using System.Xml.Serialization;
using System.Diagnostics;

namespace CRMUploader
{
    public enum JSONReturnFormat
    {
        JSONResultFormat1 = 1,
        JSONResultFormat2,
        MerchantInfoSet,
        MaintainInfoSet,
        SessionInfo,
        UserInfomation
    };

    public class SessionInfo
    {
        public Int32    m_nTotalCount;
        public string   m_strDetail;
        public Int32    m_nErrorCode;
        public bool     m_bSuccess;
        public Int32    m_nCustomerID;
        public Int32    m_nMerchantID;
    }

    public class JSONResultFormat2
    {
        public string   m_strMessage;
        public bool     m_bSuccess;
    }

    public class JSONResultFormat1 //Normal
    {
        public Int32    m_nResponse;
        public string   m_strMessage;
        public bool     m_bSuccess;
    }

    public class SystemInfo
    {
        public string m_strMacAddress;
        public string m_strDeviceBrand;
        public string m_strDeviceModel;

        public SystemInfo()
        {
            m_strMacAddress = GetMacAddress();
            m_strDeviceBrand = m_strDeviceModel = "";
        }

        public static string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址 
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "";
            }
        }
    }

    public class MD5
    {
        public static string MD5_Encrypt(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
        }
    }

    public class CRM_PrintInfo
    {

        private readonly string m_strSignKey = "86c02972fba047b0b0a9adb8123029fb";

        public string m_strMobileCode;    //Net card ID
        public string m_strMobileNumber;  //can not upload
        public string m_strDeviceBrand;
        public string m_strDeviceModel;
        public string m_strPrintType;   //always "VOP-WIN"
        public string m_strPrintMode;    //alway "VOP-WIN"
        public string m_strPrintDocType;
        public string m_strPrintCopys;
        public string m_strPrintPages;
        public string m_strPrinterModel;
        public string m_strPrinterName;
        public string m_strPrinterType;
        public string m_strPrintSuccess;   //alway true
        public string m_strVersion;
        public string m_strFlag;
        public DateTime m_time;   //yyyyMMddHHmmss, for example:20140219092408
        public string m_strSign; //MobileCode+time+key using MD5

        public CRM_PrintInfo()
        {
            m_strMobileCode = SystemInfo.GetMacAddress();
            m_strMobileNumber = "";
            m_strDeviceBrand = "";
            m_strDeviceModel = "";
            m_strPrintType = "VOP-WIN";
            m_strPrintMode = "VOP-WIN";
            m_strPrintDocType = "doc";
            m_strPrintCopys = "1";
            m_strPrintPages = "1";
            m_strPrinterModel = "Lenovo M7208W";
            m_strPrinterName = "Lenovo M7208W";
            m_strPrinterType = "SFP";
            m_strPrintSuccess = "true";
            m_strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            System.Guid guid = System.Guid.NewGuid();
            m_strFlag = guid.ToString();
            m_time = System.DateTime.Now.ToLocalTime();
        }

        public string ConvertToWebParams()
        {
            string str = "";
            try
            {
                m_strSign = MD5.MD5_Encrypt(m_strMobileCode + m_time.ToString("yyyyMMddHHmmss") + m_strSignKey);

                str = String.Format("MobileCode={0}&Mobile={1}&DeviceBrand={2}&DeviceModel={3}&PrintType={4}&PrintMode={5}&PrintDocType={6}&PrintCopys={7}&PrintPages={8}&PrinterModel={9}&PrinterName={10}&PrinterType={11}&IsSuccess={12}&version={13}&flag={14}&time={15}&sign={16}"
                    , m_strMobileCode, m_strMobileNumber, m_strDeviceBrand, m_strDeviceModel, m_strPrintType, m_strPrintMode, m_strPrintDocType,
                    m_strPrintCopys, m_strPrintPages, m_strPrinterModel, m_strPrinterName, m_strPrinterType, m_strPrintSuccess, m_strVersion, m_strFlag, m_time.ToString("yyyyMMddHHmmss"), m_strSign
                    );  
            }
            catch
            {

            }

            return str;
        }

      
    }

    public class CRM_LocalInfo
    {
        private readonly string m_strSignKey = "86c02972fba047b0b0a9adb8123029fb";

        public string m_strMobileNumber;  //can not upload
        public string m_strAppFrom; //always "VOP-WIN"
        public string m_strAppVersion;
        public DateTime m_time;   //yyyyMMddHHmmss, for example:20140219092408
        public string m_strSign; //MobileCode+time+key using MD5

        public CRM_LocalInfo()
        {
            m_strMobileNumber = "";
            m_strAppFrom = "VOP-WIN";
            m_strAppVersion = "0.001";
            m_strSign = "";
            m_time = System.DateTime.Now.ToLocalTime();
        }

        public string ConvertToWebParams()
        {
            string str = "";
            try
            {
                SystemInfo systemInfo = new SystemInfo();    //Net card ID
                m_strSign = MD5.MD5_Encrypt(systemInfo.m_strMacAddress + m_time.ToString("yyyyMMddHHmmss") + m_strSignKey);

                str = String.Format("MobileCode={0}&Mobile={1}&DeviceBrand={2}&DeviceModel={3}&appFrom={4}&version={5}&time={6}&sign={7}"
                    , systemInfo.m_strMacAddress, m_strMobileNumber, systemInfo.m_strDeviceBrand, systemInfo.m_strDeviceModel, m_strAppFrom, m_strAppVersion,
                    m_time.ToString("yyyyMMddHHmmss"), m_strSign);
            }
            catch
            {

            }

            return str;
        }
    }

    [Serializable()]
    public class CRM_PrintInfo2
    {
        private readonly string m_strSignKey = "86c02972fba047b0b0a9adb8123029fb";

        public string m_strDeviceCode;    //Net card ID
        public string m_strMobileNumber;  //can not upload
        public string m_strPlatform;
        public string m_strPrintData;
        public string m_strVersion;
        public DateTime m_time;   //yyyyMMddHHmmss, for example:20140219092408
        public string m_strSign; //MobileCode+time+key using MD5

        public CRM_PrintInfo2()
        {
            m_strDeviceCode = SystemInfo.GetMacAddress();
            m_strMobileNumber = "";
            m_strPlatform = "WinPC";
            m_strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            m_time = System.DateTime.Now.ToLocalTime();
        }

        public void AddPrintData(string printerModel, string printerId, string tonerId, uint totalPrint, uint inkLevel = 0)
        {
            m_strPrintData = String.Format("[{{\"printermodel\":\"{0}\",\"printerid\":\"{1}\",\"tonerid\":\"{2}\",\"totalprint\":\"{3}\",\"inklevel\":\"{4}\"}}]",
                                              printerModel, printerId, tonerId, totalPrint.ToString(), inkLevel.ToString());
        }

        public string ConvertToWebParams()
        {
            string str = "";
            string format = "";

            m_strSign = MD5.MD5_Encrypt(m_strDeviceCode + m_time.ToString("yyyyMMddHHmmss") + m_strSignKey);

            format = "{{" +
                     "\"devicecode\":\"{0}\"," +
                     "\"mobile\":\"{1}\"," +
                     "\"platform\":\"{2}\"," +
                     "\"version\":\"{3}\"," +
                     "\"time\":\"{4}\"," +
                     "\"sign\":\"{5}\"," +
                     "\"location\":\"\"," +
                     "\"printerdata\":{6}" +
                     "}}";


            str = String.Format(format,
                                m_strDeviceCode,
                                m_strMobileNumber,
                                m_strPlatform,
                                m_strVersion,
                                m_time.ToString("yyyyMMddHHmmss"),
                                m_strSign,
                                m_strPrintData);

            return str;
        }

    }

    public class RequestManager
    {
        public static CookieContainer m_CookieContainer = new CookieContainer();

        public static bool ParseJsonData<T>(string strSrc, JSONReturnFormat rtFormat, ref T record)
        {
            bool bSuccess = false;
            try
            {
                JObject o = JObject.Parse(strSrc);

                if (null != strSrc && null != record)
                {
                    string strValue;
                    switch (rtFormat)
                    {
                        case JSONReturnFormat.JSONResultFormat1:
                            strValue = o.GetValue("response").ToString();
                            ((JSONResultFormat1)(dynamic)record).m_nResponse = Convert.ToInt32(strValue);

                            strValue = o.GetValue("message").ToString();
                            ((JSONResultFormat1)(dynamic)record).m_strMessage = strValue;

                            strValue = o.GetValue("success").ToString();
                            if ("true" == strValue || "True" == strValue)
                                ((JSONResultFormat1)(dynamic)record).m_bSuccess = true;
                            else
                                ((JSONResultFormat1)(dynamic)record).m_bSuccess = false;

                            bSuccess = true;
                            break;
                        case JSONReturnFormat.JSONResultFormat2:
                            strValue = o.GetValue("message").ToString();
                            ((JSONResultFormat2)(dynamic)record).m_strMessage = strValue;

                            strValue = o.GetValue("success").ToString();
                            if ("true" == strValue || "True" == strValue)
                                ((JSONResultFormat2)(dynamic)record).m_bSuccess = true;
                            else
                                ((JSONResultFormat2)(dynamic)record).m_bSuccess = false;

                            bSuccess = true;
                            break;
                  
                        case JSONReturnFormat.SessionInfo:
                            strValue = o.GetValue("totalCount").ToString();
                            ((SessionInfo)(dynamic)record).m_nTotalCount = Convert.ToInt32(strValue);

                            strValue = o.GetValue("detail").ToString();
                            ((SessionInfo)(dynamic)record).m_strDetail = strValue;

                            strValue = o.GetValue("code").ToString();
                            ((SessionInfo)(dynamic)record).m_nErrorCode = Convert.ToInt32(strValue);

                            strValue = o.GetValue("success").ToString();
                            if ("true" == strValue || "True" == strValue)
                                ((SessionInfo)(dynamic)record).m_bSuccess = true;
                            else
                                ((SessionInfo)(dynamic)record).m_bSuccess = false;

                            strValue = o.GetValue("addon").ToString();
                            {
                                JObject jSub = JObject.Parse(strValue);
                                string strItemValue = jSub.GetValue("merchant_id").ToString().Trim(new char[] { '\r', '\n', ' ' });
                                ((SessionInfo)(dynamic)record).m_nMerchantID = Convert.ToInt32(strItemValue);

                                strItemValue = jSub.GetValue("customer").ToString().Trim(new char[] { '\r', '\n', ' ' });
                                jSub = JObject.Parse(strItemValue);
                                strItemValue = jSub.GetValue("customer_id").ToString().Trim(new char[] { '\r', '\n', ' ' });
                                ((SessionInfo)(dynamic)record).m_nCustomerID = Convert.ToInt32(strItemValue);
                            }

                            bSuccess = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
              
            }

            return bSuccess;
        }

        public static bool SendHttpWebRequest<T>(string url, string httpRequestMtd, string strParam, JSONReturnFormat rtFormat, ref T record, ref string strResult)
        {
            bool bSuccess = false;
            try
            {
                Trace.WriteLine(String.Format("HttpRequest {0} {1}", url, strParam));
              //  byte[] byteArray = Encoding.UTF8.GetBytes(strParam); 
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url); 
                IWebProxy webProxy = WebRequest.DefaultWebProxy;
                if (null != webProxy)
                {
                    webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                    request.Proxy = webProxy;
                }
                request.CookieContainer = m_CookieContainer;
                request.Method = httpRequestMtd;
                request.ContentType = "application/json; charset=UTF-8";      
                request.Credentials = CredentialCache.DefaultCredentials;
              //  request.ContentLength = byteArray.Length;
                StreamWriter newStream = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);

                newStream.Write(strParam);    
                newStream.Flush();
                newStream.Close();

                WebResponse response = (WebResponse)request.GetResponse();

                StreamReader sr2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string text2 = sr2.ReadToEnd();
                Trace.WriteLine(String.Format("HttpResponse {0} {1}", url, text2));
                strResult = text2;
                if (ParseJsonData<T>(text2, rtFormat, ref record))
                {
                    bSuccess = true;
                }

                response.Close();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                bSuccess = false;
            }

            return bSuccess;
        }

        public static bool GetSession(ref SessionInfo rtValue)
        {
            bool bSuccess = false;
            string url = "http://o2o.iprintworks.cn/api/common/saveCustomer";
            string strCMD = "customer_id=105";
            string strResult = "";

            if (SendHttpWebRequest<SessionInfo>(url, "POST", strCMD, JSONReturnFormat.SessionInfo, ref rtValue, ref strResult))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public static bool UploadCRM_PrintInfoToServer(ref CRM_PrintInfo _PrintInfo, ref JSONResultFormat2 rtValue)
        {
            bool bSuccess = false;
            //http://crm.iprintworks.cn/api/app_print
            string url = "http://crm.iprintworks.cn/api/app_print";//debug 
            string strCMD = _PrintInfo.ConvertToWebParams();
            string strResult = "";

            if (SendHttpWebRequest<JSONResultFormat2>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat2, ref rtValue, ref strResult))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public static bool UploadCRM_LocalInfoToServer(ref CRM_LocalInfo _lci, ref JSONResultFormat2 rtValue)
        {
            bool bSuccess = false;
            //http://crm.iprintworks.cn/api/app_open
            string url = "http://crm.iprintworks.cn/api/app_open";//debug 
            string strCMD = _lci.ConvertToWebParams();
            string strResult = "";

            if (SendHttpWebRequest<JSONResultFormat2>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat2, ref rtValue, ref strResult))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public static bool UploadCRM_PrintInfo2ToServer(CRM_PrintInfo2 _PrintInfo, ref JSONResultFormat2 rtValue)
        {
            bool bSuccess = false;
            string url = "http://crm.iprintworks.cn/api/printerinfo";
            string strCMD = _PrintInfo.ConvertToWebParams();
            string strResult = "";

            if (SendHttpWebRequest<JSONResultFormat2>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat2, ref rtValue, ref strResult))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public static bool Serialize<T>(T value, String filename)
        {
            if (value == null)
            {
                return false;
            }
            try
            {
                XmlSerializer _xmlserializer = new XmlSerializer(typeof(T));
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

        public static bool Deserialize<T>(String filename, ref T returnObj)
        {
            if (string.IsNullOrEmpty(filename))
            {
                returnObj = default(T);
                return false;
            }
            try
            {
                XmlSerializer _xmlSerializer = new XmlSerializer(typeof(T));
                TextReader stream = new StreamReader(filename);
                var result = (T)_xmlSerializer.Deserialize(stream);
                stream.Close();
                returnObj = result;
                return true;
            }
            catch (Exception)
            {
                returnObj = default(T);
                return false;
            }
        }
    }
}
