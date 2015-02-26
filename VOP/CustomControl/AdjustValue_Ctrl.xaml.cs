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
    /// Interaction logic for AdjustValue_Ctrl.xaml
    /// </summary>
    public partial class AdjustValue_Ctrl : UserControl
    {
        public AdjustValue_Ctrl()
        {
            InitializeComponent();
        }

        #region Show_Style
        public enum ShowStyle
        {
            text, 
            figure
        }

        private ShowStyle curShowStyle = ShowStyle.text;
        public ShowStyle CurShowStyle
        {
            get
            {
                return curShowStyle;
            }

            set
            {
                if (ShowStyle.text == value)
                {
                    textBox.Visibility = Visibility.Visible;
                    FigureCanvas.Visibility = Visibility.Hidden;

                    curShowStyle = value;
                }
                else if (ShowStyle.figure == value)
                {
                    textBox.Visibility = Visibility.Hidden;
                    FigureCanvas.Visibility = Visibility.Visible;

                    curShowStyle = value;
                }
            }
        }

        #endregion // Show_Style


   
        #region FigureShow_Argument
        private int cellCnts = 6;
        public int CellCnts
        {
            get
            {
                return cellCnts;
            }

            set
            {
                cellCnts = value;
                InitContentCanvas_Figure();
            }
        }
        #region CurVal
        private int currentValue = 0;
        public int CurrentValue
        {
            get
            {
                return currentValue;
            }

            set
            {
                if (ShowStyle.figure == curShowStyle)
                {
                    if ((0 <= value) && (value <= cellCnts))
                    {
                        currentValue = value;

                        Update_FigureCanvas();
                    }
                }

            }
        }
        #endregion // CurVal

        private List<Rectangle> rectangleList = new List<Rectangle>();
        #endregion // FigureShow_Argument





      
   
        public TextBlock centerTextBox
        {
            get
            {
                return textBox;
            }
            set
            {
                textBox = value;
            }
        }

        public string Text
        {
            set
            {
                textBox.Text = value;
            }

            get
            {
                return textBox.Text;
            }
        }




        void InitContentCanvas_Figure()
        {
            rectangleList.Clear();

            Size cavsSize = FigureCanvas.RenderSize;         

            // (cell_width: 195*), (blank:35), (cell_width: 195*), (blank:35) ......
            double cell_width_ratio = 195;
            double blank_ratio = 35;

            double cell_width = cavsSize.Width * cell_width_ratio / ((cell_width_ratio + blank_ratio) * cellCnts - blank_ratio);
            double blank_width = cavsSize.Width * blank_ratio / ((cell_width_ratio + blank_ratio) * cellCnts - blank_ratio);
            double cell_height_per = cavsSize.Height / (cellCnts + 1);

            for (int i = 0; i < cellCnts; ++i)
            {
                Rectangle rect = new Rectangle();
                rect.Width = cell_width;
                rect.Height = cell_height_per * (i + 2);

                int colorValue= 134 + i * 8;
             
                byte[] value =   BitConverter.GetBytes(colorValue);

                Color fillColor = new Color();
                fillColor.R = fillColor.G = fillColor.B = value[0];
                fillColor.A = 255;

                rect.Fill = new SolidColorBrush(fillColor);

                FigureCanvas.Children.Add(rect);
                Canvas.SetBottom(rect, 0);
                Canvas.SetLeft(rect, i * (cell_width + blank_width) );

                rect.Visibility = Visibility.Hidden;
                rectangleList.Add(rect);
            }

            Update_FigureCanvas();
        }

        private void Update_FigureCanvas()
        {
            for (int i = 0; i < cellCnts; ++i)
            {
                if (i < currentValue)
                {
                    rectangleList[i].Visibility = Visibility.Visible;
                }
                else
                {
                    rectangleList[i].Visibility = Visibility.Hidden;
                }  
            }
        }

    

        #region RespondEnent

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

        private void btnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (ShowStyle.figure == curShowStyle)
            {
                CurrentValue = currentValue - 1;
            }                      

            if (null != decFunction)
                decFunction();
        }

        private void btnIncrease_Click(object sender, RoutedEventArgs e)
        {
            if (ShowStyle.figure == curShowStyle)
            {
                CurrentValue = currentValue + 1;
            }           

            if (null != incFunction)
                incFunction();
        }

  
        #endregion //RespondEnent

        private void FigureCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            InitContentCanvas_Figure();
        }

    }
}