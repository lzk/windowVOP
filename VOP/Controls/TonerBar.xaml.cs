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
        #region Data_Member
        private const double minValue = 0;
        private double curPercet = 0.0;

        private LinearGradientBrush Brush_10_percent = null;
        private LinearGradientBrush Brush_30_percent = null;
        private LinearGradientBrush Brush_Normal = null;
        private ImageBrush imgBrush_Warn = null;
        private ImageBrush imgBrush_Normal = null;
        private ImageBrush imgBrush_Disable = null;

        DispatcherTimer timer = new DispatcherTimer();
       
        #endregion //Data_Member

        #region Event
        public static readonly RoutedEvent valueChangedEvent
            = EventManager.RegisterRoutedEvent("valueChanged", RoutingStrategy.Bubble,
                        typeof(RoutedEventHandler), typeof(TonerBar));

        public event RoutedEventHandler valueChanged
        {
            add { AddHandler(valueChangedEvent, value); }
            remove { RemoveHandler(valueChangedEvent, value); }
        }

        public static readonly RoutedEvent ClickEvent
            = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(TonerBar));

        public event RoutedEventHandler Click
        {
            add { AddHandler(ClickEvent, value); }
            remove { RemoveHandler(ClickEvent, value); }
        }
        #endregion // Event


        #region Property
        public int MilliSeconds
        {
            get { return (int)GetValue(MilliSecondsProperty); }
            set { SetValue(MilliSecondsProperty, value); }
        }

        public static readonly DependencyProperty MilliSecondsProperty =
            DependencyProperty.Register("MilliSeconds", typeof(int), typeof(TonerBar),
           new FrameworkPropertyMetadata(600, new PropertyChangedCallback(OnMilliSeconds_Changed)));

        private static void OnMilliSeconds_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.TonerBar _this = (VOP.TonerBar)sender;
            _this.timer.Interval = new TimeSpan(0, 0, 0, 0, _this.MilliSeconds);
        }

        
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
            _this.ValueChanged();
        }

        private double oldValue = 0.0;
        protected virtual void ValueChanged()
        {
            curPercet = CurValue * 100.0 / MaxValue;
            PaintControl();

            RoutedEventArgs argsEvent = new RoutedEventArgs();
            argsEvent.RoutedEvent = TonerBar.valueChangedEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);

          

           
        }

        public bool IsEnabled
        {
            get { return (bool)GetValue(IsEnabledProperty); }
            set { SetValue(IsEnabledProperty, value); }
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(TonerBar),
             new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsEnable_Changed)));

        private static void OnIsEnable_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.TonerBar _this = (VOP.TonerBar)sender;
            _this.PaintControl();
        }




        public bool IsShinkle
        {
            get { return (bool)GetValue(IsShinkleProperty); }
            set { SetValue(IsShinkleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShinkle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShinkleProperty =
            DependencyProperty.Register("IsShinkle", typeof(bool), typeof(TonerBar),
            new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsShinkle_Changed)));

        private static void OnIsShinkle_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.TonerBar _this = (VOP.TonerBar)sender;
            _this.IsShinkle_Changed();
        }

        private void IsShinkle_Changed()
        {
            if(IsShinkle)
            {
                timer.Start();     
            }
            else
            {
                shopCart_Img.Opacity = 1.0;
                timer.Stop();     
            }
        }
        

        #endregion // Property

        void PaintControl()
        {
            double totalWidth = TonerBackground.RenderSize.Width;
            double percentWidth = totalWidth * curPercet / 100.0;

            TonerPercent.Width = percentWidth;

            if (curPercet <= 10)
            {
                TonerPercent.Background = Brush_10_percent;           
            }
            else if (curPercet <= 30)
            {
                TonerPercent.Background = Brush_30_percent;          
            }
            else
            {
                TonerPercent.Background = Brush_Normal;
            }
           

            textblock_Tip.Text = string.Format("碳粉容量 ： {0:P0}", CurValue / 100.0);

            // Draw ShopCart Image
            if(IsEnabled)
            {
                if (curPercet <= 10)
                {
                    shopCart_Img.Fill = imgBrush_Warn;
                }
                else
                {
                    shopCart_Img.Fill = imgBrush_Normal;
                }

                if ((oldValue != curPercet) && ((10.0 == curPercet) || (20.0 == curPercet) || (30.0 == curPercet)))
                {
                    IsShinkle = true;

                    RaiseClickEvent();                    
                }

                oldValue = curPercet;
       
            }
            else
            {
                shopCart_Img.Fill = imgBrush_Disable;

                IsShinkle = false;
                textblock_Tip.Text = "碳粉容量 ：";
            }         
        }
        public TonerBar()
        {
            InitializeComponent();
            Init();
        }
        void timer_Tick(object sender, EventArgs e)
        {
            const double opacity = 0.1;

            if (opacity == shopCart_Img.Opacity)
            {
                shopCart_Img.Opacity = 1.0;
            }
            else
            {
                shopCart_Img.Opacity = opacity;
            }         
        }

        public void Init()
        {
            Brush_10_percent = (LinearGradientBrush)this.FindResource("Brush_10");
            Brush_30_percent = (LinearGradientBrush)this.FindResource("Brush_30");
            Brush_Normal = (LinearGradientBrush)this.FindResource("Brush_Normal");

            imgBrush_Warn = (ImageBrush)this.FindResource("imgBrush_Warn");
            imgBrush_Normal = (ImageBrush)this.FindResource("imgBrush_Normal");
            imgBrush_Disable = (ImageBrush)this.FindResource("imgBrush_Disable");

            timer.Interval = new TimeSpan(0, 0, 0, 0, MilliSeconds);
            timer.Tick += new EventHandler(timer_Tick);

            this.Loaded += TonerBar_Loaded;
        }

        void TonerBar_Loaded(object sender, RoutedEventArgs e)
        {
            PaintControl();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(IsEnabled)
            {
                shopCart_Img.Opacity = 1.0;

                RaiseClickEvent();
               

               
            }           
        }

        private void RaiseClickEvent()
        {
            RoutedEventArgs clickEventArgs = new RoutedEventArgs();
            clickEventArgs.RoutedEvent = TonerBar.ClickEvent;
            clickEventArgs.Source = this;
            RaiseEvent(clickEventArgs);

            IsShinkle = false;
        }
    }
}
