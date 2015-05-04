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
    public partial class TcpipView : UserControl
    {
        public bool m_is_init = false;

        TCPIPSetting TcpIpSetting = new TCPIPSetting();
        string m_strIP = "";
        string m_strMask = "";
        string m_strGate = "";

        public TcpipView()
        {
            InitializeComponent();
        }

        private void OnLoadedTcpipView(object sender, RoutedEventArgs e)
        {
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
            string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
            
            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<IpInfoRecord>(strPrinterName, ref m_rec, DllMethodType.GetIpInfo);
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
            }

            string addr_ip = "";
            string addr_mask = "";
            string addr_gate = "";

            addr_ip += TcpIpSetting.m_ip0.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSetting.m_ip1.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSetting.m_ip2.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSetting.m_ip3.ToString();

            addr_mask += TcpIpSetting.m_mask0.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSetting.m_mask1.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSetting.m_mask2.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSetting.m_mask3.ToString();

            addr_gate += TcpIpSetting.m_gate0.ToString();
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
            }
            else
            {
                tb_ip.IsEnabled = true;
                tb_gate.IsEnabled = true;
                tb_mask.IsEnabled = true;
            }
        }

        private bool ParseNetworkMask(string strIP)
        {
            bool isSuccess = false;
            byte ip0 = 0;
            byte ip1 = 0;
            byte ip2 = 0;
            byte ip3 = 0;

            Int32 nIP;
            if (ParseIP(strIP, ref ip0, ref ip1, ref ip2, ref ip3))
            {
                nIP = ip3 | (ip2 << 8) | (ip1 << 16) | (ip0 << 24);
                int nIdx = 0;
                for (; nIdx < 32; nIdx++)
                {
                    if (0 != (nIP & (0x80000000 >> nIdx)))
                        continue;
                    else
                        break;
                }

                for (int nIdx1 = 0; nIdx1 < (32 - nIdx); nIdx1++)
                {
                    if (0 != (nIP & (0x80000000 >> (nIdx + nIdx1))))
                        isSuccess = false;
                    else
                        isSuccess = true;

                    if (!isSuccess)
                        break;
                }

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

                    if (0 <= nVal0 && 255 >= nVal0
                            && 0 <= nVal1 && 255 >= nVal1
                            && 0 <= nVal2 && 255 >= nVal2
                            && 0 <= nVal3 && 255 >= nVal3)
                    {
                        ip0 = (byte)nVal0;
                        ip1 = (byte)nVal1;
                        ip2 = (byte)nVal2;
                        ip3 = (byte)nVal3;
                        isSuccess = true;
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
                {
                    tb_ip.IsEnabled = false;
                    tb_gate.IsEnabled = false;
                    tb_mask.IsEnabled = false;
                    btnApply.IsEnabled = true;
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
                        btnApply.IsEnabled = true;
                    }
                }
            }
        }

        public void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            string strText = tb.Text;
            bool bValidate = common.IsAsciiLetter(strText);
            if (false == bValidate)
            {
                if (tb.Name == "tb_ip")
                    tb_ip.Text = m_strIP;
                if (tb.Name == "tb_gate")
                    tb_gate.Text = m_strGate;
                if (tb.Name == "tb_mask")
                    tb_mask.Text = m_strMask;

                return;
            }

            if (tb.Name == "tb_ip")
                m_strIP = tb_ip.Text;
            if (tb.Name == "tb_gate")
                m_strGate = tb_gate.Text;
            if (tb.Name == "tb_mask")
                m_strMask = tb_mask.Text;

            if (tb_ip.Text.Length == 0 ||
                tb_gate.Text.Length == 0 ||
                tb_mask.Text.Length == 0)
            {
                btnApply.IsEnabled = false;
            }
            else
            {
                btnApply.IsEnabled = true;
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

            byte mode_ipversion = 0;  // 0-ipv4,1-ipv6
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
            bool isMaskOK = ParseIP(addr_mask, ref mask0, ref mask1, ref mask2, ref mask3) && ParseNetworkMask(addr_mask);

            if (mode_ipaddress == (byte)enum_addr_mode.Manual)
            {
                if ((ip0 >= 224 || ip0 == 127))
                    isIPOK = false;

                if ((ip0 == 192 && ip0 == 168 && ip0 == 186 && ip0 == 1) || 
                    (ip0 == 169 && ip0 == 254))
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
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_entered_IP_address__is_wrong__please_confirm_and_enter_again_"), (string)this.FindResource("ResStr_Warning"));
                    tb_ip.Text = "";
                    tb_ip.Focus();
                    return false;
                }
                else if (false == isGateOK)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_entered_Gateway_is_wrong__please_confirm_and_enter_again_"), (string)this.FindResource("ResStr_Warning"));
                    tb_gate.Text = "";
                    tb_gate.Focus();
                    return false;
                }
                else if (false == isMaskOK)
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_subnet_mask_input_error__please_input_again_after_confirmation"), (string)this.FindResource("ResStr_Warning"));
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
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_combination_of_IP_address_and_subnet_mask_is_invalid__All_of_the_bits_in_the_host_address_portion_of_the_IP_address_are_set_to_1__Please_enter_a_valid_combination_of_IP_address_and_subnet_mask_"), (string)this.FindResource("ResStr_Warning"));
                            tb_ip.Focus();
                            return false;
                        }
                        else if ((0x00000000 == ((~nSubMask) & nIP)))
                        {
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_combination_of_IP_address_and_subnet_mask_is_invalid__All_of_the_bits_in_the_host_address_portion_of_the_IP_address_are_set_to_0__Please_enter_a_valid_combination_of_IP_address_and_subnet_mask_"), (string)this.FindResource("ResStr_Warning"));
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

                string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
                m_rec.PrinterName = strPrinterName;
                m_rec.IpVersion = mode_ipversion;
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
                if (worker.InvokeMethod<IpInfoRecord>(strPrinterName, ref m_rec, DllMethodType.SetIpInfo))
                {
                    if (m_rec.CmdResult == EnumCmdResult._ACK)
                    {
                        TcpIpSetting.m_mode_ipversion = mode_ipversion;
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

            ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage(isSuccess ? (string)this.FindResource("ResStr_Setting_Successfully_") : (string)this.FindResource("ResStr_Setting_Fail"), Brushes.Red);
            return isSuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            apply();
        }

        private void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            string strText = e.Text;
            if (Char.IsLetterOrDigit(strText, 0))
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

        public void HandlerStateUpdate(EnumState state)
        {
            // TODO: update UI when auto machine state change.
            if (state == EnumState.stopWorking)
            {
                this.IsEnabled = false;
            }
            else
            {
                this.IsEnabled = true;
            }
        }
    }
}
