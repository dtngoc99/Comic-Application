using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ComicAPI.Models
{
    public class Comic
    {
        public int comic_id { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string author { get; set; }
        public string summary { get; set; }
        public string trans { get; set; }
        public int genre_id { get; set; }
        public Chapter[] chapters { get; set; }
        public string folderID { get; set; }

        public static string getComicFolderId(int comic_id)
        {
            string []paras = new string[1] { "comic_id" };
            object []values = new object[1] { comic_id };
            DataSet data = null;
            data = Connection.Connection.FillDataSet("sp_Comic_SELECT_ParentFolderID_byComicID", CommandType.StoredProcedure, paras, values);
            if (data.Tables[0].Rows.Count < 1) return "";
            return data.Tables[0].Rows[0]["parentFolderID"].ToString();
        }
    }
}