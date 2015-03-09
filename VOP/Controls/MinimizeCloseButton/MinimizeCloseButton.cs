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
using System.Windows.Controls.Primitives;

namespace VOP.Controls
{
    public enum MinimizeOrClose {None, Minimize, Close }

    public class MinimizeCloseButton : Button
    {

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Brush), typeof(MinimizeCloseButton));

        public static readonly DependencyProperty ContentAppearanceProperty =
        DependencyProperty.Register("ContentAppearance", typeof(ControlTemplate), typeof(MinimizeCloseButton));

        public static readonly DependencyProperty MinimizeOrCloseStyleProperty =
          DependencyProperty.Register("MinimizeOrCloseStyle", typeof(MinimizeOrClose), typeof(MinimizeCloseButton),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMinimizeCloseButtonStyleChanged)));


        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }


        internal ControlTemplate ContentAppearance
        {
            get { return (ControlTemplate)GetValue(ContentAppearanceProperty); }
            set { SetValue(ContentAppearanceProperty, value); }
        }

        public MinimizeOrClose MinimizeOrCloseStyle
        {
            get { return (MinimizeOrClose)GetValue(MinimizeOrCloseStyleProperty); }
            set { SetValue(MinimizeOrCloseStyleProperty, value); }
        }

        static MinimizeCloseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MinimizeCloseButton), new FrameworkPropertyMetadata(typeof(MinimizeCloseButton)));
        }

        public MinimizeCloseButton()
        {
            MinimizeOrCloseStyle = MinimizeOrClose.Close;
        }

        private static void OnMinimizeCloseButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MinimizeCloseButton btn = (MinimizeCloseButton)d;
            MinimizeOrClose option = (MinimizeOrClose)e.NewValue;

            btn.Style = (Style)btn.TryFindResource(new ComponentResourceKey(btn.GetType(), option.ToString()));
        }
    }
}
