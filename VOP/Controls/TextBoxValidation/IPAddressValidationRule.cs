using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Controls;
using System.Net;
using System.Net.Sockets;

namespace VOP.Controls
{
    public class IPAddressValidationRule : ValidationRule
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value,
                CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            string inputString = (value ?? string.Empty).ToString();

            IPAddress ipAddress;
            IPAddress.TryParse(inputString, out ipAddress);
            if (null != ipAddress)
            {
                int nVal0 = -1;
                int nVal1 = -1;
                int nVal2 = -1;
                int nVal3 = -1;

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string strIP = ipAddress.ToString();
                    if (null != strIP && 0 < strIP.Length)
                    {
                        var parts_ip = strIP.Split('.');
                        try
                        {
                            nVal0 = Convert.ToInt32(parts_ip[0]);
                            nVal1 = Convert.ToInt32(parts_ip[1]);
                            nVal2 = Convert.ToInt32(parts_ip[2]);
                            nVal3 = Convert.ToInt32(parts_ip[3]);

                            if (false == (0 <= nVal0 && 224 > nVal0
                                    && 0 <= nVal1 && 255 >= nVal1
                                    && 0 <= nVal2 && 255 >= nVal2
                                    && 0 <= nVal3 && 255 >= nVal3))
                            {
                                result = new ValidationResult(false, this.ErrorMessage);
                            }
                            else
                            {
                                if(nVal0 == 127)
                                {
                                    result = new ValidationResult(false, this.ErrorMessage);
                                }
                            }
                        }
                        catch
                        {
                            result = new ValidationResult(false, this.ErrorMessage);
                        }
                    }
                    else
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                }
            }
            else
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }
            return result;
        }
    }

    public class GatewayValidationRule : ValidationRule
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value,
                CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            string inputString = (value ?? string.Empty).ToString();

            IPAddress ipAddress;
            IPAddress.TryParse(inputString, out ipAddress);
            if (null != ipAddress)
            {
                int nVal0 = -1;
                int nVal1 = -1;
                int nVal2 = -1;
                int nVal3 = -1;

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string strIP = ipAddress.ToString();
                    if (null != strIP && 0 < strIP.Length)
                    {
                        var parts_ip = strIP.Split('.');
                        try
                        {
                            nVal0 = Convert.ToInt32(parts_ip[0]);
                            nVal1 = Convert.ToInt32(parts_ip[1]);
                            nVal2 = Convert.ToInt32(parts_ip[2]);
                            nVal3 = Convert.ToInt32(parts_ip[3]);

                            if (false == (0 <= nVal0 && 224 > nVal0
                                    && 0 <= nVal1 && 255 >= nVal1
                                    && 0 <= nVal2 && 255 >= nVal2
                                    && 0 <= nVal3 && 255 >= nVal3))
                            {
                                result = new ValidationResult(false, this.ErrorMessage);
                            }
                            else
                            {
                                if (nVal0 == 127)
                                {
                                    result = new ValidationResult(false, this.ErrorMessage);
                                }
                            }
                        }
                        catch
                        {
                            result = new ValidationResult(false, this.ErrorMessage);
                        }
                    }
                    else
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                }
            }
            else
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }
            return result;
        }
    }

    public class SubmaskValidationRule : ValidationRule
    {
        private string _errorMessage;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public override ValidationResult Validate(object value,
                CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);
            string inputString = (value ?? string.Empty).ToString();

            IPAddress ipAddress;
            IPAddress.TryParse(inputString, out ipAddress);
            if (null != ipAddress)
            {
                int nVal0 = -1;
                int nVal1 = -1;
                int nVal2 = -1;
                int nVal3 = -1;

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string strIP = ipAddress.ToString();
                    if (null != strIP && 0 < strIP.Length)
                    {
                        var parts_ip = strIP.Split('.');
                        try
                        {
                            nVal0 = Convert.ToInt32(parts_ip[0]);
                            nVal1 = Convert.ToInt32(parts_ip[1]);
                            nVal2 = Convert.ToInt32(parts_ip[2]);
                            nVal3 = Convert.ToInt32(parts_ip[3]);

                            if (false == (0 <= nVal0 && 255 >= nVal0
                                    && 0 <= nVal1 && 255 >= nVal1
                                    && 0 <= nVal2 && 255 >= nVal2
                                    && 0 <= nVal3 && 255 >= nVal3))
                            {
                                result = new ValidationResult(false, this.ErrorMessage);
                            }
                            else
                            {
                                Int32 nIP;
                                nIP = nVal3 | (nVal2 << 8) | (nVal1 << 16) | (nVal0 << 24);
                                int nIdx = 0;
                                for (; nIdx < 32; nIdx++)
                                {
                                    if (0 != (nIP & (0x80000000 >> nIdx)))
                                        continue;
                                    else
                                        break;
                                }
                               
                                if (nIdx < 32)
                                {
                                    for (int nIdx1 = 0; nIdx1 < (32 - nIdx); nIdx1++)
                                    {
                                        if (0 != (nIP & (0x80000000 >> (nIdx + nIdx1))))
                                        {
                                            result = new ValidationResult(false, this.ErrorMessage);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    result = new ValidationResult(false, this.ErrorMessage);
                                }
                            }
                        }
                        catch
                        {
                            result = new ValidationResult(false, this.ErrorMessage);
                        }
                    }
                    else
                    {
                        result = new ValidationResult(false, this.ErrorMessage);
                    }
                }
            }
            else
            {
                result = new ValidationResult(false, this.ErrorMessage);
            }
            return result;
        }
    }

}
