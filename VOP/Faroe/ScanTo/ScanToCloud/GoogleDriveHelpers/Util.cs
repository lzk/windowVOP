// Google.Apis.Util.Utilities class

// This is my partial class that helps with some of the work around the Google API.
// Much of this is a direct derivative of the work provided by Google in their
// samples and API documentation. The below Copyright applies to
// their code. All other code is Copyright (c) 2012 Jason Gleim
// and is licensed under the Code Project Open License (CPOL). See
// http://codeproject for more information.
//
// This is implemented as a partial class so that the functions show up in the Google.Apis.Util.Utilities namespace/class.

/*
 * Copyright (c) 2012 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
 * in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under the License
 * is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
 * or implied. See the License for the specific language governing permissions and limitations under
 * the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using VOP;

namespace Google.Apis.Util
{
    /// <summary>
    /// Utility class to wrap a lot of the overhead surrounding the Google APIs.
    /// </summary>
    public partial class Utilities
    {
        /// <summary>
        /// Retrieve a list of Files in the Google Drive (All files in all folders and subfolders).
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <returns>List of Google.Apis.Drive.v2.Data.File objects.</returns>
        /// <remarks>This function is using the synchronous "List" function from the API. For drives
        /// with a lot of files, this could take a long time. It should not be executed on the UI
        /// thread because it will cause the app to freeze. It should be called from a worker thread or
        /// an async task or some other mechanism. Alternately, the code could be altered to use
        /// one of the async versions.</remarks>
        public static List<Google.Apis.Drive.v2.Data.File> RetrieveAllFiles(DriveService service)
        {
            Win32.OutputDebugString("RetrieveAllFiles===Enter");
            // Google's "File" class collides with System.IO.File so the Google class needs to be
            // fully qualified. Here we are getting a list of the files in the user's drive.
            List<Google.Apis.Drive.v2.Data.File> result = new List<Google.Apis.Drive.v2.Data.File>();
            try
            {
                FilesResource.ListRequest request = service.Files.List();   // NOTE: This is the synchronous version of the call!

                do
                {
                    try
                    {
                        // Fetch the files specified in our Files.List request. (This will actually go get the file listing)
                        FileList files = request.Fetch();

                        // Add the results to the result object and advance the page token. 
                        // The page token allows the results to be streamed back to us.
                        result.AddRange(files.Items);
                        request.PageToken = files.NextPageToken;
                    }
                    catch (Exception e)
                    {
                        // May want to log this or do something with it other than just dumping to the console.
                        //Console.WriteLine("An error occurred: " + e.Message);
                        request.PageToken = null;
                        Win32.OutputDebugString(e.Message);
                        return null;
                    }

                    // Keep doing this until the next page token is null. (Meaning there are no more pages to send)
                } while (!String.IsNullOrEmpty(request.PageToken));
            }
            catch (Exception ex)
            {
                Win32.OutputDebugString(ex.Message);
            }


            // Return the resulting list
            Win32.OutputDebugString("RetrieveAllFiles===Leave");
            return result;
        }

        /// <summary>
        /// Inserts a new file into the Google Drive.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <param name="title">Title (name) of the file to insert, including the extension.</param>
        /// <param name="description">Description of the file to insert.</param>
        /// <param name="parentId">Parent folder's ID. (Empty string to put the file in the drive root)</param>
        /// <param name="mimeType">MIME type of the file to insert.</param>
        /// <param name="filename">Filename (including path) of the file to upload relative to the local machine.</param>
        /// <returns>Inserted file metadata, null is returned if an API error occurred.</returns>
        /// <remarks>The filename passed to this function should be the entire path and filename (with extension) for the source file on the
        /// local machine. The API will put the file in the Google Drive using the "Title" property.</remarks>
        public static Google.Apis.Drive.v2.Data.File InsertFile(DriveService service, String title, String description, String parentId, String mimeType, String filename)
        {
            // File's metadata.
            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
            body.Title = title;
            body.Description = description;
            body.MimeType = mimeType;

            // Set the parent folder.
            if (!String.IsNullOrEmpty(parentId))
            {
                body.Parents = new List<ParentReference>() { new ParentReference() { Id = parentId } };
            }

            if (mimeType != "application/vnd.google-apps.folder")
            {
                // Load the File's content and put it into a memory stream
                byte[] byteArray = System.IO.File.ReadAllBytes(filename);
                MemoryStream stream = new MemoryStream(byteArray);

                try
                {
                    // When we add a file, we create an Insert request then call the Upload method on the request.
                    // (If we were updating an existing file, we would use the Update function)
                    FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, mimeType);
                    request.Upload();

                    // Set the file object to the response of the upload
                    Google.Apis.Drive.v2.Data.File file = request.ResponseBody;

                    // Uncomment the following line to print the File ID.
                    // Console.WriteLine("File ID: " + file.Id);

                    // return the file object so the caller has a reference to it.
                    return file;
                }
                catch (Exception e)
                {
                    // May want to log this or do something with it other than just dumping to the console.
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
            else
            {

                try
                {
                    // When we add a file, we create an Insert request then call the Upload method on the request.
                    // (If we were updating an existing file, we would use the Update function)
                    FilesResource.InsertRequest request = service.Files.Insert(body);
                    request.Fetch();

                    // Set the file object to the response of the upload
                    Google.Apis.Drive.v2.Data.File file = request.Body;

                    // Uncomment the following line to print the File ID.
                    // Console.WriteLine("File ID: " + file.Id);

                    // return the file object so the caller has a reference to it.
                    return file;
                }
                catch (Exception e)
                {
                    // May want to log this or do something with it other than just dumping to the console.
                    Console.WriteLine("An error occurred: " + e.Message);
                    return null;
                }
            }
        }

        /// <summary>
        /// Update an existing file's metadata and content.
        /// </summary>
        /// <param name="service">Drive API service instance.</param>
        /// <param name="fileId">ID of the file to update.</param>
        /// <param name="newTitle">New title for the file.</param>
        /// <param name="newDescription">New description for the file.</param>
        /// <param name="newMimeType">New MIME type for the file.</param>
        /// <param name="newFilename">Filename (including path) of the new content to upload relative to the local machine.</param>
        /// <param name="newRevision">Whether or not to create a new revision for this file.</param>
        /// <returns>Updated file metadata, null is returned if an API error occurred.</returns>
        /// <remarks>This function can be used to update the contents of a file (for example, if there is a more recent version on the
        /// local machine) or simply change the metadata associated with the file, including it's name (for example, if you wanted to
        /// change the name of the file or the description).</remarks>
        public static Google.Apis.Drive.v2.Data.File UpdateFile(DriveService service, String fileId, String newTitle,
            String newDescription, String newMimeType, String newFilename, bool newRevision)
        {
            try
            {
                // First, retrieve the file from the Google Drive.
                Google.Apis.Drive.v2.Data.File file = service.Files.Get(fileId).Fetch();

                // Set the file's new metadata.
                file.Title = newTitle;
                file.Description = newDescription;
                file.MimeType = newMimeType;

                // Get the file's new content and read it into a memory stream
                byte[] byteArray = System.IO.File.ReadAllBytes(newFilename);
                System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                // Call the Update API method passing in the updated information.
                FilesResource.UpdateMediaUpload request = service.Files.Update(file, fileId, stream, newMimeType);
                // Tell Google Drive if this is a new revision of the file or not.
                request.NewRevision = newRevision;
                // Execute the update
                request.Upload();

                // Get the response back from Google Drive and set the updatedFile object to the returned File informational object
                Google.Apis.Drive.v2.Data.File updatedFile = request.ResponseBody;
                // Return the updated file object so the caller has a handle on it.
                return updatedFile;
            }
            catch (Exception e)
            {
                // May want to log this or do something with it other than just dumping to the console.
                Console.WriteLine("An error occurred: " + e.Message);
                return null;
            }
        }

        /// <summary>
        /// Returns the name of the application currently being run.
        /// </summary>
        public static string ApplicationName
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }

        /// <summary>
        /// Tries to retrieve and return the content of the clipboard. Will trim the content to the specified length.
        /// Removes all new line characters from the input.
        /// </summary>
        /// <remarks>Requires the STAThread attribute on the Main method.</remarks>
        /// <returns>Trimmed content of the clipboard, or null if unable to retrieve.</returns>
        public static string GetSingleLineClipboardContent(int maxLen)
        {
            try
            {
                string text = Clipboard.GetText().Replace("\r", "").Replace("\n", "");
                if (text.Length > maxLen)
                {
                    return text.Substring(0, maxLen);
                }
                return text;
            }
            catch (ExternalException)
            {
                return null; // Something is preventing us from getting the clipboard content -> return.
            }
        }

        /// <summary>
        /// Changes the clipboard content to the specified value.
        /// </summary>
        /// <remarks>Requires the STAThread attribute on the Main method.</remarks>
        /// <param name="text"></param>
        public static void SetClipboard(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (ExternalException) { }
        }
    }

}
