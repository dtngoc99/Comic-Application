using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ComicAPI.Models
{
    public class Base64String
    {
        public string base64String { get; set; }

        public static Image Base64ToImage(string base64String)
        {
            var str = base64String.Split('/').ToList();
            str.RemoveAt(0);
            str.RemoveAt(0);
            var joinedNames = "/" + str.Aggregate((a, b) => a + "/" + b);
            byte[] bytes = Convert.FromBase64String(joinedNames);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }
            return image;
        }
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            string base64String;
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                ms.Position = 0;
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                base64String = Convert.ToBase64String(imageBytes);
            }
            return base64String;
        }

        public string ImageUpload(Base64String base64String, string KEY)
        {
            if (Userr.checkLogin(KEY) == -1)
                return "";

            List<string> base64Strings = base64String.base64String.Split('.').ToList();
            List<string> urls = new List<string>();
            foreach (string base64 in base64Strings)
            {
                string imageUrl = "";
                try
                {
                    string folderPath = @"/Images/";
                    var baseUrl = AppDomain.CurrentDomain.BaseDirectory + folderPath;
                    string fileName = "jpg";
                    string newFileName = Guid.NewGuid().ToString() + "." + fileName;
                    string newPath = baseUrl + newFileName;
                    var list = base64.Split(',').ToList();
                    File.WriteAllBytes(newPath, Convert.FromBase64String(list[list.Count - 1]));
                    //SaveJpeg(newPath, image, 50);
                    imageUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + folderPath + newFileName;
                    urls.Add(imageUrl);
                }
                catch (Exception e) { return e.Message; }
            }
            return urls.Aggregate((a, b) => a + " " + b);
        }
    }
}