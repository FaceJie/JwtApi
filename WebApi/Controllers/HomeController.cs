using ApiBiz.Biz;
using Biz.Model;
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
        public ActionResult Other()
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
                try
                {
                    queryType = queryType.ToString() == "1" ? "API服务界面" : "API调用界面";
                    VisaLoginModel visaLoginModel = loadEntity(dt.Rows[0], queryType);
                    rs = TokenHelper<VisaLoginModel>.CreteToken(visaLoginModel);
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
                rs.Message = "无权使用,请注册，通过审核后即可使用！";
                rs.Success = false;
                rs.Token = "无权使用此接口！";
            }
            return Json(rs, JsonRequestBehavior.AllowGet);
        }

        //注册
        public ActionResult registerUser(registerUserModel model)
        {
            MsgModel ret = new MsgModel();
            if (VisaInfoBiz.CheckUserName(model.userName_r))//true 为存在用户
            {
                ret.scu = false;
                ret.msg = "姓名重复，请重新输入名称!";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            int result = VisaInfoBiz.registerUser(model);
            if (result>0)
            {
                ret.scu = true;
                ret.msg = "注册成功，等待管理员审核通过即可使用！";
            }
            else
            {
                ret.scu = false;
                ret.msg = "注册失败，请重试！";
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        public VisaLoginModel loadEntity(DataRow row,string queryType)
        {
            VisaLoginModel model = new VisaLoginModel();
            model.UserId = row["userId"] != DBNull.Value ? row["userId"].ToString() : string.Empty;
            model.Openid = row["openid"] != DBNull.Value ? row["openid"].ToString() : string.Empty;
            model.Password = row["password"] != DBNull.Value ? row["password"].ToString() : string.Empty;
            model.UserName = row["userName"] != DBNull.Value ? row["userName"].ToString() : string.Empty;
            model.UserType = row["userType"] != DBNull.Value ? row["userType"].ToString() : string.Empty;
            model.UserTlp = row["userTlp"] != DBNull.Value ? row["userTlp"].ToString() : string.Empty;
            model.HeadUrl = row["headUrl"] != DBNull.Value ? row["headUrl"].ToString() : string.Empty;
            model.UserNikeName = row["userNikeName"] != DBNull.Value ? row["userNikeName"].ToString() : string.Empty;
            model.BindPhone = row["bindPhone"] != DBNull.Value ? row["bindPhone"].ToString() : string.Empty;
            model.AddressHistory = row["addressHistory"] != DBNull.Value ? row["addressHistory"].ToString() : string.Empty;

            model.Position = row["position"] != DBNull.Value ? row["position"].ToString() : string.Empty;
            model.AddressName = row["addressName"] != DBNull.Value ? row["addressName"].ToString() : string.Empty;
            model.Token = row["token"] != DBNull.Value ? row["token"].ToString() : string.Empty;

            model.QueryType = queryType;
            if (Convert.ToDateTime(row["entryTime"]) != null)
            {
                model.EntryTime = Convert.ToDateTime(row["entryTime"]);
            }
            return model;
        }
    }
}