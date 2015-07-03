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
    /// Interaction logic for UserCenterView.xaml
    /// </summary>
    public partial class UserCenterView : UserControl
    {
        public UserCenterView()
        {
            InitializeComponent();
        }

        private void OnLoadedView(object sender, RoutedEventArgs e)
        {
            if (true == VOP.MainWindow.m_bLogon)
            {
                tbUserName.Text = VOP.MainWindow.m_strPhoneNumber;
                tbUserName.Visibility = Visibility.Visible;
                btnLogon.Visibility = Visibility.Hidden;
            }            
        }

        private void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Name == "btnLogon")
            {
                LoginWindow loginWnd = new LoginWindow();
                loginWnd.Owner = App.Current.MainWindow;
                loginWnd.ShowActivated = true;
                Nullable<bool> dialogResult = loginWnd.ShowDialog();

                if (dialogResult == true)
                {
                    VOP.MainWindow.m_bLogon = true;
                    tbUserName.Text = VOP.MainWindow.m_strPhoneNumber = loginWnd.m_strPhoneNumber;
                    tbUserName.Visibility = Visibility.Visible;
                    btnLogon.Visibility = Visibility.Hidden;

                    ((MainWindow)App.Current.MainWindow).UpdateLogonBtnStatus(true);
                }
            }
            else if (btn.Name == "btnModifyUserInfo")
            {

            }
            else if (btn.Name == "btnConsumable")
            {
                PurchaseWindow win = new PurchaseWindow();
                win.Owner = App.Current.MainWindow;
                win.ShowDialog();
            }
            else if (btn.Name == "btnRewardPoints")
            {

            }
            else if (btn.Name == "btnMaintainStation")
            {
                MaintainWindow mw = new MaintainWindow();
                mw.Owner = App.Current.MainWindow;
                mw.ShowDialog();
            }
            else if (btn.Name == "btnFWDownload")
            {

            }
            else if (btn.Name == "btnAbout")
            {
                AboutWindow wnd = new AboutWindow();
                wnd.Owner = App.Current.MainWindow;
                wnd.ShowDialog();
            }
            else if(btn.Name == "btnBack")
            {
                ((MainWindow)App.Current.MainWindow).HiddenLogonView();
            }
        }

    }
}
