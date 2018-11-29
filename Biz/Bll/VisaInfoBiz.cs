using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebHelper;

namespace ApiBiz.Biz
{
    public static class VisaInfoBiz
    {
        //web端查询
        public static DataTable GetVisaInfo(string clientName,string customerName, string passportNo, string country,string selectType, string Time)
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
            if (selectType=="1")
            {
                switch (selectType)
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
                switch (selectType)
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
        //查询用户
        public static DataTable queryUser(string userName, string password)
        {
            string sql = string.Format("SELECT userId,userName FROM VisaLogin WHERE userName='{0}' AND password='{1}' AND isInner=1", userName, password);
            return SqlHelper.GetTableText(sql)[0];
        }

        //Api端保存文件流
        public static int SaveOrder(string country, string types, string count, string fileDescribe,string uploadHttpUrl, string token)
        {
            string sql = string.Format("insert into OrderByApi(country,types,count,visaImgUrl,ApiSource,token,CreateTime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", country, types, count, uploadHttpUrl, "webApi", token, DateTime.Now);
            int result = 0;
            result = SqlHelper.ExecteNonQueryText(sql, null);
            return result;
        }

        //web端保存文件流
        public static int SaveOrderByPage(string country_b,string types,string count,string uploadHttpUrl, string token)
        {
            string sql = string.Format("insert into OrderByApi(country,types,count,visaImgUrl,ApiSource,token,CreateTime) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", country_b, types, count, uploadHttpUrl,"web页面", token,DateTime.Now);
            int result = 0;
            result = SqlHelper.ExecteNonQueryText(sql, null);
            return result;
        }
    }
}
