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
using VOP.Controls;

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

        private EnumStatus m_currentStatus = EnumStatus.Offline;

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
            m_platecontrolmode = 2;
            m_primarycoolingmode = 0;
 
            UserCfgRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            string strPrinterName = ((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter;

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<UserCfgRecord>(strPrinterName, ref m_rec, DllMethodType.GetUserConfig, this);
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
                m_platecontrolmode = m_rec.PlateControlMode;
                m_primarycoolingmode = m_rec.PrimaryCoolingMode;

                isInitSuccess = true;
            }

            spinCtlEdge.Value = m_leadingedge;
            spinCtlSide2Side.Value = m_sidetoside;
            spinCtlDensity.Value = m_imagedensity;

            if(1 == m_lowhumiditymode)
                chkHumidity.IsChecked = true;
            else
                chkHumidity.IsChecked = false;

            if (0 == m_platecontrolmode)
                chkPlateControlMode.IsChecked = true;
            else
                chkPlateControlMode.IsChecked = false;

            if (1 == m_primarycoolingmode)
                chkCoolingMode.IsChecked = true;
            else
                chkCoolingMode.IsChecked = false;

            return isInitSuccess;
        }

        private void SpinnerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                if (e.Text != "-")
                    e.Handled = true;
            }
        }

        private void SpinnerTextBox_PreviewTextInput1(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                 e.Handled = true;
            }
        }

        private void SpinnerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox textBox = sender as TextBox;
                string strText = textBox.Text;
                strText = strText.Replace("--", "-");
                int textValue = 0;

                if (int.TryParse(strText, out textValue))
                {
                    strText = String.Format("{0}", textValue);
                }
                textBox.Text = strText;
                textBox.CaretIndex = textBox.Text.Length;
            }
            catch
            {

            }
        }
        
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void OnLoadedUserConfigView(object sender, RoutedEventArgs e)
        {
            init_config();

            TextBox tb = spinCtlEdge.Template.FindName("tbTextBox", spinCtlEdge) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            TextBox tb1 = spinCtlSide2Side.Template.FindName("tbTextBox", spinCtlSide2Side) as TextBox;
            tb1.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb1.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb1.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            TextBox tb2 = spinCtlDensity.Template.FindName("tbTextBox", spinCtlDensity) as TextBox;
            tb2.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb2.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb2.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
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
                null != chkHumidity &&
                null != chkPlateControlMode &&
                null != chkCoolingMode)
            {
                leadingedge = (sbyte)spinCtlEdge.Value;
                sidetoside = (sbyte)spinCtlSide2Side.Value;
                imagedensity = (sbyte)spinCtlDensity.Value;

                if (true == chkHumidity.IsChecked)
                    lowhumiditymode = 1;
                else
                    lowhumiditymode = 0;

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
            UserCfgRecord m_rec = new UserCfgRecord(strPrinterName, leadingedge, sidetoside, imagedensity, lowhumiditymode, platecontrolmode, primarycoolingmode, true);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<UserCfgRecord>(strPrinterName, ref m_rec, DllMethodType.SetUserConfig, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
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

            if ( isApplySuccess )
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage( (string)this.FindResource("ResStr_Setting_Successfully_"), Brushes.Black );
            else
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage( (string)this.FindResource("ResStr_Setting_Fail"), Brushes.Red );

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

            if (worker.InvokeMethod<FusingResetRecord>(strPrinterName, ref m_rec, DllMethodType.SetFusingResetCmd, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    isApplySuccess = true;
                }
            }

            if ( isApplySuccess )
            {
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage( (string)this.FindResource("ResStr_Setting_Successfully_"), Brushes.Black );

                MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon, 
                    Application.Current.MainWindow, 
                    (string)this.TryFindResource("ResStr_Please_turn_off_the_printer_until_it_cools_to_room_temperature"), 
                    (string)this.TryFindResource("ResStr_Prompt"));
            }
            else
                ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage( (string)this.FindResource("ResStr_Setting_Fail"), Brushes.Red );
        }

        private void OnValidationHasErrorChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            if (false == common.IsOffline(m_currentStatus))
            {
                if (true == spinCtlEdge.ValidationHasError ||
                    true == spinCtlSide2Side.ValidationHasError ||
                    true == spinCtlDensity.ValidationHasError)
                {
                    btnApply.IsEnabled = false;
                }
                else
                {
                    btnApply.IsEnabled = true;
                }
            }
            else
            {
                btnApply.IsEnabled = false;
            }
        }

        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if ("spinCtlEdge" == spinnerCtl.Name)
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if (textValue > 2)
                            tb.Text = "2";
                        else if (textValue < -2)
                            tb.Text = "-2";
                    }
                    else
                    {
                        tb.Text = "0";
                    }
                }
                else if ("spinCtlSide2Side" == spinnerCtl.Name)
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if (textValue > 6)
                            tb.Text = "6";
                        else if (textValue < -6)
                            tb.Text = "-6";
                    }
                    else
                    {
                        tb.Text = "0";
                    }
                }
                else if ("spinCtlDensity" == spinnerCtl.Name)
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if (textValue > 3)
                            tb.Text = "3";
                        else if (textValue < -3)
                            tb.Text = "-3";
                    }
                    else
                    {
                        tb.Text = "0";
                    }
                }
                else if ("spinCtlHumidity" == spinnerCtl.Name)
                {
                    if (int.TryParse(tb.Text, out textValue))
                    {
                        if (textValue > 1)
                            tb.Text = "1";
                        else if (textValue < 0)
                            tb.Text = "0";
                    }
                    else
                    {
                        tb.Text = "0";
                    }
                }
            }
        }

        public void PassStatus(EnumStatus st, EnumMachineJob job, byte toner)
        {
            m_currentStatus = st;

            if (true == spinCtlEdge.ValidationHasError ||
                true == spinCtlSide2Side.ValidationHasError ||
                true == spinCtlDensity.ValidationHasError)
            {
                btnApply.IsEnabled = false;
            }
            else
            {
                btnApply.IsEnabled = (false == common.IsOffline(m_currentStatus));
            }
          
            btnFusing.IsEnabled = (false == common.IsOffline(m_currentStatus));
        }
    }
}
