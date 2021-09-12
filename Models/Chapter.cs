using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ComicAPI.Models
{
    public class Chapter
    {
        public int chapter_id { get; set; }
        public string name { get; set; }
        public DateTime update_time { get; set; }
        public int view { get; set; }
        public string[] link { get; set; }
        public int comic_id { get; set; }
        public static string getParentChapterFolderID(int chapter_id)
        {
            string[] paras = new string[1] { "chapter_id" };
            object[] values = new object[1] { chapter_id };
            DataSet data = Connection.Connection.FillDataSet("sp_Chapter_SELECT_ParentFolderID_byComicID", System.Data.CommandType.StoredProcedure, paras, values);
            if (data.Tables[0].Rows.Count < 1) return "";
            return data.Tables[0].Rows[0]["folderID"].ToString();
        }
        public static string getChapterFolderID(int chapter_id)
        {
            string[] paras = new string[1] { "chapter_id" };
            object[] values = new object[1] { chapter_id };
            DataSet data = Connection.Connection.FillDataSet("sp_Chapter_SELECT_FolderID_byChapterID", CommandType.StoredProcedure, paras, values);
            if (data.Tables[0].Rows.Count < 1) return "";
            return data.Tables[0].Rows[0]["folderID"].ToString();
        }
    }
}