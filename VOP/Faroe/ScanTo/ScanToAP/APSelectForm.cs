using Dropbox.Api;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace VOP
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class APSelectForm : Window
    {
        public string m_programType = "Paint";

        public APSelectForm()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall 此键的子健为本机所有注册过的软件的卸载程序,通过此思路进行遍历安装的软件
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            string[] key1 = key.GetSubKeyNames();//返回此键所有的子键名称
            List<string> key2 = key1.ToList<string>();//因为有的项木有"DisplayName"或"DisplayName"的键值的时候要把键值所在数组中的的元素进行删除
            RegistryKey subkey = null;
            try
            {
                for (int i = 0; i < key2.Count; i++)
                {
                    subkey = key.OpenSubKey(key2[i]);//通过list泛型数组进行遍历,某款软件项下的子键
                    if (subkey.GetValue("DisplayName") != null)
                    {
                        if (subkey.GetValue("DisplayIcon") != null)
                        {
                            string path = subkey.GetValue("DisplayIcon").ToString();
                            string SubPath = path.Substring(path.Length - 1, 1);//截取子键值的最后一位进行判断
                            if (SubPath == "o" || path.IndexOf("exe") == -1)//如果为o 就是ico 或 找不到exe的 表示为图标文件或只有个标识而没有地址的
                            {
                                key2.RemoveAt(i);//首先删除数组中此索引的元素
                                i -= 1;//把循环条件i的值进行从新复制,否则下面给listview的项的tag属性进行赋值的时候会报错
                                continue;
                            }
                            //listView1.Items.Add(subkey.GetValue("DisplayName").ToString());//把软件名称添加到listview控件中
                            if (SubPath == "e")//如果为e 就代表着是exe可执行文件,
                            {
                                //listView1.Items[i].Tag = path;//则表示可以直接把地址赋给tag属性
                                continue;
                            }
                            if (SubPath == "0" || SubPath == "1")//因为根据观察 取的是DisplayIcon的值 表示为图片所在路径 如果为0或1,则是为可执行文件的图标  
                            {
                                path = path.Substring(0, path.LastIndexOf("e") + 1);//进行字符串截取,
                                //listView1.Items[i].Tag = path;//则表示可以直接把地址赋给tag属性
                                continue;
                            }

                        }
                        else
                        {
                            key2.RemoveAt(i);//首先删除数组中此索引的元素
                            i -= 1;//把循环条件i的值进行从新复制,否则下面给listview的项的tag属性进行复制的时候会报错
                            continue;
                        }
                    }
                    else
                    {
                        key2.RemoveAt(i);//首先删除数组中此索引的元素
                        i = i - 1;//把循环条件i的值进行从新复制,否则下面给listview的项的tag属性进行复制的时候会报错
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            //APListBox.Items.Add(CreateListItem("PhotoShop"));
            APListBox.Items.Add(CreateListItem("Paint"));
            APListBox.Items.Add(CreateListItem("PhotoViewer"));
        }

        private ListBoxItem CreateListItem(string apName)
        {
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            if (apName == "PhotoShop")
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/Adobe Photoshop.png", UriKind.RelativeOrAbsolute);
            }
            else if (apName == "Paint")
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/mspaint.png", UriKind.RelativeOrAbsolute);
            }
            else
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/WindowsPhotoViewer.png", UriKind.RelativeOrAbsolute);
            }

            bitmapImage.DecodePixelWidth = 100;
            bitmapImage.EndInit();

            img.Source = bitmapImage;
            img.Width = 80;


            TextBlock text = new TextBlock();
            text.Text = apName;
            text.Margin = new Thickness(10, 0, 0, 0);
            text.VerticalAlignment = VerticalAlignment.Center;
            text.FontSize = 16;

            SolidColorBrush txtbrush = new SolidColorBrush();
            txtbrush.Color = Colors.DodgerBlue;
            text.Foreground = txtbrush;

            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;

            stack.Children.Add(img);
            stack.Children.Add(text);

            ListBoxItem item = new ListBoxItem();
            SolidColorBrush bgbrush = new SolidColorBrush();
            bgbrush.Color = Colors.AliceBlue;
            item.Background = bgbrush;

            item.Content = stack;

            item.Tag = apName;

            return item;
        }

        private void cboListBoxItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem item = APListBox.SelectedItem as ListBoxItem;
            string apName = item.Tag.ToString();

            m_programType = apName;
        }
     
        private void OkClick(object sender, RoutedEventArgs e)
        {
         
            DialogResult = true;
            this.Close();
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
