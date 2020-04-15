using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.PageReportModel;
using GrapeCity.ActiveReports.Rdl.Persistence;
using GrapeCity.Enterprise.Data.DataEngine.Expressions;
using GrapeCity.Enterprise.Data.DataEngine.Extensions;

namespace TextExport
{
	public static class ReportExtensions
	{
		public static Report Clone(this Report report)
		{
			var ms = new MemoryStream();
			using (var xmlWriter = new XmlTextWriter(ms, Encoding.UTF8))
			{
				new PersistenceFilter(ReportComponentFactory.Instance, report, new DefaultResourceLocator()).SerializeRoot(report, xmlWriter);	
			}

			return PersistenceFilter.Load(new StreamReader(new MemoryStream(ms.ToArray())));
		}
		
		
		/// <summary>Removes paddings and rows heights.</summary>
		/// <param name="report">The source report</param>
		/// <returns>The report without spacing.</returns>
		public static Report RemoveSpacing(this Report report, TxtSettings settings)
		{
			var result = report;

			var lineHeight = $"{settings.FontSizePt.Height}pt";

			foreach(var reportComponent in report.Flat())
			{
				switch (reportComponent)
				{
					case TextBox textBox:
						textBox.CanGrow = true;
						
						var style = textBox.Style;
						style.PaddingBottom = "0pt";
						style.PaddingTop = "0pt";
						style.ShrinkToFit = "false";
						style.MinCondenseRate = "100";
						
						switch (settings.HorizontalPaddings)
						{
							case PaddingsType.Remove:
								style.PaddingLeft = "0pt";
								style.PaddingRight = "0pt";
								break;
							case PaddingsType.Adjust:
								style.PaddingLeft = Adjust(style.PaddingLeft, settings.FontSizePt.Width);
								style.PaddingRight = Adjust(style.PaddingRight, settings.FontSizePt.Width);
								break;
						}
						
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

		private static ExpressionInfo Adjust(ExpressionInfo expr, int width) =>
			expr.IsConstant
				? (ExpressionInfo)$"{((int) (new Length(expr).ToPoints() / width)) * width}pt"
				: expr;


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