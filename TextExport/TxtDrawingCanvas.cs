using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms.VisualStyles;
using GrapeCity.ActiveReports.Drawing;
using GrapeCity.ActiveReports.Extensibility.Rendering;
using TextMetrics = GrapeCity.ActiveReports.Drawing.TextMetrics;

namespace TextExport
{
	class TxtDrawingCanvas : IDrawingCanvas
	{
		private readonly Size _settingsFontSizeTwips;

		class NoBrush : BrushEx
		{
			public override object Clone() => new NoBrush();
		}

		class NoPen : PenEx
		{
			public override object Clone() => new NoPen();

			public override Color Color { get; set; }
			public override float Width { get; set; }
			public override PenStyleEx DashStyle { get; set; }
			public override float[] DashPattern { get; set; }
			public override PenAlignment Alignment { get; set; }
			public override LineCap StartCap { get; set; }
			public override LineCap EndCap { get; set; }
			public override DashCap DashCap { get; set; }
			public override LineJoin LineJoin { get; set; }
		}

		class NoImage : ImageEx
		{
			public override object Clone() => new NoImage();
			public override SizeF Size { get; }
		}

		public BrushEx CreateSolidBrush(Color color) => new NoBrush();

		public BrushEx CreateLinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2,
			BlendEx blend = null)
			=> new NoBrush();

		public BrushEx CreateRadialGradientBrush(PointF centerPoint, float radiusX, float radiusY, Color centerColor,
			Color surroundingColor)
			=> new NoBrush();

		public BrushEx CreateHatchBrush(HatchStyleEx hatchStyle, Color foreColor, Color backColor)
			=> new NoBrush();

		public PenEx CreatePen(Color color, float width) => new NoPen();

		public PenEx CreatePen(Color color) => new NoPen();

		public ImageEx CreateImage(ImageInfo image) => new NoImage();

		public ImageEx CreateImage(ImageInfo image, string cacheID) => new NoImage();

		public void DrawImage(ImageEx image, float x, float y, float width, float height, float opacity = 100)
		{
		}

		public void DrawLine(PenEx pen, PointF @from, PointF to)
		{
		}


		public TextRenderingHintEx TextRenderingHint { get; set; }
		public SmoothingModeEx SmoothingMode { get; set; }

		public void DrawRectangle(PenEx pen, RectangleF rect) { }

		public void FillRectangle(BrushEx brush, RectangleF rect) { }

		public RectangleF ClipBounds { get; }

		public void IntersectClip(RectangleF rect) { }

		public void IntersectClip(PathEx path) { }

		public void PushState() { }

		public void PopState() { }

		public void DrawEllipse(PenEx pen, RectangleF rect) { }

		public void FillEllipse(BrushEx brush, RectangleF rect) { }

		public void DrawPolygon(PenEx pen, PointF[] polygon) { }

		public void FillPolygon(BrushEx brush, PointF[] polygon) { }

		public void DrawLines(PenEx pen, PointF[] polyLine) { }

		public Matrix3x2 Transform { get; set; } = Matrix3x2.Identity;

		public void DrawAndFillPath(PenEx pen, BrushEx brush, PathEx path) { }


		public void DrawString(string value, FontInfo font, BrushEx brush, RectangleF layoutRectangle,
			StringFormatEx format)
		{
			var pts = new[] {layoutRectangle.Location, new PointF(layoutRectangle.Right, layoutRectangle.Bottom)};
			Transform.TransformPoints(pts);

			var w = pts[1].X - pts[0].X;
			var h = pts[1].Y - pts[0].Y;


			_items.Add(new TextItem()
			{
				Value = value,
				//todo: apply transform
				Bounds = new Rectangle(
					(int) pts[0].X / (int) _settingsFontSizeTwips.Width,
					(int) pts[0].Y / (int) _settingsFontSizeTwips.Height,
					(int) w / (int) _settingsFontSizeTwips.Width,
					(int) h / (int) _settingsFontSizeTwips.Height),
				Format = format
			});
		}

		List<TextItem> _items = new List<TextItem>();

		public TxtDrawingCanvas(Size settingsFontSizeTwips)
		{
			_settingsFontSizeTwips = settingsFontSizeTwips;
		}

		class TextItem
		{
			public string Value;
			public Rectangle Bounds;
			public StringFormatEx Format;
		}

		public void Write(TextWriter writer)
		{
			var charList = new List<List<char>>();

			foreach (var item in _items)
			{
				Draw(charList, item);
			}

			foreach (var line in charList)
			{
				writer.Write(line.ToArray());
				writer.WriteLine();
			}
		}

		private void Draw(List<List<char>> charList, TextItem item)
		{
			while (charList.Count <= item.Bounds.Bottom)
				charList.Add(new List<char>());

			//todo: bounds, alignment, etc

			var lineList = charList[item.Bounds.Top];
			while (lineList.Count <= item.Bounds.Right)
				lineList.Add(' ');

			var textLayout = TextLayout.SplitLines(item.Value, item.Format, item.Bounds.Width).ToList();


			for (var y = 0; y < Math.Min(item.Bounds.Height, textLayout.Count); y++)
			{
				var line = textLayout[y];
				
				var drawLineWidth = Math.Min(item.Bounds.Width, line.Length);

				var alignOffset = item.Format.Alignment == StringAlignmentEx.Center
					? (item.Bounds.Width - drawLineWidth) / 2
					: item.Format.Alignment == StringAlignmentEx.Far 
						? item.Bounds.Width - drawLineWidth : 0;
				
				for (var x = 0; x < drawLineWidth; x++)
					lineList[x + item.Bounds.Left + alignOffset] = item.Value[line.StartIndex + x];
			}
		}
	}
	
	class MetricsProvider : ITextMetricsProvider
	{
		private readonly Size _settingsFontSizeTwips;

		public MetricsProvider(Size settingsFontSizeTwips) => _settingsFontSizeTwips = settingsFontSizeTwips;

		public TextMetrics MeasureString(FontInfo font, string contentText, StringFormatEx stringFormat,
			float boundWidth = Single.MaxValue, float boundHeight = Single.MaxValue)
		{ 
			var boundWidthChars = boundWidth == Single.MaxValue ? int.MaxValue : (int)(boundWidth / _settingsFontSizeTwips.Width);
			var boundHeightChars = boundHeight == Single.MaxValue ? int.MaxValue : (int)(boundHeight / _settingsFontSizeTwips.Height);

			var lines = TextLayout.SplitLines(contentText, stringFormat, boundHeightChars).ToList();

			if (lines.Count == 0)
				return new TextMetrics(0,0,0,0,true);

			var realWidth = lines.Max(x => x.Length);

			var fit = lines.Count <= boundHeightChars;

			var fitLinesCount = Math.Min(lines.Count, boundHeightChars);

			var lastFitLine = lines[fitLinesCount - 1];
			
			var totalFitCharsCount = lastFitLine.StartIndex + lastFitLine.Length;
			
			return new TextMetrics(
				realWidth * _settingsFontSizeTwips.Width, 
				fitLinesCount * _settingsFontSizeTwips.Height, 
				fitLinesCount,
				totalFitCharsCount * boundWidthChars, fit);
		}
				

		public FontMetrics GetFontMetrics(FontInfo font) => new FontMetrics(0,0,1);

		public string GetFontFamily(string fontFamily) => fontFamily;
	}
}