using System.IO;

namespace ACNH_Dumper
{
    /// <summary>
    /// <see cref="SARC"/> File Access Table (<see cref="SFAT"/>) Entry
    /// </summary>
    public class SFATEntry
    {
        public uint FileNameHash;
        public int FileNameOffset;
        public int FileDataStart;
        public int FileDataEnd;

        public int FileDataLength => FileDataEnd - FileDataStart;

        public SFATEntry(BinaryReader br)
        {
            FileNameHash = br.ReadUInt32();
            FileNameOffset = br.ReadInt32();
            FileDataStart = br.ReadInt32();
            FileDataEnd = br.ReadInt32();
        }
    }
}