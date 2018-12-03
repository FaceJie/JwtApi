using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApi.Content.Jwt;
using WebApi.Models;

namespace WebApi.Content.Attributes
{
    public class ControllerAuthorizeAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var authHeader = filterContext.HttpContext.Request.Headers.GetValues("token");
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
                            filterContext.RequestContext.RouteData.Values.Add("token", json);
                            return;
                        }
                        filterContext.Result = new RedirectResult("~/Home/Erorr");
                    }
                    catch (Exception ex)
                    {
                        filterContext.Result = new RedirectResult("~/Home/Erorr");
                    }
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Home/Erorr");
                }
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Home/Erorr");
            }
        }
    }
}