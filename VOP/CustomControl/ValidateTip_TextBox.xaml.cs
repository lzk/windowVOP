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
using System.Windows.Threading;

namespace VOP
{
    /// <summary>
    /// Interaction logic for ValidateTip_TextBox.xaml
    /// </summary>
    public partial class ValidateTip_TextBox : UserControl
    {
        DispatcherTimer timer = new DispatcherTimer();
        public int stayTime
        {
            get { return (int)GetValue(stayTimeProperty); }
            set { SetValue(stayTimeProperty, value); }
        }

        public static readonly DependencyProperty stayTimeProperty =
            DependencyProperty.Register("stayTime", typeof(int), typeof(ValidateTip_TextBox),
            new FrameworkPropertyMetadata(2000, new PropertyChangedCallback(OnStayTime_Changed)));

        private static void OnStayTime_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.ValidateTip_TextBox _this = sender as VOP.ValidateTip_TextBox;
            _this.timer.Interval = new TimeSpan(0, 0, 0, 0, _this.stayTime);              
        }

        #region Text_Property
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                
            }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ValidateTip_TextBox),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnText_Changed)));

        private static void OnText_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.ValidateTip_TextBox _this = sender as VOP.ValidateTip_TextBox;
            _this.textBox.Text = _this.Text;
        }
        #endregion //Text_Property


        #region TipTextInfo_Property
        public string TipTextInfo
        {
            get { return (string)GetValue(TipTextInfoProperty); }
            set { SetValue(TipTextInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TipTextInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TipTextInfoProperty =
            DependencyProperty.Register("TipTextInfo", typeof(string), typeof(ValidateTip_TextBox),
           new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTipTextInfo_Changed)));

        private static void OnTipTextInfo_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.ValidateTip_TextBox _this = sender as VOP.ValidateTip_TextBox;
            _this.tipInfo.Text = _this.TipTextInfo;
        }

        #endregion TipTextInfo_Property


        public ValidateTip_TextBox()
        {
            InitializeComponent();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 2000);
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (popusBottom.IsOpen)
            {
                popusBottom.IsOpen = false;
            }

            timer.Stop();
        }

        public delegate bool ValidateTextFunc(string text);
        private ValidateTextFunc validateText = null;
        public ValidateTextFunc ValidateText
        {
            set
            {
                validateText = value;
            }
        }       

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(null != validateText)
            {
                if (!validateText(textBox.Text.TrimStart(' ').TrimEnd(' ')))
                {
                    ErrorTip();
                }
            }
        }

        void ErrorTip()
        {
            popusBottom.HorizontalOffset = this.RenderSize.Width - 25;
            popusBottom.VerticalOffset = -(this.RenderSize.Height / 2 + 70);

            OpenPopu();
        }

        void OpenPopu()
        {
            popusBottom.IsOpen = true;

            timer.Start();
        }
    }
}