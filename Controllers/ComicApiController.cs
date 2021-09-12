using ComicAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ComicAPI.Controllers
{
    public class ComicApiController : ApiController
    {
        //get api/ComicApi/1
        public Comic get(int comic_id)
        {
            string[] paras=new string[1] { "comic_id"};
            object[] values = new object[1] { comic_id };
            DataSet data = Connection.Connection.FillDataSet("sp_Comic_SELECT_byComicID", CommandType.StoredProcedure, paras, values);
            Comic com = new Comic();
            if (data.Tables[0].Rows.Count > 0)
            {
                com.comic_id = Convert.ToInt32(data.Tables[0].Rows[0]["comic_id"].ToString());
                com.name = data.Tables[0].Rows[0]["name"].ToString();
                com.summary = data.Tables[0].Rows[0]["summary"].ToString();
                com.trans = data.Tables[0].Rows[0]["trans"].ToString();
                com.author = data.Tables[0].Rows[0]["author"].ToString();
                com.genre_id = Convert.ToInt32(data.Tables[0].Rows[0]["genre_id"].ToString());
            }

            DataSet chapData = Connection.Connection.FillDataSet("sp_Chapter_SELECT_byComicID", CommandType.StoredProcedure, paras, values);
            if (chapData.Tables[0].Rows.Count > 0)
            {
                List<Chapter> chapters = new List<Chapter>();
                for(int i = 0; i < chapData.Tables[0].Rows.Count; i++)
                {
                    Chapter chapter = new Chapter();
                    chapter.chapter_id = Convert.ToInt32(chapData.Tables[0].Rows[i]["chapter_id"].ToString());
                    chapter.name = chapData.Tables[0].Rows[i]["name"].ToString();
                    chapter.update_time = Convert.ToDateTime(chapData.Tables[0].Rows[i]["update_time"].ToString());
                    chapters.Add(chapter);
                }
                com.chapters = chapters.ToArray();
            }
            return com;
        }

        //post api/comicapi
        public bool Post([FromBody]Comic comic,string key)
        {
            if (Userr.checkLogin(key) != 1) return false;
            if (string.IsNullOrWhiteSpace(comic.name) || comic.genre_id == 0 || string.IsNullOrWhiteSpace(comic.image)
                return false;
            //create folder
            string folderId = Drive.createFolder(comic.name, Drive.folderRootID);
            if (string.IsNullOrEmpty(folderId)) return false;
            //save image to server
            Image image = Base64String.Base64ToImage(comic.image);
            string folderPath = "@/Images/";
            var baseUrl = AppDomain.CurrentDomain.BaseDirectory + folderPath;
            string fileName = ".jpg";
            string newFileName = Guid.NewGuid().ToString() + fileName;
            string newPath = baseUrl + newFileName;

            var i2 = new Bitmap(image);
            i2.Save(newPath);

            //upload image to drive
            List<String> imageName = new List<string> { baseUrl + newFileName };
            if (!Drive.uploadFile(folderId, imageName.ToArray()))
            {
                Drive.deleteFolder(folderId);
                return false;
            }
            string imageId = Drive.fileIDs.ToArray()[0];

            //save to db
            string[] paras=new string[7] { "name", "summary", "author", "trans", "genre_id", "image", "folderID" };
            object[] values = new object[7] { comic.name, comic.summary, comic.author, comic.trans, comic.genre_id, imageId, folderId };
            int result = Connection.Connection.RequestStatus("sp_Comic_INSERT", CommandType.StoredProcedure, paras, values);
            if (result == -1)
            {
                Drive.deleteFolder(folderId);
                return false;
            }
            return true;
        }

        //put api/comicapi/1
        public int Put(int comic_id, [FromBody] Comic comic)
        {
            string[] paras = new string[7] { "comic_id", "name", "summary", "author", "trans", "genre_id", "image" };
            object[] values = new object[7] { comic_id, comic.name, comic.summary, comic.author, comic.trans, comic.genre_id, comic.image };
            return Connection.Connection.RequestStatus("sp_Comic_UPDATE_byComicID", CommandType.StoredProcedure, paras, values);
        }

        // DELETE: api/ComicApi/5
        public int Delete(int comic_id)
        {
            string[] paras = new string[1] { "comic_id" };
            object[] values = new object[1] { comic_id };
            return Connection.Connection.RequestStatus("sp_Comic_DELETE_byComicID", CommandType.StoredProcedure, paras, values);
        }
    }
}
