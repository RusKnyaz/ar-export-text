using System.Collections.Generic;
using System.Linq;
using GrapeCity.ActiveReports.PageReportModel;
using GrapeCity.Enterprise.Data.DataEngine.Extensions;

namespace TextExport
{
	public static class ReportExtensions
	{
		/// <summary>Removes paddings and rows heights.</summary>
		/// <param name="report">The source report</param>
		/// <returns>The report without spacing.</returns>
		public static Report RemoveSpacing(this Report report, string lineHeight)
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
						textBox.CanGrow = true;
						break;
					case Tablix tablix:
						foreach (var tablixRow in tablix.TablixBody.TablixRows)
							tablixRow.Height = lineHeight;
						break;
					case Table table:
						foreach (var tableRow in table.GetRows())
							tableRow.Height = lineHeight;
						break;
				}
			}

			return result;
		}

		private static IEnumerable<TableRow> GetRows(this Table table)
		{
			return table.Header?.TableRows.SafeConcat(table.Details?.TableRows)
				.SafeConcat(table.Footer?.TableRows)
				.SafeConcat(table.TableGroups?.SelectMany(g => g.Header?.TableRows.SafeConcat(g.Footer?.TableRows)));
		}

		private static IEnumerable<T> SafeConcat<T>(this IEnumerable<T> col1, IEnumerable<T> col2) =>
			 (col1 ?? Enumerable.Empty<T>()).Concat(col2 ?? Enumerable.Empty<T>());


		private static IEnumerable<IReportComponent> Flat(this IReportComponent component) =>
			component is IReportComponentContainer container
				? container.Components.SelectMany(x => x.Flat()).Append(container)
				: Enumerable.Repeat(component, 1);
	}
}