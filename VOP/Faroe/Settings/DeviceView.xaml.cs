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
using System.Windows.Controls.Primitives;

namespace VOP
{
    /// <summary>
    /// Interaction logic for DeviceView.xaml
    /// </summary>
    public partial class DeviceView : UserControl
    {
        Int16 m_psavetime = 0;
        Int16 m_pofftime = 0;
        bool m_currentStatus = false;
        private RepeatButton btnSleepDecrease;
        private RepeatButton btnSleepIncrease;
        private RepeatButton btnOffDecrease;
        private RepeatButton btnOffIncrease;

        public DeviceView()
        {
            InitializeComponent();
        }

        public void init_config(bool _bDisplayProgressBar = true)
        {
            m_psavetime = 0;
            m_pofftime = 0;

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
                m_psavetime = m_rec.SleepTime;
                m_pofftime = (short)(m_rec.OffTime / 60);
            }

            if (m_psavetime < 0)
                m_psavetime = 0;
            else if (m_psavetime > 60)
                m_psavetime = 60;

            if (m_pofftime < 0)
                m_pofftime = 0;
            else if (m_pofftime > 4)
                m_pofftime = 4;

            spinnerControlAutoSleep.FormattedValue = String.Format("{0}", m_psavetime);
            TextBox tb = spinnerControlAutoSleep.Template.FindName("tbTextBox", spinnerControlAutoSleep) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            spinnerControlAutoOff.FormattedValue = String.Format("{0}", m_pofftime);
            tb = spinnerControlAutoOff.Template.FindName("tbTextBox", spinnerControlAutoOff) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
            tb.PreviewKeyDown += new KeyEventHandler(OnPreviewKeyDown);

            if (MainWindow_Rufous.g_settingData.m_DeviceName.Contains("USB"))
                btnCalibration.IsEnabled = true;
            else
                btnCalibration.IsEnabled = false;

            ScanCountRecord rec = new ScanCountRecord();

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<ScanCountRecord>(strPrinterName, ref rec, DllMethodType.GetScanCount, this);
            }
            else
            {
                rec = worker.GetScanCount();
            }

            if (null != rec && rec.CmdResult == EnumCmdResult._ACK)
            {
                tbRollerCount.Text = Convert.ToString(rec.RollerCount);
                tbACMCount.Text = Convert.ToString(rec.ACMCount);
                tbSCanCount.Text = Convert.ToString(rec.SCanCount);
            }

