using ComicAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ComicAPI.Controllers
{
    public class ChapterApiController : ApiController
    {
       // GET : api/Chapter/1
       public Chapter get(int chapter_id)
        {
            string[] paras = new string[1] { "chapter_id" };
            object[] values = new object[1] { chapter_id };
            Connection.Connection.RequestStatus("sp_Chapter_View_INC_byChapterID",CommandType.StoredProcedure, paras, values);

            Chapter chapter = new Chapter();
            DataSet chapData=Connection.Connection.FillDataSet("sp_Chapter_View_INC_byChapterID",CommandType.StoredProcedure,paras,values);
            if (chapData.Tables[0].Rows.Count < 1) return null;
            chapter.name = chapData.Tables[0].Rows[0]["name"].ToString();
            chapter.view = Convert.ToInt32(chapData.Tables[0].Rows[0]["_view"].ToString());
            chapter.update_time = Convert.ToDateTime(chapData.Tables[0].Rows[0]["update_time"].ToString());
            chapter.comic_id = Convert.ToInt32(chapData.Tables[0].Rows[0]["comic_id"].ToString());
            chapter.chapter_id = chapter_id;


            //get image
            DataSet base64StringDataSet = Connection.Connection.FillDataSet("sp_Image_SELECT_byChapterID", CommandType.StoredProcedure, paras, values);
            List<String> links = new List<string>();
            if(base64StringDataSet.Tables[0].Rows.Count > 0)
            {
                for(int i = 0; i < base64StringDataSet.Tables[0].Rows.Count; i++)
                {
                    links.Add(base64StringDataSet.Tables[0].Rows[i]["link"].ToString());
                }
                chapter.link = links.ToArray();
            }
            return chapter;
        }
    }
}
