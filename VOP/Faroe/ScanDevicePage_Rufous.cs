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

        public ScanDevicePage_Rufous()
        {
            InitializeComponent();
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

        private void FillDeviceList(bool forceRefresh)
        {
            DeviceList.Items.Clear();
            m_MainWin.SetDeviceButtonState(false);

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
                }
                else
                {
                    item.StatusText = "";
                }

                DeviceList.Items.Add(item);
            }

            if(forceRefresh == true)
            {
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                worker.InvokeQuickScanMethod(ListIP, "Searching scan device...");
            }
            else
            {
                if (ipList == null || ipList.Length == 0)
                {
                    AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                    worker.InvokeQuickScanMethod(ListIP, "Searching scan device...");
                }
            }
           

            if (ipList != null)
            {
                foreach (string ip in ipList)
                {
                    DeviceListBoxItem item = new DeviceListBoxItem();
                    item.DeviceName = ip;

                    if (MainWindow_Rufous.g_settingData.m_DeviceName == ip)
                    {
                        item.StatusText = "Connected";
                        dll.SetConnectionMode(ip, false);
                        m_MainWin.SetDeviceButtonState(true);
                    }
                    else
                    {
                        item.StatusText = "";
                    }
                  

                    DeviceList.Items.Add(item);
                }
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
                    }
                    else
                    {
                        dll.SetConnectionMode(item.DeviceName, false);
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
