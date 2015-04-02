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
using System.Windows.Shapes;

namespace VOP
{
    /// <summary>
    /// </summary>
    public partial class PrintSettingPage : Window
    {
        public PrintSettingPage()
        {
            InitializeComponent();
            m_density = 3;
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private int _density;
        public int m_density
        {
            get
            {
                return _density;
            }
            set
            {
                if (value >= 1 && value <= 7)
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
                    rect6.Fill = br2;
                    rect7.Fill = br2;

                    switch (_density)
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
                        case 6:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            rect4.Fill = br1;
                            rect5.Fill = br1;
                            rect6.Fill = br1;
                            break;
                        case 7:
                            rect1.Fill = br1;
                            rect2.Fill = br1;
                            rect3.Fill = br1;
                            rect4.Fill = br1;
                            rect5.Fill = br1;
                            rect6.Fill = br1;
                            rect7.Fill = br1;
                            break;
                    }
                }
            }
        }
        private void BtnClick(object sender, RoutedEventArgs e)
        {

            Button srcButton = e.Source as Button;
            if ("btnIncrease1" == srcButton.Name)
            {
                m_density++;
            }
            else if ("btnDecrease1" == srcButton.Name)
            {
                m_density--;
            }
            else if (srcButton.Name == "btnDecrease")
            {
                string scaling_percent = this.scaling_text.Text.Substring(0, this.scaling_text.Text.Length - 1);
                int scaling = Convert.ToInt32(scaling_percent);

                scaling--;
                scaling = scaling < 25 ? 25 : scaling;
                scaling = scaling > 400 ? 400 : scaling;
                scaling_percent = scaling.ToString();
                scaling_percent += "%";
                this.scaling_text.Text = scaling_percent;
            }
            else if (srcButton.Name == "btnIncrease")
            {
                string scaling_percent = this.scaling_text.Text.Substring(0, this.scaling_text.Text.Length - 1);
                int scaling = Convert.ToInt32(scaling_percent);

                scaling++;
                scaling = scaling > 400 ? 400 : scaling;
                scaling = scaling < 25 ? 25 : scaling;
                scaling_percent = scaling.ToString();
                scaling_percent += "%";
                this.scaling_text.Text = scaling_percent;
            }

        }

   
 
    }
}
