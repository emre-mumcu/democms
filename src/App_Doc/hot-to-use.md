# How-To-Use: Policies, Filters and Attributes

```cs
// AuthorizationPolicy

[Authorize(Policy = nameof(AuthorizationPolicyLibrary.userPolicy))]
public class AdminController : Controller
{
	[Authorize(Policy = nameof(AuthorizationPolicyLibrary.adminPolicy))]
	public async Task<IActionResult> Index() => await Task.Run(() => View());
}

// MenuItem
[MenuItem]
public class TestController : Controller
{
	[MenuItem(_ParentText: "Home", _MenuText: "Banka Bilgileri", _ParentIndex: 10, _IsSingle: false, _ParentIconClass: "fa fa-cogs")]
	public async Task<IActionResult> Index() => await Task.Run(() => View());
}

// DynamicRoleRequirement Attribute
[DynamicRoleRequirement(typeof(HomeController))]
public class HomeController : Controller
{
	[DynamicRoleRequirement(typeof(HomeController), nameof(Index))]
	public async Task<IActionResult> Index() => await Task.Run(() => Content("Deneme"));
}
```

# How-To-Use: Cache

```cs
public ActionResult Index([FromServices] IMemoryCache cache)
{
	var states = new _CacheStates(cache).GetData();
	return View();
}
```

# How-To-Use: Show Menu Tree

```cs
@await Html.PartialAsync("~/Content/Menu.cshtml")
```

# How-To-Use: Singleton

```cs
// https://csharpindepth.com/articles/singleton (4th Version)
public sealed class Singleton
{
    private static readonly Singleton instance = new Singleton();

    static Singleton() { }

    private Singleton() { }

    public static Singleton Instance
    {
        get
        {
            return instance;
        }
    }
}

// https://csharpindepth.com/articles/singleton (6th Version)
public sealed class Singleton
{
	private static readonly Lazy<Singleton> lazy = new Lazy<Singleton>(() => new Singleton());

	public static Singleton Instance { get { return lazy.Value; } }

	private Singleton()	{ }
}
```

# How-To-Use: Tasks

```cs
// Task.FromResult
public int GetInt() =1;
public async Task<int> GetIntAsync() => Task.FromResult(1);
var sync1 = Task.Run(() => GetInt());
var sync2 = Task.Factory.StartNew(() => GetInt());
var async1 = Task.Run(() => GetIntAsync());
var async2 = Task.Factory.StartNew(() => GetIntAsync());

// Here are the three ways to call an async function:
async Task<T> SomethingAsync() { ... return t; }
async Task SomethingAsync() { ... }
async void SomethingAsync() { ... }

/*
 1) In the first case, the function returns a task that eventually produces the t.
 2) In the second case, the function returns a task which has no product, but you can still await on it to know when it has run to completion.
 3) The third case is like the second case, except that you don't even get the task back. You have no way of knowing when the function's task has completed. The async void case is a "fire and forget": You start the task chain, but you don't care about when it's finished.
*/
```