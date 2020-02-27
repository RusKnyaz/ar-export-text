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
	public class TxtSettings
	{
		public Size FontSizePt = new Size(6, 12);
	}
	
	public class TextRendering
	{
		public static void Render(IReport report, TextWriter writer, TxtSettings settings)
		{
			if (report == null)
				throw new ArgumentNullException(nameof(report));


			var settingsFontSizeTwips = new Size(settings.FontSizePt.Width * 20, settings.FontSizePt.Height*20);

			var metricsProvider = new MetricsProvider(settingsFontSizeTwips);
			var targetDevice = CreateTargetDevice(report);
			var	layoutTree = GenerateLayoutTree(report, targetDevice, metricsProvider);

			var page = layoutTree.Pages.First();
			
			var canvas = new TxtDrawingCanvas(settingsFontSizeTwips);

			var context = new GraphicsRenderContext(targetDevice, canvas, metricsProvider, RenderersFactory.Instance, null);
				
			Renderer.RenderPage(context, page);

			canvas.Write(writer);
		}
		
		private static ILayoutTree GenerateLayoutTree(IReport report, 
			ITargetDevice targetDevice,
			ITextMetricsProvider metricsProvider)
		{
			
			
			var layoutInfo = new LayoutInfo(report, targetDevice, metricsProvider);
			var engine = GetLayoutEngine(report);
			return engine.BuildLayout(layoutInfo);
		}
		
		internal static ITargetDevice CreateTargetDevice(IReport report)
		{
			var targetDevice = new TargetDevice(TargetDeviceKind.Export, InteractivityType.None, null, false, true);
			return targetDevice;
		}

		private static ILayoutEngine GetLayoutEngine(IReport report)
		{
			var factory = report.GetService(typeof(ILayoutEngineFactory)) as ILayoutEngineFactory;
			if (factory == null) throw new InvalidOperationException("No required ILayoutEngineFactory service!");
			return factory.GetLayoutEngine();
		}


		
	}
}