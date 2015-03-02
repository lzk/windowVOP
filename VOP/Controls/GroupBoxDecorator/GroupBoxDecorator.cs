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

namespace VOP.Controls
{
    public class GroupBoxDecorator : ContentControl
    {

        static GroupBoxDecorator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupBoxDecorator), new FrameworkPropertyMetadata(typeof(GroupBoxDecorator)));
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
           DependencyProperty.Register("Header", typeof(string), typeof(GroupBoxDecorator));

    
    }
}
