﻿using System;
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

namespace VOP
{
    public enum JSONReturnFormat
    {
        JSONResultFormat1 = 1,
        JSONResultFormat2,
        MerchantInfoSet,
        MaintainInfoSet,
        SessionInfo,
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

    public class MerchantInfoItem
    {
        public Int32    m_nID;
        public Int32    m_nUserID;
        public string   m_strCode;
        public string   m_strCompanyName;
        public string   m_strMail;
        public string   m_strImagePath;
        public string   m_strPassword;
        public string   m_strType;
        public string   m_strProductCategory;
        public string   m_strPhone;
        public string   m_strContact;
        public string   m_strProvince;
        public string   m_strCity;
        public string   m_strDistrict;
        public string   m_strAddress;
        public string   m_strLongitude;
        public string   m_strLatitude;
        public string   m_strDesctiption;
        public string   m_strContactName;
        public string   m_strContactPhone;
        public string   m_strRemark;
        public string   m_strConfigFreight;
        public string   m_strStatus;
        public string   m_strStatusChange;
        public string   m_strTimeCreate;
        public string   m_strTimeLastModify;
    };

    public class MerchantInfoSet
    {
        public string   m_strItems;
        public Int32    m_nTotalCount;
        public string   m_strDetail;
        public Int32    m_nErrorCode;
        public bool     m_bSuccess;
        public string   m_strAddon;

        public List<MerchantInfoItem> m_listMerchantInfo = new List<MerchantInfoItem>();

        public void Clear()
        {
            m_strItems = "";
            m_nTotalCount = 0;
            m_strDetail = "";
            m_nErrorCode = 0;
            m_bSuccess = false;
            m_strAddon = "";
            m_listMerchantInfo.Clear();
        }
    }

    public class MaintainInfoItem
    {
        public Int32    m_nID;
        public Int32    m_nUserID;
        public string   m_strName;
        public string   m_strProvince;
        public string   m_strCity;
        public string   m_strAddress;
        public string   m_strProductLine;
        public string   m_strPhone;
        public string   m_strHours;
        public string   m_strRemark;
        public string   m_strLongitude;
        public string   m_strLatitude;
        public string   m_strStatus;
        public string   m_strTimeCreate;
    };

    public class MaintainInfoSet
    {
        public string   m_strItems;
        public Int32    m_nTotalCount;
        public string   m_strDetail;
        public Int32    m_nErrorCode;
        public bool     m_bSuccess;
        public string   m_strAddon;

        public List<MaintainInfoItem> m_listMaintainInfo = new List<MaintainInfoItem>();

        public void Clear()
        {
            m_strItems = "";
            m_nTotalCount = 0;
            m_strDetail = "";
            m_nErrorCode = 0;
            m_bSuccess = false;
            m_strAddon = "";
            m_listMaintainInfo.Clear();
        }
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
        private string m_strPrintType;   //always "VOP-WIN"
        private string m_strPrintMode;    //alway "VOP-WIN"
        public string m_strPrintDocType;
        public string m_strPrintCopys;
        public string m_strPrintPages;
        public string m_strPrinterModel;
        public string m_strPrinterName;
        public string m_strPrinterType;
        public string m_strPrintSuccess;   //alway true
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
            m_strPrinterModel = "Lenovo ABC";
            m_strPrinterName = "Lenovo ABC Printer";
            m_strPrinterType = "SFP";
            m_strPrintSuccess = "true";

            m_time = System.DateTime.Now.ToLocalTime();
        }

