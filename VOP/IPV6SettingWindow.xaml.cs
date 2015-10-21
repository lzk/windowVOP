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
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net;

namespace VOP
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class IPV6SettingWindow : Window
    {
        IPV6InfoRecord m_rec = null;
        bool isIPOK = false;
        bool isIPMaskOK = false;
        bool isGateOK = false;
        bool isGateMaskOK = false;

        private EnumStatus m_currentStatus = EnumStatus.Offline;
        private bool m_bIPValidate = true;
        private bool m_bSubmaskLengthValidate = true;
        private bool m_bGatewayValidate = true;

        public IPV6SettingWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        public void MyMouseButtonEventHandler(Object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // zh-CN
            {
                btnApply.FontSize = btnCancel.FontSize = 16;
            }
            else
            {
                btnApply.FontSize = btnCancel.FontSize = 14;
            }
        }

        private void OnLoadedIPV6View(object sender, RoutedEventArgs e)
        {
            InitFontSize();

            AsyncWorker worker = new AsyncWorker(this);
            worker.InvokeMethod<IPV6InfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.GetIpv6Info, this);

            if (null != m_rec)
            {
                if (1 == m_rec.DHCPv6)
                    btnDHCP.IsChecked = true;
                else
                    btnDHCP.IsChecked = false;

                if (1 == m_rec.UseManualAddress)
                    btnManual.IsChecked = true;
                else
                    btnManual.IsChecked = false;

                if (1 == m_rec.UseManualAddress)
                {
                    tb_ip.Text = m_rec.IPManualAddress;
                    tb_PreSubMask.Text = String.Format("{0}", m_rec.ManualMask);
                }
                else
                {
                    tb_ip.Text = "::";
                    tb_PreSubMask.Text = String.Format("{0}", 0);
                }

                if (1 == m_rec.UseManualAddress)
                {
                    tb_Gateway.Text = m_rec.IPv6ManualGatewayAddress;
                    tb_GatewayPreSubMask.Text = String.Format("{0}", m_rec.ManualGatewayAddressMask);
                }
                else
                {
                    tb_Gateway.Text = "::";
                    tb_GatewayPreSubMask.Text = String.Format("{0}", 0);
                }

            }
        }

        public void PassStatus(EnumStatus st, EnumMachineJob job, byte toner)
        {
            m_currentStatus = st;

            if (true == m_bSubmaskLengthValidate &&
                true == m_bIPValidate &&
                true == m_bGatewayValidate)
            {
                // btnApply.IsEnabled = (false == common.IsOffline(m_currentStatus));
            }
        }

        private void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            SetTextBoxBorder(tb, false);
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

        private void OnCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            if (btnManual == null ||
                tb_ip == null ||
                tb_PreSubMask == null ||
                tb_Gateway == null ||
                tb_GatewayPreSubMask == null)
                return;

            if (btnManual.IsChecked == false)
            {
                tb_ip.IsEnabled = false;
                tb_PreSubMask.IsEnabled = false;
                tb_Gateway.IsEnabled = false;
                tb_GatewayPreSubMask.IsEnabled = false;

                // Begin: Fixed bug #61083
                SetTextBoxBorder(tb_ip, false);
                SetTextBoxBorder(tb_PreSubMask, false);
                SetTextBoxBorder(tb_Gateway, false);
                SetTextBoxBorder(tb_GatewayPreSubMask, false);
                // End: Fixed bug #61083

                //tb_ip.Text = "::";
                //tb_PreSubMask.Text = "0";
                //tb_Gateway.Text = "::";
                //tb_GatewayPreSubMask.Text = "0";

                // btnApply.IsEnabled = (false == common.IsOffline(m_currentStatus));
            }
            else if (btnManual.IsChecked == true)
            {
                tb_ip.IsEnabled = true;
                tb_PreSubMask.IsEnabled = true;
                tb_Gateway.IsEnabled = true;
                tb_GatewayPreSubMask.IsEnabled = true;

                //if (tb_ip.Text.Length == 0 ||
                //    tb_PreSubMask.Text.Length == 0 ||
                //    tb_Gateway.Text.Length == 0 ||
                //    tb_GatewayPreSubMask.Text.Length == 0)
                //{
                //    btnApply.IsEnabled = false;
                //}
                //else
                //{
                //    btnApply.IsEnabled = true;
                //}
            }
        }

        private void SetTextBoxBorder(TextBox tb, bool bValue)
        {
            if (bValue == true)
            {
                tb.BorderThickness = new Thickness(2);
            }
            else
            {
                tb.BorderThickness = new Thickness(0);
            }
        }

        private void tbLostFocus(object sender, RoutedEventArgs e)
        {
            //TextBox tb = sender as TextBox;

            //if(tb.Name == "tb_ip")
            //{

            //}
        }

        private void OnbtnApplyClicked(object sender, RoutedEventArgs e)
        {
            bool isSuccess = false;

            if (m_rec == null)
                return;

            AsyncWorker worker = new AsyncWorker(this);

            if (btnDHCP.IsChecked == true)
                m_rec.DHCPv6 = 1;
            else
                m_rec.DHCPv6 = 0;

            if (btnManual.IsChecked == true)
                m_rec.UseManualAddress = 1;
            else
                m_rec.UseManualAddress = 0;






            IPAddress ip;
            if (IPAddress.TryParse(tb_ip.Text, out ip))
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    if (ip.Equals(IPAddress.IPv6Loopback) || ip.Equals(IPAddress.IPv6None))
                    {
                        isIPOK = false;
                    }
                    else
                    {
                        isIPOK = true;
                    }

                }
                else
                {
                    isIPOK = false;
                }
            }
            else
            {
                isIPOK = false;
            }


            if (btnManual.IsChecked == true)
            {
                if (isIPOK == true)
                {
                    m_rec.IPManualAddress = tb_ip.Text;
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_entered_IP_address__is_wrong__please_confirm_and_enter_again_"), (string)this.FindResource("ResStr_Warning"));
                    tb_ip.Text = "";
                    tb_ip.Focus();
                    SetTextBoxBorder(tb_ip, true);
                    return;
                }
            }
            else
            {
                m_rec.IPManualAddress = "::";
            }





            uint maskValue;

            if (uint.TryParse(tb_PreSubMask.Text, out maskValue))
            {
                if (maskValue >= 0 && maskValue <= 128)
                {
                    isIPMaskOK = true;
                }
                else
                {
                    isIPMaskOK = false;
                }
            }
            else
            {
                isIPMaskOK = false;
            }


            if (btnManual.IsChecked == true)
            {
                if (isIPMaskOK == true)
                {
                    m_rec.ManualMask = maskValue;
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_subnet_mask_input_error__please_input_again_after_confirmation"), (string)this.FindResource("ResStr_Warning"));
                    tb_PreSubMask.Text = "";
                    tb_PreSubMask.Focus();
                    SetTextBoxBorder(tb_PreSubMask, true);
                    return;
                }
            }
            else
            {
                m_rec.ManualMask = 0;
            }





            IPAddress ipGate;
            if (IPAddress.TryParse(tb_Gateway.Text, out ipGate))
            {
                if (ipGate.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                {
                    if (ipGate.Equals(IPAddress.IPv6Loopback) || ipGate.Equals(IPAddress.IPv6None))
                    {
                        isGateOK = false;
                    }
                    else
                    {
                        isGateOK = true;
                    }
                }
                else
                {
                    isGateOK = false;
                }
            }
            else
            {
                isGateOK = false;
            }


            if (btnManual.IsChecked == true)
            {
                if (isGateOK == true)
                {
                    m_rec.IPv6ManualGatewayAddress = tb_Gateway.Text;
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_entered_Gateway_is_wrong__please_confirm_and_enter_again_"), (string)this.FindResource("ResStr_Warning"));
                    tb_Gateway.Text = "";
                    tb_Gateway.Focus();
                    SetTextBoxBorder(tb_Gateway, true);
                    return;
                }
            }
            else
            {
                m_rec.IPv6ManualGatewayAddress = "::";
            }






            uint gatewayMaskValue;

            if (uint.TryParse(tb_GatewayPreSubMask.Text, out gatewayMaskValue))
            {
                if (gatewayMaskValue >= 0 && gatewayMaskValue <= 128)
                {
                    isGateMaskOK = true;
                }
                else
                {
                    isGateMaskOK = false;
                }
            }
            else
            {
                isGateMaskOK = false;
            }


            if (btnManual.IsChecked == true)
            {
                if (isGateMaskOK == true)
                {
                    m_rec.ManualGatewayAddressMask = gatewayMaskValue;
                }
                else
                {
                    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, Application.Current.MainWindow, (string)this.FindResource("ResStr_The_subnet_mask_input_error__please_input_again_after_confirmation"), (string)this.FindResource("ResStr_Warning"));
                    tb_GatewayPreSubMask.Text = "";
                    tb_GatewayPreSubMask.Focus();
                    SetTextBoxBorder(tb_GatewayPreSubMask, true);
                    return;
                }
            }
            else
            {
                m_rec.ManualGatewayAddressMask = 0;
            }




            if (worker.InvokeMethod<IPV6InfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.SetIpv6Info, this))
            {
                if (m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
            }
            else
            {
                isSuccess = false;
            }

            if (isSuccess)
            {
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Msg_1"), Brushes.Black);
                DialogResult = true;
            }
            else
            {
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage((string)this.FindResource("ResStr_Setting_Fail"), Brushes.Red);
            }
        }
    }
}
