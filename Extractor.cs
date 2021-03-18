using System;
using System.IO;
using ZstdNet;

namespace ACNH_Dumper
{
    public static class Extractor
    {
        public static void ExtractZSToFolder(string path, string dest)
        {
            Console.WriteLine("Dumping zs files...");
            var files = Directory.EnumerateFiles(path, "*.zs", SearchOption.AllDirectories);
            using var zs = new Decompressor();
            foreach (var f in files)
            {
                var data = File.ReadAllBytes(f);
                var result = zs.Unwrap(data);

                var rpath = Path.ChangeExtension(f.Replace(path, dest), ".sarc").Replace(".sarc.sarc", ".sarc");
                var dir = Path.GetDirectoryName(rpath);
                if (dir == null)
                    throw new Exception("Bad directory?");
                Directory.CreateDirectory(dir);
                File.WriteAllBytes(rpath, result);
                Console.WriteLine(rpath);
            }
        }

        public static void ExtractSARCToFolder(string path, string dest)
        {
            Console.WriteLine("Dumping SARC files...");
            var files = Directory.EnumerateFiles(path, "*.sarc", SearchOption.AllDirectories);
            foreach (var f in files)
            {
                try
                {
                    var sarc = new SARC(f);
                    var rpath = f.Replace(path, dest);
                    var dir = Path.GetDirectoryName(rpath);
                    if (dir == null)
                        throw new Exception("Bad directory?");
                    Directory.CreateDirectory(dir);
                    Console.WriteLine($"New SARC with {sarc.SFAT.EntryCount} files.");
                    foreach (var z in sarc.Dump(rpath))
                    {
                        Console.WriteLine(z);
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
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

                var rpath = Path.ChangeExtension(f.Replace(path, dest), ".csv");
                var dir = Path.GetDirectoryName(rpath);
                if (dir == null)
                    throw new Exception("Bad directory?");
                Directory.CreateDirectory(dir);
                File.WriteAllLines(rpath, csv);
            }
        }
    }
}
