using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input; // for MouseButtonEventArgs
using System.Windows.Media.Imaging; // for BitmapImage

namespace VOP
{

    public partial class Density : UserControl
    {
        // Density value, from 1 to 5.
        private byte _density;
        public byte m_density
        {
            get
            {
                return _density;
            }
            set
            {
                if ( value >= 1 && value <= 5 )
                {
                    _density = value;

                    SolidColorBrush br1 = new SolidColorBrush();
                    SolidColorBrush br2 = new SolidColorBrush();
                    br1.Color = Color.FromArgb(255, 136, 136, 136);
                    br2.Color = Color.FromArgb(255, 172, 172, 172);

                    rect1.Fill = br2;
                    rect2.Fill = br2;
                    rect3.Fill = br2;
                    rect4.Fill = br2;
                    rect5.Fill = br2;
                    
                    switch ( _density )
                    {
                        case 1:
                            rect1.Fill = br1;
                            break;
                        case 2:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            break;
                        case 3:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            break;
                        case 4:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            rect4.Fill = br1;
                            break;
                        case 5:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            rect4.Fill = br1;
                            rect5.Fill = br1;
                            break;
                    }
                }
            }
        }

        public Density()
        {
            InitializeComponent();

            m_density = 3;
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {

            Button srcButton = e.Source as Button;
            if ( "btnIncrease" == srcButton.Name )
            {
                m_density++;
            }
            else if ( "btnDecrease" == srcButton.Name )
            {
                m_density--;
            }

        }

    }
}
