using System;

namespace src.App_Lib.Configuration.Ext;

public static class Json
{
	public static IMvcBuilder _AddJsonOptions(this IMvcBuilder mvcBuilder)
	{

		// Newtonsoft.Json Configuration
		// dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
		mvcBuilder.AddNewtonsoftJson(options =>
		{
			options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
			options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
		});

		// System.Text.Json Configuration
		mvcBuilder.AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.WriteIndented = true;
			// Default is JsonNamingPolicy.CamelCase. 
			// Setting it to null will result in property names NOT changing while serializing
			options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
			options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never;
			options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
		});

		return mvcBuilder;
	}
}