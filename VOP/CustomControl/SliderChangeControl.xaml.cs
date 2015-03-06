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

      //  public delegate void dele_slider_ValueChanged(double oldValue, double newValue);
     //   dele_slider_ValueChanged slider_ValueChanged = null;




        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }


        public static readonly DependencyProperty MaximumProperty;



        public double Minimum
        {
            get { return ( double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        
        public static readonly DependencyProperty MinimumProperty;

        

        


 
        public static readonly DependencyProperty CurValueProperty;
        public double CurValue
        {
            get { return (double)GetValue(CurValueProperty); }
            set { SetValue(CurValueProperty, value); }
        }
        static SliderChangeControl()
        {
            CurValueProperty =
                DependencyProperty.Register("CurValue",
                typeof(double),
                typeof(SliderChangeControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCurValue_Changed)));

            MaximumProperty =
                DependencyProperty.Register("Maximum",
                typeof(double),
                typeof(SliderChangeControl),
                 new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCurValue_Changed)));


            MinimumProperty =
                DependencyProperty.Register("Minimum", 
                typeof(double),
                typeof(SliderChangeControl),
                 new FrameworkPropertyMetadata(new PropertyChangedCallback(OnCurValue_Changed)));

        }
        private static void OnCurValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.SliderChangeControl _This = (VOP.SliderChangeControl)sender;

            double newValue = (double)e.NewValue;

            if (e.Property == CurValueProperty)
            {
                if ((newValue >= 0) && (newValue <= _This.maximum))
                {
                    _This.currentValue = newValue;
                    _This.PaintControl();
                }
            }
            else if (e.Property == MaximumProperty)
            {
                if (newValue >= _This.Minimum)
                {
                    _This.maximum = newValue;
                    _This.PaintControl();
                }
            }
            else if (e.Property == MinimumProperty)
            {
                if(newValue <= _This.maximum)
                {
                    _This.Minimum = newValue;
                    _This.PaintControl();
                }

            }
        }   


        public SliderChangeControl()
        {
            InitializeComponent();

            maximum = 100;
            currentValue = 0;

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
        private void rect2_Loaded(object sender, RoutedEventArgs e)
        {
            rect2.RadiusX = rect2.RadiusY = (rect2.Width > rect2.Height) ? (rect2.Height / 2) : (rect2.Width / 2);

        }
    }
}