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
    public partial class LoginButton : UserControl
    {
        bool isMouseReallyOver;
        public static readonly RoutedEvent btnClickEvent;
        public static readonly DependencyProperty ImageSourceProperty;
        public static readonly DependencyProperty bottomTextProperty;

        public string bottomText
        {
            get { return (string)GetValue(bottomTextProperty); }
            set { SetValue(bottomTextProperty, value); }
        }


        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public LoginButton()
        {
            InitializeComponent();
        }

        static LoginButton()
        {
            btnClickEvent =
                EventManager.RegisterRoutedEvent("btnClick", RoutingStrategy.Bubble,
                        typeof(RoutedEventHandler), typeof(LoginButton));

            ImageSourceProperty =
                DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(LoginButton));

            bottomTextProperty =
                DependencyProperty.Register("bottomText", typeof(string), typeof(LoginButton));
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
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs args)
        {
            base.OnMouseLeftButtonUp(args);

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
            argsEvent.RoutedEvent = LoginButton.btnClickEvent;
            argsEvent.Source = this;
            RaiseEvent(argsEvent);
        }
    }
}
