﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using VOP.Controls;
using Microsoft.Win32;

namespace VOP
{
    /// <summary>
    /// Interaction logic for TCPIPView.xaml
    /// </summary>
    ///  
    public class TCPIPSetting
    {
        public byte m_mode_ipversion = 0;
        public byte m_mode_ipaddress = (byte)EnumIPType.DHCP;
        public byte m_ip0 = 0;
        public byte m_ip1 = 0;
        public byte m_ip2 = 0;
        public byte m_ip3 = 0;
        public byte m_mask0 = 0;
        public byte m_mask1 = 0;
        public byte m_mask2 = 0;
        public byte m_mask3 = 0;
        public byte m_gate0 = 0;
        public byte m_gate1 = 0;
        public byte m_gate2 = 0;
        public byte m_gate3 = 0;
    }

    public class MacAddressRegistry
    {
        static RegistryKey LocalKey = Registry.LocalMachine;
        static RegistryKey rootKey = null;

        public static bool Open(string printerName)
        {
            string strDrvName = "";
            string openKeyString = "";

            try
            {
                common.GetPrinterDrvName(printerName, ref strDrvName);
                openKeyString = @"Software\Lenovo\" + strDrvName + @"\Launcher\" + printerName;

                rootKey = LocalKey.OpenSubKey(openKeyString, false);

                if (rootKey == null)
                {
                    Close();
                    return false;
                }

            }
            catch (Exception)
            {
                Close();
                return false;
            }

            return true;
        }

        public static void Close()
        {
            if (rootKey != null)
                rootKey.Close();

            if (LocalKey != null)
                LocalKey.Close();
        }

        public static string GetMacAddress()
        {
            string str = "";
            try
            {
                str = rootKey.GetValue("MacAddress").ToString();
            }
            catch (Exception)
            {

            }

            return str;
        }

    }

    public partial class TcpipView_Rufous : UserControl
    {
        public bool m_is_init = false;

        TCPIPSetting TcpIpSetting = new TCPIPSetting();
        private bool m_currentStatus = false;
        private bool m_bIPValidate = true;
        private bool m_bSubmaskValidate = true;
        private bool m_bGatewayValidate = true;
        private string m_printerName = "";

        private string addr_ip = "";
        private string addr_mask = "";
        private string addr_gate = "";

        #region IP
        private static readonly DependencyProperty IPProperty =
            DependencyProperty.Register("IP", typeof(string), typeof(TcpipView_Rufous),
            new PropertyMetadata("0.0.0.0", OnFormattedValueChanged));

        private static void OnFormattedValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
          
        }

        public string IP
        {
            set
            {
                SetValue(IPProperty, value);
            }
            get
            {
                return (string)GetValue(IPProperty);
            }
        }
        #endregion

        #region Gateway
        private static readonly DependencyProperty GatewayProperty =
            DependencyProperty.Register("Gateway", typeof(string), typeof(TcpipView_Rufous),
            new PropertyMetadata("0.0.0.0", OnFormattedValueChanged));

        public string Gateway
        {
            set
            {
                SetValue(GatewayProperty, value);
            }
            get
            {
                return (string)GetValue(GatewayProperty);
            }
        }
        #endregion

        #region Submask
        private static readonly DependencyProperty SubmaskProperty =
            DependencyProperty.Register("Submask", typeof(string), typeof(TcpipView_Rufous),
            new PropertyMetadata("255.255.255.0", OnFormattedValueChanged));

        public string Submask
        {
            set
            {
                SetValue(SubmaskProperty, value);
            }
            get
            {
                return (string)GetValue(SubmaskProperty);
            }
        }
        #endregion

        public TcpipView_Rufous()
        {
            InitializeComponent();
        }

        private void OnLoadedTcpipView(object sender, RoutedEventArgs e)
        {
            if (_MainWin.settingPageIndex != 4)
                return;

            init_config();
        }

