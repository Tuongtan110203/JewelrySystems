using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace WebBanVang.Validation
{
    public class ModelValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public ModelValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var controllerActionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (controllerActionDescriptor != null)
                {
                    var controller = context.RequestServices.GetRequiredService(controllerActionDescriptor.ControllerTypeInfo.AsType());
                    var modelState = controller.GetType().GetProperty("ModelState").GetValue(controller) as ModelStateDictionary;

                    if (!modelState.IsValid)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsJsonAsync(modelState);
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

}
