using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class FileData
    {
        /// <summary>
        /// 文件集合
        /// </summary>
        public HttpFileCollection httpFileCollection { get; set; }
        /// <summary>
        /// 单个文件
        /// </summary>
        public HttpPostedFile HttpPostedFile { get; set; }
    }
}