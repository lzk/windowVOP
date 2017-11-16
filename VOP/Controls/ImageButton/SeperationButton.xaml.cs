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
    public partial class SeperationButton : UserControl
    {
        public static RoutedUICommand ButtonEnterCommand = new RoutedUICommand("ButtonEnterCommand", "ButtonEnterCommand",
                      typeof(SeperationButton), null);

        private bool _helpCanExecute = true;

        public SeperationButton()
        {
            InitializeComponent();
        }

        public ImageSource ImagePath
        {
            get { return (ImageSource)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public string BottomText
        {
            get { return (string)GetValue(BottomTextProperty); }
            set { SetValue(BottomTextProperty, value); }
        }


        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register("ImagePath", typeof(ImageSource), typeof(SeperationButton));

        public static readonly DependencyProperty BottomTextProperty =
            DependencyProperty.Register("BottomText", typeof(string), typeof(SeperationButton));


        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (e.OriginalSource.GetType() == typeof(TextBlock))
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
}
