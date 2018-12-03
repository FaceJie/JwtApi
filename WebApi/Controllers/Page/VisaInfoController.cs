using ApiBiz.Biz;
using Biz.GlobalFcu;
using Biz.Model;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

        public string GetVisaInfoByPage(int pageNum, int pageSize, string clientName, string customerName, string passportNo, string country, string selectType, string Time)
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

        public JsonResult InsertInfo(InsertInfoViewMode insertInfoViewMode)
        {
            MsgModel ret = new MsgModel();
            TestModel json;
            if (string.IsNullOrEmpty(insertInfoViewMode.token))
            {
                ret.scu = false;
                ret.msg = "token丢失，请重试！";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            else
            {
                json = TokenHelper<TestModel>.VlidateToken(insertInfoViewMode.token);
                if (json != null)
                {
                    insertInfoViewMode.oUserId = json.userId;
                    insertInfoViewMode.oName = json.userName;
                    insertInfoViewMode.oPhone = json.userTlp;
                }
                else
                {
                    ret.scu = false;
                    ret.msg = "token有误！";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            insertInfoViewMode.transportId = GenerateOrderNo();
            insertInfoViewMode.sCreateTime = DateTime.Now;
            try
            {
                insertInfoViewMode.sPositionSet = AmapUtil.GetLngLatByAddress(new AddressViewModel() { address = insertInfoViewMode.sAddress, city = "成都市" });
                insertInfoViewMode.rPositionSet = AmapUtil.GetLngLatByAddress(new AddressViewModel() { address = insertInfoViewMode.rAddress, city = "成都市" });

                int result = VisaInfoBiz.InsertInfo(insertInfoViewMode);
                if (result > 0)
                {
                    ret.scu = true;
                    ret.msg = "创建订单成功！";
                }
                else
                {
                    ret.scu = false;
                    ret.msg = "创建订单失败！";
                }
            }
            catch (Exception ex)
            {
                ret.scu = false;
                ret.msg = "创建订单出错：" + ex.Message.ToString();
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public string SelectAddresHistory(string token)
        {
            string json = "";
            if (string.IsNullOrEmpty(token))
            {
                json = "";
                return json;
            }
            TestModel model = TokenHelper<TestModel>.VlidateToken(token);
            DataTable dt=VisaInfoBiz.SelectAddresHistory(model.userId);
            if (dt!=null&& dt.Rows.Count>0)
            {
                json = dt.Rows[0]["addressHistory"].ToString();
            }
            else
            {
                json = "";
            }
            return json;
        }

        public JsonResult UpdateInfo(string token,string sName,string sPhone,string sAddress, string rName, string rPhone, string rAddress)
        {
            MsgModel ret = new MsgModel();
            AddressModel addressModel = new AddressModel();
            TestModel json;
            if (string.IsNullOrEmpty(token))
            {
                ret.scu = false;
                ret.msg = "token丢失，请重试！";
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            else
            {
                json = TokenHelper<TestModel>.VlidateToken(token);
                if (json != null)
                {
                    addressModel.userId = json.userId;
                }
                else
                {
                    ret.scu = false;
                    ret.msg = "token有误！";
                    return Json(ret, JsonRequestBehavior.AllowGet);
                }
            }
            try
            {
                addressModel.sPositionSet = AmapUtil.GetLngLatByAddress(new AddressViewModel() { address = sAddress, city = "成都市" });
                addressModel.rPositionSet = AmapUtil.GetLngLatByAddress(new AddressViewModel() { address = rAddress, city = "成都市" });
                addressModel.sName = sName;
                addressModel.sPhone = sPhone;
                addressModel.sAddress = sAddress;
                addressModel.rName = rName;
                addressModel.rPhone = rPhone;
                addressModel.rAddress = rAddress;
                string InsertHistory = GlobalFuc.ModelToJson<AddressModel>(addressModel);
                int result=VisaInfoBiz.InsertHistory(InsertHistory, json.userId);
                if (result > 0)
                {
                    ret.scu = true;
                    ret.msg = "设置地址成功！";
                }
                else
                {
                    ret.scu = false;
                    ret.msg = "设置地址失败！";
                }
            }
            catch (Exception ex)
            {
                ret.scu = false;
                ret.msg = "设置地址出错：" + ex.Message.ToString();
            }

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateOrder(string token, string country_b, string types, string count)
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
                    ret.msg = "创建订单出错：" + ex.Message.ToString();
                }
            }
            else
            {
                ret.scu = false;
                ret.msg = "token丢失，请重试！";
            }
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
        /**
           * 根据当前系统时间加随机序列来生成订单号
           * @return 订单号
          **/
        public string GenerateOrderNo()
        {
            Random ran = new Random();
            return string.Format("U{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmss"), ran.Next(999));
        }
    }



    public class TestModel : IEquatable<TestModel>
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string userTlp { get; set; }
        public string userType { get; set; }
        public string position { get; set; }
        public string addressName { get; set; }
        public string password { get; set; }
        public string addressHistory { get; set; }


        public bool Equals(TestModel other)
        {
            return (
                this.userId == other.userId && this.userName == other.userName
                && this.userTlp == other.userTlp && this.userType == other.userType
                && this.position == other.position && this.addressName == other.addressName
                && this.password == other.password&& this.addressHistory==other.addressHistory);
        }
    }
}