using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using src.App_Lib.Attributes;
using src.App_Lib.Requirements;

namespace src.App_Lib.Concrete;

public static class MenuBuilder
{
	public static IAuthorizationPolicyProvider? policyProvider;

	public static IEnumerable<MenuItemObj>? MenuItems { get; set; }

	public static void Configure(IAuthorizationPolicyProvider _policyProvider)
	{
		policyProvider = _policyProvider;

		// Assembly.GetAssembly(typeof(Program)).Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type))

		var classDeclarations = Assembly.GetExecutingAssembly()?.GetTypes() 
			.Where(t => t.IsDefined(typeof(MenuItemAttribute)))
			.SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
			.AsMenuItems()
			.ApplyMenuItemAttribute()
			.ToList()
		;

		// Assembly.GetAssembly(typeof(Program)).Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type)) 

		var methodDeclarations = Assembly.GetExecutingAssembly()?.GetTypes()            
			.SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
			.Where(t => t.IsDefined(typeof(MenuItemAttribute)))
			.AsMenuItems()
			.ApplyMenuItemAttribute()
			.ToList()
		;

		if (methodDeclarations != null && methodDeclarations.Count > 0)
		{
			classDeclarations = classDeclarations?
				.Where(c => !methodDeclarations.Any(m => m.MethodType?.FullName == c.MethodType?.FullName))
				.ToList();
		}

		if(classDeclarations is null) classDeclarations = new();

		if(methodDeclarations is null) methodDeclarations = new();

		MenuItems = classDeclarations.Union(methodDeclarations);
	}

	public record ObjectInfo(string? GUID, string? NameSpace, string? FullName, string? Name);

	public record MenuItemObj
	{
		#region Reflection

		public ObjectInfo? DeclaringType { get; set; }
		public ObjectInfo? ReturnType { get; set; }
		public ObjectInfo? MethodType { get; set; }
		public MenuItemAttribute? MenuItemAttributeInfo { get; set; }
		public IEnumerable<AuthorizeAttribute>? AuthorizeAttributeInfo { get; set; }

		#endregion Reflection


		#region Properties

		public string ControllerName { get; internal set; } = null!;
		public string ActionName { get; internal set; } = null!;

		public bool IsSingle { get; set; } = false;

		public List<string>? AllowedRoles { get; set; } = null!;

		public int ParentIndex { get; set; }
		public string ParentIconClass { get; set; } = null!;
		public string ParentText { get; set; } = null!;

		public int MenuIndex { get; set; }
		public string MenuIconClass { get; set; } = null!;
		public string MenuText { get; set; } = null!;


		#endregion Properties
	}

	public static IEnumerable<MenuItemObj> AsMenuItems<T>(this IEnumerable<T> source) where T : MethodInfo
	{
		foreach (T item in source)
		{
			yield return new MenuItemObj()
			{
				DeclaringType = new ObjectInfo(
					GUID: item.DeclaringType?.GUID.ToString(),
					NameSpace: item.DeclaringType?.Namespace,
					FullName: item.DeclaringType?.FullName,
					Name: item.DeclaringType?.Name
				),
				ReturnType = new ObjectInfo(
					GUID: item.ReturnType?.GUID.ToString(),
					NameSpace: item.ReturnType?.Namespace,
					FullName: item.ReturnType?.FullName,
					Name: item.ReturnType?.Name
				),
				MethodType = new ObjectInfo(
					GUID: item.MetadataToken.ToString(),
					NameSpace: item.ReflectedType?.Namespace,
					FullName: $"{item.ReflectedType?.FullName}.{item.Name}",
					Name: item.Name
				),
				MenuItemAttributeInfo = item?.GetCustomAttribute<MenuItemAttribute>(true) ??
					item?.DeclaringType?.GetCustomAttribute<MenuItemAttribute>(true),
				AuthorizeAttributeInfo = item?.GetCustomAttributes<AuthorizeAttribute>(true).Count() > 0 ? 
					item?.GetCustomAttributes<AuthorizeAttribute>(true) : 
					item?.DeclaringType?.GetCustomAttributes<AuthorizeAttribute>(true)
			};
		}
	}

	public static IEnumerable<MenuItemObj> ApplyMenuItemAttribute<T>(this IEnumerable<T> source) where T : MenuItemObj
	{
		foreach (T item in source)
		{
			item.ControllerName = System.Text.RegularExpressions.Regex.Replace(item.DeclaringType?.Name ?? string.Empty, "Controller$", string.Empty);
			
			item.ActionName = item.MethodType?.Name ?? "Unknown";

			item!.IsSingle = item?.MenuItemAttributeInfo?.IsSingle ?? false;

			item!.ParentIndex = item?.MenuItemAttributeInfo?.ParentIndex ?? 0;

			item!.ParentIconClass = item?.MenuItemAttributeInfo?.ParentIconClass ?? "fa-angles-right";

			item!.ParentText = item?.MenuItemAttributeInfo?.ParentText ?? item?.ControllerName ?? "Unknown";

			item!.MenuIndex = item?.MenuItemAttributeInfo?.MenuIndex ?? 0;

			item!.MenuIconClass = item?.MenuItemAttributeInfo?.MenuIconClass ?? "fa-dot-circle";

			item!.MenuText = item?.MenuItemAttributeInfo?.MenuText ?? item?.ActionName ?? "Unknown";

			item!.AllowedRoles = new();

			if (policyProvider is not null)
			{
				foreach (var attribute in item.AuthorizeAttributeInfo ?? Enumerable.Empty<AuthorizeAttribute>())
				{
					if (attribute.Policy is null) continue;
					else
					{
						AuthorizationPolicy? authorizationPolicy = policyProvider.GetPolicyAsync(attribute.Policy).Result;

						List<UserRequirement>? userRequirements = authorizationPolicy?.Requirements
							.Where(r => r is UserRequirement)
							.Select(r => (UserRequirement)r)
							.ToList();

						item.AllowedRoles.AddRange(userRequirements?.SelectMany(r => r.Roles) ?? Enumerable.Empty<string>());
					}
				}
			}

			yield return item;
		}
	}
}