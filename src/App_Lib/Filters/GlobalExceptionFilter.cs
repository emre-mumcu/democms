using System.Collections;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace src.App_Lib.Filters;

public class GlobalExceptionViewModel
{
    public string? Id { get; set; }
    public int StatusCode { get; set; }
    public bool IsStatusCodeException { get; set; }
    public string? Source { get; set; }
    public string? SourceType { get; set; }
    public Exception? ApplicationException { get; set; }
    public List<string?>? AppliedFilters { get; set; }
    public string? RouteValues { get; set; }
}

public class GlobalExceptionFilter : ExceptionFilterAttribute
{
    private ExceptionContext? _context;

    public override void OnException(ExceptionContext context)
    {
        _context = context;
        
        if (_context.ActionDescriptor.GetType().IsSubclassOf(typeof(PageActionDescriptor)) || _context.ActionDescriptor.GetType().IsSubclassOf(typeof(CompiledPageActionDescriptor)))
        {
            // For razor pages ActionDescriptor can be either PageActionDescriptor or CompiledPageActionDescriptor
            RazorPageError();
        }
        else if (_context.ActionDescriptor.GetType() == typeof(ControllerActionDescriptor) && (_context.ActionDescriptor as ControllerActionDescriptor)!.ControllerTypeInfo.IsSubclassOf(typeof(ControllerBase)) && (_context.ActionDescriptor as ControllerActionDescriptor)!.ControllerTypeInfo.IsSubclassOf(typeof(Controller)))
        {
            // View/Page implements ControllerBase and Controller
            RazorViewError();
        }
        else if (_context.ActionDescriptor.GetType() == typeof(ControllerActionDescriptor) && (_context.ActionDescriptor as ControllerActionDescriptor)!.ControllerTypeInfo.IsSubclassOf(typeof(ControllerBase)) && !(_context.ActionDescriptor as ControllerActionDescriptor)!.ControllerTypeInfo.IsSubclassOf(typeof(Controller)))
        {
            // Api implements ControllerBase but not Controller
            WebApiError();
        }
        else
        {
            UndeterminedError();
        }

        base.OnException(context);
    }

    private void RazorPageError()
    {
        _context!.ExceptionHandled = true;

        PageActionDescriptor actionDescriptor = (_context.ActionDescriptor as PageActionDescriptor)!;

        ViewResult result = new ViewResult() { ViewName = "_Exception" };

		ViewDataDictionary vdd = new ViewDataDictionary(new EmptyModelMetadataProvider(), _context.ModelState)
        {
            Model = new GlobalExceptionViewModel()
            {
                Id = _context.ActionDescriptor.Id,
                Source = $"{actionDescriptor.ViewEnginePath} ({actionDescriptor.RelativePath})",
                SourceType = _context.ActionDescriptor.GetType()?.FullName,
                IsStatusCodeException = false,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ApplicationException = _context.Exception,
                AppliedFilters = _context.Filters.Select(f => f.ToString()).ToList(),
                RouteValues = _context.ActionDescriptor.RouteValues.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2)
            }
        };

        result.ViewData = vdd;
        result.ViewData.Add("EventTime", DateTime.Now);

        _context.Result = result;
    }

    private void RazorViewError()
    {
        _context!.ExceptionHandled = true;

        ControllerActionDescriptor actionDescriptor = (_context.ActionDescriptor as ControllerActionDescriptor)!;

        ViewResult result = new ViewResult() { ViewName = "_Exception" };

        ViewDataDictionary vdd = new ViewDataDictionary(new EmptyModelMetadataProvider(), _context.ModelState)
        {
            Model = new GlobalExceptionViewModel()
            {
                Id = _context.ActionDescriptor.Id,
                Source = $"{actionDescriptor.ControllerName}/{actionDescriptor.ActionName}",
                SourceType = _context.ActionDescriptor.GetType()?.FullName,
                IsStatusCodeException = false,
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ApplicationException = _context.Exception,
                AppliedFilters = _context.Filters.Select(f => f.ToString()).ToList(),
                RouteValues = _context.ActionDescriptor.RouteValues.Select(x => x.Key + "=" + x.Value).Aggregate((s1, s2) => s1 + ";" + s2)
            }
        };

        result.ViewData = vdd;
        result.ViewData.Add("EventTime", DateTime.Now);

        _context.Result = result;
    }

    private void WebApiError()
    {
        ControllerActionDescriptor actionDescriptor = (_context!.ActionDescriptor as ControllerActionDescriptor)!;

        Dictionary<string, string?> error = new Dictionary<string, string?>
            {
                {"Source", $"{actionDescriptor.ControllerName} / {actionDescriptor.ActionName}"},
                {"Type", _context.Exception.GetType().ToString()},
                {"Message", _context.Exception.Message},
                {"StackTrace", _context.Exception.StackTrace}
            };

        foreach (DictionaryEntry data in _context.Exception.Data) error.Add(data.Key.ToString()!, data.Value?.ToString());

        string jsonException = JsonConvert.SerializeObject(error, Formatting.Indented);

        _context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        _context.HttpContext.Response.ContentType = "application/json";
        _context.Result = new JsonResult(jsonException);
    }

    private void UndeterminedError()
    {
        _context!.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        _context!.HttpContext.Response.ContentType = "application/json";

        var error = new Dictionary<string, string>
            {
                {"Type", _context.Exception.GetType().ToString()},
                {"Message", _context.Exception.Message},
                {"StackTrace", _context.Exception?.StackTrace ?? string.Empty}
            };

        foreach (DictionaryEntry data in _context?.Exception?.Data ?? new Dictionary<string, string>()) 
			error.Add(data.Key.ToString() ?? Guid.NewGuid().ToString() , data.Value?.ToString() ?? string.Empty);

        string json = JsonConvert.SerializeObject(error, Formatting.Indented);

        _context!.Result = new JsonResult(json);
    }
}