using Dropbox.Api;
using Dropbox.Api.Stone;
using Dropbox.Api.Files;
using Dropbox.Api.Team;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.IO;


namespace VOP
{
    public class ViewItemInfo
    {
        public string fileType = "";
        public string fileName = "";

        public ViewItemInfo()
        {

        }
        public ViewItemInfo(string type, string name)
        {
            fileType = type;
            fileName = name;
        }

    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FileViewer : Window
    {
        public List<string> FileList { get; set; }
        private string currentPath = @"";
        private string selectedPath = @"";
        private DropboxClient client = null;
        public bool Result { get; private set; }
        private bool IsRequesting = false;


        public FileViewer(DropboxClient client, List<string> fileList)
        {
            InitializeComponent();
            this.client = client;
            FileList = fileList;
            DropboxClientConfig.CancelTokenSrc = new CancellationTokenSource();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            if(DropBoxFlow.FlowType == CloudFlowType.SimpleView)
            {
                UploadButton.Content = "OK";
            }

            await Start();
        }

        private async Task Start()
        {
    
            try
            { 
                await ListFolder(client, currentPath);
            }
            catch(Exception ex)
            {

            }
          
        }

        private ListViewItem CreateViewItem(string fileName, string fileType="Folder", int fileIndex=0, Stream imageStream=null)
        {
            Image img = new Image();
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();

            if(fileType == "Folder")
            {
                bitmapImage.UriSource = new Uri("pack://application:,,, /Images/Folder-icon.png", UriKind.RelativeOrAbsolute);
            }
            else if (fileType == "File")
            {
                if(imageStream == null)
                {
                    bitmapImage.UriSource = new Uri("pack://application:,,, /Images/file.png", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    bitmapImage.StreamSource = imageStream;
                }
            }
           
            bitmapImage.DecodePixelWidth = 100;
            bitmapImage.EndInit();

            img.Source = bitmapImage;
            img.Width = 80;


            TextBlock text = new TextBlock();
            text.Text = fileName;
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

            ListViewItem item = new ListViewItem();
            SolidColorBrush bgbrush = new SolidColorBrush();
            bgbrush.Color = fileIndex%2 == 0 ? Colors.AliceBlue : Colors.AliceBlue;
            item.Background = bgbrush;

            item.Content = stack;
            item.MouseDoubleClick += new MouseButtonEventHandler(ViewItemDoubleClick);

            ViewItemInfo info = new ViewItemInfo(fileType, fileName);
            item.Tag = info;

            return item;
        }

        private async Task GetCurrentAccount(DropboxClient client)
        {
            var full = await client.Users.GetCurrentAccountAsync();
        }

        private async void CreateFolderButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                bool? result = null;
                FolderNameForm frm = new FolderNameForm();
                frm.Owner = Application.Current.MainWindow;

                result = frm.ShowDialog();

                if(result == true)
                {
                    string folderName = frm.m_folderName;

                    if(folderName != "")
                    {
                        string folderPath = currentPath + "/" + folderName;

                        await CreateFolder(client, folderPath);
                        await ListFolder(client, currentPath);
                    }
                 
                }

            }
            catch (Exception) { }
        }

        private async Task<FolderMetadata> CreateFolder(DropboxClient client, string path)
        {
            var folderArg = new CreateFolderArg(path);
            var folder = await client.Files.CreateFolderAsync(folderArg);

            return folder;
        }

