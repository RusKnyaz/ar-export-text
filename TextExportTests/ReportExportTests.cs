using System.IO;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.PageReportModel;
using GrapeCity.ActiveReports.Rendering;
using NUnit.Framework;
using TextExport;

namespace TextExportTests
{
	[TestFixture]
	public class ReportExportTests
	{
		[TestCase("0pt", PaddingsType.Adjust, ExpectedResult = "Hello\n")]
		[TestCase("5pt", PaddingsType.Adjust, ExpectedResult = "Hello\n")]
		[TestCase("6pt", PaddingsType.Adjust, ExpectedResult = " Hello\n")]
		[TestCase("20pt", PaddingsType.Adjust, ExpectedResult = "   Hello\n")]
		[TestCase("0pt", PaddingsType.Remove, ExpectedResult = "Hello\n")]
		[TestCase("5pt", PaddingsType.Remove, ExpectedResult = "Hello\n")]
		[TestCase("6pt", PaddingsType.Remove, ExpectedResult = "Hello\n")]
		[TestCase("20pt", PaddingsType.Remove, ExpectedResult = "Hello\n")]
		public static string TextboxPaddingsAdjust(string paddings, PaddingsType paddingsType)
		{
			var report = new Report {
				Body = {
					ReportItems = {
						new TextBox {Name = "textBox", Width = "100pt", Height = "30pt", Value = "Hello", 
						Style = {PaddingLeft = paddings}}}
				}
			};
			
			var settings = new TxtSettings {LineEnding = "\n", HorizontalPaddings = paddingsType};

			return RenderReport(report, settings);
		}
		
		private static string RenderReport(Report report, TxtSettings settings)
		{
			var rep2 = TextRendering.PrepareReport(report, settings);
			var store = new ReportStore(rep2, new DefaultResourceLocator());
			var iReport = store.BuildReport();

			var writer = new StringWriter();
			TextRendering.Render(iReport, writer, settings);

			var result = writer.ToString();
			return result;
		}
	}
}