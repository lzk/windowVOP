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
    public enum StatusDisplayType
    {
        Ready,
        Sleep,
        Offline,
        Warning,
        Busy,
        Error,
    }

    /// <summary>
    /// Interaction logic for StatusTextBox.xaml
    /// </summary>
    public partial class StatusTextBlock : UserControl
    {
        private LinearGradientBrush Brush_Pend = null;
        private LinearGradientBrush Brush_Sleeping = null;
        private LinearGradientBrush Brush_Pending = null;
        private LinearGradientBrush Brush_Preheat = null;
        private LinearGradientBrush Brush_Error = null;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(StatusTextBlock));
       

        public StatusDisplayType TypeId
        {
            get { return (StatusDisplayType)GetValue(TypeIdProperty); }
            set { SetValue(TypeIdProperty, value); }
        }

        public static readonly DependencyProperty TypeIdProperty =
            DependencyProperty.Register("TypeId",
            typeof(StatusDisplayType),
            typeof(StatusTextBlock),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTypeId_Changed)));
      
        private static void OnTypeId_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.StatusTextBlock _this = (VOP.StatusTextBlock)sender;
            _this.TypeId_Changed();
        }

        private void TypeId_Changed()
        {
            string strStatus = "";
            LinearGradientBrush br = null;
            
            switch ( TypeId )
            {
                case StatusDisplayType.Ready   :
                    strStatus = "";
                    br = Brush_Error;
                    break;
                case StatusDisplayType.Sleep   :
                    strStatus = "";
                    br = Brush_Error;
                    break;
                case StatusDisplayType.Offline :
                    strStatus = "";
                    br = Brush_Error;
                    break;
                case StatusDisplayType.Warning :
                    strStatus = "";
                    br = Brush_Error;
                    break;
                case StatusDisplayType.Busy    :
                    strStatus = "";
                    br = Brush_Error;
                    break;
                case StatusDisplayType.Error   :
                    strStatus = "";
                    br = Brush_Error;
                    break;
                default:
                    strStatus = "";
                    br = Brush_Error;
                    break;
            }

            textblock.Text = strStatus;
            rect_Background.Fill = br;
        }

        public StatusTextBlock()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {
            Brush_Pend      = (LinearGradientBrush)this.FindResource("Pend");
            Brush_Sleeping  = (LinearGradientBrush)this.FindResource("Sleeping");
            Brush_Pending = (LinearGradientBrush)this.FindResource("Pending");
            Brush_Preheat = (LinearGradientBrush)this.FindResource("Preheat");
            Brush_Error = (LinearGradientBrush)this.FindResource("Error");            
        }
    }
}
