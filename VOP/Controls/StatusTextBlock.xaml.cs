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
    /// Interaction logic for StatusTextBox.xaml
    /// </summary>
    public partial class StatusTextBlock : UserControl
    {
        private LinearGradientBrush Brush_Ready   = null;
        private LinearGradientBrush Brush_Sleep   = null;
        private LinearGradientBrush Brush_Offline = null;
        private LinearGradientBrush Brush_Warning = null;
        private LinearGradientBrush Brush_Busy    = null;
        private LinearGradientBrush Brush_Error   = null;

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
                    strStatus = (string)this.FindResource("ResStr_Ready");
                    br = Brush_Ready;
                    break;
                case StatusDisplayType.Sleep   :
                    strStatus = (string)this.FindResource("ResStr_Sleep");
                    br = Brush_Sleep;
                    break;
                case StatusDisplayType.Offline :
                    strStatus = (string)this.FindResource("ResStr_Offline");
                    br = Brush_Offline;
                    break;
                case StatusDisplayType.Warning :
                    strStatus = (string)this.FindResource("ResStr_Warning");
                    br = Brush_Warning;
                    break;
                case StatusDisplayType.Busy    :
                    strStatus = (string)this.FindResource("ResStr_Busy");
                    br = Brush_Busy;
                    break;
                case StatusDisplayType.Error   :
                    strStatus = (string)this.FindResource("ResStr_Error");
                    br = Brush_Error;
                    break;
                default:
                    strStatus = (string)this.FindResource("ResStr_Offline");
                    br = Brush_Offline;
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
            Brush_Ready   = (LinearGradientBrush)this.FindResource("Ready");
            Brush_Sleep   = (LinearGradientBrush)this.FindResource("Sleep");
            Brush_Offline = (LinearGradientBrush)this.FindResource("Offline");
            Brush_Warning = (LinearGradientBrush)this.FindResource("Warning");
            Brush_Busy    = (LinearGradientBrush)this.FindResource("Busy");
            Brush_Error   = (LinearGradientBrush)this.FindResource("Error");
        }
    }
}
