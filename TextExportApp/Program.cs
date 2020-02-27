using System;
using System.IO;
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
		        Console.WriteLine("Please specify report file name.");
		        return;
	        }

	        var reportName = args[0];

            var iReport = LoadReport(reportName).BuildReport();
            
            using(var writer = new StreamWriter(Console.OpenStandardOutput()))
            {
	            Console.SetOut(writer);
	            TextRendering.Render(iReport, writer, new TxtSettings());
            }
        }

        static ReportStore LoadReport(string fileName)
        {
	        using(var file = File.OpenRead(fileName))
	        using (var reader = new StreamReader(file))
	        {
		        var report = PersistenceFilter.Load(reader);
		        return new ReportStore(report, new DefaultResourceLocator());    
	        }
        }
    }
}