        public string ConvertToWebParams()
        {
            string str = "";
            try
            {
                m_strSign = MD5.MD5_Encrypt(m_strMobileCode + m_time.ToString("yyyyMMddHHmmss") + m_strSignKey);

                str = String.Format("MobileCode={0}&Mobile={1}&DeviceBrand={2}&DeviceModel={3}&PrintType={4}&PrintMode={5}&PrintDocType={6}&PrintCopys={7}&PrintPages={8}&PrinterModel={9}&PrinterName={10}&PrinterType={11}&IsSuccess={12}&time={13}&sign={14}"
                    , m_strMobileCode, m_strMobileNumber, m_strDeviceBrand, m_strDeviceModel, m_strPrintType, m_strPrintMode, m_strPrintDocType,
                    m_strPrintCopys, m_strPrintPages, m_strPrinterModel, m_strPrinterName, m_strPrinterType, m_strPrintSuccess, m_time.ToString("yyyyMMddHHmmss"), m_strSign
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

        public string m_strMobileCode;    //Net card ID
        public string m_strMobileNumber;  //can not upload
        public string m_strDeviceBrand;
        public string m_strDeviceModel;
        public string m_strAppFrom; //always "VOP-WIN"
        public string m_strAppVersion;
        public DateTime m_time;   //yyyyMMddHHmmss, for example:20140219092408
        public string m_strSign; //MobileCode+time+key using MD5

        public CRM_LocalInfo()
        {
            m_strMobileCode = SystemInfo.GetMacAddress();
            m_strMobileNumber = "";
            m_strDeviceBrand = "";
            m_strDeviceModel = "";
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
                m_strSign = MD5.MD5_Encrypt(m_strMobileCode + m_time.ToString("yyyyMMddHHmmss") + m_strSignKey);

                str = String.Format("MobileCode={0}&Mobile={1}&DeviceBrand={2}&DeviceModel={3}&appFrom={4}&version={5}&time={6}&sign={7}"
                    , m_strMobileCode, m_strMobileNumber, m_strDeviceBrand, m_strDeviceModel, m_strAppFrom, m_strAppVersion,
                    m_time.ToString("yyyyMMddHHmmss"), m_strSign);
            }
            catch
            {

            }

            return str;
        }
    }

    public class RequestManager
    {
        public static CookieContainer m_CookieContainer = new CookieContainer();

        public bool  ParseJsonData<T>(string strSrc, JSONReturnFormat rtFormat, ref T record)
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
                        case JSONReturnFormat.MerchantInfoSet:
                            strValue = o.GetValue("totalCount").ToString();
                            ((MerchantInfoSet)(dynamic)record).m_nTotalCount = Convert.ToInt32(strValue);

                            strValue = o.GetValue("detail").ToString();
                            ((MerchantInfoSet)(dynamic)record).m_strDetail = strValue;

                            strValue = o.GetValue("code").ToString();
                            ((MerchantInfoSet)(dynamic)record).m_nErrorCode = Convert.ToInt32(strValue);

                            strValue = o.GetValue("success").ToString();
                            if ("true" == strValue || "True" == strValue)
                                ((MerchantInfoSet)(dynamic)record).m_bSuccess = true;
                            else
                                ((MerchantInfoSet)(dynamic)record).m_bSuccess = false;

                            strValue = o.GetValue("addon").ToString();
                            ((MerchantInfoSet)(dynamic)record).m_strAddon = strValue;

                            strValue = o.GetValue("items").ToString();
                            ((MerchantInfoSet)(dynamic)record).m_strItems = strValue;

                            for (Int32 nIdx = 0; nIdx < ((MerchantInfoSet)(dynamic)record).m_nTotalCount; nIdx++)
                            {
                                MerchantInfoItem merchantInfo = new MerchantInfoItem();
                                JArray ja = (JArray)JsonConvert.DeserializeObject(strValue);

                                string strItemValue = ja[nIdx]["merchant_id"].ToString();
                                merchantInfo.m_nID = Convert.ToInt32(strItemValue);

                                strItemValue = ja[nIdx]["user_id"].ToString();
                                merchantInfo.m_nUserID = Convert.ToInt32(strItemValue);

                                merchantInfo.m_strCode = ja[nIdx]["merchant_code"].ToString();
                                merchantInfo.m_strCompanyName = ja[nIdx]["merchant_name"].ToString();
                                merchantInfo.m_strMail = ja[nIdx]["merchant_email"].ToString();
                                merchantInfo.m_strImagePath = ja[nIdx]["merchant_image"].ToString();
                                merchantInfo.m_strPassword = ja[nIdx]["merchant_password"].ToString();

                                merchantInfo.m_strType = ja[nIdx]["merchant_type"].ToString();
                                merchantInfo.m_strProductCategory = ja[nIdx]["merchant_product_category"].ToString();
                                merchantInfo.m_strPhone = ja[nIdx]["merchant_phone"].ToString();
                                merchantInfo.m_strContact = ja[nIdx]["merchant_contact"].ToString();
                                merchantInfo.m_strProvince = ja[nIdx]["merchant_province"].ToString();

                                merchantInfo.m_strCity = ja[nIdx]["merchant_city"].ToString();
                                merchantInfo.m_strDistrict = ja[nIdx]["merchant_district"].ToString();
                                merchantInfo.m_strAddress = ja[nIdx]["merchant_address"].ToString();
                                merchantInfo.m_strLongitude = ja[nIdx]["merchant_longitude"].ToString();
                                merchantInfo.m_strLatitude = ja[nIdx]["merchant_latitude"].ToString();

                                merchantInfo.m_strDesctiption = ja[nIdx]["merchant_desc"].ToString();
                                merchantInfo.m_strContactName = ja[nIdx]["merchant_contact_name"].ToString();
                                merchantInfo.m_strContactPhone = ja[nIdx]["merchant_contact_phone"].ToString();
                                merchantInfo.m_strRemark = ja[nIdx]["merchant_remark"].ToString();
                                merchantInfo.m_strConfigFreight = ja[nIdx]["merchant_config_freight"].ToString();

                                merchantInfo.m_strStatus = ja[nIdx]["merchant_status"].ToString();
                                merchantInfo.m_strStatusChange = ja[nIdx]["merchant_status_change"].ToString();
                                merchantInfo.m_strTimeCreate = ja[nIdx]["merchant_time_create"].ToString();
                                merchantInfo.m_strTimeLastModify = ja[nIdx]["merchant_time_lastmodify"].ToString();

                                ((MerchantInfoSet)(dynamic)record).m_listMerchantInfo.Add(merchantInfo);
                            }

                            if (((MerchantInfoSet)(dynamic)record).m_nTotalCount > 0)
                                bSuccess = true;
                            break;
                        case JSONReturnFormat.MaintainInfoSet:
                            strValue = o.GetValue("totalCount").ToString();
                            ((MaintainInfoSet)(dynamic)record).m_nTotalCount = Convert.ToInt32(strValue);

                            strValue = o.GetValue("detail").ToString();
                            ((MaintainInfoSet)(dynamic)record).m_strDetail = strValue;

                            strValue = o.GetValue("code").ToString();
                            ((MaintainInfoSet)(dynamic)record).m_nErrorCode = Convert.ToInt32(strValue);

                            strValue = o.GetValue("success").ToString();
                            if ("true" == strValue || "True" == strValue)
                                ((MaintainInfoSet)(dynamic)record).m_bSuccess = true;
                            else
                                ((MaintainInfoSet)(dynamic)record).m_bSuccess = false;

                            strValue = o.GetValue("addon").ToString();
                            ((MaintainInfoSet)(dynamic)record).m_strAddon = strValue;

                            strValue = o.GetValue("items").ToString();
                            ((MaintainInfoSet)(dynamic)record).m_strItems = strValue;

                            for (Int32 nIdx = 0; nIdx < ((MaintainInfoSet)(dynamic)record).m_nTotalCount; nIdx++)
                            {
                                MaintainInfoItem maintainInfoItem = new MaintainInfoItem();
                                JArray ja = (JArray)JsonConvert.DeserializeObject(strValue);

                                string strItemValue = ja[nIdx]["service_station_id"].ToString();
                                maintainInfoItem.m_nID = Convert.ToInt32(strItemValue);

                                strItemValue = ja[nIdx]["user_id"].ToString();
                                maintainInfoItem.m_nUserID = Convert.ToInt32(strItemValue);
                                maintainInfoItem.m_strName = ja[nIdx]["service_station_name"].ToString();
                                maintainInfoItem.m_strProvince = ja[nIdx]["service_station_province"].ToString();
                                maintainInfoItem.m_strCity = ja[nIdx]["service_station_city"].ToString();

                                maintainInfoItem.m_strAddress = ja[nIdx]["service_station_address"].ToString();
                                maintainInfoItem.m_strProductLine = ja[nIdx]["service_station_product_line"].ToString();
                                maintainInfoItem.m_strPhone = ja[nIdx]["service_station_phone"].ToString();
                                maintainInfoItem.m_strHours = ja[nIdx]["service_station_hours"].ToString();
                                maintainInfoItem.m_strRemark = ja[nIdx]["service_station_remark"].ToString();
                                maintainInfoItem.m_strLongitude = ja[nIdx]["service_station_longitude"].ToString();
                                maintainInfoItem.m_strLatitude = ja[nIdx]["service_station_latitude"].ToString();

                                maintainInfoItem.m_strStatus = ja[nIdx]["service_station_status"].ToString();
                                maintainInfoItem.m_strTimeCreate = ja[nIdx]["service_station_time_create"].ToString();

                                ((MaintainInfoSet)(dynamic)record).m_listMaintainInfo.Add(maintainInfoItem);
                            }
                            if (((MaintainInfoSet)(dynamic)record).m_nTotalCount > 0)
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
                                string strItemValue = jSub.GetValue("merchant_id").ToString();
                                ((SessionInfo)(dynamic)record).m_nMerchantID = Convert.ToInt32(strItemValue);

                                strItemValue = jSub.GetValue("customer").ToString();
                                jSub = JObject.Parse(strItemValue);
                                strItemValue = jSub.GetValue("customer_id").ToString();
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
              switch (rtFormat)
              {
                  case JSONReturnFormat.MerchantInfoSet:
                      if (((MerchantInfoSet)(dynamic)record).m_bSuccess)
                          bSuccess = true;
                      break;
                  case JSONReturnFormat.MaintainInfoSet:
                      if (((MaintainInfoSet)(dynamic)record).m_bSuccess)
                          bSuccess = true;
                      break;
                  default:
                      break;
              }
            }

            return bSuccess;
        }

