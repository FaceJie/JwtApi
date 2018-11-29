using ApiBiz.Biz;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.Content.Attributes;
using WebApi.Content.Jwt;
using WebApi.Models;
using WebHelper;

namespace WebApi.Controllers.Page
{
    
    public class VisaInfoController : Controller
    {
        // GET: VisaInfo
        public ActionResult Index()
        {
            return View();
        }
     
        public string GetVisaInfoByPage(int pageNum, int pageSize, string clientName, string customerName, string passportNo, string country,string selectType,string Time)
        {
            string json = "";
            int pageInt = pageNum == 0 ? 1 : pageNum;
            int rowsInt = pageSize == 10 ? 10 : pageSize;
            DataTable dt_total = VisaInfoBiz.GetVisaInfo(clientName, customerName, passportNo, country, selectType, Time);
            DataTable dt_rows = GlobalFuc.GetPagedTable(dt_total, pageInt, rowsInt);
            if (dt_rows != null && dt_rows.Rows.Count > 0)
            {
                DataRow[] drSignIn, drUnderWay, drSignOut;
                dt_rows.Columns.Add("Staus", Type.GetType("System.String"));
                drSignIn = dt_rows.Select("RealTime IS NULL");//入签
                if (drSignIn.Count() > 0)
                {
                    foreach (DataRow dr in drSignIn)
                    {
                        dr["Staus"] = "-1";
                    }
                }
                drUnderWay = dt_rows.Select("RealTime IS NOT NULL and  FinishTime IS NULL");//办理中
                if (drUnderWay.Count() > 0)
                {
                    foreach (DataRow dr in drUnderWay)
                    {
                        dr["Staus"] = "0";
                    }
                }
                drSignOut = dt_rows.Select(string.Format("RealTime IS NOT NULL and FinishTime IS NOT NULL"));//出签
                if (drSignOut.Count() > 0)
                {
                    foreach (DataRow dr in drSignOut)
                    {
                        dr["Staus"] = "1";
                    }
                }
            }
            if (dt_rows != null && dt_rows.Rows.Count > 0)
            {
                json += "{\"total\":" + dt_total.Rows.Count;
                json += ",\"rows\":" + DatatableToJson.Dtb2Json(dt_rows) + "}";
            }
            return json;
        }



        
        public JsonResult CreateOrder(string token,string country_b, string types, string count)
        {
            MsgModel ret = new MsgModel();
            
            int bufferLen = 1024;
            byte[] buffer = new byte[bufferLen];
            int contentLen = 0;
            FileStream fs = null;
            Stream uploadStream = null;
            string fileName = "";
            string Ext = "";
            string uploadPath = "";
            string uploadHttpUrl = "";
            string url = "http://" + base.HttpContext.Request.Url.Host + ":" + base.HttpContext.Request.Url.Port;
            HttpPostedFileBase postFileBase = Request.Files["file"];
            if (postFileBase != null)
            {
                try
                {
                    string uploadTempId = Guid.NewGuid().ToString("N");
                    uploadStream = postFileBase.InputStream;
                    Ext = Path.GetExtension(postFileBase.FileName);
                    fileName = uploadTempId + Ext;
                    uploadHttpUrl = url + "/temp/images/" + uploadTempId + Ext;
                    string baseUrl = Server.MapPath("/");
                    uploadPath = baseUrl + @"\temp\images\";
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }
                    fs = new FileStream(uploadPath + fileName, FileMode.Create, FileAccess.ReadWrite);
                    while ((contentLen = uploadStream.Read(buffer, 0, bufferLen)) != 0)
                    {
                        fs.Write(buffer, 0, contentLen);
                        fs.Flush();
                    }
                    fs.Close();
                }
                catch (Exception ex)
                {
                    ret.scu = false;
                    ret.msg = "上传文件出错：" + ex.Message.ToString(); 
                }
                finally
                {
                    if (null != fs)
                    {
                        fs.Close();
                    }
                    if (null != uploadStream)
                    {
                        uploadStream.Close();
                    }
                }
            }
            //获取此订单的其他信息token（token中包含了api的来源）
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    VisaInfoBiz.SaveOrderByPage(country_b, types, count, uploadHttpUrl, token);
                    ret.scu = true;
                    ret.msg = "创建订单成功！";
                }
                catch (Exception ex)
                {
                    ret.scu = false;
                    ret.msg = "创建订单出错："+ex.Message.ToString();
                }
            }
            else
            {
                ret.scu = true;
                ret.msg = "token丢失，请重试！";
            }
            return Json(ret,JsonRequestBehavior.AllowGet);
        }
    }
}