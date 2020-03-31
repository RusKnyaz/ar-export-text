using System;
using System.Drawing;
using System.Linq;
using GrapeCity.ActiveReports.Drawing;

namespace TextExport
{
	class TxtMetricsProvider : ITextMetricsProvider
	{
		private readonly Size _settingsFontSizeTwips;

		public TxtMetricsProvider(Size settingsFontSizeTwips) => _settingsFontSizeTwips = settingsFontSizeTwips;

		public TextMetrics MeasureString(FontInfo font, string contentText, StringFormatEx stringFormat,
			float boundWidth = Single.MaxValue, float boundHeight = Single.MaxValue)
		{ 
			var boundWidthChars = boundWidth == Single.MaxValue ? int.MaxValue : (int)(boundWidth / _settingsFontSizeTwips.Width);
			var boundHeightChars = boundHeight == Single.MaxValue ? int.MaxValue : (int)(boundHeight / _settingsFontSizeTwips.Height);

			var lines = TextLayout.SplitLines(contentText, stringFormat, boundWidthChars).ToList();

			if (lines.Count == 0)
				return new TextMetrics(0,0,0,0,true);

			var realWidth = lines.Max(x => x.Length);

			var fit = lines.Count <= boundHeightChars;

			var fitLinesCount = Math.Min(lines.Count, boundHeightChars);

			var lastFitLine = lines[fitLinesCount - 1];
			
			var totalFitCharsCount = lastFitLine.StartIndex + lastFitLine.Length;
			
			return new TextMetrics(
				width: realWidth * _settingsFontSizeTwips.Width, 
				height: fitLinesCount * _settingsFontSizeTwips.Height, 
				linesFilled: fitLinesCount,
				charsFitted: totalFitCharsCount,
				fitted: fit);
		}
				

		public FontMetrics GetFontMetrics(FontInfo font) => new FontMetrics(0,0,1);

		public string GetFontFamily(string fontFamily) => fontFamily;
	}
}