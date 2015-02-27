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

using System.Windows.Automation.Peers;
using System.Windows.Threading;

namespace VOP
{
    /// <summary>
    /// Interaction logic for SliderChangeControl.xaml
    /// </summary>
    public partial class SliderChangeControl : UserControl
    {
        // Brush 
        SolidColorBrush Brush_10_Percent_Normal = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        SolidColorBrush Brush_Twinkle = new SolidColorBrush(Color.FromRgb(194, 194, 194));
        SolidColorBrush Brush_30_Percent_Normal = new SolidColorBrush(Color.FromRgb(155, 250, 156));
        SolidColorBrush Brush_Normal = new SolidColorBrush(Color.FromRgb(0, 215, 0));

        private DispatcherTimer timer = new DispatcherTimer();

        private int cnt_Twinkle = 0;

        double maximum = 100;
        double currentValue = 0;

        public delegate void dele_slider_ValueChanged(double oldValue, double newValue);

        dele_slider_ValueChanged slider_ValueChanged = null;


        public SliderChangeControl()
        {
            InitializeComponent();

            timer.Interval = TimeSpan.FromMilliseconds(400);
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            cnt_Twinkle++;

            if (cnt_Twinkle % 2 == 0)
            {
                if ((currentValue > 10) && (currentValue <= 30))
                {
                    rect.Fill = Brush_30_Percent_Normal;
                }
                else if ((currentValue >= 0) && (currentValue <= 10))
                {
                    rect.Fill = Brush_10_Percent_Normal;   
                }
            }
            else
            {
                rect.Fill = Brush_Twinkle;
            }
        }


        private void Loaded(object sender, RoutedEventArgs e)
        {
            PaintControl();
        }

        void PaintControl()
        {
            rect2.RadiusX = rect2.RadiusY = (rect2.Width > rect2.Height) ? (rect2.Height / 2) : (rect2.Width / 2);

            rect.Width = rect2.Width * currentValue / maximum;
            {
                if (currentValue > 30)
                {
                    rect.Fill = Brush_Normal;

                    timer.Stop();
                }
                else if ((currentValue > 10) && (currentValue <= 30))
                {
                    rect.Fill = Brush_30_Percent_Normal;

                    timer.Start();

                }
                else if ((currentValue >= 0) && (currentValue <= 10))
                {
                    rect.Fill = Brush_10_Percent_Normal;

                    timer.Start();
                }
            }

            clipRectangle(rect);
        }


        private void clipRectangle(Rectangle rect)
        {
            double BackRadius = (rect2.Width > rect2.Height) ? rect2.Height / 2 : rect2.Width / 2;
            double actualWidth = rect.Width;

            if (actualWidth < 2 * BackRadius)
            {
                if (actualWidth > 0)
                {
                    CombinedGeometry combineGeometry = new CombinedGeometry();

                    EllipseGeometry geometry1 = new EllipseGeometry();
                    geometry1.Center = new Point(BackRadius, rect.Height / 2);
                    geometry1.RadiusX = geometry1.RadiusY = BackRadius;

                    EllipseGeometry geometry2 = new EllipseGeometry();
                    geometry2.Center = new Point(actualWidth - BackRadius, rect.Height / 2);
                    geometry2.RadiusX = geometry2.RadiusY = BackRadius;

                    combineGeometry.Geometry1 = geometry1;
                    combineGeometry.Geometry2 = geometry2;

                    combineGeometry.GeometryCombineMode = GeometryCombineMode.Intersect;

                    rect.Clip = combineGeometry;
                }
                else if (0 == actualWidth)
                {
                    //foreGrid.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                double radius = (rect.Width > rect.Height) ? rect.Height / 2 : rect.Width / 2;
                GeometryGroup clipGeometry3 = new GeometryGroup();
                clipGeometry3.FillRule = FillRule.Nonzero;

                RectangleGeometry rectGeometry = new RectangleGeometry();

                rectGeometry.RadiusX = rectGeometry.RadiusY = radius;

                rectGeometry.Rect = new Rect(0, 0, rect.Width, rect.Height);

                clipGeometry3.Children.Add(rectGeometry);

                rect.Clip = clipGeometry3;
            }
        }


        public double Value
        {
            set
            {
                if ((value >= 0) && (value <= maximum))
                {
                    double oldValue = currentValue;

                    currentValue = value;

                    PaintControl();

                    if (null != slider_ValueChanged)
                        slider_ValueChanged(oldValue, currentValue);
                }
            }
            get { return currentValue; }
        }


        public double Maximum
        {
            set
            {
                if (value >= 0)
                {
                    maximum = value;

                }
            }
            get { return maximum; }
        }


        public dele_slider_ValueChanged Slider_ValueChanged
        {
            set
            {
                if (null != value)
                {
                    slider_ValueChanged = value;

                }
            }
        }

        private void rect2_Loaded(object sender, RoutedEventArgs e)
        {
            rect2.RadiusX = rect2.RadiusY = (rect2.Width > rect2.Height) ? (rect2.Height / 2) : (rect2.Width / 2);

        }
    }
}