        public bool SendHttpWebRequest<T>(string url, string httpRequestMtd, string strParam, JSONReturnFormat rtFormat, ref T record, ref string strResult)
        {
            bool bSuccess = false;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(strParam); // 转化
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  //新建一个WebRequest对象用来请求或者响应url
                dll.OutputDebugStringToFile_("####### SendHttpWebRequest WebRequest.Create(url) ######");
                IWebProxy webProxy = WebRequest.DefaultWebProxy;
                if (null != webProxy)
                {
                    dll.OutputDebugStringToFile_("####### WebRequest.DefaultWebProxy ######");
                    webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                    dll.OutputDebugStringToFile_("####### DefaultNetworkCredentials ######");
                    request.Proxy = webProxy;
                    dll.OutputDebugStringToFile_("####### webProxy ######");
                }
                request.CookieContainer = m_CookieContainer;
                dll.OutputDebugStringToFile_("####### m_CookieContainer ######");

                request.Method = httpRequestMtd;                                          //请求方式是POST
                dll.OutputDebugStringToFile_("####### httpRequestMtd ######");
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";       //请求的内容格式为application/x-www-form-urlencoded
                dll.OutputDebugStringToFile_("####### request.ContentType ######");
                request.Credentials = CredentialCache.DefaultCredentials;
                dll.OutputDebugStringToFile_("####### request.Credentials ######");
                request.ContentLength = byteArray.Length;
                dll.OutputDebugStringToFile_("####### request.ContentLength ######");
                Stream newStream = request.GetRequestStream();           //返回用于将数据写入 Internet 资源的 Stream。
                dll.OutputDebugStringToFile_("####### request.GetRequestStream() ######");

                newStream.Write(byteArray, 0, byteArray.Length);    //写入参数
                newStream.Flush();
                newStream.Close();
                dll.OutputDebugStringToFile_("####### Stream.Close() ######");

                WebResponse response = (WebResponse)request.GetResponse();
                dll.OutputDebugStringToFile_("####### request.GetResponse() ######");

                StreamReader sr2 = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string text2 = sr2.ReadToEnd();
                dll.OutputDebugStringToFile_(text2);
                strResult = text2;
                if (ParseJsonData<T>(text2, rtFormat, ref record))
                {
                    bSuccess = true;
                }

                response.Close();
            }
            catch (Exception ex)
            {
                dll.OutputDebugStringToFile_(ex.Message);
                bSuccess = false;
            }

