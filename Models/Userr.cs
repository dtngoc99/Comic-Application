using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ComicAPI.Models
{
    public class Userr
    {
        public static int checkLogin(string key)
        {
            string[] paras = new string[1] { "KEY" };
            object[] values = new object[1] { key };
            return Connection.Connection.RequestStatus("sp_User_Login", System.Data.CommandType.StoredProcedure, paras, values);
        }
    }
}