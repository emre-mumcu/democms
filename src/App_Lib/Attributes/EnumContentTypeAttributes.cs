namespace src.App_Lib.Attributes;

public class FileContentTypeAttribute : Attribute
{
	public string ContentType { get; private set; }

	public FileContentTypeAttribute(string contentType)
	{
		ContentType = contentType;
	}
}

public class FileContentLengthAttribute : Attribute
{
	public int ContentLenght { get; private set; }

	public FileContentLengthAttribute(int contentLength)
	{
		ContentLenght = contentLength;
	}
}

public class FileContentByteAttribute : Attribute
{
	public byte[] ContentByte { get; private set; }

	public FileContentByteAttribute(byte[] contentByte)
	{
		ContentByte = contentByte;
	}
}

public class FileIconAttribute : Attribute
{
	public string FileIcon { get; private set; }

	public FileIconAttribute(string fileIcon)
	{
		FileIcon = fileIcon;
	}
}

public class FileIconColorAttribute : Attribute
{
	public string FileIconColor { get; private set; }

	public FileIconColorAttribute(string fileIconColor)
	{
		FileIconColor = fileIconColor;
	}
}