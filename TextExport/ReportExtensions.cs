using System.Collections.Generic;
using System.Linq;
using GrapeCity.ActiveReports.PageReportModel;

namespace TextExport
{
	public static class ReportExtensions
	{
		/// <summary>Removes paddings and rows heights.</summary>
		/// <param name="report">The source report</param>
		/// <returns>The report without spacing.</returns>
		public static Report RemoveSpacing(this Report report)
		{
			//var result = report.Clone();
			var result = report;

			foreach(var reportComponent in report.Flat())
			{
				switch (reportComponent)
				{
					case TextBox textBox:
						textBox.Style.PaddingBottom = "0pt";
						textBox.Style.PaddingTop = "0pt";
						textBox.Style.PaddingLeft = "0pt";
						textBox.Style.PaddingRight = "0pt";
						textBox.Style.ShrinkToFit = "false";
						textBox.Style.MinCondenseRate = "100";
						break;
					case TableRow tableRow:
						tableRow.Height = "0pt";
						break;
					case TablixRow tablixRow:
						tablixRow.Height = "0pt";
						break;
					case MatrixRow matrixRow:
						matrixRow.Height = "0pt";
						break;
				}
			}

			return result;
		}


		private static IEnumerable<IReportComponent> Flat(this IReportComponent component) =>
			component is IReportComponentContainer container
				? container.Components.SelectMany(x => x.Flat())
				: Enumerable.Repeat(component, 1);
	}
}