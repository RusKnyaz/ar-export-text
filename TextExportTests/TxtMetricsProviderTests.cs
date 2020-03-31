using System.Drawing;
using GrapeCity.ActiveReports.Drawing;
using NUnit.Framework;
using TextExport;

namespace TextExportTests
{
	[TestFixture]
	public class TxtMetricsProviderTests
	{
		[Test]
		public void CharWrap()
		{
			var textMetrics = new TxtMetricsProvider(new Size(1, 1));
			var result = textMetrics.MeasureString(null, "ABCDEF", new StringFormatEx()
			{
				WrapMode = WrapMode.CharWrap
			}, 3);
			
			Assert.AreEqual(2, result.LinesFilled);
			Assert.AreEqual(2, result.Height);
			Assert.AreEqual(3, result.Width);
		}

		[Test]
		public void CharWrapNoSpace()
		{
			var textMetrics = new TxtMetricsProvider(new Size(1, 1));
			var result = textMetrics.MeasureString(null, "ABCDEF", new StringFormatEx()
			{
				WrapMode = WrapMode.CharWrap
			}, 3, 1);
			
			Assert.AreEqual(1, result.LinesFilled);
			Assert.AreEqual(1, result.Height);
			Assert.AreEqual(3, result.Width);
			Assert.IsFalse(result.Fitted);
			Assert.AreEqual(3, result.CharsFitted);
		}

		[Test]
		public void Size()
		{
			var textMetrics = new TxtMetricsProvider(new Size(1, 1));
			var result = textMetrics.MeasureString(null, "ABCD\r\nEF", new StringFormatEx());
			Assert.AreEqual(2, result.LinesFilled);
			Assert.AreEqual(2, result.Height);
			Assert.AreEqual(4, result.Width);
			Assert.IsTrue(result.Fitted);
			Assert.AreEqual(8, result.CharsFitted);
		}
	}
}