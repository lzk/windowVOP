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
        private string currentFolderName = @"";
        private GraphServiceClient client = null;
        public bool Result { get; private set; }

        private bool IsRequesting = false;

        private DriveItem _sourceItem;

        private GraphServiceClient graphClient { get; set; }
        private ClientType clientType { get; set; }

        private DriveItem CurrentFolder { get; set; }
        private DriveItem UpperFolder { get; set; }
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
            if (PathText.Text == "")
                UpFolderButton.IsEnabled = false;
            else
                UpFolderButton.IsEnabled = true;
            FileBrowser.SelectedIndex = 0;
            FileBrowser.Focus();
        }       
        private async Task Start()
        {
            try
            {
                await SignIn();
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
                    string folderName = frm.m_folderName.TrimStart();
                    folderName = folderName.TrimEnd();
                    if(CheckFolder(folderName))
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name_exist"),//"The folder Name already exists",
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                        return;
                    }
                    else
                    {
                        if (folderName != "")
                        {
                            var folderToCreate = new DriveItem { Name = folderName, Folder = new Folder() };
                            var newFolder =
                                await this.graphClient.Drive.Items[this.SelectedItem.Id].Children.Request()
                                    .AddAsync(folderToCreate);

                            if (newFolder != null)
                            {
                                if (PathText.Text != "")
                                {
                                    await LoadFolderFromPath(PathText.Text);
                                    UpFolderButton.IsEnabled = true;
                                }
                                else
                                {
                                    await LoadFolderFromPath();
                                    UpFolderButton.IsEnabled = false;
                                }
                            }
                        }
                        else
                        {
                            string str = (string)Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
                            string content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder");
                            string message = string.Format(str, "Folder");
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                                Application.Current.MainWindow,
                                message,//"The folder cannot be empty", 
                                (string)this.TryFindResource("ResStr_Warning"));
                            return;
                        }
                    } 
                }
            }
            catch (Exception)
            {
                string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Invalid_xxx");
                message = string.Format(message, "Folder Name");
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                    Application.Current.MainWindow,
                    message,//"Invalid folder name.", 
                    (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
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
                    currentFolderName = temp.Substring(temp.LastIndexOf('/')+1);
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
                    if (PathText.Text == "")
                        UpFolderButton.IsEnabled = false;
                    else
                        UpFolderButton.IsEnabled = true;
                }
                else
                {
                    await LoadFolderFromPath();
                    currentPath = "";
                    PathText.Text = "";
                    UpFolderButton.IsEnabled = false;
                    selectedPath = "";
                }
            }
            catch (Exception) { }
           
        }
        private async Task SignIn(string path = null)
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
                //    PresentServiceException(exception);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                    Application.Current.MainWindow, 
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Connect_OneDrive_Fail"),//"Connection Onedirive failed!", 
                    (string)this.TryFindResource("ResStr_Warning"));
                this.Close();
            }
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
                //    PresentServiceException(exception);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                    Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Connect_OneDrive_Fail"),//"Connection Onedrive failed!", 
                    (string)this.TryFindResource("ResStr_Warning"));
                return;
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
        private bool CheckFolder(string foldername)
        {
            LoadProperties(CurrentFolder);
            if (CurrentFolder.Folder != null && CurrentFolder.Children != null && CurrentFolder.Children.CurrentPage != null)
            {
                foreach (var obj in CurrentFolder.Children.CurrentPage)
                {
                    if ((obj.File == null) && (obj.Folder != null))
                    {
                        if (obj.Name == foldername)
                            return true;
                    }                   
                }
            }
            return false;
        }
        private async Task CheckUploadFolder(string path = null)
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
                UpperFolder = folder;
            }
            catch (Exception exception)
            {
                //    PresentServiceException(exception);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Connect_OneDrive_Fail"),//"Connection Onedrive failed!", 
                    (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
        }
        private bool CheckUploadFolderName(string foldername)
        {
            if (UpperFolder.Folder != null && UpperFolder.Children != null && UpperFolder.Children.CurrentPage != null)
            {
                foreach (var obj in UpperFolder.Children.CurrentPage)
                {
                    if ((obj.File == null) && (obj.Folder != null))
                    {
                        if (obj.Name == foldername)
                            return true;
                    }
                }
            }
            return false;
        }
        private bool CheckFileName(string filename)
        {
            LoadProperties(CurrentFolder);
            if (CurrentFolder.Folder != null && CurrentFolder.Children != null && CurrentFolder.Children.CurrentPage != null)
            {
                foreach (var obj in CurrentFolder.Children.CurrentPage)
                {
                    if ((obj.File != null) && (obj.Folder == null))
                    {
                        if (obj.Name == filename)
                            return true;
                    }
                }
            }
            return false;
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
        private async Task ViewItem(string path = null)
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
                currentPath = path;
                PathText.Text = currentPath;
                if (PathText.Text == "")
                    UpFolderButton.IsEnabled = false;
                else
                    UpFolderButton.IsEnabled = true;
            }
            catch (Exception exception)
            {
                //    PresentServiceException(exception);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                    Application.Current.MainWindow,
                    (string)Application.Current.MainWindow.TryFindResource("ResStr_Connect_OneDrive_Fail"),//"Connection Onedrive failed!", 
                    (string)this.TryFindResource("ResStr_Warning"));
                return;
            }

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
                    currentFolderName = info.fileName;
                    temp = currentPath + "/" + info.fileName;
                    selectedPath = temp;
                    await ViewItem(temp);                    
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
                string message = "";
                if (currentPath != "")
                {
                    string temp = "";
                    temp = currentPath.Remove(currentPath.LastIndexOf('/'), currentPath.Length - currentPath.LastIndexOf('/'));
                    if (temp != "")
                    {
                        await CheckUploadFolder(temp);
                    }
                    else
                    {
                        await CheckUploadFolder();
                    }
                    if (UpperFolder != null)
                    {

                        if (!CheckUploadFolderName(currentFolderName))
                        {
                            message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Not_Exist");
                            message = string.Format(message/*"The folder {0} does not exist."*/, currentFolderName);
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow, message, (string)this.TryFindResource("ResStr_Error"));
                            return;
                        }
                    }
                    else
                        return;
                    
                }                
                else
                {
                    await CheckUploadFolder();
                    if (UpperFolder == null)
                        return;                    
                    try
                    {                        
                        foreach (string filePath in FileList)
                        {
                            string fileName = System.IO.Path.GetFileName(filePath);
                            UploadStaus.Text = "Uploading file " + fileName;
                            if (CheckFileName(fileName))
                            {
                                message = (string)Application.Current.MainWindow.TryFindResource("ResStr_File_exist_Do_You_overwrite");
                                message = string.Format(message,/*"The {0} already exists£¬Do you want to overwrite?",*/fileName);
                                VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo_NoIcon, this,
                                 message,(string)this.TryFindResource("ResStr_Prompt"));

                                if (VOP.Controls.MessageBoxExResult.Yes == ret)
                                {
                                    await Upload(client, filePath);
                                }
                            }
                            else
                            {
                                await Upload(client, filePath);
                            }

                        }                       
                    }
                    catch (Exception ex)
                    {
                        UploadStaus.Text = "Picture uploading error : " + ex.Message;
                    }
                }
            }                   
        }
        private void btnClose_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Close();
                e.Handled = true;
            }
        }
        private async Task Upload(GraphServiceClient client, string filePath)
        {
            var targetFolder = CurrentFolder;
            string Filename = "";
            string message = "";
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
                    catch (Exception exception)
                    {
                        //                        PresentServiceException(exception);
                        message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Update_File_fail");
                        message = string.Format(message,/*"Upload {0} failed!",*/ Filename);
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning, 
                            Application.Current.MainWindow, message, (string)this.TryFindResource("ResStr_Warning"));
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
