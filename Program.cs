using System;

namespace ACNH_Dumper
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string defaultPath = @"D:\Kurt\Desktop\v19"; // replace me? else just drop your folder on the exe
            var path = args.Length > 1 ? args[1] : defaultPath;
            var settings = new DumpSettings(path);

            if (settings.DumpBCSV)
                Extractor.ExtractBCSVToFolder(path, settings.PathBCSV);

            if (settings.DumpZS)
                Extractor.ExtractZSToFolder(path, settings.PathZS);

            if (settings.DumpSARC)
                Extractor.ExtractSARCToFolder(settings.PathZS, settings.PathSARC);

            Console.WriteLine("Done!");
        }
    }
}
