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
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Threading;

namespace VOP
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class ScanWaitWindow_Rufous : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public ScanWaitWindow_Rufous()
        {
            InitializeComponent();

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        public ScanWaitWindow_Rufous(int timeOutSeconds)
        {
            InitializeComponent();

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, timeOutSeconds);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //add by yunying shang 2017-10-12 for BMS1082 and 842
            if (ScanTask.WM_VOPSCAN_PROGRESS == msg)
            {
                handled = true;
                // progressBar1.Value = wParam.ToInt32();
                busyScan.BusyContent = "Scanning...";
            }
            else
                if (ScanTask.WM_VOPSCAN_UPLOAD == msg)
            {
                handled = true;
                busyScan.BusyContent = "Uploading...";
            }
            else if (ScanTask.WM_VOPSCAN_PAGECOMPLETE == msg)
            {
                string str = string.Format("Page {0} finished.", wParam.ToInt32());
                //if (wParam.ToInt32() == 0)
                //{
                //    str = string.Format("Page 1 finished.");
                //}

                handled = true;
                busyScan.BusyContent = str;                
            }
            ///////////////////////////////
            return IntPtr.Zero;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            dll.ADFCancel();
            //add by yunying shang 2017-12-11 for BMS 1740
            Thread.Sleep(200);
            this.Close();
            //<<===================
        }
    }
}
