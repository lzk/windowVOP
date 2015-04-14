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
using System.Windows.Shapes;

namespace VOP.Controls
{
    /// <summary>
    /// Interaction logic for IdCardTypeSelectWindow.xaml
    /// </summary>
    public partial class IdCardEditWindow : Window
    {
        public static Size A4Size = new Size(21, 29.7); //unit cm
        public static List<BitmapSource> croppedImageList = new List<BitmapSource>();

        public IdCardTypeItem SelectedTypeItem { get; set; }

        private enum EditWindowState { Edit, Preview}
        EditWindowState currentState;

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

        public IdCardEditWindow()
        {
            InitializeComponent();
            currentState = EditWindowState.Edit;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void GreenOkButton_Click(object sender, RoutedEventArgs e)
        {
            switch (currentState)
            {
                case EditWindowState.Edit:

                    croppedImageList.Add(myCropper.GetCroppedImage());

                    PrintPreview printPreview = new PrintPreview();
                    printPreview.PaperWidth = A4Size.Width; //A4
                    printPreview.PaperHeight = A4Size.Height;
                    printPreview.SelectedTypeItem = SelectedTypeItem;

                    if (SelectedTypeItem.PrintSides == enumIdCardPrintSides.OneSide)
                    {

                        borderContainer.Child = printPreview;
                        printPreview.Update(); //update after being a child
                        TitleBarText.Text = "打印预览";
                    }
                    else
                    {
                        if (FileSelectionPage.imageFileCount < 1)
                        {
                            this.DialogResult = true;
                        }
                        else
                        {
                            borderContainer.Child = printPreview;
                            printPreview.Update();
                            TitleBarText.Text = "打印预览";
                        }
                    }
                    currentState = EditWindowState.Preview;
                    break;
                case EditWindowState.Preview:
                    this.DialogResult = true;
                    break;
                default:
                    break;
            }
        }
    }
}
