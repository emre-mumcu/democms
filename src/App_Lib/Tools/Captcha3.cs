using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using src.App_Lib.Configuration;
using src.App_Lib.Configuration.Ext;

// dotnet add package SixLabors.ImageSharp 
// dotnet add package SixLabors.ImageSharp.Drawing

namespace src.App_Lib.Tools;

public static class Captcha3
{
	public static bool ValidateCaptchaCode(string userInputCaptcha, HttpContext context)
	{
		var isValid = (userInputCaptcha == context.Session.GetKey<string>(Literals.SessionKey_Captcha));
		context.Session.RemoveKey(Literals.SessionKey_Captcha);
		return isValid;
	}

	public static CaptchaResult GenerateCaptchaImage(HttpContext context)
	{
		CaptchaResult cr = GenerateCaptchaImage(captchaCode: GenerateCaptchaCode());
		context.Session.SetKey<string>(Literals.SessionKey_Captcha, cr.CaptchaCode);
		return cr;
	}

	private static string GenerateCaptchaCode(int characterCount = 4)
	{
		StringBuilder sb = new StringBuilder();

		for (int i = 0; i < characterCount; i++)
		{
			int index = RandomGenerator.Next(0, Letters.Length);
			sb.Append(Letters[index]);
		}

		return sb.ToString();
	}

	// TODO: Captcha Settings
	private const string Letters = "0123456789";	
	public static string[] FontFamilies { get; set; } = new string[] { "Arial", "Verdana", "Times New Roman" };
	public static Color[] TextColor { get; set; } = new Color[] { Color.Blue, Color.Black, Color.Black, Color.Brown, Color.Gray, Color.Green };
	public static Color[] DrawLinesColor { get; set; } = new Color[] { Color.Blue, Color.Black, Color.Black, Color.Brown, Color.Gray, Color.Green };
	public static Color[] NoiseRateColor { get; set; } = new Color[] { Color.Gray };
	public static Color[] BackgroundColor { get; set; } = new Color[] { Color.White };

	public static CaptchaResult GenerateCaptchaImage(string captchaCode, int width = 100, int height = 36)
	{
		AffineTransformBuilder getRotation(int w, int h)
		{
			Random random = new Random();
			var builder = new AffineTransformBuilder();
			var width = random.Next(10, w);
			var height = random.Next(10, h);
			var pointF = new PointF(width, height);
			var rotationDegrees = random.Next(5, 10);//0,5
			var result = builder.PrependRotationDegrees(rotationDegrees, pointF);
			return result;
		}

		float GenerateNextFloat(double min = -3.40282347E+38, double max = 3.40282347E+38)
		{
			Random random = new Random();
			double range = max - min;
			double sample = random.NextDouble();
			double scaled = (sample * range) + min;
			float result = (float)scaled;
			return result;
		}

		byte[] result;

		using (var imgText = new Image<Rgba32>(width, height))
		{
			float position = 0;
			var random = new Random();
			var startWith = (byte)random.Next(5, 10);
			imgText.Mutate(ctx => ctx.BackgroundColor(Color.Transparent));

			var fontName = FontFamilies[random.Next(0, FontFamilies.Length)];
			var font = SystemFonts.CreateFont(fontName, 24, FontStyle.Regular);

			foreach (var c in captchaCode)
			{
				var location = new PointF(startWith + position, random.Next(6, 13));
				imgText.Mutate(ctx => ctx.DrawText(c.ToString(), font, TextColor[random.Next(0, TextColor.Length)], location));
				position += TextMeasurer.MeasureSize(c.ToString(), new TextOptions(font)).Width;
			}

			// add rotation
			var rotation = getRotation(width, height);
			imgText.Mutate(ctx => ctx.Transform(rotation));

			// add the dynamic image to original image
			var size = (ushort)TextMeasurer.MeasureSize(captchaCode, new TextOptions(font)).Width;
			var img = new Image<Rgba32>(size + 10 + 5, height);
			img.Mutate(ctx => ctx.BackgroundColor(BackgroundColor[random.Next(0, BackgroundColor.Length)]));

			// Background Lines
			Parallel.For(0, 1, i => // 3
			{
				var x0 = random.Next(0, random.Next(0, 30));
				var y0 = random.Next(10, img.Height);
				var x1 = random.Next(img.Width - random.Next(0, (int)(img.Width * 0.25)), img.Width);
				var y1 = random.Next(0, img.Height);
				img.Mutate(ctx =>
					ctx.DrawLine(DrawLinesColor[random.Next(0, DrawLinesColor.Length)],
								  GenerateNextFloat(0.7f, 2.0f),
								  new PointF[] { new(x0, y0), new(x1, y1) })
					);
			});

			img.Mutate(ctx => ctx.DrawImage(imgText, 0.80f));

			// Backgroung Noise
			Parallel.For(0, 150, i => // 250
			{
				var x0 = random.Next(0, img.Width);
				var y0 = random.Next(0, img.Height);
				img.Mutate(
						ctx => ctx
							.DrawLine(NoiseRateColor[random.Next(0, NoiseRateColor.Length)],
							GenerateNextFloat(0.5, 1.5), new PointF[] { new Vector2(x0, y0), new Vector2(x0, y0) })
					);
			});

			img.Mutate(x => x.Resize(width, height));

			using (var ms = new MemoryStream())
			{
				// img.Save(ms, new JpegEncoder());
				img.Save(ms, new PngEncoder());
				result = ms.ToArray();
			}
		}

		return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = result, Timestamp = DateTime.Now };

	}

	public class CaptchaResult
	{
		public string CaptchaCode { get; set; } = string.Empty;
		public byte[] CaptchaByteData { get; set; } = new byte[] { 0 };
		public string CaptchBase64Data => Convert.ToBase64String(CaptchaByteData);
		public DateTime Timestamp { get; set; }
	}
}