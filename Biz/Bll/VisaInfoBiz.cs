using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biz.Model;
using WebHelper;

namespace ApiBiz.Biz
{

    public static class VisaInfoBiz
    {
        static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //web端查询
        public static DataTable GetVisaInfo(string clientName, string customerName, string passportNo, string country, string selectType, string Time)
        {
            string sql = "SELECT GroupNo,PassportNo, RealTime, FinishTime, client, InTime, OutTime, Name, EnglishName, Sex, Birthday, PassportNo, EntryTime,Country, Types FROM VisaInfoWeChatView WHERE Types = '个签'";
            List<SqlParameter> para = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(clientName))
            {
                sql += " and client=@clientName ";
                para.Add(new SqlParameter("@clientName", clientName));
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                sql += " and Name=@customerName ";
                para.Add(new SqlParameter("@customerName", customerName));
            }
            if (!string.IsNullOrEmpty(passportNo))
            {
                para.Add(new SqlParameter("@PassportNo", passportNo));
                sql += " and PassportNo=@PassportNo ";
            }
            if (!string.IsNullOrEmpty(country))
            {
                para.Add(new SqlParameter("@Country", country));
                sql += " and Country=@Country ";
            }
            if (selectType == "1")
            {
                switch (Time)
                {
                    case "1":
                        sql += " and datediff(day,RealTime,getdate())=0";
                        break;
                    case "2":
                        sql += " and datediff(week,RealTime,getdate())=0";
                        break;
                    case "3":
                        sql += " and datediff(month,RealTime,getdate())=0";
                        break;
                    case "4":
                        sql += " and datediff(qq,RealTime,getdate())=0";
                        break;
                    default:
                        break;
                }
            }
            else if (selectType == "2")
            {
                switch (Time)
                {
                    case "1":
                        sql += " and datediff(day,FinishTime,getdate())=0";
                        break;
                    case "2":
                        sql += " and datediff(week,FinishTime,getdate())=0";
                        break;
                    case "3":
                        sql += " and datediff(month,FinishTime,getdate())=0";
                        break;
                    case "4":
                        sql += " and datediff(qq,FinishTime,getdate())=0";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //不处理
            }
            sql += " ORDER BY EntryTime DESC";
            return SqlHelper.GetTableTextByParams(sql, para)[0];
        }

