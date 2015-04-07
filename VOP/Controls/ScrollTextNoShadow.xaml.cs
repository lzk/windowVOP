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
    /// Interaction logic for ScrollTextNoShadow.xaml
    /// </summary>
    public partial class ScrollTextNoShadow : UserControl
    {
        private System.Windows.Media.Animation.Storyboard BeginStory = null; 

        public bool IsScrollText
        {
            get { return (bool)GetValue(IsScrollTextProperty); }
            set { SetValue(IsScrollTextProperty, value); }
        }

        public static readonly DependencyProperty IsScrollTextProperty =
            DependencyProperty.Register("IsScrollText", typeof(bool), typeof(ScrollTextNoShadow),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(OnIsScrollText_Changed)));

        private static void OnIsScrollText_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.Controls.ScrollTextNoShadow _this = sender as VOP.Controls.ScrollTextNoShadow;

            if(_this.IsScrollText)
            {
                _this.BeginStroyboard();
            }
            else
            {
                _this.EndStroyboard();
            }         
        }

        void EndStroyboard()
        {
            if(null != BeginStory)
            {
                this.BeginStory.Stop();
            }
        }

        void BeginStroyboard()
        {          
            if (null != BeginStory)
            {
                this.BeginStory.Begin();
            }               
        }

        public ScrollTextNoShadow()
        {
            InitializeComponent();

            BeginStory = FindResource("TextMoveStoryboard") as System.Windows.Media.Animation.Storyboard;
        }
        public string ScrollText
        {
            get { return (string)GetValue(ScrollTextProperty); }
            set { SetValue(ScrollTextProperty, value); }
        }

        public static readonly DependencyProperty ScrollTextProperty =
           DependencyProperty.Register("ScrollText", typeof(string), typeof(ScrollTextNoShadow));

        public Brush ScrollForeground
        {
            get { return (Brush)GetValue(ScrollForegroundProperty); }
            set { SetValue(ScrollForegroundProperty, value); }
        }

        public static readonly DependencyProperty ScrollForegroundProperty =
           DependencyProperty.Register("ScrollForeground", typeof(Brush), typeof(ScrollTextNoShadow));


        public double ScrollFontSize
        {
            get { return (double)GetValue(ScrollFontSizeProperty); }
            set { SetValue(ScrollFontSizeProperty, value); }
        }

        public static readonly DependencyProperty ScrollFontSizeProperty =
           DependencyProperty.Register("ScrollFontSize", typeof(double), typeof(ScrollTextNoShadow));

    }
}
