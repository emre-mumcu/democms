using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace src.App_Lib.Extensions;

public static class HttpContextExtensions
{
    public static string GetClientIP(this IHttpContextAccessor httpContextAccessor)
    {
        return httpContextAccessor.HttpContext!.GetClientIP();
    }

    public static string GetClientIP(this HttpContext context)
    {
        string clientIP =
            (IPAddress.TryParse(context?.GetHeaderValue("X-Forwarded-For"), out IPAddress? ip2) ? ip2 : null)?.ToString() ??
            (IPAddress.TryParse(context?.GetHeaderValue("X-Original-Forwarded-For"), out IPAddress? ip3) ? ip3 : null)?.ToString() ??
            (IPAddress.TryParse(context?.GetHeaderValue("X-Real-IP"), out IPAddress? ip4) ? ip4 : null)?.ToString() ??
            (IPAddress.TryParse(context?.GetHeaderValue("REMOTE-ADDR"), out IPAddress? ip5) ? ip5 : null)?.ToString() ??
            (IPAddress.TryParse(context?.GetHeaderValue("CF-Connecting-IP"), out IPAddress? ip1) ? ip1 : null)?.ToString() ??
            context?.Features?.Get<IHttpConnectionFeature>()?.RemoteIpAddress?.ToString() ??
            context?.Request?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ??
            "client.ip.unknown";

        return clientIP;
    }

    public static string? GetServerIP(this HttpContext context)
    {
        return
            context?.Features?.Get<IHttpConnectionFeature>()?.LocalIpAddress?.ToString() ??
            context?.Request?.HttpContext?.Connection?.LocalIpAddress?.ToString() ??
            "server.ip.unknown";
    }

	[Obsolete("GetLocalIP & GetMachineName should be combined or reviewed")]
    public static (IPAddress? Ip, string IpList) GetLocalIP()
    {
        System.Net.NetworkInformation.NetworkInterface[] nics 
			= System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
        
		foreach (System.Net.NetworkInformation.NetworkInterface adapter in nics)
		{
			// adapter'den ip al
		}

        List<IPAddress> list = Dns.GetHostEntry(Dns.GetHostName()).AddressList.ToList();

        return (list.FirstOrDefault(i => i.AddressFamily == AddressFamily.InterNetwork), string.Join(',', list));
    }

	[Obsolete("GetLocalIP & GetMachineName should be combined or reviewed")]
	public static string GetMachineName(this IHttpContextAccessor httpContextAccessor)
    {
        try
        {
            string hostName = httpContextAccessor.HttpContext?.Features?.Get<IServerVariablesFeature>()?["REMOTE_HOST"]
                ?? httpContextAccessor?.HttpContext?.Request?.Headers["REMOTE-ADDR"].FirstOrDefault()
                ?? string.Empty;

            return hostName;
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public static string? GetProtocol(this HttpContext context)
    {
        return context.GetHeaderValue("x-forwarded-proto");
    }

    public static string? GetHost(this HttpContext context)
    {
        return context.GetHeaderValue("x-forwarded-host");
    }

    public static string? GetUserAgent(this HttpContext context)
    {
        return context.GetHeaderValue("user-agent");
    }

    public static string? GetHeaderValue(this HttpContext context, string HeaderKey)
    {
        if (context.Request.Headers.TryGetValue(HeaderKey, out StringValues HeaderValue))
            return HeaderValue;
        else
            return null;
    }
}
