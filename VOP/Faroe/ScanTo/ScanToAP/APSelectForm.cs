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

            //HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall �˼����ӽ�Ϊ��������ע����������ж�س���,ͨ����˼·���б�����װ�����
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            string[] key1 = key.GetSubKeyNames();//���ش˼����е��Ӽ�����
            List<string> key2 = key1.ToList<string>();//��Ϊ�е���ľ��"DisplayName"��"DisplayName"�ļ�ֵ��ʱ��Ҫ�Ѽ�ֵ���������еĵ�Ԫ�ؽ���ɾ��
            RegistryKey subkey = null;
            try
            {
                for (int i = 0; i < key2.Count; i++)
                {
                    subkey = key.OpenSubKey(key2[i]);//ͨ��list����������б���,ĳ��������µ��Ӽ�
                    if (subkey.GetValue("DisplayName") != null)
                    {
                        if (subkey.GetValue("DisplayIcon") != null)
                        {
                            string path = subkey.GetValue("DisplayIcon").ToString();
                            string SubPath = path.Substring(path.Length - 1, 1);//��ȡ�Ӽ�ֵ�����һλ�����ж�
                            if (SubPath == "o" || path.IndexOf("exe") == -1)//���Ϊo ����ico �� �Ҳ���exe�� ��ʾΪͼ���ļ���ֻ�и���ʶ��û�е�ַ��
                            {
                                key2.RemoveAt(i);//����ɾ�������д�������Ԫ��
                                i -= 1;//��ѭ������i��ֵ���д��¸���,���������listview�����tag���Խ��и�ֵ��ʱ��ᱨ��
                                continue;
                            }
                            //listView1.Items.Add(subkey.GetValue("DisplayName").ToString());//�����������ӵ�listview�ؼ���
                            if (SubPath == "e")//���Ϊe �ʹ�������exe��ִ���ļ�,
                            {
                                //listView1.Items[i].Tag = path;//���ʾ����ֱ�Ӱѵ�ַ����tag����
                                continue;
                            }
                            if (SubPath == "0" || SubPath == "1")//��Ϊ���ݹ۲� ȡ����DisplayIcon��ֵ ��ʾΪͼƬ����·�� ���Ϊ0��1,����Ϊ��ִ���ļ���ͼ��  
                            {
                                path = path.Substring(0, path.LastIndexOf("e") + 1);//�����ַ�����ȡ,
                                //listView1.Items[i].Tag = path;//���ʾ����ֱ�Ӱѵ�ַ����tag����
                                continue;
                            }

                        }
                        else
                        {
                            key2.RemoveAt(i);//����ɾ�������д�������Ԫ��
                            i -= 1;//��ѭ������i��ֵ���д��¸���,���������listview�����tag���Խ��и��Ƶ�ʱ��ᱨ��
                            continue;
                        }
                    }
                    else
                    {
                        key2.RemoveAt(i);//����ɾ�������д�������Ԫ��
                        i = i - 1;//��ѭ������i��ֵ���д��¸���,���������listview�����tag���Խ��и��Ƶ�ʱ��ᱨ��
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
