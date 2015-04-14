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
    /// Interaction logic for UserConfigView.xaml
    /// </summary>
    public partial class UserConfigView : UserControl
    {
        public bool m_is_init = false;

        sbyte m_leadingedge = 0;
        sbyte m_sidetoside = 0;
        sbyte m_imagedensity = 0;
        sbyte m_lowhumiditymode = 0;
        sbyte m_platecontrolmode = 0;
        sbyte m_primarycoolingmode = 0;
        
        public UserConfigView()
        {
            InitializeComponent();
        }
        public bool init_config(bool _bDisplayProgressBar = true)
        {
            bool isInitSuccess = false;

            m_is_init = true;

            m_leadingedge = 0;
            m_sidetoside = 0;
            m_imagedensity = 0;
            m_lowhumiditymode = 0;

            UserCfgRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<UserCfgRecord>(strPrinterName, ref m_rec, DllMethodType.GetUserConfig);
            }
            else
            {
                m_rec = worker.GetUserCfg(strPrinterName);
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                m_leadingedge = m_rec.LeadingEdge;
                m_sidetoside = m_rec.SideToSide;
                m_imagedensity = m_rec.ImageDensity;
                m_lowhumiditymode = m_rec.LowHumidityMode;
                isInitSuccess = true;
            }

            spinCtlEdge.Value = m_leadingedge;
            spinCtlSide2Side.Value = m_sidetoside;
            spinCtlHumidity.Value = m_lowhumiditymode;
            spinCtlDensity.Value = m_imagedensity;

            return isInitSuccess;
        }

        private void OnLoadedUserConfigView(object sender, RoutedEventArgs e)
        {
            init_config();
        }

        private void GetUIValues(
            out sbyte leadingedge,
            out sbyte sidetoside,
            out sbyte imagedensity,
            out sbyte lowhumiditymode,
            out sbyte platecontrolmode,
            out sbyte primarycoolingmode)
        {
            leadingedge = 0;
            sidetoside = 0;
            imagedensity = 0;
            lowhumiditymode = 0;
            platecontrolmode = 2;
            primarycoolingmode = 0;
            if (null != spinCtlEdge &&
                null != spinCtlSide2Side &&
                null != spinCtlDensity &&
                null != spinCtlHumidity &&
                null != chkPlateControlMode &&
                null != chkCoolingMode)
            {
                leadingedge = (sbyte)spinCtlEdge.Value;
                sidetoside = (sbyte)spinCtlSide2Side.Value;
                imagedensity = (sbyte)spinCtlDensity.Value;
                lowhumiditymode = (sbyte)spinCtlHumidity.Value;
                
                if(true == chkPlateControlMode.IsChecked)
                    platecontrolmode = 0;
                else
                    platecontrolmode = 2;

                if (true == chkCoolingMode.IsChecked)
                    primarycoolingmode = 1;
                else
                    primarycoolingmode = 0;

            }
        }

        public bool apply()
        {
            bool isApplySuccess = false;

            sbyte leadingedge = 1;
            sbyte sidetoside = 1;
            sbyte imagedensity = 0;
            sbyte lowhumiditymode = 0;
            sbyte platecontrolmode = 2;
            sbyte primarycoolingmode = 0;

            GetUIValues(out leadingedge, out sidetoside, out imagedensity, out lowhumiditymode, out platecontrolmode, out primarycoolingmode);

            string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
            UserCfgRecord m_rec = new UserCfgRecord(strPrinterName, leadingedge, sidetoside, imagedensity, lowhumiditymode, platecontrolmode, primarycoolingmode);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<UserCfgRecord>(strPrinterName, ref m_rec, DllMethodType.SetUserConfig))
            {
                if (m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    m_leadingedge = leadingedge;
                    m_sidetoside = sidetoside;
                    m_imagedensity = imagedensity;
                    m_lowhumiditymode = lowhumiditymode;
                    m_platecontrolmode = platecontrolmode;
                    m_primarycoolingmode = primarycoolingmode;

                    isApplySuccess = true;
                }

            }

            ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage(isApplySuccess ? "设置成功" : "设置失败");

            return isApplySuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            apply();
        }

        private void btnFusing_Click(object sender, RoutedEventArgs e)
        {
            bool isApplySuccess = false;

            string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;
            FusingResetRecord m_rec = new FusingResetRecord(strPrinterName);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<FusingResetRecord>(strPrinterName, ref m_rec, DllMethodType.SetFusingResetCmd))
            {
                if (m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    isApplySuccess = true;
                }
            }     
        }
    }
}
