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

            if (ScanTask.WM_VOPSCAN_PROGRESS == msg)
            {
                handled = true;

                //progressBar1.Value = wParam.ToInt32();
            }
        
            return IntPtr.Zero;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            dll.ADFCancel();
        }
    }
}
