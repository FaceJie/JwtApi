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
                    //������֤
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    //Token��֤
                    c.ApiKey("apiKey").Description("Token��Ȩ").Name("token").In("header");
                    //����
                    c.CustomProvider((defaultProvider) => new CachingSwaggerProvider(defaultProvider));
                    //����ļ�guolv
                    c.OperationFilter<SwaggerFileUploadFilter>();

                })
                .EnableSwaggerUi(c =>
                {
                    //����token
                    c.EnableApiKeySupport("token", "header");
                   
                    //�Զ�����ʽ��JS
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
