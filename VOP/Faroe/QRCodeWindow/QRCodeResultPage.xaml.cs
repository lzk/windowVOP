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
using System.Diagnostics;
using ZXing;
using ZXing.Client.Result;

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for ImagePreview.xaml
    /// </summary>
    public partial class QRCodeResultPage : UserControl
    {
        public static double rectMargin = 10;
        public double imageMargin = 7;
        public double designerItemWHRatio = 1;
        bool IsFitted = false;
        public double imageToTop = 0;
        public double imageToLeft = 0;
        public double imageWidth = 0;
        public double imageHeight = 0;
        double scaleRatioApply = 1.0;
        double whRatio = 1.0;
        double scaleRatioX = 1.0;
        double scaleRatioY = 1.0;

        public static Rect redRect = Rect.Empty;

        public QRCodeWindow ParentWin { get; set; }

        public QRCodeResultPage()
        {
            InitializeComponent();
        }

        public QRCodeResultPage(QRCodeWindow win)
        {
            InitializeComponent();
            ParentWin = win;
        }

        private BitmapSource qRCodeImageSource = null;
        public BitmapSource QRCodeImageSource
        {
            set
            {
                if (value != null)
                {
                    qRCodeImageSource = value;
                    JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(value));

                    using (MemoryStream ms = new MemoryStream())
                    {
                        encoder.Save(ms);
                        BitmapImage myBitmapImage = new BitmapImage();
                        myBitmapImage.BeginInit();
                        myBitmapImage.StreamSource = new MemoryStream(ms.ToArray());
                        myBitmapImage.DecodePixelWidth = 400;
                        myBitmapImage.EndInit();

                        ImageView.Source = myBitmapImage;
                    }

                }
            }
            get
            {
                return qRCodeImageSource;
            }

        }

        private QRCodeResult[] qRCodeResult = null;
        public QRCodeResult[] QRCodeResult
        {
            set
            {
                flowTable.RowGroups.Clear();

                if (value != null)
                {
                    qRCodeResult = value;
                    TableRowGroup trg = new TableRowGroup();
                    trg.SetValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);

                    TableRow tr = new TableRow();
                    tr.FontFamily = new FontFamily("Arial");
                    tr.FontSize = 14;
                    tr.FontWeight = FontWeights.Bold;
                    tr.Background = Brushes.Beige;

                    Paragraph paraHeader1 = new Paragraph();
                    paraHeader1.Inlines.Add(new Run("Code Type"));
                    Paragraph paraHeader2 = new Paragraph();
                    paraHeader2.Inlines.Add(new Run("Result Type"));
                    Paragraph paraHeader3 = new Paragraph();
                    paraHeader3.Inlines.Add(new Run("Content"));

                    tr.Cells.Add(new TableCell(paraHeader1));
                    tr.Cells.Add(new TableCell(paraHeader2));
                    tr.Cells.Add(new TableCell(paraHeader3));

                    trg.Rows.Add(tr);

                    foreach(var v in value)
                    {
                        //Exclude Rss_14
                        if (v.result.BarcodeFormat != BarcodeFormat.RSS_14)
                        {
                            ParsedResult res = ResultParser.parseResult(v.result);

                            tr = new TableRow();

                            tr.MouseLeftButtonDown += new MouseButtonEventHandler(TableRow_MouseLeftButtonDown);
                            tr.MouseEnter += new MouseEventHandler(TableRow_MouseEnter);
                            tr.MouseLeave += new MouseEventHandler(TableRow_MouseLeave);
                            tr.Tag = v;

                            tr.FontFamily = new FontFamily("Arial");
                            tr.FontSize = 12;
                            tr.Background = Brushes.LightGray;

                            Paragraph paraCodeType = new Paragraph();
                            paraCodeType.Inlines.Add(new Run(v.result.BarcodeFormat.ToString()));

                            Paragraph paraResultType = new Paragraph();
                            paraResultType.Inlines.Add(new Run(res.Type.ToString()));

                            tr.Cells.Add(new TableCell(paraCodeType));
                            tr.Cells.Add(new TableCell(paraResultType));

                            switch (res.Type)
                            {
                                case ParsedResultType.URI:
                                    {
                                        try
                                        {
                                            Paragraph paraResult = new Paragraph();
                                            Hyperlink hl = new Hyperlink(new Run(v.result.Text));
                                            hl.FontSize = 11;
                                            hl.FontFamily = new FontFamily("Arial");
                                            hl.NavigateUri = new Uri(v.result.Text);
                                            hl.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
                                            paraResult.Inlines.Add(hl);

                                            tr.Cells.Add(new TableCell(paraResult));
                                        }
                                        catch (Exception ex)
                                        {
                                            goto default;
                                        }

                                    }
                                    break;
                                default:
                                    {
                                        Paragraph paraResult = new Paragraph();
                                        paraResult.Inlines.Add(new Run(v.result.Text));

                                        tr.Cells.Add(new TableCell(paraResult));
                                    }
                                    break;
                            }

                            trg.Rows.Add(tr);
                        }

                    }

                    flowTable.RowGroups.Add(trg);
                  
                }
                else
                {
                    TableRowGroup trg = new TableRowGroup();
                    trg.SetValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);

                    TableRow tr = new TableRow();
                    tr.FontFamily = new FontFamily("Arial");
                    tr.FontSize = 14;
                    tr.FontWeight = FontWeights.Bold;
                    tr.Background = Brushes.Beige;

                    Paragraph paraHeader1 = new Paragraph();
                    paraHeader1.Inlines.Add(new Run("No code recognized"));

                    TableCell tc = new TableCell();
                    tc.ColumnSpan = 3;
                    tc.Blocks.Add(paraHeader1);

                    tr.Cells.Add(tc);
                  
                    trg.Rows.Add(tr);

                    flowTable.RowGroups.Add(trg);
                }
            }
            get
            {
                return qRCodeResult;
            }
        }

        private bool IsResultPointValided(ResultPoint[] points)
        {
            if (points == null)
                return false;

            foreach(ResultPoint point in points)
            {
                if (point.X < 0
                 || point.Y < 0
                 || point.X > qRCodeImageSource.PixelWidth
                 || point.Y > qRCodeImageSource.PixelHeight)
                {
                    return false;
                }
            }

            return true;
        }

        private void TableRow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TableRow tr = sender as TableRow;

            QRCodeResult res = (QRCodeResult)tr.Tag;
            Int32Rect subRec = res.subRect;

            ResultPoint[] points = res.result.ResultPoints;

            if(IsResultPointValided(points) == false)
            {
                return;
            }

            QRCodeResultPage.redRect = Rect.Empty;

            if (points.Length >= 3)
            {
                Rect rect = Rect.Empty;
                rect = new Rect(new Point(points[0].X, points[0].Y), new Point(points[2].X, points[2].Y));
               
                if (!subRec.IsEmpty)
                {
                    rect.X += (double)subRec.X;
                    rect.Y += (double)subRec.Y;
                }

                redRect = rect;

                rect.Scale(1 / scaleRatioApply, 1 / scaleRatioApply);

                Rectangle designerItem = BarcodeRect;
                Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;
                designerItem.Width = rect.Width + rectMargin * 2;
                designerItem.Height = rect.Height + rectMargin * 2;
                Canvas.SetTop(designerItem, imageToTop + rect.Y - rectMargin);
                Canvas.SetLeft(designerItem, imageToLeft + rect.X - rectMargin);
                BarcodeRect.Visibility = Visibility.Visible;
            }
            else if (points.Length == 2)
            {
                float w = ResultPoint.distance(points[0], points[1]);
                Rect rect = Rect.Empty;

                if (points[0].X != points[1].X)
                {
                    if (points[0].X < points[1].X)
                    {
                        rect = new Rect(points[0].X, points[0].Y, w, w);
                    }
                    else
                    {
                        rect = new Rect(points[1].X, points[1].Y, w, w);
                    }
                }
                else
                {
                    if (points[0].Y < points[1].Y)
                    {
                        rect = new Rect(points[0].X, points[0].Y, w, w);
                    }
                    else
                    {
                        rect = new Rect(points[1].X, points[1].Y, w, w);
                    }
                }


                if (!subRec.IsEmpty)
                {
                    rect.X += (double)subRec.X;
                    rect.Y += (double)subRec.Y;
                }

                redRect = rect;

                rect.Scale(1 / scaleRatioApply, 1 / scaleRatioApply);
             
                Line designerItem = BarcodeLine;
                Canvas canvas = VisualTreeHelper.GetParent(designerItem) as Canvas;

                if (points[0].X != points[1].X)
                {
                    designerItem.X1 = imageToLeft + rect.X;
                    designerItem.Y1 = imageToTop + rect.Y;

                    designerItem.X2 = imageToLeft + rect.X + rect.Width;
                    designerItem.Y2 = imageToTop + rect.Y;
                }
                else
                {
                    designerItem.X1 = imageToLeft + rect.X;
                    designerItem.Y1 = imageToTop + rect.Y;

                    designerItem.X2 = imageToLeft + rect.X;
                    designerItem.Y2 = imageToTop + rect.Y + rect.Width;
                }

                BarcodeLine.Visibility = Visibility.Visible;
            }
   
        }

        private double PixelToUnit(int p, double dpi)
        {
            return (double)p / dpi * 96;
        }

        private int UnitToPixel(double u, double dpi)
        {
            return (int)(u / 96 * dpi);
        }
        private void ReArrangeDesignerItem(int width, int height, double dpiX, double dpiY)
        {
     
            imageToTop = imageMargin;
            imageToLeft = imageMargin;
            imageWidth = 0;
            imageHeight = 0;

            //imageWidth = PixelToUnit(width, dpiX);
            //imageHeight = PixelToUnit(height, dpiY);

            imageWidth = width;
            imageHeight = height;

            if (imageWidth < (this.ImageContainer.ActualWidth - 2 * imageMargin )&& imageHeight < (this.ImageContainer.ActualHeight - 2 * imageMargin))
            {
                this.ImageView.Stretch = Stretch.None;
                IsFitted = true;
            }
            else
            {
                this.ImageView.Stretch = Stretch.Uniform;
                IsFitted = false;
            }

            whRatio = imageWidth / imageHeight;
            scaleRatioX = imageWidth / (this.ImageContainer.ActualWidth - 2 * imageMargin);
            scaleRatioY = imageHeight / (this.ImageContainer.ActualHeight - 2 * imageMargin);

            if (IsFitted == true)
            {
                imageToTop = (this.ImageContainer.ActualHeight - imageHeight) / 2;
                imageToLeft = (this.ImageContainer.ActualWidth - imageWidth) / 2;
            }
            else
            {
                //Image uniformlly strech to control, calculate the gap on unfitted side
                if (scaleRatioX > scaleRatioY)
                {
                    imageWidth = this.ImageContainer.ActualWidth - 2 * imageMargin;
                    imageHeight = imageWidth / whRatio;
                    imageToTop = (this.ImageContainer.ActualHeight - imageHeight) / 2;
                    scaleRatioApply = scaleRatioX;
                }
                else if (scaleRatioX < scaleRatioY)
                {
                    imageHeight = this.ImageContainer.ActualHeight - 2 * imageMargin;
                    imageWidth = imageHeight * whRatio;
                    imageToLeft = (this.ImageContainer.ActualWidth - imageWidth) / 2;
                    scaleRatioApply = scaleRatioY;
                }
                else
                {
                    imageWidth = this.ImageContainer.ActualWidth - 2 * imageMargin;
                    imageHeight = this.ImageContainer.ActualHeight - 2 * imageMargin;
                    scaleRatioApply = scaleRatioX;
                }
            }

        }

        private void TableRow_MouseEnter(object sender, MouseEventArgs e)
        {
            TableRow tr = sender as TableRow;

            Mouse.OverrideCursor = Cursors.Hand;
            tr.Background = Brushes.LightGray;
            
        }

        private void TableRow_MouseLeave(object sender, MouseEventArgs e)
        {
            TableRow tr = sender as TableRow;

            Mouse.OverrideCursor = Cursors.Arrow;
            tr.Background = Brushes.AliceBlue;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Hyperlink link = sender as Hyperlink;

            try
            {
                System.Diagnostics.Process.Start(link.NavigateUri.AbsoluteUri);
            }
            catch (Exception)
            {

            }
        }

        public void UpdateView()
        {
            TransformGroup transformGroup = null;

            switch (QRCodeManualPage.imageRotationList[0])
            {
                case 270:

                    RotateTransform rotateTransform = new RotateTransform();
                    rotateTransform.Angle = -90;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

                    break;
                case 0:

                    rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 0;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

                    break;
                case 90:

                    rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 90;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

                    break;
                case 180:

                    rotateTransform = new RotateTransform();
                    rotateTransform.Angle = 180;

                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(rotateTransform);

               
                    break;
            }

            ImageView.RenderTransformOrigin = new Point(0.5, 0.5);
            ImageView.RenderTransform = transformGroup;
            ImageView.Source = QRCodeManualPage.croppedImageList[0];
            QRCodeImageSource = QRCodeManualPage.croppedImageList[0];

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWin != null)
                ParentWin.GotoManualPage();
        }

        private void myControl_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapSource src = QRCodeImageSource as BitmapSource;

            if (src != null)
            {
                ReArrangeDesignerItem(src.PixelWidth, src.PixelHeight, src.DpiX, src.DpiY);
            }

            if (flowTable.RowGroups != null)
            {
                if(flowTable.RowGroups.Count == 1)
                {
                    if(flowTable.RowGroups[0].Rows.Count > 1)
                    {
                        flowTable.RowGroups[0].Rows[1].RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left)
                        {
                            RoutedEvent = Mouse.MouseDownEvent,
                            Source = this,
                        });
                    }
                }
            }
        }
    }
}