        public void init_config(bool _bDisplayProgressBar = true)
        {
            m_is_init = true;

            TcpIpSetting.m_mode_ipversion = 0;
            TcpIpSetting.m_mode_ipaddress = (byte)EnumIPType.DHCP;
            TcpIpSetting.m_ip0 = 0;
            TcpIpSetting.m_ip1 = 0;
            TcpIpSetting.m_ip2 = 0;
            TcpIpSetting.m_ip3 = 0;
            TcpIpSetting.m_mask0 = 0;
            TcpIpSetting.m_mask1 = 0;
            TcpIpSetting.m_mask2 = 0;
            TcpIpSetting.m_mask3 = 0;
            TcpIpSetting.m_gate0 = 0;
            TcpIpSetting.m_gate1 = 0;
            TcpIpSetting.m_gate2 = 0;
            TcpIpSetting.m_gate3 = 0;

            IpInfoRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            string strPrinterName = "";
            m_printerName = strPrinterName;
            
            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<IpInfoRecord>(strPrinterName, ref m_rec, DllMethodType.GetIpInfo, this);
            }
            else
            {
                m_rec = worker.GetIpInfo(strPrinterName);
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                byte[] arr = m_rec.Ip.GetAddressBytes();
                TcpIpSetting.m_ip0 = arr[0];
                TcpIpSetting.m_ip1 = arr[1];
                TcpIpSetting.m_ip2 = arr[2];
                TcpIpSetting.m_ip3 = arr[3];

                arr = m_rec.Mask.GetAddressBytes();
                TcpIpSetting.m_mask0 = arr[0];
                TcpIpSetting.m_mask1 = arr[1];
                TcpIpSetting.m_mask2 = arr[2];
                TcpIpSetting.m_mask3 = arr[3];

                arr = m_rec.Gate.GetAddressBytes();
                TcpIpSetting.m_gate0 = arr[0];
                TcpIpSetting.m_gate1 = arr[1];
                TcpIpSetting.m_gate2 = arr[2];
                TcpIpSetting.m_gate3 = arr[3];

                TcpIpSetting.m_mode_ipversion = m_rec.IpVersion;
                TcpIpSetting.m_mode_ipaddress = (byte)m_rec.IpAddressMode;
            }

            //modified by yunying shang 2017-10-13 for BMS 1017
            //string addr_ip = "";
            //string addr_mask = "";
            //string addr_gate = "";

            addr_ip = TcpIpSetting.m_ip0.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSetting.m_ip1.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSetting.m_ip2.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSetting.m_ip3.ToString();

            addr_mask = TcpIpSetting.m_mask0.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSetting.m_mask1.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSetting.m_mask2.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSetting.m_mask3.ToString();

            addr_gate = TcpIpSetting.m_gate0.ToString();
            addr_gate += ".";
            addr_gate += TcpIpSetting.m_gate1.ToString();
            addr_gate += ".";
            addr_gate += TcpIpSetting.m_gate2.ToString();
            addr_gate += ".";
            addr_gate += TcpIpSetting.m_gate3.ToString();

            tb_ip.Text = addr_ip;
            tb_mask.Text = addr_mask;
            tb_gate.Text = addr_gate;

            rdbtn_dhcp.IsChecked = ((byte)EnumIPType.DHCP == TcpIpSetting.m_mode_ipaddress);
            //Add by KevinYin for BMS Bug 56149 begin
            rdbtn_static.IsChecked = !rdbtn_dhcp.IsChecked;
            //Add by KevinYin for BMS Bug 56149 end

