using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for DeviceView.xaml
    /// </summary>
    public partial class DeviceView : UserControl
    {
        byte m_psavetime = 1;
        bool m_currentStatus = false;

        public DeviceView()
        {
            InitializeComponent();
        }

        public void init_config(bool _bDisplayProgressBar = true)
        {
            m_psavetime = 1;

            PowerSaveTimeRecord m_rec = null;
            string strPrinterName = "";
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<PowerSaveTimeRecord>(strPrinterName, ref m_rec, DllMethodType.GetPowerSaveTime, this);
            }
            else
            {
                m_rec = worker.GetPowerSaveTime(strPrinterName);
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                m_psavetime = m_rec.Time;
            }

            spinnerControl1.FormattedValue = String.Format("{0}", m_psavetime);

            TextBox tb = spinnerControl1.Template.FindName("tbTextBox", spinnerControl1) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);
            // UpdateApplyBtnStatus();
        }

        private void SpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if (int.TryParse(tb.Text, out textValue))
                {
                    if (textValue > 30)
                        tb.Text = "30";
                    else if (textValue < 1)
                        tb.Text = "1";
                }
                else
                {
                    tb.Text = "1";
                }
            }
        }

        private void SpinnerTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int textValue = 0;

            if (!int.TryParse(e.Text, out textValue))
            {
                e.Handled = true;
            }
        }

        private void SpinnerTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            int textValue = 0;

            if (int.TryParse(textBox.Text, out textValue))
            {
                textBox.Text = String.Format("{0}", textValue);
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        private void UpdateApplyBtnStatus()
        {
            byte psavetime = m_psavetime;

            try
            {
                psavetime = Convert.ToByte(spinnerControl1.Value);
            }
            catch
            {
                psavetime = m_psavetime;
            }

            if (null != btnApply)
            {
                if (psavetime != m_psavetime)
                {
                    btnApply.IsEnabled = true;
                }
                else
                {
                    btnApply.IsEnabled = false;
                }
            }
        }

        public bool apply()
        {
            bool isApplySuccess = false;

            byte psavetime = Convert.ToByte(spinnerControl1.Value);
            if (psavetime < 1 || 30 < psavetime)
                psavetime = 1;

            string strPrinterName = "";

            PowerSaveTimeRecord m_rec = new PowerSaveTimeRecord(strPrinterName, psavetime);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<PowerSaveTimeRecord>(strPrinterName, ref m_rec, DllMethodType.SetPowerSaveTime, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    m_psavetime = psavetime;
                    isApplySuccess = true;
                }

            }

            if (isApplySuccess)
            {

            }
            else
            {

            }
            return isApplySuccess;
        }

        private void btnCalibration_Click(object sender, RoutedEventArgs e)
        {
            dll.DoCalibration();
        }

        private void handler_text_changed(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (null != tb)
            {
                if ("tb_powersave" == tb.Name)
                {
                    int nVal = m_psavetime;
                    try
                    {
                        if (tb.Text.Length > 0)
                            nVal = Convert.ToInt32(tb.Text);
                        else
                            nVal = 1;
                    }
                    catch
                    {
                    }

                    if (nVal <= 0)
                        nVal = 1;

                    if (nVal >= 30)
                        nVal = 30;

                    tb.Text = nVal.ToString();
                    tb.CaretIndex = tb.Text.Length;
                }

                //UpdateApplyBtnStatus();
            }
        }

        private void spinnerControl1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            //  UpdateApplyBtnStatus();
        }

        private void spinnerControl1_ValidationHasErrorChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            VOP.Controls.SpinnerControl sc = sender as VOP.Controls.SpinnerControl;
            if (false == m_currentStatus)
            {
                if (sc.ValidationHasError == true)
                    btnApply.IsEnabled = false;
                else
                    btnApply.IsEnabled = true;
            }
            else
            {
                btnApply.IsEnabled = false;
            }
        }

        public void PassStatus(bool online)
        {
            m_currentStatus = online;

            btnApply.IsEnabled = online;
        }

        private MainWindow_Rufous _MainWin = null;

        public MainWindow_Rufous m_MainWin
        {
            set
            {
                _MainWin = value;
            }

            get
            {
                if (null == _MainWin)
                {
                    return (MainWindow_Rufous)App.Current.MainWindow;
                }
                else
                {
                    return _MainWin;
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
/*
            if (MainWindow_Rufous.g_settingData.m_isUsbConnect == true)
            {
                dll.SetConnectionMode(MainWindow_Rufous.g_settingData.m_DeviceName, true);
            }
            else
            {
                dll.SetConnectionMode(MainWindow_Rufous.g_settingData.m_DeviceName, false);
            }
*/
            init_config();
        }

        private void btnApply_Click_1(object sender, RoutedEventArgs e)
        {
            apply();
        }
    }
}
