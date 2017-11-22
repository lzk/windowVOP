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
    public class CloseButton : Button
    {

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor", typeof(Brush), typeof(CloseButton));

        //public static readonly DependencyProperty ContentAppearanceProperty =
        //DependencyProperty.Register("ContentAppearance", typeof(ControlTemplate), typeof(CloseButton));

        //public static readonly DependencyProperty MinimizeOrCloseStyleProperty =
        //  DependencyProperty.Register("MinimizeOrCloseStyle", typeof(MinimizeOrClose), typeof(CloseButton),
        //  new FrameworkPropertyMetadata(new PropertyChangedCallback(OnMinimizeCloseButtonStyleChanged)));


        public Brush ForegroundColor
        {
            get { return (Brush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        //internal ControlTemplate ContentAppearance
        //{
        //    get { return (ControlTemplate)GetValue(ContentAppearanceProperty); }
        //    set { SetValue(ContentAppearanceProperty, value); }
        //}

        //public MinimizeOrClose MinimizeOrCloseStyle
        //{
        //    get { return (MinimizeOrClose)GetValue(MinimizeOrCloseStyleProperty); }
        //    set { SetValue(MinimizeOrCloseStyleProperty, value); }
        //}

        static CloseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CloseButton), new FrameworkPropertyMetadata(typeof(CloseButton)));
        }

        //public CloseButton()
        //{
        //    MinimizeOrCloseStyle = MinimizeOrClose.Close;
        //}     


        //private static void OnMinimizeCloseButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    CloseButton btn = (CloseButton)d;
        //    CloseButton option = (CloseButton)e.NewValue;

        //    btn.Style = (Style)btn.TryFindResource(new ComponentResourceKey(btn.GetType(), option.ToString()));
        //}
    }
}
