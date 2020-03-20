using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ACNH_Dumper
{
    /// <summary>
    /// Simple (?) ARChive
    /// </summary>
    public sealed class SARC : IDisposable
    {
        private const string Identifier = nameof(SARC);

        public string Magic;
        public ushort HeaderSize;
        public ushort Endianness;
        public uint FileSize;
        public uint DataOffset;
        public uint Unknown;

        public SFAT SFAT;
        public SFNT SFNT;

        // Assigned Properties
        public string FileName;
        public string FilePath;
        public string Extension;
        public readonly bool Valid;

        /// <summary>
        /// The required <see cref="Magic"/> matches the first 4 bytes of the file data.
        /// </summary>
        public bool SigMatches => Magic == Identifier;
        private readonly Stream stream;
        private readonly BinaryReader br;

        /// <summary>
        /// Initializes a <see cref="SARC"/> from a file location.
        /// </summary>
        /// <param name="path"></param>
        public SARC(string path)
        {
            FileName = Path.GetFileNameWithoutExtension(path);
            FilePath = Path.GetDirectoryName(path) ?? string.Empty;
            Extension = Path.GetExtension(path);

            stream = File.OpenRead(path);

            br = new BinaryReader(stream);
            Magic = new string(br.ReadChars(4));
            HeaderSize = br.ReadUInt16();
            Endianness = br.ReadUInt16();
            FileSize = br.ReadUInt32();
            DataOffset = br.ReadUInt32();
            Unknown = br.ReadUInt32();

            SFAT = new SFAT(br);
            SFNT = new SFNT(br);
            Valid = true;
        }

        /// <summary>
        /// Gets the entry filename for a given <see cref="SFATEntry"/>.
        /// </summary>
        /// <param name="entry">Entry to fetch data for</param>
        /// <returns>File Name</returns>
        public string GetFileName(SFATEntry entry) => GetFileName(entry.FileNameOffset);

        /// <summary>
        /// Gets the entry data for a given <see cref="SFATEntry"/>,
        /// </summary>
        /// <param name="entry">Entry to fetch data for</param>
        /// <returns>Data array</returns>
        public byte[] GetData(SFATEntry entry) => GetData(entry.FileDataStart, entry.FileDataLength);

        /// <summary>
        /// Exports the entry data for a given <see cref="SFATEntry"/> at a provided path with its assigned <see cref="SFATEntry"/> file name via the <see cref="SFNT"/> name table.
        /// </summary>
        /// <param name="t">Entry to export</param>
        /// <param name="outpath">Path to export to. If left null, will output to the <see cref="SARC"/> FilePath, if it is assigned.</param>
        public string ExportFile(SFATEntry t, string? outpath = null)
        {
            outpath ??= FilePath;
            if (outpath == null)
                throw new ArgumentNullException(nameof(outpath));
            byte[] data = GetData(t);
            string name = GetFileName(t);

            var dir = Path.GetDirectoryName(name);
            if (dir == null)
                throw new ArgumentException(name);
            string location = Path.Combine(outpath, dir);
            Directory.CreateDirectory(location);

            var filepath = Path.Combine(outpath, name);
            File.WriteAllBytes(filepath, data);
            return filepath;
        }

        /// <summary>
        /// Dumps the contents of the <see cref="SARC"/> to a provided folder. If no location is provided, it will dump to the SARC's location.
        /// </summary>
        /// <param name="path">Path to create dump folder in</param>
        /// <param name="folder">Folder to dump contents to</param>
        public IEnumerable<string> Dump(string? path = null, string? folder = null)
        {
            path ??= FilePath;
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (File.Exists(path))
                path = Path.GetDirectoryName(path);
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            folder ??= FileName ?? "sarc";
            string dir = Path.Combine(path, folder);

            Directory.CreateDirectory(dir);
            foreach (SFATEntry t in SFAT.Entries)
                yield return ExportFile(t, dir);
        }

        private string GetFileName(int offset)
        {
            stream.Seek(SFNT.StringOffset, SeekOrigin.Begin);
            stream.Seek((offset & 0x00FFFFFF) * 4, SeekOrigin.Current);
            StringBuilder sb = new StringBuilder();
            for (char c = (char)stream.ReadByte(); c != 0; c = (char)stream.ReadByte())
                sb.Append(c);

            string name = sb.ToString().Replace('/', Path.DirectorySeparatorChar);
            return name;
        }

        private byte[] GetData(int offset, int length)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            byte[] fileBuffer = new byte[length];
            stream.Seek(offset + DataOffset, SeekOrigin.Begin);
            stream.Read(fileBuffer, 0, length);
            return fileBuffer;
        }

        /// <summary>
        /// Disposes of the <see cref="stream"/> and <see cref="br"/> objects and frees the <see cref="FileName"/> if originally loaded from that location.
        /// </summary>
        public void Dispose()
        {
            stream?.Dispose();
            br?.Dispose();
        }
    }
}