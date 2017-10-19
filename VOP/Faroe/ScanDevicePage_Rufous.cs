using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage
using VOP.Controls;
using Microsoft.Win32;
using System.Collections.Generic;

namespace VOP
{
    public partial class ScanDevicePage_Rufous : UserControl
    {
        private enum ScanSelectionState { Exit }

        public MainWindow_Rufous m_MainWin { get; set; }
        public static string[] ipList = null;
        private static object lockobj = new object();

        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connectionDescription, int reservedValue);


        public ScanDevicePage_Rufous()
        {
            InitializeComponent();
            btnConnect.IsEnabled = false; //add by yunying shang for BMS1018
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDeviceList(false);

            if (MainWindow_Rufous.g_settingData.m_DeviceName == "")
            {
                DeviceList.SelectedIndex = 0;
                OnConnected();
            }
        }


        public static bool ListIP()
        {
            lock(lockobj)
            {
                if (dll.SearchValidedIP2(out ipList) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
          
        }

        public bool IsOnLine()
        {
            try
            {
                var connection = 0;
                return InternetGetConnectedState(out connection, 0);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void FillDeviceList(bool forceRefresh)
        {
            bool canConnected = false;
            DeviceList.Items.Clear();
            m_MainWin.SetDeviceButtonState(false);

            btnConnect.IsEnabled = false; //add by yunying shang for BMS1018
            StringBuilder usbName = new StringBuilder(50);
            if(dll.CheckUsbScan(usbName) == 1)
            {
                DeviceListBoxItem item = new DeviceListBoxItem();
                item.DeviceName = usbName.ToString();

                if (MainWindow_Rufous.g_settingData.m_DeviceName == item.DeviceName)
                {
                    item.StatusText = "Connected";
                    dll.SetConnectionMode(item.DeviceName, true);
                    m_MainWin.SetDeviceButtonState(true);
                    canConnected = true;
                    MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                }
                else
                {
                    item.StatusText = "";
                }

                DeviceList.Items.Add(item);

            }

            if(forceRefresh == true)
            {
                //add by yunying shnag 2017-10-16 for BMS 1165
                if (IsOnLine() == false)
                {
                    VOP.Controls.MessageBoxEx.Show(MessageBoxExStyle.Simple, Application.Current.MainWindow,
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Faroe_not_on_line"), 
                        (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                }//===============1165
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                worker.InvokeQuickScanMethod(ListIP, (string)this.TryFindResource("ResStr_Faroe_search_dev"));
            }
            else
            {
                if (dll.CheckUsbScan(usbName) == 0 && (ipList == null || ipList.Length == 0 ))
                {
                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                    worker.InvokeQuickScanMethod(ListIP, (string)this.TryFindResource("ResStr_Faroe_search_dev"));
                }
            }


            if (ipList != null)
            {
                //modified by yunying shang 2017-10-17 for BMS 1175
                string connectip = string.Empty;
                     
                foreach (string ip in ipList)
                {
                    DeviceListBoxItem item = new DeviceListBoxItem();
                    item.DeviceName = ip;

                    if (MainWindow_Rufous.g_settingData.m_DeviceName == ip)
                    {
                        item.StatusText = "Connected";
                        dll.SetConnectionMode(ip, false);
                        m_MainWin.SetDeviceButtonState(true);
                        canConnected = true;
                        DeviceList.Items.Add(item);
                        connectip = ip;
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                        break;
                    }                    
                }


                foreach (string ip in ipList)
                {
                    DeviceListBoxItem item = new DeviceListBoxItem();
                    item.DeviceName = ip;

                    if (connectip != ip)
                    {
                        item.StatusText = "";
                        DeviceList.Items.Add(item);
                    }
                }//<<=================1175
            }

            if (canConnected == false)
            {
                foreach (DeviceListBoxItem item in DeviceList.Items)
                {
                    if (dll.CheckUsbScan(usbName) == 1)
                    {
                        if (item.DeviceName == usbName.ToString())
                        {
                            btnConnect.IsEnabled = true; //add by yunying shang for BMS1018
                            item.StatusText = "Connected";
                            dll.SetConnectionMode(item.DeviceName, true);
                            m_MainWin.SetDeviceButtonState(true);
                            MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                            break;
                        }
                    }
                    else
                    {
                        if (ScanDevicePage_Rufous.ipList != null)
                        {
                            if (item.DeviceName == ScanDevicePage_Rufous.ipList[0])
                            {
                                btnConnect.IsEnabled = true; //add by yunying shang for BMS1018
                                item.StatusText = "Connected";
                                dll.SetConnectionMode(item.DeviceName, false);
                                m_MainWin.SetDeviceButtonState(true);
                                MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                btnConnect.IsEnabled = true; //add by yunying shang for BMS1018
            }
        }

        private void RefreshClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FillDeviceList(true);
        }
        private void OkClick(object sender, RoutedEventArgs e)
        {
            OnConnected();
        }

        private void OnConnected()
        {
            if (DeviceList.SelectedIndex == -1)
            {
                return;
            }

            m_MainWin.SetDeviceButtonState(false);

            foreach (DeviceListBoxItem item in DeviceList.Items)
            {
                if (item.Equals(DeviceList.SelectedItem as DeviceListBoxItem))
                {
                    item.StatusText = "Connected";
                    MainWindow_Rufous.g_settingData.m_DeviceName = item.DeviceName;

                    if (item.DeviceName.ToLower().Contains("usb"))
                    {
                        dll.SetConnectionMode(item.DeviceName, true);
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = true;
                    }
                    else
                    {
                        dll.SetConnectionMode(item.DeviceName, false);
                        MainWindow_Rufous.g_settingData.m_isUsbConnect = false;
                    }

                    m_MainWin.SetDeviceButtonState(true);
                }
                else
                {
                    item.StatusText = "";
                }
            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            m_MainWin.GotoPage("ScanSelectionPage", null);        

        }
    }
}
