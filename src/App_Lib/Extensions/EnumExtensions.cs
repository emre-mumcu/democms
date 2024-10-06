using System.ComponentModel;
using System.Reflection;
using src.App_Data.Types;
using src.App_Lib.Attributes;
using System.Linq;

namespace src.App_Lib.Extensions;

public static class EnumExtensions
{
	public static IEnumerable<TEnum> GetAllValues<TEnum>(this TEnum enumType) where TEnum : Enum
	{
		return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
	}

	public static IEnumerable<TAttribute> GetAllAttributes<TEnum, TAttribute>(this TEnum enumType) where TEnum : Enum where TAttribute: Attribute
	{
		IEnumerable<TAttribute> attributes = Enumerable.Empty<TAttribute>();

		Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList().ForEach(item => {
			attributes.Concat(item.GetAttributes<TAttribute>() ?? Enumerable.Empty<TAttribute>());
		});
		
		return attributes;
	}

	public static IEnumerable<T>? GetAttributes<T>(this Enum enumValue) where T : Attribute
	{
		FieldInfo? fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

		if (fieldInfo != null)
		{
			IEnumerable<T>? attributes = fieldInfo.GetCustomAttributes<T>(false);
			return attributes;
		}
		else throw new ArgumentException($"Field has no attributes", nameof(fieldInfo));
	}

	public static string GetDescription(this Enum enumValue)
	{
		var attributes = enumValue.GetAttributes<DescriptionAttribute>();

		return (attributes != null && attributes.Count() > 0)
			? string.Join(";", attributes.Select(c => c.Description).ToArray())
			: enumValue.ToString();
	}

	public static string GetContentType(this Enum enumValue)
	{
		var attributes = enumValue.GetAttributes<FileContentTypeAttribute>();

		return (attributes != null && attributes.Count() > 0)
			? attributes.Select(c => c.ContentType).First()
			: throw new ArgumentException($"Enum has no {nameof(FileContentTypeAttribute)} attribute", nameof(enumValue));
	}

	public static int GetContentLenght(this Enum enumValue)
	{
		var attributes = enumValue.GetAttributes<FileContentLengthAttribute>();

		return (attributes != null && attributes.Count() > 0)
			? attributes.Select(c => c.ContentLenght).First()
			: throw new ArgumentException($"Enum has no {nameof(FileContentTypeAttribute)} attribute", nameof(enumValue));
	}

	public static byte[] GetContentByte(this Enum enumValue)
	{
		var attributes = enumValue.GetAttributes<FileContentByteAttribute>();

		return (attributes != null && attributes.Count() > 0)
			? attributes.Select(c => c.ContentByte).First()
			: throw new ArgumentException($"Enum has no {nameof(FileContentTypeAttribute)} attribute", nameof(enumValue));
	}

	public static string GetIconName(this Enum enumValue)
	{
		var attributes = enumValue.GetAttributes<FileIconAttribute>();

		return (attributes != null && attributes.Count() > 0)
			? attributes.Select(c => c.FileIcon).First()
			: throw new ArgumentException($"Enum has no {nameof(FileContentTypeAttribute)} attribute", nameof(enumValue));
	}

	public static string GetIconColor(this Enum enumValue)
	{
		var attributes = enumValue.GetAttributes<FileIconColorAttribute>();

		return (attributes != null && attributes.Count() > 0)
			? attributes.Select(c => c.FileIconColor).First()
			: throw new ArgumentException($"Enum has no {nameof(FileContentTypeAttribute)} attribute", nameof(enumValue));
	}

	public static EnumContentTypes GetFileContentTypeEnum(this string contentType)
	{
		var match = ((EnumContentTypes[])Enum.GetValues(typeof(EnumContentTypes)))
			.ToList()
			.Where(e => e.GetContentType() == contentType)
			.FirstOrDefault()
		;
			
		return match;
	}
}