        public static bool CheckUserName(string userName_r)
        {
            string sql = "select * from VisaLogin where userName=@userName";
            SqlParameter[] pars ={
                                    new SqlParameter("@userName",userName_r)
                                };
            DataTable dt = SqlHelper.GetTableText(sql, pars)[0];

            if (dt != null && dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int registerUser(registerUserModel model)
        {
            try
            {
                string sql = "insert into VisaLogin(userName,password,userType,userTlp,entryTime) output inserted.userId  values(@userName,@password,@userType, @userTlp,@entryTime)";
                SqlParameter[] pars ={
                                    new SqlParameter("@userName",model.userName_r),
                                    new SqlParameter("@password",model.password_r),
                                    new SqlParameter("@userType","2"),
                                    new SqlParameter("@userTlp",model.userTlp_r),
                                    new SqlParameter("@entryTime",DateTime.Now)
                                };

                int result = SqlHelper.ExecteNonQueryText(sql, pars);
                if (result > 0)
                {
                    string sqllog = "insert into VisaLoginLog(UserName,EntryTime, Account, LoginState,LoginCheck) values ('" + model.userName_r + "','" + DateTime.Now + "','" + model.userTlp_r + "','用户注册！','账号注册登录')";
                    SqlHelper.ExecteNonQueryText(sqllog, null);
                    return result;
                }
                else
                {
                    string sqllog = "insert into VisaLoginLog(UserName,EntryTime, Account, LoginState,LoginCheck) values ('" + model.userName_r + "','" + DateTime.Now + "','" + model.userTlp_r + "','用户注册！','账号注册失败')";
                    SqlHelper.ExecteNonQueryText(sqllog, null);
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static DataTable SelectAddresHistory(string userId)
        {
            string sql = string.Format("select * from VisaLogin where userId='{0}'", userId);
            return SqlHelper.GetTableText(sql, null)[0];
        }

        //API查询
        public static DataTable GetVisaInfoByApi(string clientName, string customerName, string passportNo, string country)
        {
            string sql = "SELECT GroupNo,PassportNo, RealTime, FinishTime, client, InTime, OutTime, Name, EnglishName, Sex, Birthday, PassportNo, EntryTime,Country, Types FROM VisaInfoWeChatView WHERE Types = '个签'";
            List<SqlParameter> para = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(clientName))
            {
                sql += " and client=@clientName ";
                para.Add(new SqlParameter("@clientName", clientName));
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                sql += " and Name=@customerName ";
                para.Add(new SqlParameter("@customerName", customerName));
            }
            if (!string.IsNullOrEmpty(passportNo))
            {
                para.Add(new SqlParameter("@PassportNo", passportNo));
                sql += " and PassportNo=@PassportNo ";
            }
            if (!string.IsNullOrEmpty(country))
            {
                para.Add(new SqlParameter("@Country", country));
                sql += " and Country=@Country ";
            }
            sql += " ORDER BY EntryTime DESC";
            return SqlHelper.GetTableTextByParams(sql, para)[0];
        }
        //web端创建订单
        public static int InsertInfo(InsertInfoViewMode insertInfoViewMode)
        {
            int result = 0;
            string sql = "insert into QuickOrder(transportId,oUserId,oName,oPhone,sCreateTime,sName,sPhone,sAddress,sPositionSet,rName,rPhone,rAddress,rPositionSet,orderStatus) values(@transportId,@oUserId,@oName,@oPhone,@sCreateTime,@sName,@sPhone,@sAddress,@sPositionSet,@rName,@rPhone,@rAddress,@rPositionSet,@orderStatus)";
            SqlParameter[] para = new SqlParameter[] {
                new SqlParameter("@transportId",insertInfoViewMode.transportId),
                new SqlParameter("@oUserId",insertInfoViewMode.oUserId),
                new SqlParameter("@oName",insertInfoViewMode.oName),
                new SqlParameter("@oPhone",insertInfoViewMode.oPhone),
                new SqlParameter("@sCreateTime", Convert.ToDateTime(insertInfoViewMode.sCreateTime)),
                new SqlParameter("@sName",insertInfoViewMode.sName),
                new SqlParameter("@sPhone",insertInfoViewMode.sPhone),
                new SqlParameter("@sAddress",insertInfoViewMode.sAddress),
                new SqlParameter("@sPositionSet",insertInfoViewMode.sPositionSet),
                new SqlParameter("@rName",insertInfoViewMode.rName),
                new SqlParameter("@rPhone",insertInfoViewMode.rPhone),
                new SqlParameter("@rAddress",insertInfoViewMode.rAddress),
                new SqlParameter("@rPositionSet",insertInfoViewMode.rPositionSet),
                new SqlParameter("@orderStatus", "待接单"),
            };
            try
            {
                result = SqlHelper.ExecteNonQueryText(sql, para);
                LogModelSet ls = new LogModelSet(new LogModel(insertInfoViewMode.oUserId, insertInfoViewMode.oName, insertInfoViewMode.oName + "在web端发起一条资料收取请求，订单号：" + insertInfoViewMode.transportId, 1), log);
            }
            catch (Exception ex)
            {
                LogModelSet ls = new LogModelSet(new LogModel(insertInfoViewMode.oUserId, insertInfoViewMode.oName, "在web端插入订单时：" + ex.ToString(), 3), log);
            }
            return result;
        }
        public static int InsertInfoByApi(InsertInfoViewMode insertInfoViewMode)
        {
            int result = 0;
            string sql = "insert into QuickOrder(transportId,oUserId,oName,oPhone,sCreateTime,sName,sPhone,sAddress,sPositionSet,rName,rPhone,rAddress,rPositionSet,orderStatus) values(@transportId,@oUserId,@oName,@oPhone,@sCreateTime,@sName,@sPhone,@sAddress,@sPositionSet,@rName,@rPhone,@rAddress,@rPositionSet,@orderStatus)";
            SqlParameter[] para = new SqlParameter[] {
                new SqlParameter("@transportId",insertInfoViewMode.transportId),
                new SqlParameter("@oUserId",insertInfoViewMode.oUserId),
                new SqlParameter("@oName",insertInfoViewMode.oName),
                new SqlParameter("@oPhone",insertInfoViewMode.oPhone),
                new SqlParameter("@sCreateTime", Convert.ToDateTime(insertInfoViewMode.sCreateTime)),
                new SqlParameter("@sName",insertInfoViewMode.sName),
                new SqlParameter("@sPhone",insertInfoViewMode.sPhone),
                new SqlParameter("@sAddress",insertInfoViewMode.sAddress),
                new SqlParameter("@sPositionSet",insertInfoViewMode.sPositionSet),
                new SqlParameter("@rName",insertInfoViewMode.rName),
                new SqlParameter("@rPhone",insertInfoViewMode.rPhone),
                new SqlParameter("@rAddress",insertInfoViewMode.rAddress),
                new SqlParameter("@rPositionSet",insertInfoViewMode.rPositionSet),
                new SqlParameter("@orderStatus", "待接单"),
            };
            try
            {
                result = SqlHelper.ExecteNonQueryText(sql, para);
                LogModelSet ls = new LogModelSet(new LogModel(insertInfoViewMode.oUserId, insertInfoViewMode.oName, insertInfoViewMode.oName + "在Api端发起一条资料收取请求，订单号：" + insertInfoViewMode.transportId, 1), log);
            }
            catch (Exception ex)
            {
                LogModelSet ls = new LogModelSet(new LogModel(insertInfoViewMode.oUserId, insertInfoViewMode.oName, "在Api端插入订单时：" + ex.ToString(), 3), log);
            }
            return result;
        }
        public static int InsertHistory(string addHistory, string userId)
        {
            string sql = string.Format("update VisaLogin set addressHistory='{0}' where userId='{1}'", addHistory, userId);
            return SqlHelper.ExecteNonQueryText(sql, null);
        }


        //查询用户
        public static DataTable queryUser(string userName, string password)
        {
            string sql = string.Format("SELECT * FROM VisaLogin WHERE userName='{0}' AND password='{1}' AND isInner=1", userName, password);
            return SqlHelper.GetTableText(sql)[0];
        }

        //Api端保存文件流
        public static int SaveOrder(string country, string types, string count, string fileDescribe, string uploadHttpUrl, string token)
        {
            string sql = string.Format("insert into OrderByApi(country,types,count,visaImgUrl,ApiSource,token,CreateTime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", country, types, count, uploadHttpUrl, "webApi", token, DateTime.Now);
            int result = 0;
            result = SqlHelper.ExecteNonQueryText(sql, null);
            return result;
        }

        //web端保存文件流
        public static int SaveOrderByPage(string country_b, string types, string count, string uploadHttpUrl, string token)
        {
            string sql = string.Format("insert into OrderByApi(country,types,count,visaImgUrl,ApiSource,token,CreateTime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", country_b, types, count, uploadHttpUrl, "web页面", token, DateTime.Now);
            int result = 0;
            result = SqlHelper.ExecteNonQueryText(sql, null);
            return result;
        }
    }
}
