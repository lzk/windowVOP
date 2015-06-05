﻿using System;
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
    /// Interaction logic for RefreshButton.xaml
    /// </summary>
    public partial class RefreshButton : UserControl
    {
        DispatcherTimer timer = new DispatcherTimer();
        public bool IsRefresh
        {
            get { return (bool)GetValue(IsRefreshProperty); }
            set { SetValue(IsRefreshProperty, value); }
        }

        public static readonly DependencyProperty IsRefreshProperty =
            DependencyProperty.Register("IsRefresh", typeof(bool), typeof(RefreshButton),
             new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsRefresh_Changed)));

        public ImageSource ImagePath
        {
            get { return (ImageSource)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(ImageSource), typeof(RefreshButton));

        private static void OnIsRefresh_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.RefreshButton _This = sender as VOP.RefreshButton;
            if (null == _This) return;
            if (_This.IsRefresh)
            {
                _This.timer.Start();
                _This.ImagePath = new BitmapImage(new Uri("pack://application:,,,/" + @"../Images/RefreshClick.tif", UriKind.RelativeOrAbsolute));
            }
            else
            {
                _This.timer.Stop();
                _This.ImagePath = new BitmapImage(new Uri("pack://application:,,,/" + @"../Images/RefreshNormal.tif", UriKind.RelativeOrAbsolute));
                _This.CurRotateAngle = 0;
            }
        }

        public RefreshButton()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer.Tick += new EventHandler(timer_Tick);

            ImagePath = new BitmapImage(new Uri("pack://application:,,,/" + @"../Images/RefreshNormal.tif", UriKind.RelativeOrAbsolute));
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
    }
    public class RefreshImagePath
    {

        public static readonly DependencyProperty ImageProperty;

        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }

        static RefreshImagePath()
        {
            //register attached dependency property
            var metadata = new FrameworkPropertyMetadata((ImageSource)null);
            ImageProperty = DependencyProperty.RegisterAttached("Image",
                                                                typeof(ImageSource),
                                                                typeof(RefreshImagePath), metadata);
        }
    }
}
