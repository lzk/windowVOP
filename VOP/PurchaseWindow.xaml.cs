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
using System.Windows.Interop;

namespace VOP
{
    /// <summary>
    /// Interaction logic for PurchaseWindow.xaml
    /// </summary>
    public partial class PurchaseWindow : Window
    {
        MerchantInfoSet m_MerchantInfoSet = new MerchantInfoSet();
        List<string> m_listProvince = new List<string>();
        List<KeyValuePair<string, string>> m_listProvinceCity = new List<KeyValuePair<string, string>>();       
        bool m_bInit = false;
        int  m_nExpiredMonth = 0; 
       
        public PurchaseWindow()
        {
            InitializeComponent();
            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void OnLoadWindow(object sender, RoutedEventArgs e)
        {
            MerchantList.Children.Clear();
            MerchantInfoSet maintainSet = new MerchantInfoSet();

            bool bSuccess = false;

            m_MerchantInfoSet.Clear();
            m_listProvince.Clear();
            m_listProvinceCity.Clear();
            string strResult = "";

            if (true == VOP.MainWindow.m_RequestManager.GetMerchantSet(0, 5, ref maintainSet, ref strResult))
            {
                int nTotalCount = maintainSet.m_nTotalCount;

                if (true == VOP.MainWindow.m_RequestManager.GetMerchantSet(0, nTotalCount, ref m_MerchantInfoSet, ref strResult))
                {
                    bSuccess = true;
                }
            }

            if (!bSuccess)
            {
                m_MerchantInfoSet.Clear();

                DateTime dtSaveTime = new DateTime();
                string strMerchantInfo = "";
                string strMaintainInfo = "";
                if (true == VOP.MainWindow.ReadCRMDataFromXamlFile("Merchant.xaml", ref dtSaveTime, ref strMerchantInfo))
                {
                    DateTime newDate = DateTime.Now;
                    TimeSpan ts = newDate - dtSaveTime;
                    int differenceInDays = ts.Days;
                    m_nExpiredMonth = differenceInDays / 30;

                    if (m_nExpiredMonth >= 3)
                        Win32.PostMessage((IntPtr)0xffff, App.WM_CHECK_MERCHANT_INFO_Expired, IntPtr.Zero, IntPtr.Zero);
                }
                else
                {
                    VOP.MainWindow.ReadInfoDataFromXamlFile(ref strMerchantInfo, ref strMaintainInfo);
                    VOP.MainWindow.SaveCRMDataIntoXamlFile("Merchant.xaml", DateTime.Now, strResult);
                }

                VOP.MainWindow.m_RequestManager.ParseJsonData<MerchantInfoSet>(strMerchantInfo, JSONReturnFormat.MerchantInfoSet, ref m_MerchantInfoSet);
            }
            else
            {
                VOP.MainWindow.SaveCRMDataIntoXamlFile("Merchant.xaml", DateTime.Now, strResult);
            }

            for (int nIdx = 0; nIdx < m_MerchantInfoSet.m_nTotalCount; nIdx++)
            {
                if (!m_listProvince.Contains(m_MerchantInfoSet.m_listMerchantInfo[nIdx].m_strProvince))
                {
                    m_listProvince.Add(m_MerchantInfoSet.m_listMerchantInfo[nIdx].m_strProvince);
                }

                KeyValuePair<string, string> pair = new KeyValuePair<string, string>(m_MerchantInfoSet.m_listMerchantInfo[nIdx].m_strProvince, m_MerchantInfoSet.m_listMerchantInfo[nIdx].m_strCity);
                if (!m_listProvinceCity.Contains(pair))
                {
                    m_listProvinceCity.Add(pair);
                }
            }

            for (int nIdx = 0; nIdx < m_listProvince.Count; nIdx++)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = m_listProvince[nIdx];
                item.Width = 80;
                item.IsSelected = false;
                item.Style = (Style)this.FindResource("customComboBoxItem");
                cboProvince.Items.Add(item);
            }

            cboCity.IsEnabled = false;
            m_bInit = true;

            AddMessageHook();
        }

        private System.IntPtr _handle = IntPtr.Zero;
        public System.IntPtr WindowHandle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                    _handle = (new WindowInteropHelper(App.Current.MainWindow)).Handle;
                return _handle;
            }
        }

        private void AddMessageHook()
        {
            HwndSource src = HwndSource.FromHwnd(WindowHandle);
            src.AddHook(new HwndSourceHook(this.WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            System.Windows.Forms.Message m = new System.Windows.Forms.Message();
            m.HWnd = hwnd;
            m.Msg = msg;
            m.WParam = wParam;
            m.LParam = lParam;

            if (handled)
                return IntPtr.Zero;

            if (msg == App.WM_CHECK_MERCHANT_INFO_Expired)
            {
                if (m_nExpiredMonth >= 3)
                {
                    try
                    {
                        string strMessage = String.Format("经销商信息已经{0}个月未更新，为保证信息的准确性，请在线更新。", m_nExpiredMonth);
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple, this, strMessage, (string)this.FindResource("ResStr_Warning"));
                    }
                    catch
                    {

                    }
                }
            }
          
            return IntPtr.Zero;
        }

        private void cboProvince_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.Name == "cboProvince")
            {
                if (m_bInit)
                {
                    cboCity.Items.Clear();
                    if (-1 != cb.SelectedIndex)
                    {
                        cboCity.IsEnabled = true;
                    }

                    List<string> listCity = new List<string>();
                    for (int nIdx = 0; nIdx < m_listProvinceCity.Count; nIdx++)
                    {
                        if (m_listProvinceCity[nIdx].Key == ((ComboBoxItem)cb.SelectedItem).Content.ToString())
                        {
                            if (!listCity.Contains(m_listProvinceCity[nIdx].Value))
                            {
                                listCity.Add(m_listProvinceCity[nIdx].Value);
                            }
                        }
                    }

                    foreach (string str in listCity)
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = str;
                        item.Width = 80;
                        item.IsSelected = false;
                        item.Style = (Style)this.FindResource("customComboBoxItem");
                        cboCity.Items.Add(item);
                    }
                }
            }
            else if (cb.Name == "cboCity")
            {
                MerchantList.Children.Clear();

                if (cb.Items.Count > 0 && cboProvince.Items.Count > 0)
                {
                    foreach (MerchantInfoItem item in m_MerchantInfoSet.m_listMerchantInfo)
                    {
                        if (item.m_strProvince == ((ComboBoxItem)cboProvince.SelectedItem).Content.ToString() &&
                            item.m_strCity == ((ComboBoxItem)cboCity.SelectedItem).Content.ToString())
                        {
                            MerchantInfo maintainInfo = new MerchantInfo(item.m_strCompanyName, "电话：" + item.m_strPhone);
                            MerchantList.Children.Add(maintainInfo);
                        }
                    }
                }
            }
        }

        private void ButtonEx_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://www.jd.com");
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            HwndSource src = HwndSource.FromHwnd(WindowHandle);
            src.RemoveHook(new HwndSourceHook(this.WndProc));  
        }
    }
}
