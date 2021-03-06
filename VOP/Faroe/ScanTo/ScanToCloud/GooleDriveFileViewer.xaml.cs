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
using DotNetOpenAuth.OAuth2;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Util;
using Google.Apis.Helper;

namespace VOP
{
    public class FolderItem
    {
        public string name;
        public string id;
        public string parentID;

        public FolderItem()
        {

        }
        public FolderItem(string folder, string parentid, string currentID)
        {
            name = folder;
            parentID = parentid;
            id = currentID;
        }
    }
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GoogleDriveFileViewer : Window
    {
        public List<File> FileList { get; set; }

        private string currentPath = @"";
        private string selectedPath = @"";
        private string currentFolderName = @"";
        public bool Result { get; private set; }
        private CloudFlowType flowType { get; set; }
        private string CurrentFolder = @"";
        private string UpperFolder = @"";
        private string rootid = @"";
        private List<FolderItem> folderList = new List<FolderItem>();
        private List<string> uploadFiles { get; set; }
        private DriveService _service = null;
        private string ParantID = "";
        private string currentID = "";
        private string Parent = "";
 

        public GoogleDriveFileViewer(List<File> fileList, List<string> uploadList, string folder, CloudFlowType type)
        {
            InitializeComponent();

            Win32.OutputDebugString("GoogleDriveFileViewer===>Initial");

            FileList = fileList;
            CurrentFolder = folder;
            flowType = type;
            uploadFiles = uploadList;
        }

        public DriveService Service
        {
            get
            {
                return _service;
            }
            set
            {
                if (value != null)
                    _service = value;
            }
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TitleBar.MouseLeftButtonDown += new MouseButtonEventHandler(title_MouseLeftButtonDown);

            if (flowType == CloudFlowType.SimpleView)
            {
                UploadButton.Content = "OK";
            }

            if (PathText.Text == "")
                UpFolderButton.IsEnabled = false;
            else
                UpFolderButton.IsEnabled = true;


            Win32.OutputDebugString("Google viewer Windows Load");

            GetFolderList();
            InitFileandFodlers();

            if (FileBrowser.Items.Count > 0)
            {
                int i = 0;
                foreach (ListViewItem item in FileBrowser.Items)
                {
                    ViewItemInfo info = item.Tag as ViewItemInfo;
                    if (info.fileName == CurrentFolder && info.fileType == "Folder")
                    {
                        FileBrowser.SelectedIndex = i;
                        ParantID = info.parentid;
                    }
                    i++;
                }
                FileBrowser.Focus();
            }
        }

        private void GetFolderList()
        {
            Win32.OutputDebugString("GetFolderList===>Enter");
            bool isHasFolder = false;

            if (FileList == null)
            {
                Win32.OutputDebugString("GetFolderList===>Leave");
                return;
            }

            folderList.Clear();
            foreach (File file in FileList)
            {
                FolderItem item = new FolderItem();

                if (rootid == "")
                {
                    if (file.Parents.Count > 0 && file.Parents.Count == 1)
                    {
                        if (file.Parents[0].IsRoot == true)
                        {
                            rootid = file.Parents[0].Id;
                            item.name = "root";
                            item.id = rootid;
                            item.parentID = rootid;
                            folderList.Add(item);
                        }
                    }
                }
                if (file.MimeType == "application/vnd.google-apps.folder")
                {
                    item.name = file.Title;
                    item.parentID = file.Parents[0].Id;
                    item.id = file.Id;
                    folderList.Add(item);
                    isHasFolder = true;

                    if (CurrentFolder == item.name)
                    {
                        ParantID = item.parentID;
                    }
                }

            }
            if (FileList != null && isHasFolder == false && rootid == "" && FileList.Count > 0)
            {
                if (FileList[0].Parents.Count > 0 && FileList[0].Parents.Count == 1)
                {
                    if (FileList[0].Parents[0].IsRoot == true)
                    {
                        rootid = FileList[0].Parents[0].Id;
                    }
                }
            }
            if (CurrentFolder == "")
            {
                ParantID = rootid;
            }
            Win32.OutputDebugString("GetFolderList===>Leave");
        }


        private void InitFileandFodlers()
        {
            int index = 0;
            Win32.OutputDebugString("InitFileandFodlers===>Enter");
            if (FileList == null)
            {
                FileList = new List<File>();
            }

            if (FileList.Count > 0)
                FileBrowser.Items.Clear();

            string str = string.Format("get the file count is {0}", FileList.Count);
            Win32.OutputDebugString(str);

            Win32.OutputDebugString("Add the folders");
            foreach (File file in FileList)
            {
                if (file != null && file.Parents.Count > 0 && file.Parents[0].IsRoot == true)
                {
                    if (file.MimeType == "application/vnd.google-apps.folder")
                    {
                        ListViewItem viewItem = null;
                        viewItem = CreateViewItem(file.Title, "root", ParantID, file.Id, "Folder", index);
                        if (viewItem != null)
                        {
                            FileBrowser.Items.Add(viewItem);
                            index++;
                        }
                    }
                }
            }

            Win32.OutputDebugString("Add the Files");
            foreach (File file in FileList)
            {
                if (file != null && file.Parents.Count > 0 && file.Parents[0].IsRoot == true)
                {
                    if (file.MimeType != "application/vnd.google-apps.folder")
                    {
                        ListViewItem viewItem = null;
                        viewItem = CreateViewItem(file.Title, "root", ParantID, file.Id, "File", index);
                        if (viewItem != null)
                        {
                            FileBrowser.Items.Add(viewItem);
                            index++;
                        }
                    }
                }
            }

            if (flowType != CloudFlowType.SimpleView && FileBrowser.Items.Count > 0)
                FileBrowser.SelectedIndex = 0;
            else
                FileBrowser.SelectedIndex = -1;

            ParantID = rootid;
            currentID = rootid;
            Win32.OutputDebugString("InitFileandFodlers===>Leave");
        }

        private void UpdateFileAndFolders(string parent, string parentid)
        {
            Win32.OutputDebugString("UpdateFileAndFolders===>Enter");
            if (FileList == null)
            {
                FileList = new List<File>();
            }

            int index = 0;

            if (FileList.Count > 0)
                FileBrowser.Items.Clear();

            string str = string.Format("get the file count is {0}", FileList.Count);
            Win32.OutputDebugString(str);

            foreach (File file in FileList)
            {
                if (file.MimeType == "application/vnd.google-apps.folder" &&
                    file.Parents[0].Id == parentid)
                {
                    ListViewItem viewItem = null;

                    viewItem = CreateViewItem(file.Title, parent, ParantID, file.Id, "Folder", index);
                    if (viewItem != null)
                    {
                        FileBrowser.Items.Add(viewItem);
                        index++;
                    }
                }
            }

            foreach (File file in FileList)
            {
                if (file.MimeType != "application/vnd.google-apps.folder" &&
                    file.Parents[0].Id == parentid)
                {
                    ListViewItem viewItem = null;

                    viewItem = CreateViewItem(file.Title, parent, ParantID, file.Id, "File", index);
                    if (viewItem != null)
                    {
                        FileBrowser.Items.Add(viewItem);
                        index++;
                    }
                }
            }
            FileBrowser.SelectedIndex = 0;
            Win32.OutputDebugString("UpdateFileAndFolders===>leave");
        }

        private ListViewItem CreateViewItem(string fileName, string parent, string parentid, string id, string fileType = "Folder", int fileIndex = 0, System.IO.Stream imageStream = null)
        {
            Win32.OutputDebugString("CreateViewItem===>Enter");
            ListViewItem item = null;
            try
            {
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();

                if (fileType == "Folder")
                {
                    bitmapImage.UriSource = new Uri("pack://application:,,, /Images/Folder-icon.png", UriKind.RelativeOrAbsolute);
                }
                else if (fileType == "File")
                {
                    if (imageStream == null)
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
                txtbrush.Color = Colors.Black;// DodgerBlue;
                text.Foreground = txtbrush;

                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Horizontal;

                stack.Children.Add(img);
                stack.Children.Add(text);

                item = new ListViewItem();
                SolidColorBrush bgbrush = new SolidColorBrush();
                bgbrush.Color = fileIndex % 2 == 0 ? Colors.AliceBlue : Colors.AliceBlue;
                item.Background = bgbrush;

                item.Content = stack;
                item.MouseDoubleClick += new MouseButtonEventHandler(ViewItemDoubleClick);

                ViewItemInfo info = new ViewItemInfo(fileType, fileName, parent, parentid, id);
                item.Tag = info;
            }
            catch (Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
                return null;
            }
            Win32.OutputDebugString("CreateViewItem===>Leave");
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

                if (result == true)
                {
                    string folderName = frm.m_folderName.TrimStart();
                    folderName = folderName.TrimEnd();

                    if (CheckFolder(folderName, this.currentID) == false)
                    {
                        VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                            Application.Current.MainWindow,
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name_exist"),//"The folder Name already exists",
                            (string)Application.Current.MainWindow.TryFindResource("ResStr_Warning"));
                        return;
                    }
                    else
                    {
                        string fullpath = currentPath + "\\" + folderName;

                        if (UploadFolder(folderName, fullpath))
                        {
                            FileList = Utilities.RetrieveAllFiles(_service);
                            List<File> templist = new List<File>();
                            foreach (File item in FileList)
                            {
                                templist.Add(item);
                            }

                            foreach (File item in templist)
                            {
                                if (item.ExplicitlyTrashed == true)
                                {
                                    FileList.Remove(item);
                                }
                            }
                            CurrentFolder = folderName;
                            GetFolderList();
                            UpdateFileAndFolders(CurrentFolder, this.currentID);
                        }
                        //else
                        //{
                        //    string str = (string)Application.Current.MainWindow.TryFindResource("ResStr_could_not_be_empty");
                        //    string content = (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder");
                        //    string message = string.Format(str, (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name"));
                        //    VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                        //        Application.Current.MainWindow,
                        //        message,//"The folder cannot be empty", 
                        //        (string)this.TryFindResource("ResStr_Warning"));
                        //    return;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Invalid_xxx");
                message = string.Format(message, (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Name"));
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow,
                    message,//"Invalid folder name.", 
                    (string)this.TryFindResource("ResStr_Warning"));
                return;
            }
        }


        private async void UpFolderButtonClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UploadStaus.Text = "";

            try
            {

                if (currentPath != "")
                {
                    string temp = "";
                    temp = currentPath.Remove(currentPath.LastIndexOf('/'), currentPath.Length - currentPath.LastIndexOf('/'));
                    currentFolderName = temp.Substring(temp.LastIndexOf('/') + 1);
                    selectedPath = temp;
                    currentPath = temp;
                    if (currentPath != "")
                    {
                        currentID = ParantID;
                        CurrentFolder = currentFolderName;

                        foreach (FolderItem folder in folderList)
                        {
                            if (currentID == folder.id)
                            {
                                ParantID = folder.parentID;
                                break;
                            }
                        }
                        UpdateFileAndFolders(Parent, currentID);
                    }
                    else
                    {
                        InitFileandFodlers();
                    }

                    PathText.Text = currentPath;
                    if (PathText.Text == "")
                        UpFolderButton.IsEnabled = false;
                    else
                        UpFolderButton.IsEnabled = true;
                }
                else
                {
                    // await LoadFolderFromPath();
                    currentPath = "";
                    PathText.Text = "";
                    UpFolderButton.IsEnabled = false;
                    selectedPath = "";
                }
                FileBrowser.SelectedIndex = 0;
                FileBrowser.Focus();
            }
            catch (Exception) { }

        }

        private bool CheckFolder(string foldername, string parentid)
        {
            foreach (FolderItem folder in folderList)
            {
                if (folder.name == foldername &&
                    folder.parentID == parentid)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckUploadFolderName(string foldername, string id)
        {

            foreach (FolderItem folder in folderList)
            {
                if (folder.name == foldername &&
                    folder.id == id)
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckFileName(string filepath, string parentid)
        {
            string filename = System.IO.Path.GetFileName(filepath);
            foreach (File file in FileList)
            {
                if (file.OriginalFilename == filename &&
                    file.Parents[0].Id == parentid)
                {
                    return false;
                }
            }
            return true;
        }

        private void ViewItemDoubleClick(object sender, RoutedEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            ViewItemInfo info = item.Tag as ViewItemInfo;

            UploadStaus.Text = "";

            try
            {
                if (info.fileType == "Folder")
                {
                    currentFolderName = info.fileName;
                    CurrentFolder = info.fileName;
                    selectedPath = currentPath + "/" + info.fileName;
                    currentPath = selectedPath;
                    PathText.Text = currentPath;
                    if (PathText.Text == "")
                        UpFolderButton.IsEnabled = false;
                    else
                        UpFolderButton.IsEnabled = true;

                    ParantID = currentID;
                    Parent = info.fileName;
                    currentID = info.id;
                    UpdateFileAndFolders(info.parent, info.id);
                }
            }
            catch (Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (flowType == CloudFlowType.SimpleView)
            {
                if (selectedPath == "")
                {
                    //if (FileBrowser.SelectedIndex != -1)
                    //{
                    //    ListViewItem item = FileBrowser.Items[FileBrowser.SelectedIndex] as ListViewItem;
                    //    ViewItemInfo info = item.Tag as ViewItemInfo;
                    //    if (info.fileType != "File")
                    //    {
                    //        selectedPath = currentPath + "/" + info.fileName;
                    //        Googledocsflow.SavePath = selectedPath;
                    //        Googledocsflow.FolderID = info.parentid;
                    //    }
                    //}
                    //else
                    {
                        Googledocsflow.SavePath = "/";
                        Googledocsflow.FolderID = rootid;
                    }
                }
                else
                {
                    Googledocsflow.SavePath = selectedPath;
                    Googledocsflow.FolderID = this.currentID;
                }

                if (Googledocsflow.SavePath == "")
                {
                    Googledocsflow.SavePath = "/";
                    Googledocsflow.FolderID = "";
                }

                this.Close();
            }
            else
            {
                if (uploadFiles == null)
                    return;

                string message = "";
                string temp = "";

                UploadStaus.Text = "";

                if (currentPath != "")
                {
                    temp = currentPath.Substring(currentPath.LastIndexOf('/') + 1);

                    if (temp != "")
                    {
                        UpperFolder = temp;
                    }
                    else
                    {
                        UpperFolder = "";
                    }

                    if (UpperFolder != "")
                    {
                        FileList = Utilities.RetrieveAllFiles(_service);
                        List<File> templist = new List<File>();
                        foreach (File item in FileList)
                        {
                            templist.Add(item);
                        }

                        foreach (File item in templist)
                        {
                            if (item.ExplicitlyTrashed == true)
                            {
                                FileList.Remove(item);
                            }
                        }
                        GetFolderList();
                        if (!CheckUploadFolderName(UpperFolder, currentID))
                        {
                            message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Folder_Not_Exist");
                            message = string.Format(message/*"The folder {0} does not exist."*/, currentFolderName);
                            VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple,
                                Application.Current.MainWindow, message, (string)this.TryFindResource("ResStr_Error"));
                            UpFolderButtonClick(null, null);
                            return;
                        }
                    }
                    else
                        return;
                }
                else
                {
                    if (UpperFolder == null)
                        return;
                }
                try
                {
                    string fileName = "";
                    bool bExistFile = false;
                    List<File> existfiles = new List<File>();

                    

                    foreach (string filepath in uploadFiles)
                    {
                        string filename = System.IO.Path.GetFileName(filepath);

                        foreach (File file in FileList)
                        {                           
                            if (file.OriginalFilename == filename &&
                                file.Parents[0].Id == currentID)
                            {
                                bExistFile = true;
                                existfiles.Add(file);
                                break;
                            }
                        }

                        if (!bExistFile)
                        {

                            File fileItem = new File();
                            fileItem.Id = "";
                            existfiles.Add(fileItem);
                        }                  
                    }

                    UploadStaus.Text = "Uploading file " + uploadFiles[0] + "...";

                    if (bExistFile)
                    {
                        message = (string)Application.Current.MainWindow.TryFindResource("ResStr_File_exist_Do_You_overwrite");
                        //message = string.Format(message,/*"The {0} already exists��Do you want to overwrite?",*/fileName);
                        VOP.Controls.MessageBoxExResult ret = VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.YesNo_NoIcon, this,
                         message, (string)this.TryFindResource("ResStr_Warning"));
                        if (VOP.Controls.MessageBoxExResult.Yes != ret)
                        {
                            return;
                        }

                        int i = 0;
                        foreach (string filepath in uploadFiles)
                        {
                            UploadStaus.Text = "Uploading file " + filepath;
                            // Yes... overwrite the file
                            File item = existfiles[i];
                            try
                            {
                                if (item.Id != "")
                                {
                                    Utilities.UpdateFile(_service, item.Id, item.Title, item.Description, item.MimeType, filepath, true);
                                }
                                else
                                {
                                    Utilities.InsertFile(_service, System.IO.Path.GetFileName(filepath), "Scanning Image", this.currentID, "image/jpeg", filepath);
                                }
                            }

                            catch (Exception ex)
                            {
                                message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Update_File_fail");
                                message = string.Format(message, filepath);
                                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                                    Application.Current.MainWindow, message, (string)this.TryFindResource("ResStr_Warning"));
                            }
                            i++;
                        }
                    }
                    else
                    {
                        foreach (string filePath in uploadFiles)
                        {
                           // UploadStaus.Text = "Uploading file " + filePath;

                            await Upload(filePath);
 
                        }
                    }

                    
                    FileList = Utilities.RetrieveAllFiles(_service);
                    List<File> templist = new List<File>();
                    foreach (File item in FileList)
                    {
                        templist.Add(item);
                    }

                    foreach (File item in templist)
                    {
                        if (item.ExplicitlyTrashed == true)
                        {
                            FileList.Remove(item);
                        }
                    }
                    GetFolderList();
                    UpdateFileAndFolders(currentPath, this.currentID);
                    UploadStaus.Text = "Upload files success!";
                }
                catch (Exception ex)
                {
                    UploadStaus.Text = "Uploading File error : " + ex.Message;
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

        private async Task Upload(string filepath)
        {
            string filename = System.IO.Path.GetFileName(filepath);
            try
            {
                Utilities.InsertFile(_service, filename, "Scanning Image", this.currentID, "image/jpeg", filepath);
            }
            catch (Exception ex)
            {
                string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Update_File_fail");
                message = string.Format(message, filename);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow, message, (string)this.TryFindResource("ResStr_Warning"));
            }
        }

        private bool UploadFolder(string folder, string fullpath)
        {
            try
            {
                File file = Utilities.InsertFile(_service, folder, "New Folder", this.currentID, "application/vnd.google-apps.folder", fullpath);
            }
            catch (Exception ex)
            {
                string message = (string)Application.Current.MainWindow.TryFindResource("ResStr_Create_Folder_fail");
                message = string.Format(message, folder);
                VOP.Controls.MessageBoxEx.Show(VOP.Controls.MessageBoxExStyle.Simple_Warning,
                    Application.Current.MainWindow, message, (string)this.TryFindResource("ResStr_Warning"));
                return false;
            }
            return true;
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

        private void FileSelectChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileBrowser.SelectedIndex != -1)
            {
                ListViewItem item = FileBrowser.Items[FileBrowser.SelectedIndex] as ListViewItem;
                if (item != null)
                {
                    ViewItemInfo info = item.Tag as ViewItemInfo;
                    if (info.fileType == "Folder")
                        selectedPath = info.fileName;
                    else
                        selectedPath = "";
                }
            }
        }
    }
}