            // UpdateApplyBtnStatus();
        }

        private void AutoSleepSpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if (int.TryParse(tb.Text, out textValue))
                {
                    if (textValue > 60)
                        tb.Text = "60";
                    else if (textValue < 0)
                        tb.Text = "0";
                }
                else
                {
                    tb.Text = "0";
                }
            }
        }

        private void AutoOffSpinnerTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VOP.Controls.SpinnerControl spinnerCtl = sender as VOP.Controls.SpinnerControl;
            TextBox tb = spinnerCtl.Template.FindName("tbTextBox", spinnerCtl) as TextBox;
            int textValue = 0;
            if (!spinnerCtl.IsFocused)
            {
                if (int.TryParse(tb.Text, out textValue))
                {
                    if (textValue > 4)
                        tb.Text = "4";
                    else if (textValue < 0)
                        tb.Text = "0";
                }
                else
                {
                    tb.Text = "0";
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
            Int16 psavetime = m_psavetime;
            Int16 pofftime = m_pofftime;

            try
            {
                psavetime = Convert.ToByte(spinnerControlAutoSleep.Value);
                pofftime = Convert.ToByte(spinnerControlAutoOff.Value);
            }
            catch
            {
            }

            if (null != btnApply)
            {
                if (psavetime != m_psavetime || pofftime != m_pofftime)
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

            if (m_MainWin.CheckDeviceStatus() == -1)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                   Application.Current.MainWindow,
                   (string)Application.Current.MainWindow.TryFindResource("ResStr_Device_disconnected"),//"Device is diconnected, could not Apply setting!",
                  (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                return false;
            }

            byte psavetime = Convert.ToByte(spinnerControlAutoSleep.Value);
            if (psavetime < 0)
                psavetime = 0;
            else if (60 < psavetime)
                psavetime = 60;

            m_psavetime = psavetime;
            spinnerControlAutoSleep.FormattedValue = String.Format("{0}", m_psavetime);

            byte pofftime = Convert.ToByte(spinnerControlAutoOff.Value);
            pofftime *= 60;
            if (pofftime <= 0)
                pofftime = 0;
            else if(pofftime<=psavetime)
            {
                pofftime = (byte)((int)psavetime + 1);
            }

            if (240 < pofftime)
                pofftime = 240;

            m_pofftime = (short)(pofftime / 60);
            spinnerControlAutoOff.FormattedValue = String.Format("{0}", m_pofftime);

            string strPrinterName = "";

            PowerSaveTimeRecord m_rec = new PowerSaveTimeRecord(strPrinterName, m_psavetime, pofftime);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<PowerSaveTimeRecord>(strPrinterName, ref m_rec, DllMethodType.SetPowerSaveTime, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    isApplySuccess = true;
                }

            }

            if (isApplySuccess)
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                     System.Windows.Application.Current.MainWindow,
                    (string)this.FindResource("ResStr_Setting_Successfully_"),
                    (string)this.TryFindResource("ResStr_Prompt"));

                UpdateApplyBtnStatus();
            }
            else
            {
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_NoIcon,
                     System.Windows.Application.Current.MainWindow,
                    (string)this.FindResource("ResStr_Setting_Fail"),
                    (string)this.TryFindResource("ResStr_Prompt"));
            }
            return isApplySuccess;
        }

        private void btnCalibration_Click(object sender, RoutedEventArgs e)
        {
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
            CalibrationRecord m_rec = new CalibrationRecord();
            if (worker.InvokeMethod<CalibrationRecord>("", ref m_rec, DllMethodType.DoCalibration, this))
            {
                if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
                {
                }
            }
        }

        private void spinnerControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<decimal> e)
        {
            CheckPowerSaveValue();

            UpdateApplyBtnStatus();
        }

        private void OnValidationHasErrorChanged(object sender, RoutedPropertyChangedEventArgs<bool> e)
        {
            //add by yunying shang 2018-01-04 for BMS 1982
            if (true == spinnerControlAutoSleep.ValidationHasError)
            {
                btnSleepDecrease.IsEnabled = false;
                btnSleepIncrease.IsEnabled = false;
            }
            if (true == spinnerControlAutoOff.ValidationHasError)
            {
                btnOffDecrease.IsEnabled = false;
                btnOffIncrease.IsEnabled = false;
            }//<<===============1982

        }

        public void PassStatus(bool online)
        {
            m_currentStatus = online;

           // btnApply.IsEnabled = online;
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
            btnSleepDecrease = spinnerControlAutoSleep.Template.FindName("btnDecrease", spinnerControlAutoSleep) as RepeatButton;
            btnSleepIncrease = spinnerControlAutoSleep.Template.FindName("btnIncrease", spinnerControlAutoSleep) as RepeatButton;

            btnOffDecrease = spinnerControlAutoOff.Template.FindName("btnDecrease", spinnerControlAutoOff) as RepeatButton;
            btnOffIncrease = spinnerControlAutoOff.Template.FindName("btnIncrease", spinnerControlAutoOff) as RepeatButton;

            CheckPowerSaveValue();

            init_config();
            btnCalibration.Focus();
        }

        private void CheckPowerSaveValue() // BMS #1195
        {
            if (spinnerControlAutoSleep.Value <= spinnerControlAutoSleep.Minimum)
            {
                btnSleepDecrease.IsEnabled = false;
                btnSleepIncrease.IsEnabled = true;
            }
            else if (spinnerControlAutoSleep.Value >= spinnerControlAutoSleep.Maximum)
            {
                btnSleepIncrease.IsEnabled = false;
                btnSleepDecrease.IsEnabled = true;
            }
            else
            {
                btnSleepIncrease.IsEnabled = true;
                btnSleepDecrease.IsEnabled = true;
            }

            if (spinnerControlAutoOff.Value <= spinnerControlAutoOff.Minimum)
            {
                btnOffDecrease.IsEnabled = false;
                btnOffIncrease.IsEnabled = true;
            }
            else if (spinnerControlAutoOff.Value >= spinnerControlAutoOff.Maximum)
            {
                btnOffIncrease.IsEnabled = false;
                btnOffDecrease.IsEnabled = true;
            }
            else
            {
                btnOffIncrease.IsEnabled = true;
                btnOffDecrease.IsEnabled = true;
            }

        }

        private void btnApply_Click_1(object sender, RoutedEventArgs e)
        {
            apply();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name == "btnClear1")
            {
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                ScanCountRecord rec = new ScanCountRecord();
                rec.Mode = 0;
                if (worker.InvokeMethod<ScanCountRecord>("", ref rec, DllMethodType.ClearScanCount, this))
                {
                    if (null != rec && rec.CmdResult == EnumCmdResult._ACK)
                    {
                        tbRollerCount.Text = "0";
                    }
                }
            }
            else
            {
                AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);
                ScanCountRecord rec = new ScanCountRecord();
                rec.Mode = 1;
                if (worker.InvokeMethod<ScanCountRecord>("", ref rec, DllMethodType.ClearScanCount, this))
                {
                    if (null != rec && rec.CmdResult == EnumCmdResult._ACK)
                    {
                        tbACMCount.Text = "0";
                    }
                }
            }
            
        }
    }
}
