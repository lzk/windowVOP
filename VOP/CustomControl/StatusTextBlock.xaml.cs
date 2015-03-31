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
    public enum StatusType
    {
        Pend,
        Sleeping,
        Pending,
        Preheat,
        Error
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
       

        public StatusType TypeId
        {
            get { return (StatusType)GetValue(TypeIdProperty); }
            set { SetValue(TypeIdProperty, value); }
        }

        public static readonly DependencyProperty TypeIdProperty =
            DependencyProperty.Register("TypeId",
            typeof(StatusType),
            typeof(StatusTextBlock),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(OnTypeId_Changed)));
      
        private static void OnTypeId_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            VOP.StatusTextBlock _this = (VOP.StatusTextBlock)sender;
            _this.TypeId_Changed();
        }

        private void TypeId_Changed()
        {
            StatusType typeId = TypeId;           

            if (StatusType.Pend == typeId)
            {
                rect_Background.Fill = Brush_Pend;
               // textblock.Text = "等待";
            }
            else if (StatusType.Pending == typeId)
            {
                rect_Background.Fill = Brush_Pending;
               // textblock.Text = "等待中";
            }
            else if (StatusType.Error == typeId)
            {
                rect_Background.Fill = Brush_Error;
               // textblock.Text = "错误";
            }
            else if (StatusType.Preheat == typeId)
            {
                rect_Background.Fill = Brush_Preheat;
              //  textblock.Text = "预热";
            }

            else if (StatusType.Sleeping == typeId)
            {
                rect_Background.Fill = Brush_Sleeping;
               // textblock.Text = "休眠中";
            }

            if (StatusType.Error == typeId)
            {
               TipInfo = "故障 ： 请复位机器，如果故障依旧，请与服务人员联系";
            }
            else
            {
               TipInfo = "";
            }

        }



        public string TipInfo
        {
            get { return (string)GetValue(TipInfoProperty); }
            set { SetValue(TipInfoProperty, value); }
        }

        public static readonly DependencyProperty TipInfoProperty =
            DependencyProperty.Register("TipInfo", typeof(string), typeof(StatusTextBlock));


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
