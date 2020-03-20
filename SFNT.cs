using System;
using System.IO;

namespace ACNH_Dumper
{
    /// <summary>
    /// <see cref="SARC"/> File Name Table
    /// </summary>
    public class SFNT
    {
        public const string Identifier = nameof(SFNT);

        /// <summary>
        /// The required <see cref="Magic"/> matches the first 4 bytes of the file data.
        /// </summary>
        public bool SigMatches => Magic == Identifier;

        public string Magic;
        public ushort HeaderSize;
        public ushort Unknown;
        public uint StringOffset;

        public SFNT(BinaryReader br)
        {
            Magic = new string(br.ReadChars(4));
            if (!SigMatches)
                throw new FormatException(nameof(SFNT));

            HeaderSize = br.ReadUInt16();
            Unknown = br.ReadUInt16();
            StringOffset = (uint)br.BaseStream.Position;
        }
    }
}