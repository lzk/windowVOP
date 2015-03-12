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
    public partial class TonerBar : UserControl
    {
        private const double minValue = 0;
        private const int defaultMilliseconds = 300;
        private double curPercet = 0.0;

        private LinearGradientBrush Brush_10_percent = null;
        private LinearGradientBrush Brush_30_percent = null;
        private LinearGradientBrush Brush_Normal = null;
        private ImageBrush imgBrush_Warn = null;
        private ImageBrush imgBrush_Normal = null;

        public static readonly RoutedEvent valueChangedEvent
            =EventManager.RegisterRoutedEvent("valueChanged", RoutingStrategy.Bubble,
                        typeof(RoutedEventHandler), typeof(TonerBar));

        public event RoutedEventHandler valueChanged
        {
            add { AddHandler(valueChangedEvent, value); }
            remove { RemoveHandler(valueChangedEvent, value); }
        }

        protected virtual void OnCurValueChanged()
        {
            textblock_Tip.Text = string.Format("碳粉容量 ： {0:P0}", CurValue / 100.0);

            // Draw ShopCart Image
            if (curPercet <= 10)
            {
                shopCart_Img.Fill = imgBrush_Warn;
            }
            else
            {
                shopCart_Img.Fill = imgBrush_Normal;
            }           


            RoutedEventArgs argsEvent = new RoutedEventArgs();
            argsEvent.RoutedEvent = TonerBar.valueChangedEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);
        }


        DispatcherTimer timer = new DispatcherTimer();

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(TonerBar),
            new FrameworkPropertyMetadata(100.0, new PropertyChangedCallback(OnValue_Changed)));


        public double CurValue
        {
            get { return (double)GetValue(CurValueProperty); }
            set { SetValue(CurValueProperty, value); }
        }
        public static readonly DependencyProperty CurValueProperty =
            DependencyProperty.Register("CurValue", typeof(double), typeof(TonerBar),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnValue_Changed)));



        private static void OnValue_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.TonerBar _this = (VOP.TonerBar)sender;

            _this.curPercet = _this.CurValue * 100.0 / _this.MaxValue;

            _this.PaintControl();

            if(e.Property == CurValueProperty)
            {
                _this.OnCurValueChanged();
            }
        }

        void PaintControl()
        {           
  
            double totalWidth = TonerBackground.RenderSize.Width;
            double percentWidth = totalWidth * curPercet / 100.0;

            TonerPercent.Width = percentWidth;

            if (curPercet <= 10)
            {
                TonerPercent.Background = Brush_10_percent;

                timer.Start();
            }
            else if (curPercet <= 30)
            {
                TonerPercent.Background = Brush_30_percent;

                timer.Start();
            }
            else if (curPercet <= 60)
            {
                TonerPercent.Background = Brush_Normal;
            }            
        }


        public TonerBar()
        {
            InitializeComponent();
            Init();

           // this.Loaded += TonerBar_Loaded;

            timer.Interval = new TimeSpan(0, 0, 0, 0, defaultMilliseconds);
            timer.Tick += new EventHandler(timer_Tick);
        }


        private int shrinkCnts = 0;
        void timer_Tick(object sender, EventArgs e)
        {
            ++shrinkCnts;

            if(0 == shrinkCnts%2)
            {
                if (curPercet <= 10)
                {
                    TonerPercent.Background = Brush_10_percent;
                }
                else if (curPercet <= 30)
                {
                    TonerPercent.Background = Brush_30_percent;
                }     
            }
            else
            {
                TonerPercent.Background = Brush_Normal;
            }

        }

   
        public void Init()
        {
            Brush_10_percent = (LinearGradientBrush)this.FindResource("Brush_10");
            Brush_30_percent = (LinearGradientBrush)this.FindResource("Brush_30");
            Brush_Normal = (LinearGradientBrush)this.FindResource("Brush_Normal");

            imgBrush_Warn = (ImageBrush)this.FindResource("imgBrush_Warn"); ;
            imgBrush_Normal = (ImageBrush)this.FindResource("imgBrush_Normal"); ;

            OnCurValueChanged();
        }
    }
}