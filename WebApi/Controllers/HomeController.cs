using ApiBiz.Biz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Content.Jwt;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Erorr()
        {
            return View("~/Views/Home/Index.cshtml");
        }
        public ActionResult UpLoadPage()
        {
            return View();
        }

        public ActionResult ApiService()
        {
            return View();
        }
        /// <summary>
        /// 单点登录入口，验证用户，返回token
        /// </summary>
        /// <returns></returns>
        public JsonResult queryUser(string userName,string password,string queryType)
        {
            LoginResult rs = new LoginResult();
            DataTable dt = VisaInfoBiz.queryUser(userName, password);
            if (dt != null&&dt.Rows.Count>0)
            {
                AuthInfo info = new AuthInfo { UserName = dt.Rows[0]["UserName"].ToString(), UserId = dt.Rows[0]["UserId"].ToString(), QueryType = queryType.ToString()=="1"? "API服务界面" : "API调用界面" };
                try
                {
                    rs = TokenHelper.CreteToken(info);
                }
                catch (Exception ex)
                {
                    rs.Message = ex.Message;
                    rs.Success = false;
                    rs.Token = "创建token失败！";

                }
            }
            else
            {
                rs.Message = "无权创建token";
                rs.Success = false;
                rs.Token = "无权使用此接口！";
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }
    }
}