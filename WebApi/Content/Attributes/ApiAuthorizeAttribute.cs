using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using WebApi.Content.Jwt;
using WebApi.Models;

namespace WebApi.Content.Attributes
{

    /// <summary>
    /// 过滤器
    /// </summary>
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// 验证身份
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var authHeader = from t in actionContext.Request.Headers where t.Key == "token" select t.Value.FirstOrDefault();
            if (authHeader != null)
            {
                string token = authHeader.FirstOrDefault();
               
                if (!string.IsNullOrEmpty(token))
                {
                    try
                    {
                        AuthInfo json = TokenHelper<AuthInfo>.VlidateToken(token);
                        if (json != null)
                        {
                            actionContext.RequestContext.RouteData.Values.Add("token", json);
                            return true;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}