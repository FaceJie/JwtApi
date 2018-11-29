
using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Description;

namespace WebApi.Content.Attributes
{
    public class SwaggerFileUploadFilter : IOperationFilter
    {
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            var parameters = apiDescription.ActionDescriptor.GetParameters();
            foreach (HttpParameterDescriptor parameterDesc in parameters)
            {
                var fileUploadAttr = parameterDesc.GetCustomAttributes<SwaggerFileUploadAttribute>().FirstOrDefault();
                if (fileUploadAttr != null)
                {
                    operation.consumes.Add("multipart/form-data");

                    operation.parameters.Add(new Parameter
                    {
                        name = parameterDesc.ParameterName + "_file",
                        @in = "formData",
                        description = "请选择文件",
                        required = fileUploadAttr.Required,
                        type = "file"
                    });
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class SwaggerFileUploadAttribute : Attribute
    {
        public bool Required { get; private set; }

        public SwaggerFileUploadAttribute(bool Required = true)
        {
            this.Required = Required;
        }
    }
}