            if (true == rdbtn_dhcp.IsChecked)
            {
                tb_ip.IsEnabled = false;
                tb_gate.IsEnabled = false;
                tb_mask.IsEnabled = false;
                btnApply.IsEnabled = true;
            }
            else
            {
                tb_ip.IsEnabled = true;
                tb_gate.IsEnabled = true;
                tb_mask.IsEnabled = true;                
            }///////////////1017
        }

        private bool ParseNetworkMask(string strIP, ref byte ip0, ref byte ip1, ref byte ip2, ref byte ip3)
        {
            bool isSuccess = true;
            IPAddress ipAddress;
            IPAddress.TryParse(strIP, out ipAddress);
            
            if (null != ipAddress)
            {
                int nVal0 = -1;
                int nVal1 = -1;
                int nVal2 = -1;
                int nVal3 = -1;

                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    string strIP1 = ipAddress.ToString();
                    if (null != strIP && 0 < strIP1.Length)
                    {
                        var parts_ip = strIP1.Split('.');
                        try
                        {
                            nVal0 = Convert.ToInt32(parts_ip[0]);
                            nVal1 = Convert.ToInt32(parts_ip[1]);
                            nVal2 = Convert.ToInt32(parts_ip[2]);
                            nVal3 = Convert.ToInt32(parts_ip[3]);

                            ip0 = (byte)nVal0;
                            ip1 = (byte)nVal1;
                            ip2 = (byte)nVal2;
                            ip3 = (byte)nVal3;

                            if (false == (0 <= nVal0 && 255 >= nVal0
                                    && 0 <= nVal1 && 255 >= nVal1
                                    && 0 <= nVal2 && 255 >= nVal2
                                    && 0 <= nVal3 && 255 >= nVal3))
                            {
                                isSuccess = false;
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
                                            isSuccess = false;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    isSuccess = false;
                                }
                            }
                        }
                        catch
                        {
                            isSuccess = false;
                        }
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            else
            {
                isSuccess = false;
            }

            return isSuccess;
        }
        
        private bool ParseIP(string strIP, ref byte ip0, ref byte ip1, ref byte ip2, ref byte ip3)
        {
            bool isSuccess = false;

            int nVal0 = -1;
            int nVal1 = -1;
            int nVal2 = -1;
            int nVal3 = -1;
            if (null != strIP)
            {
                IPAddress ipAddress;
                IPAddress.TryParse(strIP, out ipAddress);
                if (null != ipAddress)
                {
                    if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        strIP = ipAddress.ToString();
                    }
                }
            }

            if (null != strIP && 0 < strIP.Length)
            {
                var parts_ip = strIP.Split('.');
                try
                {
                    nVal0 = Convert.ToInt32(parts_ip[0]);
                    nVal1 = Convert.ToInt32(parts_ip[1]);
                    nVal2 = Convert.ToInt32(parts_ip[2]);
                    nVal3 = Convert.ToInt32(parts_ip[3]);

                    ip0 = (byte)nVal0;
                    ip1 = (byte)nVal1;
                    ip2 = (byte)nVal2;
                    ip3 = (byte)nVal3;

                    if (false == (0 <= nVal0 && 224 > nVal0
                         && 0 <= nVal1 && 255 >= nVal1
                         && 0 <= nVal2 && 255 >= nVal2
                         && 0 <= nVal3 && 255 >= nVal3))
                    {
                        isSuccess = false;
                    }
                    else
                    {
                        if (nVal0 == 192 && nVal1 == 168 && nVal2 == 186)
                        {
                            isSuccess = false;
                        }
                        else
                        {
                            if (nVal0 == 127)
                            {
                                isSuccess = false;
                            }
                            else
                            {
                                isSuccess = true;
                            }
                        }
                    }
                }
                catch
                {
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        private void btn_click(object sender, RoutedEventArgs e)
        {
            RadioButton btn = sender as RadioButton;

            if (null != btn)
            {
                if (btn.Name == "rdbtn_dhcp")
                {//modified by yunying shang 2017-10-13 for BMS 1017
                    tb_ip.IsEnabled = false;
                    tb_gate.IsEnabled = false;
                    tb_mask.IsEnabled = false;
                    btnApply.IsEnabled = true;//modified by yunying shang 2019-10-11 for BMS 1017
                    tb_ip.Text = addr_ip;
                    tb_mask.Text = addr_mask;
                    tb_gate.Text = addr_gate;
                    //==============1017
                }
                else if (btn.Name == "rdbtn_static")
                {
                    tb_ip.IsEnabled = true;
                    tb_gate.IsEnabled = true;
                    tb_mask.IsEnabled = true;
                    if (tb_ip.Text.Length == 0 ||
                        tb_gate.Text.Length == 0 ||
                        tb_mask.Text.Length == 0)
                    {
                        btnApply.IsEnabled = false;
                    }
                    else
                    {
                        btnApply.IsEnabled = m_currentStatus;
                    }
                }
            }
        }

        public void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string strText = tb.Text;
            
            if (null != tb_ip &&
                null != tb_gate &&
                null != tb_mask &&
                null != btnApply)
            {
                bool bValidate = false;
                byte ip0 = 0;
                byte ip1 = 0;
                byte ip2 = 0;
                byte ip3 = 0;

                if (tb.Name == "tb_ip")
                {
                    if (true == ParseIP(strText, ref ip0, ref ip1, ref ip2, ref ip3))
                    {
                        m_bIPValidate = true;
                        bValidate = true;
                    }
                    else
                    {
                        m_bIPValidate = false;
                    }
                }
                else if(tb.Name == "tb_gate")
                {
                    if (true == ParseIP(strText, ref ip0, ref ip1, ref ip2, ref ip3))
                    {
                        m_bGatewayValidate = true;
                        bValidate = true;
                    }
                    else
                    {
                        m_bGatewayValidate = false;
                    }
                }
                else if (tb.Name == "tb_mask")
                {
                    if (true == ParseNetworkMask(strText, ref ip0, ref ip1, ref ip2, ref ip3))
                    {
                        m_bSubmaskValidate = true;
                        bValidate = true;
                    }
                    else
                    {
                        m_bSubmaskValidate = false;
                    }
                }

                if (false == rdbtn_dhcp.IsChecked)
                {
                    if (bValidate)
                    {
                        btnApply.IsEnabled = true;
                    }
                    else
                    {
                        btnApply.IsEnabled = false;
                    }
                }
                else
                {
                    btnApply.IsEnabled = true;
                }
            }
        }

        public bool apply()
        {
            bool isSuccess = false;
            // get network info
            string addr_ip = tb_ip.Text;
            string addr_mask = tb_mask.Text;
            string addr_gate = tb_gate.Text;

            enum_addr_mode addr_mode = enum_addr_mode.Manual;
            addr_mode = rdbtn_dhcp.IsChecked == true ? enum_addr_mode.DHCP : enum_addr_mode.Manual;

           // byte mode_ipversion = 0;  // 0-ipv4,1-ipv6
            byte mode_ipaddress = (byte)EnumIPType.DHCP;
            byte ip0 = 0;
            byte ip1 = 0;
            byte ip2 = 0;
            byte ip3 = 0;
            byte mask0 = 0;
            byte mask1 = 0;
            byte mask2 = 0;
            byte mask3 = 0;
            byte gate0 = 0;
            byte gate1 = 0;
            byte gate2 = 0;
            byte gate3 = 0;
            mode_ipaddress = (byte)addr_mode;

            bool isIPOK = ParseIP(addr_ip, ref ip0, ref ip1, ref ip2, ref ip3);
            bool isGateOK = ParseIP(addr_gate, ref gate0, ref gate1, ref gate2, ref gate3);
            bool isMaskOK = ParseNetworkMask(addr_mask, ref mask0, ref mask1, ref mask2, ref mask3);

            if (mode_ipaddress == (byte)enum_addr_mode.Manual)
            {
                if ((ip0 >= 224 || ip0 == 127))
                    isIPOK = false;

                if ((ip0 == 192 && ip0 == 168 && ip0 == 186 && ip0 == 1))
                {
                    isIPOK = false;
                }
            }

            if (isGateOK)
            {
                if ((gate0 >= 224 || gate0 == 127))
                    isGateOK = false;
            }

            if (mode_ipaddress == (byte)enum_addr_mode.Manual)
            {
                if (false == isIPOK)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                        Application.Current.MainWindow, 
                        (string)this.FindResource("ResStr_The_entered_IP_address__is_wrong__please_confirm_and_enter_again_"),
                        (string)this.FindResource("ResStr_Warning"));
                    tb_ip.Text = "";
                    tb_ip.Focus();
                    return false;
                }
                else if (false == isGateOK)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                        Application.Current.MainWindow, 
                        (string)this.FindResource("ResStr_The_entered_Gateway_is_wrong__please_confirm_and_enter_again_"), 
                        (string)this.FindResource("ResStr_Warning"));
                    tb_gate.Text = "";
                    tb_gate.Focus();
                    return false;
                }
                else if (false == isMaskOK)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                        Application.Current.MainWindow, (string)this.FindResource("ResStr_The_subnet_mask_input_error__please_input_again_after_confirmation"),
                        (string)this.FindResource("ResStr_Warning"));
                    tb_mask.Text = "";
                    tb_mask.Focus();
                    return false;
                }

                if (isIPOK && isMaskOK)
                {
                    Int32 nIP = ip3 | (ip2 << 8) | (ip1 << 16) | (ip0 << 24);
                    Int32 nSubMask = mask3 | (mask2 << 8) | (mask1 << 16) | (mask0 << 24);
                    if (isIPOK && (nIP != 0x00000000 && nSubMask != 0x00000000) && (0xffffffff == ((nIP | nSubMask) & 0xffffffff) || (0x00000000 == ((~nSubMask) & nIP))))
                    {
                        isIPOK = false;
                        if (0xffffffff == ((nIP | nSubMask) & 0xffffffff))
                        {
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                                Application.Current.MainWindow, 
                                (string)this.FindResource("ResStr_The_combination_of_IP_address_and_subnet_mask_is_invalid__All_of_the_bits_in_the_host_address_portion_of_the_IP_address_are_set_to_1__Please_enter_a_valid_combination_of_IP_address_and_subnet_mask_"), 
                                (string)this.FindResource("ResStr_Warning"));
                            tb_ip.Focus();
                            return false;
                        }
                        else if ((0x00000000 == ((~nSubMask) & nIP)))
                        {
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                                Application.Current.MainWindow,
                                (string)this.FindResource("ResStr_The_combination_of_IP_address_and_subnet_mask_is_invalid__All_of_the_bits_in_the_host_address_portion_of_the_IP_address_are_set_to_0__Please_enter_a_valid_combination_of_IP_address_and_subnet_mask_"), 
                                (string)this.FindResource("ResStr_Warning"));
                            tb_ip.Focus();
                            return false;
                        }
                    }
                }
            }

            if ((mode_ipaddress == (byte)enum_addr_mode.Manual && true == isIPOK
                    && true == isMaskOK
                    && true == isGateOK) || mode_ipaddress == (byte)enum_addr_mode.DHCP)
            {
                IpInfoRecord m_rec = new IpInfoRecord();
                byte[] arr = new byte[4];

                string strPrinterName = "";
                m_rec.PrinterName = strPrinterName;
                m_rec.IpVersion = TcpIpSetting.m_mode_ipversion;
                m_rec.IpAddressMode = (EnumIPType)mode_ipaddress;

                arr[0] = ip0;
                arr[1] = ip1;
                arr[2] = ip2;
                arr[3] = ip3;
                m_rec.Ip = new IPAddress(arr);

                arr[0] = mask0;
                arr[1] = mask1;
                arr[2] = mask2;
                arr[3] = mask3;
                m_rec.Mask = new IPAddress(arr);

                arr[0] = gate0;
                arr[1] = gate1;
                arr[2] = gate2;
                arr[3] = gate3;
                m_rec.Gate = new IPAddress(arr);

                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                if (worker.InvokeMethod<IpInfoRecord>(strPrinterName, ref m_rec, DllMethodType.SetIpInfo, this))
                {
                    if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                       // TcpIpSetting.m_mode_ipversion = mode_ipversion;
                        TcpIpSetting.m_mode_ipaddress = mode_ipaddress;
                        TcpIpSetting.m_ip0 = ip0;
                        TcpIpSetting.m_ip1 = ip1;
                        TcpIpSetting.m_ip2 = ip2;
                        TcpIpSetting.m_ip3 = ip3;
                        TcpIpSetting.m_mask0 = mask0;
                        TcpIpSetting.m_mask1 = mask1;
                        TcpIpSetting.m_mask2 = mask2;
                        TcpIpSetting.m_mask3 = mask3;
                        TcpIpSetting.m_gate0 = gate0;
                        TcpIpSetting.m_gate1 = gate1;
                        TcpIpSetting.m_gate2 = gate2;
                        TcpIpSetting.m_gate3 = gate3;

                        //Add by KevinYin for BMS Bug 55147 begin
                        IPAddress ipAddress;
                        IPAddress.TryParse(addr_ip, out ipAddress);
                        if (null != ipAddress)
                        {
                            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                tb_ip.Text = ipAddress.ToString();
                            }
                        }

                        IPAddress.TryParse(addr_mask, out ipAddress);
                        if (null != ipAddress)
                        {
                            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                tb_mask.Text = ipAddress.ToString();
                            }
                        }

                        IPAddress.TryParse(addr_gate, out ipAddress);
                        if (null != ipAddress)
                        {
                            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                tb_gate.Text = ipAddress.ToString();
                            }
                        }
                        //Add by KevinYin for BMS Bug 55147 end
                        isSuccess = true;
                    }

                }
            }

            if (isSuccess)
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                          Application.Current.MainWindow,
                         (string)this.FindResource("ResStr_Msg_1"),
                         (string)this.TryFindResource("ResStr_Prompt"));
            else
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                 Application.Current.MainWindow,
                (string)this.FindResource("ResStr_Setting_Fail"),
                (string)this.TryFindResource("ResStr_Prompt"));

            return isSuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (m_MainWin.CheckDeviceStatus() == -1)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Device_disconnected"),//"Device is diconnected, could not Apply setting!",
                  (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return;
            }
            apply();
        }

        private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string strText = e.Text;
            if (strText.Length > 0 && Char.IsLetterOrDigit(strText, 0))
            {
                if (Char.IsLetter(strText, 0))
                {
                   e.Handled = true;
                }
            }
            else
            {
                if (e.Text != ".")
                    e.Handled = true;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void tb_ip_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string strIP = tb.Text;

            IPAddressValidationRule rule = new IPAddressValidationRule();
            ValidationResult result = rule.Validate(strIP, null);

            if (result.IsValid == true)
            {
                tb.Text = strIP;
            }
            else
            {
                tb.Text = "0.0.0.0";
            }       
        }

        private void tb_mask_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string strIP = tb.Text;

            SubmaskValidationRule rule = new SubmaskValidationRule();
            ValidationResult result = rule.Validate(strIP, null);

            if (result.IsValid == true)
            {
                tb.Text = strIP;
            }
            else
            {
                tb.Text = "255.255.255.0";
            }
        }

        public void PassStatus(bool online)
        {
            m_currentStatus = online;

            if (false == rdbtn_dhcp.IsChecked)
            {
                if (true == m_bSubmaskValidate &&
                true == m_bIPValidate &&
                true == m_bGatewayValidate)
                {
                    btnApply.IsEnabled = online;
                }
            }
            else
            {
                btnApply.IsEnabled = true;
            }
        }

        private string IpRemovePercentMark(string ip)
        {
            if (ip.Contains("%"))
            {
                string[] a = ip.Split(new char[] { '%' });
                return a[0];
            }

            return ip;
        }

        private int ResetPortIP()
        {
            string printerName = "";
            string strIpAddress = "";
            StringBuilder ipAddress = null;
            string mac = "";
            bool isSFP = true;
            bool isIpv4 = true;
            StringBuilder ipFound = null;

            printerName = m_printerName;

            ipAddress = new StringBuilder(50);
            EnumPortType type = (EnumPortType)dll.CheckPortAPI2(printerName, ipAddress);

            if (type != EnumPortType.PT_TCPIP && type != EnumPortType.PT_WSD)
                return 0;

            if (ipAddress.Length == 0)
                return 0;

            strIpAddress = IpRemovePercentMark(ipAddress.ToString());            

            IPAddress ip;
            if (!IPAddress.TryParse(strIpAddress, out ip))
                return 0;

            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                isIpv4 = false;

            if (MacAddressRegistry.Open(printerName))
            {
                mac = MacAddressRegistry.GetMacAddress();
                MacAddressRegistry.Close();
            }

            if (mac == "")
                return 0;

            string strDrvName = "";
            common.GetPrinterDrvName(printerName, ref strDrvName);
            isSFP = common.IsSFPPrinter(strDrvName);

            ipFound= new StringBuilder(50);
            dll.SearchValidedIP(mac, isIpv4, isSFP, ipFound);
            ipFound.Append("192.168.6.115");
            if (ipFound.Length != 0)
            {
                string strIpFound = IpRemovePercentMark(ipFound.ToString());
              //  if(strIpAddress != strIpFound)
                {

                    ProcessStartInfo procInfo = new ProcessStartInfo();

                    //string pass = "123";
                    //string name = "sonnyzhang";
                    //System.Security.SecureString str;

                    //char[] chArray = pass.ToCharArray();

                    //unsafe
                    //{
                    //    fixed (char* chRef = chArray)
                    //    {
                    //        str = new System.Security.SecureString(chRef, chArray.Length);
                    //    }
                    //}

                    //procInfo.Password = str;
                    //procInfo.UserName = name;

                    procInfo.FileName = @".\SetPortAdmin.exe";
                    procInfo.Arguments = string.Format("/p {0} /p {1}", printerName, ipFound.ToString());
                    procInfo.CreateNoWindow = true;
                    procInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    procInfo.UseShellExecute = true;
                    procInfo.Verb = "runas";

                    try
                    {
                        Process p = Process.Start(procInfo);
                        p.WaitForExit();
                    }
                    catch (Exception)
                    {

                    }
                    
                }
            }
            else
            {
                return 0;
            }

            return 1;
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            bool isSuccess = true;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeDoWorkMethod(ResetPortIP, "") == 1)
                isSuccess = true;
            else
                isSuccess = false;

            if (isSuccess)
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                       Application.Current.MainWindow,
                      (string)this.FindResource("ResStr_Setting_Successfully_"),
                      (string)this.TryFindResource("ResStr_Prompt"));
            else
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                 Application.Current.MainWindow,
                (string)this.FindResource("ResStr_Setting_Fail"),
                (string)this.TryFindResource("ResStr_Prompt"));
        }

        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }
    }
}
