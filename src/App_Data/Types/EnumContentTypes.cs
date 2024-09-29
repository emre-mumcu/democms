using src.App_Lib.Extensions;

namespace src.App_Data.Types;

public enum EnumContentTypes
{
	Unknown = 0,

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
}