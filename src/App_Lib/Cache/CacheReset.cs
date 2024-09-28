using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using src.App_Data;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Tools;

namespace src.App_Lib.Cache;

public class CacheReset
{
    private readonly DataOptions dataOptions;
    private readonly AppDbContext context;

    public CacheReset(IOptions<DataOptions> options, AppDbContext _context)
    {
        dataOptions = options.Value;
        context = _context;
    }
    
    public void ResetCache(string cacheName)
    {
        List<string>? serverUrls = new List<string>() {"https://127.0.0.1:5000", "https://127.0.0.2:5000" };

        foreach (string url in serverUrls)
        {
            CacheResetRequest(cacheName, url);
        }
    }

    private void CacheResetRequest(string cacheName, string serverUrl)
    {
        string serverResponse = string.Empty;

        try
        {
            JWTFactory jwt = new JWTFactory(
                claimsIdentity: null,
                issuer: "",
                signKey: "",
                encryptKey: ""
                );

            Uri baseAddress = new Uri(serverUrl);

            using (HttpClient client = new HttpClient() { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt.CreateToken());

                HttpResponseMessage response = client.GetAsync($"/services/WebTools/CacheReset?CacheKey={cacheName}").Result;

                if (response.IsSuccessStatusCode)
                {
                    var readTask = response.Content.ReadAsStringAsync();
                    readTask.Wait();
                    serverResponse = readTask.Result;
                }
                else
                {
                    serverResponse = response.StatusCode.ToString();
                }
            }            
        }
        catch (Exception ex)
        {
            
        }
        finally
        {

        }
    }
}