            return bSuccess;
        }

        public bool SendVerifyCode(string strPhoneNumber, ref JSONResultFormat1 rtValue)
        {
            bool bSuccess = false;
            string url = "http://function.iprintworks.cn:8001/smsauth/mt_u.php";
            string strCMD = "phoneNum=" + strPhoneNumber;
            string strResult = "";

            if (SendHttpWebRequest<JSONResultFormat1>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat1, ref rtValue, ref strResult))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;//*/
        }

        public bool CheckVerifyCode(string strPhoneNumber, string strVerifyCode, ref JSONResultFormat1 rtValue)
        {
            bool bSuccess = false;
            string url = "http://function.iprintworks.cn:8001/smsauth/authCode.php";
            string strCMD = "phoneNum=" + strPhoneNumber + "&authCode=" + strVerifyCode;
            string strResult = "";

            if (SendHttpWebRequest<JSONResultFormat1>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat1, ref rtValue, ref strResult) || rtValue.m_bSuccess)
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }
            
            return bSuccess;
        }

        public bool GetSession(ref SessionInfo rtValue)
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

        public bool GetMerchantSet(Int32 nStart, Int32 nLimit, ref MerchantInfoSet rtValue, ref string strResult )
        {
            bool bSuccess = false;
            string url = "http://o2o.iprintworks.cn/api/data/getMerchantList";
            string strCMD = String.Format("start={0}&limit={1}", nStart, nLimit);
            strResult = "";
           
            int nCount = 2;
            while (nCount-- > 0)
            {
                rtValue.m_bSuccess = bSuccess = false;
                
                if (SendHttpWebRequest<MerchantInfoSet>(url, "POST", strCMD, JSONReturnFormat.MerchantInfoSet, ref rtValue, ref strResult))
                {
                    if (rtValue.m_bSuccess)
                    {
                        bSuccess = true;
                    }
                }

                if ((!bSuccess) && (!rtValue.m_bSuccess) && m_CookieContainer.Count == 0)
                {
                    SessionInfo session = new SessionInfo();
                    GetSession(ref session);
                }
                else
                {
                    break;
                }
            }

            return bSuccess;
        }

        public bool GetMaintainInfoSet(Int32 nStart, Int32 nLimit, ref MaintainInfoSet rtValue, ref string strResult)
        {
            bool bSuccess = false;
            string url = "http://o2o.iprintworks.cn/api/data/getServiceStationList";//请求登录的URL
            string strCMD = String.Format("start={0}&limit={1}", nStart, nLimit);
            strResult = "";
            int nCount = 2;
            while(nCount-- > 0)
            {
                rtValue.m_bSuccess = bSuccess = false;
                if (SendHttpWebRequest<MaintainInfoSet>(url, "POST", strCMD, JSONReturnFormat.MaintainInfoSet, ref rtValue, ref strResult) || rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }

                if ((!bSuccess) && (!rtValue.m_bSuccess) && rtValue.m_nErrorCode == 401)
                {
                    SessionInfo session = new SessionInfo();
                    GetSession(ref session);
                }
                else
                {
                    break;
                }
            }
            

            return bSuccess;
        }

        public bool UploadCRM_PrintInfoToServer(ref CRM_PrintInfo _PrintInfo, ref JSONResultFormat2 rtValue)
        {
            bool bSuccess = false;
            //http://crm.iprintworks.cn/api/app_print
            string url = "http://123.57.255.92/api/app_print";//debug 
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

        public bool UploadCRM_LocalInfoToServer(ref CRM_LocalInfo _lci, ref JSONResultFormat2 rtValue)
        {
            bool bSuccess = false;
            //http://crm.iprintworks.cn/api/app_open
            string url = "http://123.57.255.92/api/app_open";//debug 
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
    }
}
