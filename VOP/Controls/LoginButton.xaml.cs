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
    /// Interaction logic for LoginButton.xaml
    /// </summary>
    public partial class LogonButton : UserControl
    {
        bool isMouseReallyOver;
        public static readonly RoutedEvent btnClickEvent;
        public static readonly DependencyProperty bottomTextProperty;
        public static readonly DependencyProperty IsLogonProperty;

        private ImageBrush tmpImg = null;
        private ImageBrush imgNotLogon = null;
        private ImageBrush imgLogon = null;
        private ImageBrush imgLogonPressed = null;

        public bool IsLogon
        {
            get { return (bool)GetValue(IsLogonProperty); }
            set { SetValue(IsLogonProperty, value); }
        }

        public string bottomText
        {
            get { return (string)GetValue(bottomTextProperty); }
            set { SetValue(bottomTextProperty, value); }
        }

        public LogonButton()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            imgNotLogon = (ImageBrush)this.FindResource("imgNotLogon");
            imgLogon = (ImageBrush)this.FindResource("imgLogon");
            imgLogonPressed = (ImageBrush)this.FindResource("imgLogonPressed");
        }

        static LogonButton()
        {
            btnClickEvent =
                EventManager.RegisterRoutedEvent("btnClick", RoutingStrategy.Bubble,
                        typeof(RoutedEventHandler), typeof(LogonButton));

            bottomTextProperty =
                DependencyProperty.Register("bottomText", typeof(string), typeof(LogonButton));

            IsLogonProperty =
                DependencyProperty.Register("IsLogon", typeof(bool), typeof(LogonButton),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsLogon_Changed)));
        }


        private static void OnIsLogon_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.LogonButton _this = sender as VOP.LogonButton;
            if (null == _this) return;

            _this.LoginStatus_Changed();
        }

        private void LoginStatus_Changed()
        {
            if (IsLogon)
            {
                rect_Image.Fill = imgLogon;
            }
            else
            {
                rect_Image.Fill = imgNotLogon;
            }
        }


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

            tmpImg = (ImageBrush)rect_Image.Fill;
            btn.Focus();
            rect_Image.Fill = imgLogonPressed;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonUp(args);

            rect_Image.Fill = tmpImg;

            if (IsMouseCaptured)
            {
                if (isMouseReallyOver)
                {
                    OnYclick();
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

        protected override void OnKeyDown(KeyEventArgs args)
        {
            base.OnKeyDown(args);
            if (args.Key == Key.Space || args.Key == Key.Enter)
                args.Handled = true;
        }

        protected override void OnKeyUp(KeyEventArgs args)
        {
            base.OnKeyUp(args);
            if (args.Key == Key.Space || args.Key == Key.Enter)
            {
                OnYclick();
                args.Handled = true;
            }
        }

        protected virtual void OnYclick()
        {
            RoutedEventArgs argsEvent = new RoutedEventArgs();
            argsEvent.RoutedEvent = LogonButton.btnClickEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);
        }
    }
}
