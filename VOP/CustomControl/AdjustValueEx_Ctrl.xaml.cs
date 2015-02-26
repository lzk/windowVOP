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
    /// Interaction logic for AdjustValueEx_Ctrl.xaml
    /// </summary>
    public partial class AdjustValueEx_Ctrl : UserControl
    {
        public AdjustValueEx_Ctrl()
        {
            InitializeComponent();
        }
        public string LeftText
        {
            set
            {
                leftText.Text = value;
            }
        }

        public double LeftText_FontSize
        {
            set
            {
                leftText.FontSize = value;
            }
        }

        public string Text
        {
            set
            {
                adjustCtrl.Text = value;
            }
            get
            {
                return adjustCtrl.Text;
            }
        }

     
        #region RespondEnent
        private void IncFuncCallback()
        {

            if (null != incFunction)
                incFunction();

        }

        private void DecFuncCallback()
        {
            if (null != decFunction)
                decFunction();

        }

        public delegate void IncOrDec();
        #region incFunc
        public IncOrDec incFunction = null;
        public IncOrDec IncFunction
        {
            set
            {
                incFunction = value;
            }
        }
        #endregion

        #region decFunc
        IncOrDec decFunction = null;
        public IncOrDec DecFunction
        {
            set
            {
                decFunction = value;
            }
        }
        #endregion

        #endregion //RespondEnent

    }
}