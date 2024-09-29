```zsh
% git init
% git add -A
% git commit -m "first commit"
% git branch -M main
% git remote add origin https://github.com/emre-mumcu/democms.git
% git push -u origin main

% git init -b main
% git add -A
% git commit -m "First Commit"
% git remote add origin https://github.com/emre-mumcu/democms.git
% git push -u origin main
```

//dotnet tool install --global dotnet-ef
//dotnet tool update --global dotnet-ef
//dotnet ef migrations add Migration1 -o App_Data\Migrations
//dotnet ef database update

// dotnet add package Microsoft.EntityFrameworkCore.Sqlite
// dotnet add package Microsoft.EntityFrameworkCore.Design

//dotnet add package Microsoft.EntityFrameworkCore.Relational

//dotnet add package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore

```cs
/*
using var ctx = new BlogContext();

var services = new ServiceCollection()
    .AddEntityFrameworkDesignTimeServices()
    .AddDbContextDesignTimeServices(ctx);

var npgsqlDesignTimeServices = new NpgsqlDesignTimeServices();
npgsqlDesignTimeServices.ConfigureDesignTimeServices(services);

var serviceProvider = services.BuildServiceProvider();
var scaffolder = serviceProvider.GetRequiredService<IMigrationsScaffolder>();
var migration = scaffolder.ScaffoldMigration(
    "MyMigration",
    "MyApp.Data");

Console.WriteLine(migration.MigrationCode);

*/

/*
https://stackoverflow.com/questions/66383701/passing-parameter-to-dbcontext-with-di

You cannot pass dependency to DbContext in this way.

First, create a class:

class MyClass 
{
    public string MyProperty { get; set; }
}
Then pass MyClass as a dependency to DbContext:

MyDbContext(DbContextOptions<MyDbContext> options, MyClass myParam)
    : base(options)
{           
    //* Intentionally empty            
}
Then register MyClass as a singleton to ServiceCollection:

services.AddSingletone(new MyClass { MyProperty = "xx" });
services.AddDbContext<MyDbContext>(options =>options.UseSqlServer(con));
*/
```


```cs
// CacheReset
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
		List<string>? serverUrls = new List<string>() { "https://127.0.0.1:5000", "https://127.0.0.2:5000" };

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



// CacheResetService
using Microsoft.Extensions.Options;
using src.App_Data;
using src.App_Data.Types;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Extensions;

namespace src.App_Lib.Cache
{
	public class CacheResetService : IHostedService, IDisposable
	{
		private Timer? _timer = null;
		IOptions<DataOptions> _options;
		private int executionCount = 0;
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<CacheResetService> _logger;

		public CacheResetService(ILogger<CacheResetService> logger, IServiceProvider serviceProvider, IOptions<DataOptions> options)
		{
			_logger = logger;
			_serviceProvider = serviceProvider;
			_options = options;

		}

		public Task StartAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("CacheResetService Started...");

			_timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));

			return Task.CompletedTask;
		}

		private void DoWork(object? state)
		{
			var count = Interlocked.Increment(ref executionCount);

			CacheReset cacheReset = new CacheReset(_options, _serviceProvider.CreateScope().ServiceProvider.GetRequiredService<AppDbContext>());
			cacheReset.ResetCache(EnumCacheNames.City.GetEnumDescription());


		}

		public Task StopAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation("CacheResetService Stopped...");

			_timer?.Change(Timeout.Infinite, 0);

			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}
	}
}

// StartupCache
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data;
using src.App_Data.Entities;

namespace src.App_Lib.Cache
{
	public static class StartupCache
	{
		private static IMemoryCache? _memCache;
		private static MemoryCacheEntryOptions? _cacheOptions;

		public static void StartupCacheConfig(IMemoryCache memCache)
		{
			_memCache = memCache;

			_cacheOptions = new MemoryCacheEntryOptions
			{
				AbsoluteExpiration = DateTime.Now.AddMinutes(60),
				SlidingExpiration = new TimeSpan(0, 20, 0),
				Priority = CacheItemPriority.Normal
			};
		}

		public static async Task<List<RoleMatrix>> GetRoleMatrix(string? roleCode = null, bool refresh = false)
		{
			string itemKeyName = "DbYetkiler";

			if (refresh) _memCache!.Remove(itemKeyName);

			List<RoleMatrix>? list;

			if (!_memCache!.TryGetValue(itemKeyName, out list))
			{
				using (var context = new AppDbContext())
				{
					list = await context.RoleMatrixes.ToListAsync();
				}

				_memCache.Set(itemKeyName, list, _cacheOptions);
			}

			if (roleCode != null) return list.Where(i => i.RoleCode == roleCode).ToList();
			else return list;
		}
	}
}


//
	public static string GetEnumDescription(this Enum enumValue)
	{
		var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

		if (fieldInfo != null)
		{
			var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

			return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
		}
		else throw new ArgumentException(nameof(fieldInfo));
	}

```