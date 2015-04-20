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
using System.Windows.Input;

namespace VOP
{
    /// <summary>
    /// Interaction logic for PowerSaveView.xaml
    /// </summary>
    public partial class PowerSaveView : UserControl
    {
        byte m_psavetime = 1;

        public PowerSaveView()
        {
            InitializeComponent();
        }

        public void init_config(bool _bDisplayProgressBar = true)
        {
            m_psavetime = 1;

            PowerSaveTimeRecord m_rec = null;
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (_bDisplayProgressBar)
            {
                worker.InvokeMethod<PowerSaveTimeRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.GetPowerSaveTime);
            }
            else
            {
                m_rec = worker.GetPowerSaveTime(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter);
            }

            if (null != m_rec && m_rec.CmdResult == EnumCmdResult._ACK)
            {
                m_psavetime = m_rec.Time;
            }

            spinnerControl1.FormattedValue = String.Format("{0}", m_psavetime);
            
            TextBox tb = spinnerControl1.Template.FindName("tbTextBox", spinnerControl1) as TextBox;
            tb.TextChanged += new TextChangedEventHandler(SpinnerTextBox_TextChanged);
            tb.PreviewTextInput += new TextCompositionEventHandler(SpinnerTextBox_PreviewTextInput);
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
                    else if(textValue < 1)
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
 
            }

        }

        private void OnLoadedPowerSaveView(object sender, RoutedEventArgs e)
        {
            init_config();
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

            PowerSaveTimeRecord m_rec = new PowerSaveTimeRecord(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, psavetime);
            AsyncWorker worker = new AsyncWorker(Application.Current.MainWindow);

            if (worker.InvokeMethod<PowerSaveTimeRecord>(((MainWindow)App.Current.MainWindow).statusPanelPage.m_selectedPrinter, ref m_rec, DllMethodType.SetPowerSaveTime))
            {
                if (m_rec.CmdResult == EnumCmdResult._ACK)
                {
                    m_psavetime = psavetime;
                    isApplySuccess = true;
                }

            }

            ((MainWindow)App.Current.MainWindow).statusPanelPage.ShowMessage(isApplySuccess ? "设置成功" : "设置失败", StatusDisplayType.Ready );
            return isApplySuccess;
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            apply();

            //UpdateApplyBtnStatus();
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
            if (sc.ValidationHasError == true)
                btnApply.IsEnabled = false;
            else
                btnApply.IsEnabled = true;
        }
    }
}
