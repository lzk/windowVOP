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
    public partial class IPV6StatusWindow : Window
    {
        IPV6InfoRecord m_rec = null;

        public IPV6StatusWindow()
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
                btnBack.FontSize = 16;
            }
            else
            {
                btnBack.FontSize = 14;
            }
        }

        private void OnLoadedIPV6View(object sender, RoutedEventArgs e)
        {
            InitFontSize();

            AsyncWorker worker = new AsyncWorker(this);
            worker.InvokeMethod<IPV6InfoRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.GetIpv6Info, this);

            if (null != m_rec)
            {
                tbStatelessAddress1.Text = m_rec.StatelessAddress1;
                tbStatelessAddress2.Text = m_rec.StatelessAddress2;
                tbStatelessAddress3.Text = m_rec.StatelessAddress3;
                tbAutoStatefullAddress.Text = m_rec.AutoStatefulAddress;
                tbLinkLocalAddress.Text = m_rec.IPLinkLocalAddress;
                tbAutoConfigureGatewayAddress.Text = m_rec.AutoGatewayAddress;
            }
        }
    }
}
