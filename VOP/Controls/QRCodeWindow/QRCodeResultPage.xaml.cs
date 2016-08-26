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

        public BitmapSource QRCodeImageSource
        {
            set
            {
                if (value != null)
                {
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
        }


        public Result[] QRCodeResult
        {
            set
            {
                flowTable.RowGroups.Clear();

                if (value != null)
                {
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
                        ParsedResult res = ResultParser.parseResult(v);

                        tr = new TableRow();
                        tr.FontFamily = new FontFamily("Arial");
                        tr.FontSize = 12;
                        tr.Background = Brushes.LightGray;

                        Paragraph paraCodeType = new Paragraph();
                        paraCodeType.Inlines.Add(new Run(v.BarcodeFormat.ToString()));

                        Paragraph paraResultType = new Paragraph();
                        paraResultType.Inlines.Add(new Run(res.Type.ToString()));

                        tr.Cells.Add(new TableCell(paraCodeType));
                        tr.Cells.Add(new TableCell(paraResultType));

                        switch(res.Type)
                        {
                            case ParsedResultType.URI:
                                {
                                    Paragraph paraResult = new Paragraph();
                                    Hyperlink hl = new Hyperlink(new Run(v.Text));
                                    hl.FontSize = 11;
                                    hl.FontFamily = new FontFamily("Arial");
                                    hl.NavigateUri = new Uri(v.Text);
                                    hl.RequestNavigate += new RequestNavigateEventHandler(Hyperlink_RequestNavigate);
                                    paraResult.Inlines.Add(hl);

                                    tr.Cells.Add(new TableCell(paraResult));
                                }
                                break;
                            default:
                                {
                                    Paragraph paraResult = new Paragraph();
                                    paraResult.Inlines.Add(new Run(v.Text));

                                    tr.Cells.Add(new TableCell(paraResult));
                                }
                                break;
                        }

                        trg.Rows.Add(tr);
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
                    paraHeader1.Inlines.Add(new Run("No barcode recognized"));

                    TableCell tc = new TableCell();
                    tc.ColumnSpan = 3;
                    tc.Blocks.Add(paraHeader1);

                    tr.Cells.Add(tc);
                  
                    trg.Rows.Add(tr);

                    flowTable.RowGroups.Add(trg);
                }
            }
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

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ParentWin != null)
                ParentWin.GotoManualPage();
        }
    }
}
