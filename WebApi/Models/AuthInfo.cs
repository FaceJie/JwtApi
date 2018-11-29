using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class AuthInfo
    {
        //模拟JWT的payload
        public string UserName { get; set; }

        public string UserId { get; set; }
        public string QueryType { get; set; }
    }
}