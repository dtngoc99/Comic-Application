using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace ComicAPI.Models
{
    public class Drive
    {
        public static string folderRootID = "1fsVm2FZJ1cmZeVKA0qdd97QqWdx1Ix4T";
        static string[] Scopes = { "https://www.googleapis.com/auth/drive.appdata", "https://www.googleapis.com/auth/drive" };
        static string ApplicationName = "Comic App";
        public static List<String> fileIDs = new List<string>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderName"></param>
        /// <param name="parentFolderID"></param>
        /// <returns>folderID</returns>
        public static string createFolder(string folderName, string parentFolderID)
        {
            UserCredential credential;
            try
            {
                using (var stream =
                new FileStream(@"C:\Users\Welcome\source\repos\ComicApiWeb\ComicApiWeb\client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = @"C:\Users\Welcome\source\repos\ComicApiWeb\ComicApiWeb\token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                //get folder id by name
                var fileMetadatas = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = folderName,
                    MimeType = "application/vnd.google-apps.folder",
                    Parents = new List<String> { parentFolderID }
                };
                var requests = service.Files.Create(fileMetadatas);
                requests.Fields = "id";
                var files = requests.Execute();
                return files.Id;
            }
            catch { };
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns>true: success</returns>
        public static bool deleteFolder(string folderName)
        {
            UserCredential credential;
            try
            {
                using (var stream =
                new FileStream(@"C:\Users\Welcome\source\repos\ComicApiWeb\ComicApiWeb\client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = @"C:\Users\Welcome\source\repos\ComicApiWeb\ComicApiWeb\token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                var requests = service.Files.Delete(folderName);
                return true;
            }
            catch { };
            return false;
        }
        public static bool uploadFile(string parentFolderID, string[] fileNames)
        {
            fileIDs = new List<string>();
            UserCredential credential;
            try
            {
                using (var stream =
                new FileStream(@"C:\Users\Welcome\source\repos\ComicApiWeb\ComicApiWeb\client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    // The file token.json stores the user's access and refresh tokens, and is created
                    // automatically when the authorization flow completes for the first time.
                    string credPath = @"{";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
                foreach (string filename in fileNames)
                {
                    if (!UploadImage(filename, service, parentFolderID)) return false;
                }
            }
            catch { return false; };
            return true;
        }
        private static bool UploadImage(string path, DriveService service, string folderUpload)
        {
            try
            {
                //create metadata file
                var fileMetadata = new Google.Apis.Drive.v3.Data.File();
                fileMetadata.Name = Path.GetFileName(path);
                fileMetadata.MimeType = "image/*";

                fileMetadata.Parents = new List<string> { folderUpload };

                //upload file to drive
                FilesResource.CreateMediaUpload request;
                using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, stream, "image/*");
                    request.Fields = "id";
                    request.Upload();
                    fileIDs.Add(request.ResponseBody.Id);
                }
                return true;
            }
            catch { };
            return false;
        }
    }
}