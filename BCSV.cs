using System;
using System.Linq;
using System.Text;

namespace ACNH_Dumper
{
    public class BCSV
    {
        public const int MAGIC = 0x42435356; // BCSV

        public readonly byte[] Data;

        public uint EntryCount { get; set; }
        public uint EntryLength { get; set; }
        public ushort FieldCount { get; set; }
        public bool HasBCSVHeader { get; set; }
        public bool Flag2 { get; set; }

        public uint Magic { get; set; }
        public int Unknown { get; set; }
        public int Unknown1 { get; set; }
        public int Unknown2 { get; set; }

        private readonly int FieldTableStart;

        private readonly FieldParam[] FieldOffsets;
        public readonly Func<byte[], int, string>?[] FieldReaders;

        public BCSV(byte[] data)
        {
            Data = data;

            EntryCount = BitConverter.ToUInt32(data, 0x0);
            EntryLength = BitConverter.ToUInt32(data, 0x4);
            FieldCount = BitConverter.ToUInt16(data, 0x8);
            HasBCSVHeader = data[0xA] == 1;
            Flag2 = data[0xB] == 1;
            if (HasBCSVHeader)
            {
                Magic = BitConverter.ToUInt32(data, 0xC);
                if (Magic != MAGIC)
                    throw new ArgumentException(nameof(Magic));
                Unknown = BitConverter.ToInt32(data, 0x10);
                Unknown1 = BitConverter.ToInt32(data, 0x14);
                Unknown2 = BitConverter.ToInt32(data, 0x18);
                FieldTableStart = 0x1C;
            }
            else
            {
                FieldTableStart = 0x0C;
            }

            var fields = new FieldParam[FieldCount];
            for (int i = 0; i < fields.Length; i++)
            {
                var ofs = FieldTableStart + (i * 8);
                var ident = BitConverter.ToUInt32(data, ofs);
                var fo = BitConverter.ToInt32(data, ofs + 4);

                fields[i] = new FieldParam(ident, fo);
            }

            FieldOffsets = fields;
            FieldReaders = new Func<byte[], int, string>?[fields.Length];
        }

        private class FieldParam
        {
            public readonly uint Label;
            public readonly int Offset;

            public FieldParam(uint l, int o)
            {
                Label = l;
                Offset = o;
            }
        }

        private int GetFirstEntryOffset() => FieldTableStart + (FieldCount * 8);
        private int GetEntryOffset(int start, int i) => start + (i * (int)EntryLength);

        public string[] ReadCSV(string delim = "\t")
        {
            var result = new string[EntryCount + 1];
            result[0] = string.Join(delim, FieldOffsets.Select(z => $"0x{z.Label:X8}"));

            var start = GetFirstEntryOffset();
            for (int i = 0; i < EntryCount; i++)
            {
                var ofs = GetEntryOffset(start, i);
                string[] fields = new string[FieldCount];
                for (int f = 0; f < fields.Length; f++)
                {
                    var fo = ofs + FieldOffsets[f].Offset;
                    var reader = FieldReaders[f];
                    if (reader != null)
                        fields[f] = reader(Data, fo);
                    else
                        fields[f] = ReadFieldUnknownType(fo, f);
                }

                var line = string.Join(delim, fields);
                result[i + 1] = line;
            }

            return result;
        }

        private string ReadFieldUnknownType(in int fo, in int i)
        {
            var len = GetFieldLength(i);
            switch (len)
            {
                case 1: return Data[fo].ToString();
                case 2: return BitConverter.ToInt16(Data, fo).ToString();
                case 4: return "0x" + BitConverter.ToUInt32(Data, fo).ToString("X8");
                case 8: return "0x" + BitConverter.ToUInt64(Data, fo).ToString("X16");

                default: return Encoding.UTF8.GetString(Data, fo, len);
            }
        }

        private int GetFieldLength(in int i)
        {
            var next = (i + 1 == FieldCount) ? (int)(EntryLength) : FieldOffsets[i + 1].Offset;
            var ofs = FieldOffsets[i].Offset;
            return next - ofs;
        }

        private byte[] Slice(int ofs, int len)
        {
            var result = new byte[len];
            Array.Copy(Data, ofs, result, 0, len);
            return result;
        }
    }
}