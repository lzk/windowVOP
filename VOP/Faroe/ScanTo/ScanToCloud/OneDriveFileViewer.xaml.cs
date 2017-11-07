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

using System.Net.Http.Headers;
using Microsoft.Graph;
using Microsoft.Identity.Client;
namespace VOP
{
   
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class OneDriveFileViewer : Window
    {
        public List<string> FileList { get; set; }
        private string currentPath = @"";
        private string selectedPath = @"";
        private GraphServiceClient client = null;
        public bool Result { get; private set; }

        private bool IsRequesting = false;

        private DriveItem _sourceItem;

        private GraphServiceClient graphClient { get; set; }
        private ClientType clientType { get; set; }

        private DriveItem CurrentFolder { get; set; }
        private DriveItem SelectedItem { get; set; }
        public OneDriveFileViewer(GraphServiceClient client, List<string> fileList)
        {
            InitializeComponent();
            this.graphClient = client;
            FileList = fileList;
        }        

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            if(OneDriveFlow.FlowType == CloudFlowType.SimpleView)
            {
                UploadButton.Content = "OK";
            }

            await Start();
        }       
        private async Task Start()
        {
            try
            {
                await LoadFolderFromPath();
            }
            catch (ServiceException exception)
            {
                PresentServiceException(exception);
                this.graphClient = null;
            }
        }

