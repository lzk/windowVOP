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
        public static List<int> imageRotationList = new List<int>();

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

            RadioButton rb = colorPanel.Template.FindName("WhiteRadioButton", colorPanel) as RadioButton;
            rb.Checked += new RoutedEventHandler(RadioColor_Checked);

            rb = colorPanel.Template.FindName("GreenRadioButton", colorPanel) as RadioButton;
            rb.Checked += new RoutedEventHandler(RadioColor_Checked);

            rb = colorPanel.Template.FindName("GrayRadioButton", colorPanel) as RadioButton;
            rb.Checked += new RoutedEventHandler(RadioColor_Checked);

            this.Width = this.Width * App.gScalingRate;
            this.Height = this.Height * App.gScalingRate;
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
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
            switch (currentState)
            {
                case EditWindowState.Edit:

                    if (myCropper.GetCroppedImage() != null)
                    {
                        croppedImageList.Add(myCropper.GetCroppedImage());
                        imageRotationList.Add(myCropper.GetCurrentImageRotation());
                    }

                    PrintPreview printPreview = new PrintPreview();
                    printPreview.PaperWidth = A4Size.Width; //A4
                    printPreview.PaperHeight = A4Size.Height;
                    printPreview.SelectedTypeItem = SelectedTypeItem;

                    if (SelectedTypeItem.PrintSides == enumIdCardPrintSides.OneSide)
                    {
                        borderContainer.Child = printPreview;
                        printPreview.Update(); //update after being a child
                        TitleBarText.Text = (string)this.TryFindResource("ResStr_Print_Preview");

                        colorPanel.Visibility = System.Windows.Visibility.Hidden;
                        RotationButton.Visibility = System.Windows.Visibility.Hidden;
                        GreenOkButton.Margin = new Thickness(-120, 0, 0, 5);
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
                            TitleBarText.Text = (string)this.TryFindResource("ResStr_Print_Preview");

                            colorPanel.Visibility = System.Windows.Visibility.Hidden;
                            RotationButton.Visibility = System.Windows.Visibility.Hidden;
                            GreenOkButton.Margin = new Thickness(-120, 0, 0, 5);
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
