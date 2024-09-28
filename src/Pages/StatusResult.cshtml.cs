using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace src.Pages
{
    public class StatusResultModel : PageModel
    {
        public string RequestUrl { get; set; }
        public int? ServerStatusCode { get; set; }

        //([Bind(Prefix = "id")] int? statusCode = null)
        public IActionResult OnGet(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                HttpContext.Response.StatusCode = statusCode.Value;
                ServerStatusCode = statusCode;
            }

            IStatusCodeReExecuteFeature feature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            StatusCodeReExecuteFeature reExecuteFeature = feature as StatusCodeReExecuteFeature;
            IExceptionHandlerFeature exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            IExceptionHandlerPathFeature exceptionPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            IStatusCodePagesFeature statusCodePagesFeature = HttpContext.Features.Get<IStatusCodePagesFeature>();

            RequestUrl = string.Empty;

            if (feature != null)
            {
                RequestUrl =
                    feature?.OriginalPathBase
                    + feature?.OriginalPath
                    + feature?.OriginalQueryString;
            }

            // return Content($"Status Code: {statusCode}, Path: {OriginalURL}");

            return Page();
        }
    }
}
