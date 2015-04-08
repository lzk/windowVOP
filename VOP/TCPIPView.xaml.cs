using System;
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

        TCPIPSetting TcpIpSettingInit = new TCPIPSetting();
        TCPIPSetting TcpIpSetting = new TCPIPSetting();

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

            TcpIpSettingInit.m_mode_ipversion = 0;
            TcpIpSettingInit.m_mode_ipaddress = (byte)EnumIPType.DHCP;
            TcpIpSettingInit.m_ip0 = 0;
            TcpIpSettingInit.m_ip1 = 0;
            TcpIpSettingInit.m_ip2 = 0;
            TcpIpSettingInit.m_ip3 = 0;
            TcpIpSettingInit.m_mask0 = 0;
            TcpIpSettingInit.m_mask1 = 0;
            TcpIpSettingInit.m_mask2 = 0;
            TcpIpSettingInit.m_mask3 = 0;
            TcpIpSettingInit.m_gate0 = 0;
            TcpIpSettingInit.m_gate1 = 0;
            TcpIpSettingInit.m_gate2 = 0;
            TcpIpSettingInit.m_gate3 = 0;

            IpInfoRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<IpInfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.GetIpInfo);
            }
            else
            {
                m_rec = worker.GetIpInfo(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter);
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                TcpIpSettingInit.m_mode_ipversion = m_rec.IpVersion;
                TcpIpSettingInit.m_mode_ipaddress = (byte)m_rec.IpAddressMode;

                byte[] arr = m_rec.Ip.GetAddressBytes();
                TcpIpSettingInit.m_ip0 = arr[0];
                TcpIpSettingInit.m_ip1 = arr[1];
                TcpIpSettingInit.m_ip2 = arr[2];
                TcpIpSettingInit.m_ip3 = arr[3];

                arr = m_rec.Mask.GetAddressBytes();
                TcpIpSettingInit.m_mask0 = arr[0];
                TcpIpSettingInit.m_mask1 = arr[1];
                TcpIpSettingInit.m_mask2 = arr[2];
                TcpIpSettingInit.m_mask3 = arr[3];

                arr = m_rec.Gate.GetAddressBytes();
                TcpIpSettingInit.m_gate0 = arr[0];
                TcpIpSettingInit.m_gate1 = arr[1];
                TcpIpSettingInit.m_gate2 = arr[2];
                TcpIpSettingInit.m_gate3 = arr[3];

            }

            string addr_ip = "";
            string addr_mask = "";
            string addr_gate = "";

            addr_ip += TcpIpSettingInit.m_ip0.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSettingInit.m_ip1.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSettingInit.m_ip2.ToString();
            addr_ip += ".";
            addr_ip += TcpIpSettingInit.m_ip3.ToString();

            addr_mask += TcpIpSettingInit.m_mask0.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSettingInit.m_mask1.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSettingInit.m_mask2.ToString();
            addr_mask += ".";
            addr_mask += TcpIpSettingInit.m_mask3.ToString();

            addr_gate += TcpIpSettingInit.m_gate0.ToString();
            addr_gate += ".";
            addr_gate += TcpIpSettingInit.m_gate1.ToString();
            addr_gate += ".";
            addr_gate += TcpIpSettingInit.m_gate2.ToString();
            addr_gate += ".";
            addr_gate += TcpIpSettingInit.m_gate3.ToString();

            tb_ip.Text = addr_ip;
            tb_mask.Text = addr_mask;
            tb_gate.Text = addr_gate;

            rdbtn_dhcp.IsChecked = ((byte)EnumIPType.DHCP == TcpIpSettingInit.m_mode_ipaddress);
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
                }
                else if (btn.Name == "rdbtn_static")
                {
                    tb_ip.IsEnabled = true;
                    tb_gate.IsEnabled = true;
                    tb_mask.IsEnabled = true;
                }
            }


            //if (null != event_config_dirty)
            //    event_config_dirty(is_dirty());
        }


        public void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            //if (null != event_config_dirty)
            //    event_config_dirty(is_dirty());
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
