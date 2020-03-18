using System;
using System.Collections.Generic;
using System.IO;

namespace ACNH_Dumper
{
    /// <summary>
    /// <see cref="SARC"/> File Access Table
    /// </summary>
    public class SFAT
    {
        public const string Identifier = nameof(SFAT);

        /// <summary>
        /// The required <see cref="Magic"/> matches the first 4 bytes of the file data.
        /// </summary>
        public bool SigMatches => Magic == Identifier;

        public string Magic;
        public ushort HeaderSize;
        public ushort EntryCount;
        public uint HashMult;
        public List<SFATEntry> Entries;

        public SFAT() { }

        public SFAT(BinaryReader br)
        {
            Magic = new string(br.ReadChars(4));
            if (!SigMatches)
                throw new FormatException(nameof(SFAT));

            HeaderSize = br.ReadUInt16();
            EntryCount = br.ReadUInt16();
            HashMult = br.ReadUInt32();
            Entries = new List<SFATEntry>();

            for (int i = 0; i < EntryCount; i++)
                Entries.Add(new SFATEntry(br));
        }
    }
}