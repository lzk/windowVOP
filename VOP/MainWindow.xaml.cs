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

namespace VOP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CopyPage    winCopyPage   = new CopyPage   ();
        FileSelectionPage winFileSelectionPage = new FileSelectionPage();
        PrintPage   winPrintPage  = new PrintPage  ();
        ScanPage    winScanPage   = new ScanPage   ();
        SettingPage winSettingPage= new SettingPage();

        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += MyMouseButtonEventHandler;
        }

        public void LoadedMainWindow( object sender, RoutedEventArgs e )
        {
            this.subPageView.Child = winScanPage;
            setTabItemFromIndex(2);
        }

        public void MyMouseButtonEventHandler( Object sender, MouseButtonEventArgs e)
        {
            Point position = Mouse.GetPosition(this);
            if ( position.Y < 40 && position.Y > 0 )
                this.DragMove();
        }

        private void ControlBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if ( null != btn )
            {
                if ( "btnClose" == btn.Name )
                {
                    this.Close();
                }
            }
        }

        private void NvgBtnClick(object sender, RoutedEventArgs e)
        {
            Control btn = sender as Control;

            if ( null != btn )
            {
                if ( "btnPrint" == btn.Name )
                {                  
                     this.subPageView.Child = winPrintPage;

                     setTabItemFromIndex(0);

                }
                else if ( "btnCopy" == btn.Name )
                {
                    this.subPageView.Child = winCopyPage;

                    setTabItemFromIndex(1);

                }
                else if ( "btnScan" == btn.Name )
                {
                    this.subPageView.Child = winScanPage;

                    setTabItemFromIndex(2);
                }
                else if ( "btnSetting" == btn.Name )
                {
                    this.subPageView.Child = winSettingPage;

                    setTabItemFromIndex(3);
                }
                else if ( "btnLogin" == btn.Name )
                {
                    this.subPageView.Child = winFileSelectionPage;
                }
            }

        }

        #region Set_TabItemIndex
        private bool setTabItemFromName(string tabItemName)
        {
            if (0 == String.Compare(tabItemName, "printer", true))
            {
                tabItem_Printer.IsSelect = true;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;    
            }
            else if (0 == String.Compare(tabItemName, "copy", true))
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = true;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (0 == String.Compare(tabItemName, "scan", true))
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = true;
                tabItem_Setting.IsSelect = false;
            }
            else if (0 == String.Compare(tabItemName, "setting", true))
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = true;
            } 
            else
            {
                return false;
            }

            return true;
        }
        private bool setTabItemFromIndex(int index)
        {
            if (0 == index)
            {
                tabItem_Printer.IsSelect = true;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (1 == index)
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = true;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = false;
            }
            else if (2 == index)
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = true;
                tabItem_Setting.IsSelect = false;
            }
            else if (3 == index)
            {
                tabItem_Printer.IsSelect = false;
                tabItem_Copy.IsSelect = false;
                tabItem_Scan.IsSelect = false;
                tabItem_Setting.IsSelect = true;
            }
            else
            {
                return false;
            }

            return true;
        }


        #endregion // end Set_TabItemIndex

        private void btnLogin_btnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