        private async void UpFolderButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (currentPath != "")
                {
                    string temp = "";
                    temp = currentPath.Remove(currentPath.LastIndexOf('/'), currentPath.Length - currentPath.LastIndexOf('/'));
                    selectedPath = temp;

                    ListFolderResult res = await ListFolder(client, temp);

                    if (res != null)
                    {
                        currentPath = temp;
                        PathText.Text = currentPath;
                    }

                }
                else
                {
                    await ListFolder(client, "");
                    currentPath = "";
                    PathText.Text = "";
                    selectedPath = "";
                }
            }
            catch (Exception) { }
           
        }

        private async void ViewItemDoubleClick(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            ViewItemInfo info = item.Tag as ViewItemInfo;

            try
            {
                if (info.fileType == "Folder")
                {
                    string temp = "";
                    temp = currentPath + "/" + info.fileName;
                    selectedPath = temp;

                    ListFolderResult res =  await ListFolder(client, temp);

                    if(res != null)
                    {
                        currentPath = temp;
                        PathText.Text = currentPath;
                    }
                  
                }
            }
            catch (Exception) { }
            
        }

        private async Task<ListFolderResult> ListFolder(DropboxClient client, string path)
        {
            if (IsRequesting)
            {
                DropboxClientConfig.CancelTokenSrc.Cancel();
                DropboxClientConfig.CancelTokenSrc = new CancellationTokenSource();
            }

            IsRequesting = true;
            int index = 0;
            //await GetCurrentAccount(client);

            try
            {
                var list = await client.Files.ListFolderAsync(path);

                FileBrowser.Items.Clear();
                // show folders then files
                foreach (var item in list.Entries.Where(i => i.IsFolder))
                {
                    ListViewItem viewItem = CreateViewItem(item.AsFolder.Name, "Folder", index);
                    FileBrowser.Items.Add(viewItem);
                    index++;
                }

                foreach (var item in list.Entries.Where(i => i.IsFile))
                {

                    var file = item.AsFile;
                    string filePath = path + "/" + file.Name;
                    Stream x = null;
                    ListViewItem viewItem = null;

                    string fileExt = System.IO.Path.GetExtension(file.Name).ToLower();

                    if (
                            fileExt == ".jpg"
                            || fileExt == ".jpeg"
                            || fileExt == ".png"
                            || fileExt == ".tiff"
                            || fileExt == ".tif"
                            || fileExt == ".gif"
                            || fileExt == ".bmp"
                            || fileExt == ".webp"
                            || fileExt == ".wbmp")
                    {
                        try
                        {
                            IDownloadResponse<FileMetadata> data = await client.Files.GetThumbnailAsync(filePath);
                            x = await data.GetContentAsStreamAsync();
                        }
                        catch (Exception ex)
                        {
                        }


                    }

                    viewItem = CreateViewItem(file.Name, "File", index, x);
                    FileBrowser.Items.Add(viewItem);
                    index++;
                }

                IsRequesting = false;

                if (list.HasMore)
                {

                }

                return list;
            }
            catch(Exception ex)
            {
                IsRequesting = false;
                return null;
            }

        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (DropBoxFlow.FlowType == CloudFlowType.SimpleView)
            {
                DropBoxFlow.SavePath = selectedPath;
                this.Close();
            }
            else
            {
                if (FileList == null)
                    return;

                try
                {
                    foreach (string filePath in FileList)
                    {
                        string fileName = System.IO.Path.GetFileName(filePath);
                        UploadStaus.Text = "Uploading file " + fileName;
                        await Upload(client, currentPath, fileName, filePath);
                    }

                    UploadStaus.Text = "";
                    await ListFolder(client, currentPath);
                }
                catch (Exception) { }
            }
        }

        private async Task Upload(DropboxClient client, string folder, string fileName, string fileContent)
        {

            using (var stream = new MemoryStream(System.Text.UTF8Encoding.UTF8.GetBytes(fileContent)))
            {
                var response = await client.Files.UploadAsync(folder + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);
            }
        }


        private async Task ChunkUpload(DropboxClient client, string folder, string fileName)
        {
            // Chunk size is 128KB.
            const int chunkSize = 128 * 1024;

            // Create a random file of 1MB in size.
            var fileContent = new byte[1024 * 1024];
            new Random().NextBytes(fileContent);

            using (var stream = new MemoryStream(fileContent))
            {
                int numChunks = (int)Math.Ceiling((double)stream.Length / chunkSize);

                byte[] buffer = new byte[chunkSize];
                string sessionId = null;

                for (var idx = 0; idx < numChunks; idx++)
                {
                    var byteRead = stream.Read(buffer, 0, chunkSize);

                    using (MemoryStream memStream = new MemoryStream(buffer, 0, byteRead))
                    {
                        if (idx == 0)
                        {
                            var result = await client.Files.UploadSessionStartAsync(body: memStream);
                            sessionId = result.SessionId;
                        }

                        else
                        {
                            UploadSessionCursor cursor = new UploadSessionCursor(sessionId, (ulong)(chunkSize * idx));

                            if (idx == numChunks - 1)
                            {
                                await client.Files.UploadSessionFinishAsync(cursor, new CommitInfo(folder + "/" + fileName), memStream);
                            }

                            else
                            {
                                await client.Files.UploadSessionAppendV2Async(cursor, body: memStream);
                            }
                        }
                    }
                }
            }
        }

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
