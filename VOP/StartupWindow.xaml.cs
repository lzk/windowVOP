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
    public partial class StartupWindow : Window
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public StartupWindow()
        {
            InitializeComponent();

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        public StartupWindow(int timeOutSeconds)
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
    }
}
