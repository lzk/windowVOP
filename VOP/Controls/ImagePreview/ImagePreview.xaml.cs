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

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class ImagePreview : UserControl
    {
        private enum ImagePreviewState 
        {  
            Init, 
            Finalize, 
            Render, 
            Ready, 
            First,
            Middle,
            Last,          
            OnePage,
            CheckExt,
            Unvalid,
            Pre,
            Next,
            TiffState
        }

        private enum ImagePreviewTiffState
        {
            TiffReady,
            TiffPre,
            TiffNext,
        }


        ImagePreviewState currentState;
        ImagePreviewTiffState currentTiffState;

        string currentFileExt = "";
        public static readonly DependencyProperty CurrentFileNameProperty =
            DependencyProperty.Register("CurrentFileName", typeof(string), typeof(ImagePreview));

        public string CurrentFileName
        {
            get { return (string)GetValue(CurrentFileNameProperty); }
            set { SetValue(CurrentFileNameProperty, value); }
        }

        public ImagePreview()
        {
            InitializeComponent();
            currentState = ImagePreviewState.Init;
        }

        int listCount = 0;
        int currentListIndex = 0;
        int TiffCount = 0;
        int currentTiffIndex = 0;
        List<string> imagePaths = new List<string>();
        TiffBitmapDecoder currentTiffDecoder = null;

        public List<string> ImagePaths
        {
            set
            {              
                imagePaths = value;
                currentState = ImagePreviewState.Init;
                Update();
            }
            get
            {
                return imagePaths;
            }
        }

        public BitmapSource IdCardPreviewSource
        {
            set
            {
                if(value != null)
                {
                    PreImageButton.Visibility = System.Windows.Visibility.Hidden;
                    NextImageButton.Visibility = System.Windows.Visibility.Hidden;
                    PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                    NextTiffButton.Visibility = System.Windows.Visibility.Hidden;

                    CurrentFileName = null;
                    ImageView.Visibility = System.Windows.Visibility.Visible;
                    ImageUnvalid.Visibility = System.Windows.Visibility.Hidden;

                    FormatConvertedBitmap fcb = new FormatConvertedBitmap(value, PixelFormats.Gray8, null, 0);
                    ImageView.Source = fcb;
                }
            }
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Name == "PreImageButton")
            {
                currentState = ImagePreviewState.Pre;
            }
            else if (button.Name == "NextImageButton")
            {
                currentState = ImagePreviewState.Next;
            }
            else if (button.Name == "PreTiffButton")
            {
                currentState = ImagePreviewState.TiffState;
                currentTiffState = ImagePreviewTiffState.TiffPre;
            }
            else if (button.Name == "NextTiffButton")
            {
                currentState = ImagePreviewState.TiffState;
                currentTiffState = ImagePreviewTiffState.TiffNext;
            }

            Update();
        }

        public void Update()
        {
            while(currentState != ImagePreviewState.Finalize)
            {
                switch(currentState)
                {
                    case ImagePreviewState.Init:

                         listCount = 0;
                         currentListIndex = 0;
                         TiffCount = 0;
                         currentTiffIndex = 0;

                         if (imagePaths != null)
                         {
                             listCount = imagePaths.Count();

                             if (listCount == 0)
                             {
                                 currentState = ImagePreviewState.Unvalid;
                             }
                             else
                             {
                                 currentState = ImagePreviewState.Ready;
                             }
                         }
                         else
                         {
                             currentState = ImagePreviewState.Unvalid;
                         }

                        break;
                    case ImagePreviewState.Ready:
                        PreImageButton.Visibility = System.Windows.Visibility.Hidden;
                        NextImageButton.Visibility = System.Windows.Visibility.Hidden;
                        PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                        NextTiffButton.Visibility = System.Windows.Visibility.Hidden;
                                  
                        CurrentFileName = System.IO.Path.GetFileName(imagePaths[currentListIndex]);
                        currentFileExt = System.IO.Path.GetExtension(imagePaths[currentListIndex]).ToLower();
                           
                        ImageView.Visibility = System.Windows.Visibility.Visible;
                        ImageUnvalid.Visibility = System.Windows.Visibility.Hidden;

                        if (listCount == 1)
                        {
                            currentState = ImagePreviewState.OnePage;
                        }
                        else
                        {
                            if (currentListIndex == 0)
                            {
                                currentState = ImagePreviewState.First;
                            }
                            else if (currentListIndex == listCount - 1)
                            {
                                currentState = ImagePreviewState.Last;
                            }
                            else
                            {
                                currentState = ImagePreviewState.Middle;
                            }
                        }
   
                        break;
                    case ImagePreviewState.First:
                        PreImageButton.Visibility = System.Windows.Visibility.Hidden;
                        NextImageButton.Visibility = System.Windows.Visibility.Visible;
                        PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                        NextTiffButton.Visibility = System.Windows.Visibility.Hidden;

                        currentState = ImagePreviewState.CheckExt;
                        break;
                    case ImagePreviewState.Middle:
                        PreImageButton.Visibility = System.Windows.Visibility.Visible;
                        NextImageButton.Visibility = System.Windows.Visibility.Visible;
                        PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                        NextTiffButton.Visibility = System.Windows.Visibility.Hidden;

                        currentState = ImagePreviewState.CheckExt;
                        break;
                    case ImagePreviewState.Last:
                        PreImageButton.Visibility = System.Windows.Visibility.Visible;
                        NextImageButton.Visibility = System.Windows.Visibility.Hidden;
                        PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                        NextTiffButton.Visibility = System.Windows.Visibility.Hidden;

                        currentState = ImagePreviewState.CheckExt;
                        break;
                    case ImagePreviewState.OnePage:
                        PreImageButton.Visibility = System.Windows.Visibility.Hidden;
                        NextImageButton.Visibility = System.Windows.Visibility.Hidden;
                        PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                        NextTiffButton.Visibility = System.Windows.Visibility.Hidden;
     
                        currentState = ImagePreviewState.CheckExt;
                        break;
                    case ImagePreviewState.CheckExt:

                        if (currentFileExt == ".tif")
                        {
                            currentTiffIndex = 0;
                            currentState = ImagePreviewState.TiffState;
                            currentTiffState = ImagePreviewTiffState.TiffReady;
                        }
                        else if (currentFileExt == ".jpg" || currentFileExt == ".bmp" || currentFileExt == ".png")
                        {
                            currentState = ImagePreviewState.Render;
                        }
                        else
                        {
                            currentState = ImagePreviewState.Unvalid;
                        }
                        break;
                    case ImagePreviewState.Render:

                       // bool IsFitted = false;
                        if(File.Exists(imagePaths[currentListIndex]))
                        {
                            BitmapSource bitmapSource;
                            if (currentFileExt == ".tif")
                            {
                                bitmapSource = currentTiffDecoder.Frames[currentTiffIndex];

                                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

                                using (MemoryStream ms = new MemoryStream())
                                {
                                    encoder.Save(ms);
                                    BitmapImage myBitmapImage = new BitmapImage();
                                    myBitmapImage.BeginInit();
                                    myBitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                                    myBitmapImage.DecodePixelWidth = 2000;
                                    myBitmapImage.EndInit();

                                    bitmapSource = myBitmapImage;
                                }

                            }
                            else
                            {
                                BitmapImage myBitmapImage = new BitmapImage();
                                myBitmapImage.BeginInit();
                                myBitmapImage.UriSource = new Uri(imagePaths[currentListIndex], UriKind.RelativeOrAbsolute);
                                myBitmapImage.DecodePixelWidth = 2000;
                                myBitmapImage.EndInit();

                                bitmapSource = myBitmapImage;
                            }

                            //if (bitmapSource.PixelWidth < this.ActualWidth - 60 && bitmapSource.PixelHeight < this.ActualHeight - 60)
                            //{
                            //    ImageView.Stretch = Stretch.None;
                            //    //IsFitted = true;
                            //}
                            //else
                            //{
                                ImageView.Stretch = Stretch.Uniform;
                                //IsFitted = false;
                            //}

                            FormatConvertedBitmap fcb = new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray8, null, 0);
                            ImageView.Source = fcb;
                            currentState = ImagePreviewState.Finalize;
                        }
                        else
                        {
                            currentState = ImagePreviewState.Unvalid;
                        }
                     
                        break;
                    case ImagePreviewState.Finalize:
                        break;
                    case ImagePreviewState.Unvalid:
                        ImageView.Visibility = System.Windows.Visibility.Hidden;
                        ImageUnvalid.Visibility = System.Windows.Visibility.Visible;
                        currentState = ImagePreviewState.Finalize;
                        break;
                    case ImagePreviewState.Pre:
                        if(currentListIndex > 0)
                        {
                            currentListIndex--;
                        }
                        currentState = ImagePreviewState.Ready;
                        break;
                    case ImagePreviewState.Next:
                        if (currentListIndex < listCount - 1)
                        {
                            currentListIndex++;
                        }
                        currentState = ImagePreviewState.Ready;
                        break;
                    case ImagePreviewState.TiffState:

                        while (currentState != ImagePreviewState.Render)
                        {
                            switch (currentTiffState)
                            {
                                case ImagePreviewTiffState.TiffReady:
                                    PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                                    NextTiffButton.Visibility = System.Windows.Visibility.Hidden;

                                    if(File.Exists(imagePaths[currentListIndex]))
                                    {
                                        Uri myUri = new Uri(imagePaths[currentListIndex], UriKind.RelativeOrAbsolute);
                                        currentTiffDecoder = new TiffBitmapDecoder(myUri, BitmapCreateOptions.None, BitmapCacheOption.None);
                                        TiffCount = currentTiffDecoder.Frames.Count();

                                        if (TiffCount == 1)
                                        {
                                            PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                                            NextTiffButton.Visibility = System.Windows.Visibility.Hidden;
                                        }
                                        else
                                        {
                                            if (currentTiffIndex == 0)
                                            {
                                                PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
                                                NextTiffButton.Visibility = System.Windows.Visibility.Visible;
                                            }
                                            else if (currentTiffIndex == TiffCount - 1)
                                            {
                                                PreTiffButton.Visibility = System.Windows.Visibility.Visible;
                                                NextTiffButton.Visibility = System.Windows.Visibility.Hidden;
                                            }
                                            else
                                            {
                                                PreTiffButton.Visibility = System.Windows.Visibility.Visible;
                                                NextTiffButton.Visibility = System.Windows.Visibility.Visible;
                                            }
                                        }
                                    }     
                                    currentState = ImagePreviewState.Render;
                                    break;
                                case ImagePreviewTiffState.TiffPre:
                                    if (currentTiffIndex > 0)
                                    {
                                        currentTiffIndex--;
                                    }
                                    currentTiffState = ImagePreviewTiffState.TiffReady;
                                    break;
                                case ImagePreviewTiffState.TiffNext:
                                    if (currentTiffIndex < TiffCount - 1)
                                    {
                                        currentTiffIndex++;
                                    }
                                    currentTiffState = ImagePreviewTiffState.TiffReady;
                                    break;
                            }
                        }
                     
                        break;
                }
            }
        }

        private void myControl_Loaded(object sender, RoutedEventArgs e)
        {
            //PreImageButton.Visibility = System.Windows.Visibility.Hidden;
            //NextImageButton.Visibility = System.Windows.Visibility.Hidden;
            //PreTiffButton.Visibility = System.Windows.Visibility.Hidden;
            //NextTiffButton.Visibility = System.Windows.Visibility.Hidden;
            //currentState = ImagePreviewState.Init;
            //Update();
        }

    }
}