        private ListViewItem CreateViewItem(string fileName, string fileType="Folder", int fileIndex=0, Stream imageStream=null)
        {
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
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
                        var folderToCreate = new DriveItem { Name = folderName, Folder = new Folder() };
                        var newFolder =
                            await this.graphClient.Drive.Items[this.SelectedItem.Id].Children.Request()
                                .AddAsync(folderToCreate);

                        if (newFolder != null)
                        {
                            await LoadFolderFromPath();
                        }                        
                    }                 
                }
            }
            catch (Exception) { }
        }
       
        private async Task LoadFolderFromId(string id)
        {
            if (null == this.graphClient) return;
            try
            {
                var expandString = this.clientType == ClientType.Consumer
                    ? "thumbnails,children($expand=thumbnails)"
                    : "thumbnails,children";

                var folder =
                    await this.graphClient.Drive.Items[id].Request().Expand(expandString).GetAsync();

                ProcessFolder(folder);
            }
            catch (Exception exception)
            {
                PresentServiceException(exception);
            }
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
                    currentPath = temp;
                    if (currentPath != "")
                    {
                        await LoadFolderFromPath(temp);
                    }
                    else
                    {
                        await LoadFolderFromPath();

                    }
                    PathText.Text = currentPath;
                }
                else
                {
                    await LoadFolderFromPath();
                    currentPath = "";
                    PathText.Text = "";
                    selectedPath = "";
                }
            }
            catch (Exception) { }
           
        }
        
        private async Task LoadFolderFromPath(string path = null)
        {
            if (null == this.graphClient) return;
            try
            {
                DriveItem folder;

                var expandValue = this.clientType == ClientType.Consumer
                    ? "thumbnails,children($expand=thumbnails)"
                    : "thumbnails,children";

                if (path == null)
                {
                    folder = await this.graphClient.Drive.Root.Request().Expand(expandValue).GetAsync();
                }
                else
                {
                    folder =
                        await
                            this.graphClient.Drive.Root.ItemWithPath("/" + path)
                                .Request()
                                .Expand(expandValue)
                                .GetAsync();
                }

                ProcessFolder(folder);
            }
            catch (Exception exception)
            {
                PresentServiceException(exception);
            }

        }

        private void ProcessFolder(DriveItem folder)
        {
            if (folder != null)
            {
                CurrentFolder = folder;

                LoadProperties(folder);

                if (folder.Folder != null && folder.Children != null && folder.Children.CurrentPage != null)
                {
                    LoadChildren(folder.Children.CurrentPage);
                }
            }
        }

        private void LoadChildren(IList<DriveItem> items)
        {
            FileBrowser.Items.Clear();
            int index = 0;
            // Load the children
            foreach (var obj in items)
            {
                ListViewItem viewItem = null;
                if ((obj.File == null) && (obj.Folder != null))
                {
                    viewItem = CreateViewItem(obj.Name, "Folder", index);
                    FileBrowser.Items.Add(viewItem);
                    index++;
                }
                else if ((obj.File != null) && (obj.Folder == null))
                {
                    viewItem = CreateViewItem(obj.Name, "File", index);
                    FileBrowser.Items.Add(viewItem);
                    index++;
                }
//                LoadThumbnail();
            }            
        }       

        private async void LoadThumbnail()
        {
            var thumbnail = await this.ThumbnailUrlAsync("medium");
            if (null != thumbnail)
            {
                string thumbnailUri = thumbnail.Url;
            }
        }

        /// <summary>
        /// Retrieve a specific size thumbnail's metadata. If it isn't already 
        /// available make a call to the service to retrieve it.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<Microsoft.Graph.Thumbnail> ThumbnailUrlAsync(string size = "large")
        {
            bool loadedThumbnails = this._sourceItem != null && this._sourceItem.Thumbnails != null &&
                                    this._sourceItem.Thumbnails.CurrentPage != null;
            if (loadedThumbnails)
            {
                // See if we already have that thumbnail
                Thumbnail thumbnail = null;
                ThumbnailSet thumbnailSet = null;

                switch (size.ToLower())
                {
                    case "small":
                        thumbnailSet = this._sourceItem.Thumbnails.CurrentPage.FirstOrDefault(set => set.Small != null);
                        thumbnail = thumbnailSet == null ? null : thumbnailSet.Small;
                        break;
                    case "medium":
                        thumbnailSet = this._sourceItem.Thumbnails.CurrentPage.FirstOrDefault(set => set.Medium != null);
                        thumbnail = thumbnailSet == null ? null : thumbnailSet.Medium;
                        break;
                    case "large":
                        thumbnailSet = this._sourceItem.Thumbnails.CurrentPage.FirstOrDefault(set => set.Large != null);
                        thumbnail = thumbnailSet == null ? null : thumbnailSet.Large;
                        break;
                    default:
                        thumbnailSet = this._sourceItem.Thumbnails.CurrentPage.FirstOrDefault(set => set[size] != null);
                        thumbnail = thumbnailSet == null ? null : thumbnailSet[size];
                        break;
                }

                if (thumbnail != null)
                {
                    return thumbnail;
                }

            }

            if (!loadedThumbnails)
            {
                try
                {
                    // Try to load the thumbnail from the service if we haven't loaded thumbnails.
                    return await this.graphClient.Drive.Items[this._sourceItem.Id].Thumbnails["0"][size].Request().GetAsync();
                }
                catch (ServiceException)
                {

                    // Just swallow not found. We don't want an error popup and we just won't render a thumbnail
                    return null;
                }
            }

            return null;
        }
        private void LoadProperties(DriveItem item)
        {
            this.SelectedItem = item;
            FileBrowser.SelectedItem = item;
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
                    await LoadFolderFromPath(temp);
                    currentPath = temp;
                    PathText.Text = currentPath;                                
                }
            }
            catch (Exception) { }            
        }
       

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (OneDriveFlow.FlowType == CloudFlowType.SimpleView)
            {
                if (selectedPath == "")
                    OneDriveFlow.SavePath = "/";
                else
                    OneDriveFlow.SavePath = selectedPath;                
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
                        await Upload(client, filePath);
                    }
                    UploadStaus.Text = "";
                    if (currentPath != "")
                    {
                        await LoadFolderFromPath(currentPath);
                    }
                    else
                    {
                        await LoadFolderFromPath();
                    }
                }
                catch (Exception ex)
                {
                    UploadStaus.Text = "Picture uploading error : " + ex.Message;
                }
            }
        }

        private async Task Upload(GraphServiceClient client, string filePath)
        {
            var targetFolder = CurrentFolder;
            string Filename = "";
            using (var stream = GetFileStreamForUpload(targetFolder.Name, filePath, out Filename))
            {
                if (stream != null)
                {
                    try
                    {
                        var uploadedItem =
                            await
                                this.graphClient.Drive.Items[targetFolder.Id].ItemWithPath(Filename).Content.Request()
                                    .PutAsync<DriveItem>(stream);                   
                    }
                    catch (Exception exception)
                    {
                        PresentServiceException(exception);
                    }
                }
            }

            //using (var stream = new FileStream(fileContent, FileMode.Open, FileAccess.Read))
            //{
            //    //                var response = await client.Files.UploadAsync(folder + "/" + fileName, WriteMode.Overwrite.Instance, body: stream);
            //    if (stream != null)
            //    {
            //        try
            //        {
            //            var uploadedItem =
            //                await
            //                    this.graphClient.Drive.Items[targetFolder.Id].ItemWithPath(fileName).Content.Request()
            //                        .PutAsync<DriveItem>(stream);

            //            AddItemToFolderContents(uploadedItem);

            //            MessageBox.Show("Uploaded with ID: " + uploadedItem.Id);
            //        }
            //        catch (Exception exception)
            //        {
            //            PresentServiceException(exception);
            //        }
            //    }

            //}
        }
        private static void PresentServiceException(Exception exception)
        {
            string message = null;
            var oneDriveException = exception as ServiceException;
            if (oneDriveException == null)
            {
                message = exception.Message;
            }
            else
            {
                message = string.Format("{0}{1}", Environment.NewLine, oneDriveException.ToString());
            }

            MessageBox.Show(string.Format("OneDrive reported the following error: {0}", message));
        }
        private System.IO.Stream GetFileStreamForUpload(string targetFolderName, string filename, out string originalFilename)
        { 
            try
            {
                originalFilename = System.IO.Path.GetFileName(filename);
                return new System.IO.FileStream(filename, System.IO.FileMode.Open);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error uploading file: " + ex.Message);
                originalFilename = null;
                return null;
            }
        }       

        private void title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}