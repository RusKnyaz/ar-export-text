using System;
using System.Drawing;
using System.IO;
using System.Linq;
using GrapeCity.ActiveReports.Drawing;
using GrapeCity.ActiveReports.Extensibility.Layout;
using GrapeCity.ActiveReports.Extensibility.Rendering.Components;
using GrapeCity.ActiveReports.ImageRenderers;
using GrapeCity.ActiveReports.PageReportModel;
using GrapeCity.ActiveReports.Rendering.Export;
using GrapeCity.ActiveReports.Rendering.Tools;

namespace TextExport
{
	/// <summary> Describe how the item's paddings should be handled. </summary>
	public enum PaddingsType
	{
		/// <summary> Adjust the paddings so that they become a multiple of the size of the font. </summary>
		Adjust,
		/// <summary>Remove paddings. </summary>
		Remove,
		/// <summary>Keeps the paddings as is.</summary>
		Keep
	
	}
	
	public class TxtSettings
	{
		public Size FontSizePt = new Size(6, 12);
		public PaddingsType HorizontalPaddings = PaddingsType.Adjust;
		public string LineEnding = Environment.NewLine;
	}
	
	public class TextRendering
	{
		public static void Render(IReport report, TextWriter writer, TxtSettings settings)
		{
			if (report == null)
				throw new ArgumentNullException(nameof(report));

			var settingsFontSizeTwips = new Size(settings.FontSizePt.Width * 20, settings.FontSizePt.Height*20);

			var metricsProvider = new TxtMetricsProvider(settingsFontSizeTwips);
			var targetDevice = (ITargetDevice) new TargetDevice(TargetDeviceKind.Export, InteractivityType.None, null, false, true);
			var	layoutTree = GenerateLayoutTree(report, targetDevice, metricsProvider);

			var page = layoutTree.Pages.First();
			
			var canvas = new TxtDrawingCanvas(settingsFontSizeTwips, settings.LineEnding);

			var context = new GraphicsRenderContext(targetDevice, canvas, metricsProvider, RenderersFactory.Instance, null);
				
			Renderer.RenderPage(context, page);

			canvas.Write(writer);
		}

		public static Report PrepareReport(Report report, TxtSettings settings) =>
			report.Clone().RemoveSpacing(settings);
		
		private static ILayoutTree GenerateLayoutTree(IReport report, 
			ITargetDevice targetDevice,
			ITextMetricsProvider metricsProvider)
		{
			var layoutInfo = new LayoutInfo(report, targetDevice, metricsProvider);
			var engine = GetLayoutEngine(report);
			return engine.BuildLayout(layoutInfo);
		}

		private static ILayoutEngine GetLayoutEngine(IReport report)
		{
			var factory = report.GetService(typeof(ILayoutEngineFactory)) as ILayoutEngineFactory;
			if (factory == null) throw new InvalidOperationException("No required ILayoutEngineFactory service!");
			return factory.GetLayoutEngine();
		}


		
	}
}