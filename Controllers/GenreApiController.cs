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
    public class GenreApiController : ApiController
    {
        //get api/GenreApi
        public IEnumerable<Genre> Get()
        {
            DataSet data = Connection.Connection.FillDataSet("sp_Genre_SELECT", CommandType.StoredProcedure);
            List<Genre> genres = new List<Genre>();
            if (data.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < data.Tables[0].Rows.Count; i++)
                {
                    Genre gen = new Genre();
                    gen.genre_id = Convert.ToInt32(data.Tables[0].Rows[i]["genre_id"].ToString());
                    gen.name = data.Tables[0].Rows[i]["name"].ToString();
                    gen.description = data.Tables[0].Rows[i]["description"].ToString();
                    genres.Add(gen);
                }
            }
            return genres;
        }

        //get api/genreapi?genre_id=1
        public Genre Get(int genre_id)
        {
            string[] paras = new string[1] { "genre_id" };
            object[] values = new object[1] { genre_id };
            DataSet data = Connection.Connection.FillDataSet("sp_Genre_SELECT_byGenreID", CommandType.StoredProcedure, paras, values);
            if (data.Tables[0].Rows.Count > 0)
            {
                Genre gen = new Genre();
                gen.genre_id = genre_id;
                gen.name = data.Tables[0].Rows[0]["name"].ToString();
                gen.description = data.Tables[0].Rows[0]["description"].ToString();
                return gen;
            }
            return null;
        }

        //post api/genreapi
        public bool Post([FromBody]Genre genre,string key)
        {
            if (Userr.checkLogin(key) != 1) return false;
            if (string.IsNullOrWhiteSpace(genre.name)) return false;

            string[] paras = new string[2] { "name", "description" };
            object[] values = new object[2] { genre.name, genre.description };

            int result = Connection.Connection.RequestStatus("sp_Genre_INSERT", CommandType.StoredProcedure, paras, values);
            if (result == -1) return false;
            return true;
        }
        //put api/GenreApi/5
        public int Put(int genre_id, [FromBody]Genre genre)
        {
            string[] paras = new string[3] { "genre_id", "name", "description" };
            object[] values = new object[3] { genre.genre_id, genre.name, genre.description };
            return Connection.Connection.RequestStatus("sp_Genre_UPDATE_byGenreID", CommandType.StoredProcedure, paras, values);
        }
        //delete api/GenreApi/5
        public int Delete(int genre_id)
        {
            string[] paras = new string[1] { "genre_id" };
            object[] values = new object[1] { genre_id };
            return Connection.Connection.RequestStatus("sp_Genre_DELETE_byGenreID", CommandType.StoredProcedure, paras, values);
        }
    }
}
