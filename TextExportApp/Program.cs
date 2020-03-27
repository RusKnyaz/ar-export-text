using System;
using System.Drawing;
using System.IO;
using System.Text;
using GrapeCity.ActiveReports;
using GrapeCity.ActiveReports.Rdl.Persistence;
using GrapeCity.ActiveReports.Rendering;
using TextExport;

namespace TextExportApp
{
    class Program
    {
        static void Main(string[] args)
        {
	        if (args.Length == 0)
	        {
		        var helpStringBuilder = new StringBuilder();
		        helpStringBuilder.AppendLine("Usage: TextExportApp.exe <ReportName> [options]");
		        helpStringBuilder.AppendLine(
			        "Options: -FontSize=W_H - Specifies font size for measurement in points, like 6_12");
		        Console.WriteLine(helpStringBuilder.ToString());
		        return;
	        }

	        var reportName = args[0];

	        var textSettings = new TxtSettings(){
		        FontSizePt = new Size(6, 30)
	        };

	        if (args.Length > 1)
	        {
		        try
		        {
			        var str = args[1].Split('=')[1].Split('_');
			        textSettings.FontSizePt.Width = int.Parse(str[0]);
			        textSettings.FontSizePt.Height = int.Parse(str[1]);
		        }
		        catch
		        {
			        Console.WriteLine("Invalid options");
			        return;
		        }
	        }

	        var reportDef = LoadReport(reportName);

            var iReport = reportDef.BuildReport();
            
            using(var writer = new StreamWriter(Console.OpenStandardOutput()))
            {
	            Console.SetOut(writer);
	            TextRendering.Render(iReport, writer, textSettings);
            }
        }

        static ReportStore LoadReport(string fileName)
        {
	        using(var file = File.OpenRead(fileName))
	        using (var reader = new StreamReader(file))
	        {
		        var report = PersistenceFilter.Load(reader).RemoveSpacing();
		        return new ReportStore(report, new DefaultResourceLocator());    
	        }
        }
    }
}
