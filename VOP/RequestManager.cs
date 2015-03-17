using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VOP
{
    enum JSONReturnFormat
    {
        JSONResultFormat1 = 1,
        JSONResultFormat2,
        MerchantInfoSet,
        ServiceStationInfoSet,
        SessionInfo,
    };

    class SessionInfo
    {
        public Int32    m_nTotalCount;
        public string   m_strDetail;
        public Int32    m_nErrorCode;
        public bool     m_bSuccess;
        public Int32    m_nCustomerID;
        public Int32    m_nMerchantID;
    }

    class MerchantInfo
    {
        public Int32    m_nID;
        public Int32    m_nUserID;
        public string   m_strCode;
        public string   m_strName;
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

    class MerchantInfoSet
    {
        public string   m_strItems;
        public Int32    m_nTotalCount;
        public string   m_strDetail;
        public Int32    m_nErrorCode;
        public bool     m_bSuccess;
        public string   m_strAddon;

        public List<MerchantInfo> m_listMerchantInfo = new List<MerchantInfo>();
    }

    class ServiceStationInfo
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

    class ServiceStationInfoSet
    {
        public string   m_strItems;
        public Int32    m_nTotalCount;
        public string   m_strDetail;
        public Int32    m_nErrorCode;
        public bool     m_bSuccess;
        public string   m_strAddon;

        public List<ServiceStationInfo> m_listServiceStationInfo = new List<ServiceStationInfo>();
    }
    class JSONResultFormat2
    {
        public string   m_strMessage;
        public bool     m_bSuccess;
    }

    class JSONResultFormat1 //Normal
    {
        public Int32    m_nResponse;
        public string   m_strMessage;
        public bool     m_bSuccess;
    }
    class RequestManager
    {
        public static CookieContainer m_CookieContainer = new CookieContainer();

        bool ParseJsonData<T>(string strSrc, JSONReturnFormat rtFormat, ref T record)
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
                                MerchantInfo merchantInfo = new MerchantInfo();
                                JArray ja = (JArray)JsonConvert.DeserializeObject(strValue);

                                string strItemValue = ja[nIdx]["merchant_id"].ToString();
                                merchantInfo.m_nID = Convert.ToInt32(strItemValue);

                                strItemValue = ja[nIdx]["user_id"].ToString();
                                merchantInfo.m_nUserID = Convert.ToInt32(strItemValue);

                                merchantInfo.m_strCode = ja[nIdx]["merchant_code"].ToString();
                                merchantInfo.m_strName = ja[nIdx]["merchant_name"].ToString();
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

                            bSuccess = true;
                            break;
                        case JSONReturnFormat.ServiceStationInfoSet:
                            strValue = o.GetValue("totalCount").ToString();
                            ((ServiceStationInfoSet)(dynamic)record).m_nTotalCount = Convert.ToInt32(strValue);

                            strValue = o.GetValue("detail").ToString();
                            ((ServiceStationInfoSet)(dynamic)record).m_strDetail = strValue;

                            strValue = o.GetValue("code").ToString();
                            ((ServiceStationInfoSet)(dynamic)record).m_nErrorCode = Convert.ToInt32(strValue);

                            strValue = o.GetValue("success").ToString();
                            if ("true" == strValue || "True" == strValue)
                                ((ServiceStationInfoSet)(dynamic)record).m_bSuccess = true;
                            else
                                ((ServiceStationInfoSet)(dynamic)record).m_bSuccess = false;

                            strValue = o.GetValue("addon").ToString();
                            ((ServiceStationInfoSet)(dynamic)record).m_strAddon = strValue;

                            strValue = o.GetValue("items").ToString();
                            ((ServiceStationInfoSet)(dynamic)record).m_strItems = strValue;

                            for (Int32 nIdx = 0; nIdx < ((ServiceStationInfoSet)(dynamic)record).m_nTotalCount; nIdx++)
                            {
                                ServiceStationInfo serviceStationInfo = new ServiceStationInfo();
                                JArray ja = (JArray)JsonConvert.DeserializeObject(strValue);

                                string strItemValue = ja[nIdx]["service_station_id"].ToString();
                                serviceStationInfo.m_nID = Convert.ToInt32(strItemValue);

                                strItemValue = ja[nIdx]["user_id"].ToString();
                                serviceStationInfo.m_nUserID = Convert.ToInt32(strItemValue);
                                serviceStationInfo.m_strName = ja[nIdx]["service_station_name"].ToString();
                                serviceStationInfo.m_strProvince = ja[nIdx]["service_station_province"].ToString();
                                serviceStationInfo.m_strCity = ja[nIdx]["service_station_city"].ToString();

                                serviceStationInfo.m_strAddress = ja[nIdx]["service_station_address"].ToString();
                                serviceStationInfo.m_strProductLine = ja[nIdx]["service_station_product_line"].ToString();
                                serviceStationInfo.m_strPhone = ja[nIdx]["service_station_phone"].ToString();
                                serviceStationInfo.m_strHours = ja[nIdx]["service_station_hours"].ToString();
                                serviceStationInfo.m_strRemark = ja[nIdx]["service_station_remark"].ToString();
                                serviceStationInfo.m_strLongitude = ja[nIdx]["service_station_longitude"].ToString();
                                serviceStationInfo.m_strLatitude = ja[nIdx]["service_station_latitude"].ToString();

                                serviceStationInfo.m_strStatus = ja[nIdx]["service_station_status"].ToString();
                                serviceStationInfo.m_strTimeCreate = ja[nIdx]["service_station_time_create"].ToString();

                                ((ServiceStationInfoSet)(dynamic)record).m_listServiceStationInfo.Add(serviceStationInfo);
                            }

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

            }

            return bSuccess;
        }

        bool SendHttpWebRequest<T>(string url, string httpRequestMtd, string strBuf, JSONReturnFormat rtFormat, ref T record)
        {
            bool bSuccess = false;
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(strBuf); // 转化
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);  //新建一个WebRequest对象用来请求或者响应url
                IWebProxy webProxy = WebRequest.DefaultWebProxy;
                webProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.Proxy = webProxy;

                request.CookieContainer = m_CookieContainer;

                request.Method = httpRequestMtd;                                          //请求方式是POST
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";       //请求的内容格式为application/x-www-form-urlencoded
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentLength = byteArray.Length;
                Stream newStream = request.GetRequestStream();           //返回用于将数据写入 Internet 资源的 Stream。

                newStream.Write(byteArray, 0, byteArray.Length);    //写入参数
                newStream.Flush();
                newStream.Close();

                WebResponse response2 = (WebResponse)request.GetResponse();

                StreamReader sr2 = new StreamReader(response2.GetResponseStream(), Encoding.UTF8);

                string text2 = sr2.ReadToEnd();
                if (ParseJsonData<T>(text2, rtFormat, ref record))
                {
                    bSuccess = true;
                }
            }
            catch
            {
                bSuccess = false;
            }

            return bSuccess;
        }

        public bool SendVerifyCode(string strPhoneNumber, ref JSONResultFormat1 rtValue)
        {
            bool bSuccess = false;
            string url = "http://function.iprintworks.cn:8001/smsauth/mt_u.php";
            string strCMD = "phoneNum=" + strPhoneNumber;

            if (SendHttpWebRequest<JSONResultFormat1>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat1, ref rtValue))
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

            if (SendHttpWebRequest<JSONResultFormat1>(url, "POST", strCMD, JSONReturnFormat.JSONResultFormat1, ref rtValue))
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

            if (SendHttpWebRequest<SessionInfo>(url, "POST", strCMD, JSONReturnFormat.SessionInfo, ref rtValue))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public bool GetMerchantSet(Int32 nStart, Int32 nLimit, ref MerchantInfoSet rtValue)
        {
            bool bSuccess = false;
            string url = "http://o2o.iprintworks.cn/api/data/getMerchantList";
            string strCMD = String.Format("start={0}&limit={1}", nStart, nLimit);

            if (SendHttpWebRequest<MerchantInfoSet>(url, "POST", strCMD, JSONReturnFormat.MerchantInfoSet, ref rtValue))
            {
                if (rtValue.m_bSuccess)
                {
                    bSuccess = true;
                }
            }

            return bSuccess;
        }

        public bool GetServiceStationSet(Int32 nStart, Int32 nLimit, ref ServiceStationInfoSet rtValue)
        {
            bool bSuccess = false;
            string url = "http://o2o.iprintworks.cn/api/data/getServiceStationList";//请求登录的URL
            string strCMD = String.Format("start={0}&limit={1}", nStart, nLimit);

            if (SendHttpWebRequest<ServiceStationInfoSet>(url, "POST", strCMD, JSONReturnFormat.ServiceStationInfoSet, ref rtValue))
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
