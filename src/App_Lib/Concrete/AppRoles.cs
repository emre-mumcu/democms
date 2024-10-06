using System.Reflection;
using src.App_Lib.Attributes;
using src.Areas.Admin.Models;

namespace src.App_Lib.Concrete;

/// <summary>
/// Application roles are listed using Controllers, Actions and Pages with DynamicRoleRequirementAttribute
/// </summary>
public class AppRoles
{
	public List<AppRole>? GetControllers()
	{
		Assembly? asm = Assembly.GetAssembly(typeof(Program));

		var controllers = asm?.GetTypes()
			.Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type))
			.Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
			.Select(i => new AppRole
			{
				TypeName = i.BaseType!.FullName!,
				Namespace = i.Namespace!,
				FullName = i.FullName!,
				Name = i.Name,
				MemberName = "*",
				ReturnType = "NOT-APPLICABLE",
				GuidString = i.GUID.ToString()
			})
			.ToList()
		;
		
		return controllers;
	}

	public List<AppRole>? GetActions()
	{
		Assembly? asm = Assembly.GetAssembly(typeof(Program));

		var actions = asm?.GetTypes()
			.Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type))
			.SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
			.Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
			.Select(i => new AppRole
			{
				TypeName = i.MemberType.ToString(),
				Namespace = i.DeclaringType!.Namespace!,
				FullName = i.DeclaringType!.FullName!,
				Name = i.DeclaringType.Name,
				MemberName = i.Name,
				ReturnType = i.ReturnType.Name,
				GuidString = i.ReflectedType!.GUID.ToString()
			})
			.ToList()
		;

		return actions;
	}

	public List<AppRole>? GetPages()
	{
		Assembly? asm = Assembly.GetAssembly(typeof(Program));

		var pages = asm?.GetTypes()
			.Where(type => typeof(Microsoft.AspNetCore.Mvc.RazorPages.PageModel).IsAssignableFrom(type))
			.Where(t => t.IsDefined(typeof(DynamicRoleRequirementAttribute)))
			.Select(i => new AppRole
			{
				TypeName = i.MemberType.ToString(),
				Namespace = i.Namespace!,
				FullName = i.FullName!,
				Name = i.Name,
				MemberName = "*",
				ReturnType = "NOT-APPLICABLE",
				GuidString = i.GUID.ToString()
			})
			.ToList()
		;

		return pages;
	}

	public List<AppRole>? GetAll()
	{
		List<AppRole>? all = Enumerable.Empty<AppRole>()
			.Concat(GetControllers() ?? Enumerable.Empty<AppRole>())
			.Concat(GetActions() ?? Enumerable.Empty<AppRole>())
			.Concat(GetActions() ?? Enumerable.Empty<AppRole>())
			.ToList()
		;		

		return all;
	}
}
