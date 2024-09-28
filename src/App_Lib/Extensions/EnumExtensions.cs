using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace src.App_Lib.Extensions;

public static class EnumExtensions
{
    public static FileContentTypeEnum GetEnumFromContentType(this string contentType)
    {
        foreach (FileContentTypeEnum fileContent in Enum.GetValues(typeof(FileContentTypeEnum)))
        {
            if (contentType == GetEnumContentType(fileContent))
            {
                return fileContent;
            }
        }

        return FileContentTypeEnum.Bilinmeyen;
    }


    public static string GetEnumDescription(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        return descriptionAttributes.Length > 0 ? descriptionAttributes[0].Description : enumValue.ToString();
    }


    public static string GetEnumContentType(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var contetntTypeAttributes = (FileContentTypeAttribute[])fieldInfo.GetCustomAttributes(typeof(FileContentTypeAttribute), false);

        return contetntTypeAttributes.Length > 0 ? contetntTypeAttributes[0].ContentType : enumValue.ToString();
    }



    public static int GetEnumContentLenght(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var contetntLengthAttributes = (FileContentLengthAttribute[])fieldInfo.GetCustomAttributes(typeof(FileContentLengthAttribute), false);

        return contetntLengthAttributes.Length > 0 ? contetntLengthAttributes[0].ContentLenght : Convert.ToInt32(enumValue);
    }


    public static byte[] GetEnumContentByte(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var contetntByteAttributes = (FileContentByteAttribute[])fieldInfo.GetCustomAttributes(typeof(FileContentByteAttribute), false);

        return contetntByteAttributes[0].ContentByte;
    }



    public static string GetEnumIcon(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var iconAttributes = (FileIconAttribute[])fieldInfo.GetCustomAttributes(typeof(FileIconAttribute), false);

        return iconAttributes.Length > 0 ? iconAttributes[0].FileIcon : enumValue.ToString();
    }

    public static string GetEnumIconColor(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());

        var iconColorAttributes = (FileIconColorAttribute[])fieldInfo.GetCustomAttributes(typeof(FileIconColorAttribute), false);

        return iconColorAttributes.Length > 0 ? iconColorAttributes[0].FileIconColor : enumValue.ToString();
    }



}

public enum FileContentTypeEnum
{
    
    [FileContentType("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 80, 75, 3, 4 })]
    [FileIcon("fas fa-file-excel")]
    [FileIconColor("green")]
    Excel = 1,

    [FileContentType("application/vnd.openxmlformats-officedocument.wordprocessingml.document")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 80, 75, 3, 4 })]
    [FileIcon("fas fa-file-word")]
    [FileIconColor("blue")]
    Word = 2,

    [FileContentType("image/jpeg")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 255, 216, 255, 224 })]
    Image = 3,

    [FileContentType("application/pdf")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 37, 80, 68, 70 })]
    [FileIcon("fas fa-file-pdf")]
    [FileIconColor("red")]
    Pdf = 4,

    [FileContentType("application/msword")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 208, 207, 17, 224 })]
    [FileIcon("fas fa-file-word")]
    [FileIconColor("blue")]
    MsWord = 5,

    [FileContentType("application/vnd.ms-excel")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 208, 207, 17, 224 })]
    [FileIcon("fas fa-file-excel")]
    [FileIconColor("green")]
    MsXls = 6,

    [FileContentType("application/x-zip-compressed")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 80, 75, 3, 4 })]
    [FileIcon("fas fa-file-archive")]
    [FileIconColor("orange")]
    Zip = 7,

    [FileContentType("image/svg+xml")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 60, 115, 118, 103 })]
    Svg = 8,


    [FileContentType("application/vnd.openxmlformats-officedocument.presentationml.presentation")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 80, 75, 3, 4 })]
    [FileIcon("fas fa-file-powerpoint")]
    [FileIconColor("#FF7F50")]
    PowerPoint = 9,

    [FileContentType("application/octet-stream")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 82, 97, 114, 33 })]
    [FileIcon("fas fa-file-archive")]
    [FileIconColor("orange")]
    Rar = 10,

    [FileContentType("image/jpeg")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 255, 216, 255, 225 })]
    ImageJpg = 11,

    [FileContentType("image/png")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 137, 80, 78, 71 })]
    ImagePng = 12,

    [FileContentType("application/xml")]
    [FileContentLength(500000000)]
    [FileIcon("fas fa-file-xml")]
    [FileContentByte(new byte[] { 60, 63, 120, 109 })]
    Xsd = 13,

    [FileContentType("text/xml")]
    [FileContentLength(500000000)]
    [FileIcon("fas fa-file-xml")]
    [FileContentByte(new byte[] { 60, 63, 120, 109 })]
    Xml = 14,

    [FileContentType("application/vnd.ms-excel")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 80, 75, 3, 4 })]
    [FileIcon("fas fa-file-excel")]
    [FileIconColor("green")]
    MsXls97 = 15,


    [FileContentType("application/vnd.ms-powerpoint")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 208, 207, 17, 224 })]
    [FileIcon("fas fa-file-powerpoint")]
    [FileIconColor("#FF7F50")]
    PowerPoint97 = 16,


    [FileContentType("image/svg+xml")]
    [FileContentLength(500000000)]
    [FileContentByte(new byte[] { 60, 63, 120, 109 })]
    Svg2 = 17,

    
    Bilinmeyen = -1




}

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