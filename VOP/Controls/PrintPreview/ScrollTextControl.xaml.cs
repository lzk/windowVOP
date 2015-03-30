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
    /// <summary>
    /// Interaction logic for ScrollTextControl.xaml
    /// </summary>
    public partial class ScrollTextControl : UserControl
    {
        public ScrollTextControl()
        {
            InitializeComponent();
        }

        public string ScrollText
        {
            get { return (string)GetValue(ScrollTextProperty); }
            set { SetValue(ScrollTextProperty, value); }
        }

        public static readonly DependencyProperty ScrollTextProperty =
           DependencyProperty.Register("ScrollText", typeof(string), typeof(ScrollTextControl));

        public Brush ScrollForeground
        {
            get { return (Brush)GetValue(ScrollForegroundProperty); }
            set { SetValue(ScrollForegroundProperty, value); }
        }

        public static readonly DependencyProperty ScrollForegroundProperty =
           DependencyProperty.Register("ScrollForeground", typeof(Brush), typeof(ScrollTextControl));


        public double ScrollFontSize
        {
            get { return (double)GetValue(ScrollFontSizeProperty); }
            set { SetValue(ScrollFontSizeProperty, value); }
        }

        public static readonly DependencyProperty ScrollFontSizeProperty =
           DependencyProperty.Register("ScrollFontSize", typeof(double), typeof(ScrollTextControl));

     
    }
}
