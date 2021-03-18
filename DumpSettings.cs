namespace ACNH_Dumper
{
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
}
