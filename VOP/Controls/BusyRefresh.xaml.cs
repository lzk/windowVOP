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
using System.Windows.Threading;

namespace VOP
{
    /// <summary>
    /// Interaction logic for BusyRefresh.xaml
    /// </summary>
    public partial class BusyRefresh : UserControl
    {
        DispatcherTimer timer = new DispatcherTimer();

        public BusyRefresh()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CurRotateAngle += 10;
        }

        private double curRotateAngle = 0.0;
        private double CurRotateAngle
        {
            get { return curRotateAngle; }
            set
            {
                curRotateAngle = value;

                container.RenderTransformOrigin = new Point(0.5, 0.5);
                RotateTransform rotateTransform = new RotateTransform();
                rotateTransform.Angle = curRotateAngle;
                container.RenderTransform = rotateTransform;
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {            
            timer.Start();
        }
    }
}
