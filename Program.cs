using System;
using System.IO;

namespace ACNH_Dumper
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            const string path = @"E:\ac\rom2"; // replace me, should include "rom" in the final folder name
            var settings = new DumpSettings(path);

            if (settings.DumpBCSV)
                ACNHExtractor.ExtractBCSVToFolder(path, settings.PathBCSV);

            if (settings.DumpZS)
                ACNHExtractor.ExtractZSToFolder(path, settings.PathZS);

            if (settings.DumpSARC)
                ACNHExtractor.ExtractSARCToFolder(settings.PathZS, settings.PathSARC);

            Console.WriteLine("Done!");
        }
    }

    public class DumpSettings
    {
        public string Path;
        public string PathZS;
        public string PathSARC;
        public string PathBCSV;

        public DumpSettings(string path)
        {
            Path = path;
            PathZS = path.Replace("rom", "romZS");
            PathSARC = path.Replace("rom", "romSARC");
            PathBCSV = path.Replace("rom", "romBCSV");
        }

        public bool DumpZS { get; set; } = true;
        public bool DumpSARC { get; set; } = true;
        public bool DumpBCSV { get; set; } = true;
    }

    public static class ACNHExtractor
    {
        public static void ExtractZSToFolder(string path, string dest)
        {
            Console.WriteLine("Dumping zs files...");
            var files = Directory.EnumerateFiles(path, "*.zs", SearchOption.AllDirectories);
            using var zs = new ZstdNet.Decompressor();
            foreach (var f in files)
            {
                var data = File.ReadAllBytes(f);
                var result = zs.Unwrap(data);

                var rpath = f.Replace(path, dest);
                var dir = Path.GetDirectoryName(rpath);
                Directory.CreateDirectory(dir);
                File.WriteAllBytes(rpath, result);
                Console.WriteLine(rpath);
            }
        }

        public static void ExtractSARCToFolder(string path, string dest)
        {
            Console.WriteLine("Dumping SARC files...");
            var files = Directory.EnumerateFiles(path, "*.zs", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                try
                {
                    var sarc = new SARC(f);
                    var rpath = f.Replace(path, dest);
                    var dir = Path.GetDirectoryName(rpath);
                    Directory.CreateDirectory(dir);
                    Console.WriteLine($"New SARC with {sarc.SFAT.EntryCount} files.");
                    foreach (var z in sarc.Dump(rpath))
                    {
                        Console.WriteLine(z);
                    }
                }
                catch
                {
                }
            }
        }

        public static void ExtractBCSVToFolder(string path, string dest)
        {
            Console.WriteLine("Dumping BCSV files...");
            var files = Directory.EnumerateFiles(path, "*.bcsv", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                var data = File.ReadAllBytes(f);
                var bcsv = new BCSV(data);
                var csv = bcsv.ReadCSV();

                var rpath = f.Replace(path, dest).Replace(".bcsv", ".csv");
                var dir = Path.GetDirectoryName(rpath);
                Directory.CreateDirectory(dir);
                File.WriteAllLines(rpath, csv);
            }
        }
    }
}
