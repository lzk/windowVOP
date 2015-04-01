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

        private static void OnIsSelect_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.customTabItem _this = sender as VOP.customTabItem;
            if (null == _this) return;

            if (_this.IsSelect) // On
            {
                _this.NoSelected.Visibility = Visibility.Hidden;
                _this.Selected.Visibility = Visibility.Visible;
            }
            else // Default: Off
            {
                _this.NoSelected.Visibility = Visibility.Visible;
                _this.Selected.Visibility = Visibility.Hidden;
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
