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
using System.Windows.Threading;


namespace VOP
{
    /// <summary>
    /// Interaction logic for ImagePreviewControl.xaml
    /// </summary>
    public partial class ImagePreviewControl : UserControl
    {
        public static readonly DependencyProperty ImgSetPathProperty;
        public string ImgSetPath
        {
            get { return (string)GetValue(ImgSetPathProperty); }
            set { SetValue(ImgSetPathProperty, value); }
        }

        static ImagePreviewControl()
        {
            ImgSetPathProperty =
                DependencyProperty.Register("ImgSetPath",
                typeof(string),
                typeof(ImagePreviewControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnImgSetPath_Changed)));            
        }
        
        
        private static void OnImgSetPath_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.ImagePreviewControl _this = sender as VOP.ImagePreviewControl;
            if (null == _this) return;

            _this.EnumPathAllFiles();
        }
 

        private ImageBrush imgBrush = new ImageBrush();
        private FileInfo[] fiList;
        private long CurrentImageIndex = 0;

        public ImagePreviewControl()
        {
            InitializeComponent();
        }

        private void Init()
        {
            long fileCnts = fiList.Length;

            if (0 != fileCnts)
            {
                CurrentImageIndex = 0;
                string currentImagePath = ImgSetPath + @"\" + fiList[CurrentImageIndex];

                updateImage(currentImagePath);
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (null == fiList) return;

            long fileCnts = fiList.Length;

            if (0 != fileCnts)
            {
                if (CurrentImageIndex - 1 < 0)
                {
                    CurrentImageIndex = fileCnts - 1;
                }
                else
                {
                    CurrentImageIndex -= 1;
                }

                string imagePath = ImgSetPath + @"\" + fiList[CurrentImageIndex];

                updateImage(imagePath);

            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (null == fiList) return;

            long fileCnts = fiList.Length;

            if (0 != fileCnts)
            {
                if (CurrentImageIndex + 1 >= fileCnts)
                {
                    CurrentImageIndex = 0;
                }
                else
                {
                    CurrentImageIndex += 1;
                }

                string imagePath = ImgSetPath + @"\" + fiList[CurrentImageIndex];

                updateImage(imagePath);

            }
        }

        bool updateImage(string _imagePath)
        {
            string pathTest = ImgSetPath + @"\" + fiList[CurrentImageIndex];

            imgBrush.ImageSource = new BitmapImage(
                  new Uri(_imagePath, UriKind.RelativeOrAbsolute));

            imagePreviewRegon.Fill = imgBrush;

            textBox.Text = fiList[CurrentImageIndex].ToString();

            

            

            return true;
        }

        void EnumPathAllFiles()
        {
            // path = @"F:\百度相册_ben";//文件夹路径
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(ImgSetPath);
            if (dir.Exists)
            {
                fiList = dir.GetFiles();

                Init();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("click....");
        }
    }
}