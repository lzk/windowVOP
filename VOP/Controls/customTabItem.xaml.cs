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
            VOP.customTabItem _this = sender as VOP.customTabItem;
            if (null == _this) return;

            if (_this.IsSelect) // On
            {
                _this.rectLeft_1.Fill = _this.rectLeft_2.Fill = 
                _this.rectMiddle.Fill =
                _this.rectRight_1.Fill = _this.rectRight_2.Fill =
                _this.rectSingle_1.Fill = _this.rectSingle_2.Fill = _this.rectSingle_3.Fill = 
                new SolidColorBrush(Color.FromRgb(0x6D, 0x6D, 0x6D));

                _this.text.Foreground = new SolidColorBrush(Colors.White);
            }
            else // Default: Off
            {
                _this.rectLeft_1.Fill = _this.rectLeft_2.Fill =
                _this.rectMiddle.Fill =
                _this.rectRight_1.Fill = _this.rectRight_2.Fill =
                _this.rectSingle_1.Fill = _this.rectSingle_2.Fill = _this.rectSingle_3.Fill = 
               new SolidColorBrush(Colors.White);

                _this.text.Foreground = new SolidColorBrush(Colors.Black);
            }
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
        }

        public customTabItem()
        {
            InitializeComponent();
        }
    }
}
