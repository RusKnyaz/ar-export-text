using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using GrapeCity.ActiveReports.Drawing;
using GrapeCity.ActiveReports.Extensibility.Rendering;

namespace TextExport
{
	class TxtDrawingCanvas : IDrawingCanvas
	{
		private readonly Size _settingsFontSizeTwips;
		private readonly Stack<CanvasState> _states = new Stack<CanvasState>();
		private readonly List<TextItem> _items = new List<TextItem>();

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

		public RectangleF ClipBounds { get; private set; }

		public void IntersectClip(RectangleF rect) { }

		public void IntersectClip(PathEx path) { }

		struct CanvasState
		{
			public Matrix3x2 Transform;
			public RectangleF ClipBounds;
		}

		public void PushState()
		{
			_states.Push(new CanvasState{Transform =  Transform, ClipBounds = ClipBounds});
		}

		public void PopState()
		{
			var state = _states.Pop();
			Transform = state.Transform;
			ClipBounds = state.ClipBounds;
		}

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

			var textItem = new TextItem()
			{
				Value = value,
				Bounds = new Rectangle(
					(int) (pts[0].X / _settingsFontSizeTwips.Width),
					(int) (pts[0].Y / _settingsFontSizeTwips.Height),
					(int) (w / _settingsFontSizeTwips.Width),
					(int) (h / _settingsFontSizeTwips.Height)),
				Format = format
			};

			_items.Add(textItem);
		}


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
}