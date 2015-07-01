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

namespace VOP
{
    /// <summary>
    /// Interaction logic for customTabItem.xaml
    /// </summary>
    /// 

    public enum CustomTabItemStyle
    {
        Left,
        Middle,
        Right,
        Single
    }

    public partial class customTabItem : UserControl
    {
        public static readonly DependencyProperty IsSelectProperty;
        
        public bool IsSelect
        {
            get { return (bool)GetValue(IsSelectProperty); }
            set { SetValue(IsSelectProperty, value); }
        }

        public static readonly DependencyProperty TextProperty;
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public CustomTabItemStyle tabItemStyle
        {
            get { return (CustomTabItemStyle)GetValue(tabItemStyleProperty); }
            set { SetValue(tabItemStyleProperty, value); }
        }

        public static readonly DependencyProperty tabItemStyleProperty =
            DependencyProperty.Register("tabItemStyle", typeof(CustomTabItemStyle), typeof(customTabItem),
                new FrameworkPropertyMetadata(CustomTabItemStyle.Middle, new PropertyChangedCallback(OntabIemStyle_Changed)));

        private static void OntabIemStyle_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.customTabItem _this = sender as VOP.customTabItem;

            if (CustomTabItemStyle.Left == _this.tabItemStyle)
            {
                _this.Left.Visibility = Visibility.Visible;
                _this.Middle.Visibility = Visibility.Hidden;
                _this.Right.Visibility = Visibility.Hidden;
                _this.Single.Visibility = Visibility.Hidden;

            }
            else if (CustomTabItemStyle.Middle == _this.tabItemStyle)
            {
                _this.Left.Visibility = Visibility.Hidden;
                _this.Middle.Visibility = Visibility.Visible;
                _this.Right.Visibility = Visibility.Hidden;
                _this.Single.Visibility = Visibility.Hidden;
            }
            else if (CustomTabItemStyle.Right == _this.tabItemStyle)
            {
                _this.Left.Visibility = Visibility.Hidden;
                _this.Middle.Visibility = Visibility.Hidden;
                _this.Right.Visibility = Visibility.Visible;
                _this.Single.Visibility = Visibility.Hidden;
            }
            else if (CustomTabItemStyle.Single == _this.tabItemStyle)
            {
                _this.Left.Visibility = Visibility.Hidden;
                _this.Middle.Visibility = Visibility.Hidden;
                _this.Right.Visibility = Visibility.Hidden;
                _this.Single.Visibility = Visibility.Visible;
            }
        }
        
        private static void OnIsSelect_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as customTabItem).ReDrawCtrl();
        }

        static customTabItem()
        {
            FrameworkPropertyMetadata IsSecectMetaData = new FrameworkPropertyMetadata();
            IsSecectMetaData.DefaultValue = false;
            IsSecectMetaData.PropertyChangedCallback += new PropertyChangedCallback(OnIsSelect_Changed);
            IsSelectProperty =
                DependencyProperty.Register(
                "IsSelect",
                typeof(Boolean),
                typeof(customTabItem),
                IsSecectMetaData);

            TextProperty =
                DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(customTabItem));

            IsEnabledProperty.OverrideMetadata(typeof(customTabItem),
            new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsEnabled_Changed)));

            btnClickEvent =
               EventManager.RegisterRoutedEvent("btnClick", RoutingStrategy.Bubble,
                       typeof(RoutedEventHandler), typeof(customTabItem));
        }

        private static void OnIsEnabled_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as customTabItem).ReDrawCtrl();
        }

        private void ReDrawCtrl()
        {
            if (IsEnabled)
            {
                if (IsSelect) // On
                {
                    rectLeft_1.Fill = rectLeft_2.Fill =
                    rectMiddle.Fill =
                    rectRight_1.Fill = rectRight_2.Fill =
                    rectSingle_1.Fill = rectSingle_2.Fill = rectSingle_3.Fill =
                    (Brush)this.FindResource("selectedBrush");

                    text.Foreground = new SolidColorBrush(Colors.White);
                }
                else // Default: Off
                {
                    rectLeft_1.Fill = rectLeft_2.Fill =
                    rectMiddle.Fill =
                    rectRight_1.Fill = rectRight_2.Fill =
                    rectSingle_1.Fill = rectSingle_2.Fill = rectSingle_3.Fill =
                   new SolidColorBrush(Colors.White);

                    text.Foreground = new SolidColorBrush(Color.FromRgb(0x2A, 0x2A, 0x2A));
                }
            }
            else
            {
                rectLeft_1.Fill = rectLeft_2.Fill =
                   rectMiddle.Fill =
                   rectRight_1.Fill = rectRight_2.Fill =
                   rectSingle_1.Fill = rectSingle_2.Fill = rectSingle_3.Fill =
                  new SolidColorBrush(Colors.White);

                text.Foreground = new SolidColorBrush(Color.FromRgb(0xCE, 0xCE, 0xCE));
            }
        }

        public customTabItem()
        {
            InitializeComponent();
        }


        #region Add ClickEvent Process 
        bool isMouseReallyOver;
        public static readonly RoutedEvent btnClickEvent;

        public event RoutedEventHandler btnClick
        {
            add { AddHandler(btnClickEvent, value); }
            remove { RemoveHandler(btnClickEvent, value); }
        }

        protected override void OnMouseEnter(MouseEventArgs args)
        {
            base.OnMouseEnter(args);
            InvalidateVisual();
        }
        protected override void OnMouseLeave(MouseEventArgs args)
        {
            base.OnMouseLeave(args);
            InvalidateVisual();
        }
        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            Point pt = args.GetPosition(this);
            bool isReallyOverNow = (pt.X >= 0 && pt.X < ActualWidth &&
                                    pt.Y >= 0 && pt.Y < ActualHeight);
            if (isReallyOverNow != isMouseReallyOver)
            {
                isMouseReallyOver = isReallyOverNow;
                InvalidateVisual();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonDown(args);
            CaptureMouse();
            InvalidateVisual();
            args.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonUp(args);

            if (IsMouseCaptured)
            {
                if (isMouseReallyOver)
                {
                    OnClick();
                }
                args.Handled = true;
                Mouse.Capture(null);
            }
        }

        protected override void OnLostMouseCapture(MouseEventArgs args)
        {
            base.OnLostMouseCapture(args);
            InvalidateVisual();
        }

        protected virtual void OnClick()
        {
            RoutedEventArgs argsEvent = new RoutedEventArgs();
            argsEvent.RoutedEvent = customTabItem.btnClickEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);
        }
        #endregion
    }
}
