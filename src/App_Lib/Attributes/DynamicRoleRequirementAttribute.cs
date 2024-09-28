using Microsoft.AspNetCore.Mvc;
using src.App_Lib.Filters;

namespace src.App_Lib.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class DynamicRoleRequirementAttribute : TypeFilterAttribute
{
	/// <param name="type">Parent class (controller)</param>
	/// <param name="name">If no name is specified, * is ued for the filter to be applied for all sub-types suc as actions in controller</param>
	public DynamicRoleRequirementAttribute(Type type, string name = "*") : base(typeof(DynamicRoleRequirementFilter))
	{
		Arguments = new object[] { type, name };
	}
}