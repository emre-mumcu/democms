using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using src.App_Data.Entities;
using src.App_Lib.Cache;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;
using src.App_Lib.Tools;

namespace src.App_Lib.Filters;

public class DynamicRoleRequirementFilter : IAuthorizationFilter
{
	private readonly Type _type;
	private readonly string? _memberName;

	public DynamicRoleRequirementFilter(Type type, string? memberName)
	{
		_type = type;
		_memberName = memberName;
	}

	public async void OnAuthorization(AuthorizationFilterContext context)
	{
		// AppDbContext dbContext = context.HttpContext.RequestServices.GetService(typeof(AppDbContext)) as AppDbContext;

		string? ErisilmekIstenenKaynak_AdAlani = _type.Namespace;
		string? ErisilmekIstenenKaynak_TamAdi = _type.FullName;
		string? ErisilmekIstenenKaynak_UyeAdi = _type.Name;

		string? userSelectedRoleEnc = context.HttpContext.Session.GetKey<string>(Literals.SessionKey_SelectedRole);
		
		string? userSelectedRole = userSelectedRoleEnc != null 
			? Security.Decrypt(userSelectedRoleEnc, context.HttpContext.Session.Id) 
			: null
		;

		var memCache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

		List<DynamicRole>? roleMatrix = await new _CacheRoleMatrix(memCache).GetCache();

		IEnumerable<DynamicRole>? RolCodeIcinVeriTabanindakiYetkiTanimlari = roleMatrix?.Where(i => i.RoleCode == userSelectedRole);

		bool RedEdildi = DenyCheck(
			redListesi: RolCodeIcinVeriTabanindakiYetkiTanimlari.Where(y => y.Allow == false).ToList(),
			ErisilmekIstenenKaynak: $"{ErisilmekIstenenKaynak_TamAdi}.{_memberName}",
			typeName: _type?.BaseType?.Name);
		if (RedEdildi)
		{
			context.Result = new ForbidResult();
			return;
		}

		bool IzinVerildi = AllowCheck(
			izinListesi: RolCodeIcinVeriTabanindakiYetkiTanimlari.Where(y => y.Allow == true).ToList(),
			ErisilmekIstenenKaynak: $"{ErisilmekIstenenKaynak_TamAdi}.{_memberName}",
			typeName: _type?.BaseType?.Name);
		if (!IzinVerildi)
		{
			context.Result = new ForbidResult();
			return;
		}
	}

	/// <summary>
	/// DenyCheck must run before AllowCheck
	/// True if deny, False if NOT
	/// </summary>
	public static bool DenyCheck(List<DynamicRole> redListesi, string ErisilmekIstenenKaynak, string? typeName = nameof(Controller))
	{
		foreach (DynamicRole yetki in redListesi)
		{
			if (typeName == nameof(PageModel))
			{
				if (yetki.FullName == ErisilmekIstenenKaynak)
				{
					return true;
				}
			}

			if (typeName == nameof(Controller))
			{
				if (yetki.FullName == ErisilmekIstenenKaynak)
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// AllowCheck must run after DenyCheck
	/// True if allow, False if NOT
	/// </summary>
	public static bool AllowCheck(List<DynamicRole> izinListesi, string ErisilmekIstenenKaynak, string? typeName = nameof(Controller))
	{
		foreach (DynamicRole yetki in izinListesi)
		{
			if (typeName == nameof(PageModel))
			{
				if (yetki.FullName == ErisilmekIstenenKaynak)
				{
					return true;
				}
			}

			if (typeName == nameof(Controller))
			{
				if (yetki.FullName == ErisilmekIstenenKaynak)
				{
					return true;
				}
			}
		}

		return false;
	}
}