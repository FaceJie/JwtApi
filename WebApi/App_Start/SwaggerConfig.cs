using System.Web.Http;
using WebActivatorEx;
using WebApi;
using Swashbuckle.Application;
using System;
using WebApi.Models;
using WebApi.Content.Attributes;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace WebApi
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                {
                    c.SingleApiVersion("v1", "SwaggerUi");
                    //加入验证
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    //Token验证
                    c.ApiKey("apiKey").Description("Token授权").Name("token").In("header");
                    //缓存
                    c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                    //添加文件guolv
                    c.OperationFilter<SwaggerFileUploadFilter>();

                })
                .EnableSwaggerUi(c =>
                {
                    //启用token
                    c.EnableApiKeySupport("token", "header");
                   
                    //自定义样式和JS
                    c.InjectStylesheet(thisAssembly, "WebApi.Content.UserSource.css.site.css");
                    c.InjectJavaScript(thisAssembly, "WebApi.Content.UserSource.js.swagger_lang.js");
                    c.InjectJavaScript(thisAssembly, "WebApi.Content.UserSource.js.site.js");
                   
                });
        }
        private static string GetXmlCommentsPath()
        {
            return String.Format(@"{0}/bin/WebApi.XML", System.AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}
