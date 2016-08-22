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
using System.IO;
using System.Drawing;
using ZXing;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Rendering;

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for QRCodeManualPage.xaml
    /// </summary>
    public partial class QRCodeManualPage : UserControl
    {
        public static List<BitmapSource> croppedImageList = new List<BitmapSource>();
        public static List<int> imageRotationList = new List<int>();

        public QRCodeWindow ParentWin { get; set; }

        public Uri ImageUri
        {
            set
            {
                if(myCropper != null)
                {
                    myCropper.ImagePath = value;
                }
            }
        }

        public QRCodeManualPage()
        {
            InitializeComponent();
        }

        public QRCodeManualPage(QRCodeWindow win)
        {
            InitializeComponent();
            ParentWin = win;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
            RadioButton rb = colorPanel.Template.FindName("WhiteRadioButton", colorPanel) as RadioButton;
            rb.Checked += new RoutedEventHandler(RadioColor_Checked);

            rb = colorPanel.Template.FindName("GreenRadioButton", colorPanel) as RadioButton;
            rb.Checked += new RoutedEventHandler(RadioColor_Checked);

            rb = colorPanel.Template.FindName("GrayRadioButton", colorPanel) as RadioButton;
            rb.Checked += new RoutedEventHandler(RadioColor_Checked);     

            InitFontSize();
        }

        void InitFontSize()
        {
            if (App.LangId == 0x804) // en-US
            {
                colorPanel.FontSize = 16.0;
            }
            else
            {
                colorPanel.FontSize = 12.0;
            }
        }


        private void RadioColor_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if(rb.Name == "WhiteRadioButton")
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.White;
                myCropper.SetDesignerItemColor(brush);
            }
            else if (rb.Name == "GreenRadioButton")
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.Green;
                myCropper.SetDesignerItemColor(brush);
            }
            else if (rb.Name == "GrayRadioButton")
            {
                SolidColorBrush brush = new SolidColorBrush();
                brush.Color = Colors.DarkGray;
                myCropper.SetDesignerItemColor(brush);
            }
        }

        private void RotatedButton_Click(object sender, RoutedEventArgs e)
        {
            myCropper.Rotated90();
        }

        private void GreenOkButton_Click(object sender, RoutedEventArgs e)
        {
            croppedImageList.Clear();
            imageRotationList.Clear();

            if (myCropper.GetCroppedImage() != null)
            {
                croppedImageList.Add(myCropper.GetCroppedImage());
                imageRotationList.Add(myCropper.GetCurrentImageRotation());
            }

            AsyncWorker worker = new AsyncWorker(ParentWin);
            Result[] results = worker.InvokeQRCodeMethod(ParentWin.Decode, QRCodeWindow.BitmapFromSource(croppedImageList[0]));

            if (ParentWin != null)
                ParentWin.GotoResultPage(results, null, true);
        }
    }
}
