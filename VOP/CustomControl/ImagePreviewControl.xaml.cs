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
        private ImageBrush imgBrush = new ImageBrush();
        private FileInfo[] fiList;
        private long CurrentImageIndex = 0;

        private string path;
        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                EnumPathAllFiles();
            }
        }

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
                string currentImagePath = path + @"\" + fiList[CurrentImageIndex];

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

                string imagePath = path + @"\" + fiList[CurrentImageIndex];

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

                string imagePath = path + @"\" + fiList[CurrentImageIndex];

                updateImage(imagePath);
            }
        }

        bool updateImage(string _imagePath)
        {
            string pathTest = path + @"\" + fiList[CurrentImageIndex];

            imgBrush.ImageSource = new BitmapImage(
                  new Uri(_imagePath, UriKind.RelativeOrAbsolute));

            imagePreviewRegon.Fill = imgBrush;

            textBox.Text = fiList[CurrentImageIndex].ToString();

            return true;
        }

        void EnumPathAllFiles()
        {
            // path = @"F:\百度相册_ben";//文件夹路径
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
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