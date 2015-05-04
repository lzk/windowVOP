using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VOP.Controls
{
    public class ResizeRotateChrome : Control
    {
        static ResizeRotateChrome()
        {
            //FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeRotateChrome), new FrameworkPropertyMetadata(typeof(ResizeRotateChrome)));
        }

        public static readonly DependencyProperty DesignerItemColorProperty =
          DependencyProperty.Register("DesignerItemColor", typeof(SolidColorBrush), typeof(ResizeRotateChrome));

        public Brush DesignerItemColor
        {
            get { return (SolidColorBrush)GetValue(DesignerItemColorProperty); }
            set { SetValue(DesignerItemColorProperty, value); }
        }
    }
}
