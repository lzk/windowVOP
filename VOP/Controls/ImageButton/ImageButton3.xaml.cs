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
    /// Interaction logic for ImageButton.xaml
    /// </summary>
    public partial class ImageButton3 : UserControl
    {
        public static RoutedUICommand ButtonEnterCommand = new RoutedUICommand("ButtonEnterCommand", "ButtonEnterCommand",
                      typeof(ImageButton3), null);

        private bool _helpCanExecute = true;

        public ImageButton3()
        {
            InitializeComponent();
        }

        public ImageSource ImagePath
        {
            get { return (ImageSource)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(ImageSource), typeof(ImageButton3));

        public ImageSource PressImagePath
        {
            get { return (ImageSource)GetValue(PressImagePathProperty); }
            set { SetValue(PressImagePathProperty, value); }
        }

        public static readonly DependencyProperty PressImagePathProperty =
            DependencyProperty.Register("PressImagePath", typeof(ImageSource), typeof(ImageButton3));


        public ImageSource DisableImagePath
        {
            get { return (ImageSource)GetValue(DisableImagePathProperty); }
            set { SetValue(DisableImagePathProperty, value); }
        }

        public static readonly DependencyProperty DisableImagePathProperty =
            DependencyProperty.Register("DisableImagePath", typeof(ImageSource), typeof(ImageButton3));

        protected override void OnPreviewMouseLeftButtonDown( MouseButtonEventArgs e )
        {
            if(e.OriginalSource.GetType() == typeof(TextBlock))
            {
                e.Handled = true;
            }
        }

        private void ImageButtonCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _helpCanExecute;
            e.Handled = true;
        }

        private void ImageButtonExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            myButton.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
            {
                RoutedEvent = Mouse.PreviewMouseDownEvent,
                Source = this,
            });
        }

    }

    public class ThreeState
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

        public static readonly DependencyProperty PressImageProperty;


        public static ImageSource GetPressImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(PressImageProperty);
        }


        public static void SetPressImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(PressImageProperty, value);
        }

        public static readonly DependencyProperty DisableImageProperty;


        public static ImageSource GetDisableImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(DisableImageProperty);
        }


        public static void SetDisableImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(DisableImageProperty, value);
        }


        static ThreeState()
        {
            //register attached dependency property
            var metadata = new FrameworkPropertyMetadata((ImageSource)null);
            ImageProperty = DependencyProperty.RegisterAttached("Image",
                                                                typeof(ImageSource),
                                                                typeof(ThreeState), metadata);

            var metadata1 = new FrameworkPropertyMetadata((ImageSource)null);
            PressImageProperty = DependencyProperty.RegisterAttached("PressImage",
                                                                typeof(ImageSource),
                                                                typeof(ThreeState), metadata1);

            var metadata2 = new FrameworkPropertyMetadata((ImageSource)null);
            DisableImageProperty = DependencyProperty.RegisterAttached("DisableImage",
                                                                typeof(ImageSource),
                                                                typeof(ThreeState), metadata2);
        }


    }

}
