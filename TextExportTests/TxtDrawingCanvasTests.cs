using System.Drawing;
using System.IO;
using GrapeCity.ActiveReports.Drawing;
using NUnit.Framework;
using TextExport;

namespace TextExportTests
{
	[TestFixture]
	public class TxtDrawingCanvasTests
	{
		private static string DrawText(string txt, RectangleF bounds, StringFormatEx format = null)
		{
			var canvas = new TxtDrawingCanvas(new Size(10,10), "\n");
			canvas.DrawString(txt, null, null, bounds, format ?? StringFormatEx.GetGenericDefault);
			using var writer = new StringWriter();
			canvas.Write(writer);
			return writer.ToString();
		}
		
		[Test]
		public  static void DrawTextAtZero()
		{
			var result = DrawText("HELLO", new RectangleF(0,0,100,10));
			Assert.AreEqual("HELLO", result);
		}

		[Test]
		public static void DrawTextOffset()
		{
			var result = DrawText("HELLO", new RectangleF(10,10,100,10));
			Assert.AreEqual("\n HELLO", result);
		}

		[Test]
		public static void DrawNothing()
		{
			var canvas = new TxtDrawingCanvas(new Size(10,10));
			using var writer = new StringWriter();
			canvas.Write(writer);
			Assert.AreEqual("", writer.ToString());
		}
	}
}