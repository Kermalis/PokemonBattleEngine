using Kermalis.EndianBinaryIO;
using System;
using System.IO;

namespace Kermalis.PokemonBattleEngineTesting
{
    class NARC : IDisposable
    {
        public MemoryStream[] Files;

        public NARC(string path)
        {
            using (var br = new EndianBinaryReader(File.OpenRead(path), Endianness.LittleEndian))
            {
                uint numFiles = br.ReadUInt32(0x18);
                Files = new MemoryStream[numFiles];
                var startOffsets = new uint[numFiles];
                var endOffsets = new uint[numFiles];
                for (uint i = 0; i < numFiles; i++)
                {
                    startOffsets[i] = br.ReadUInt32();
                    endOffsets[i] = br.ReadUInt32();
                }
                long BTNFOffset = br.BaseStream.Position;
                long GMIFOffset = br.ReadUInt32(BTNFOffset + 0x4) + BTNFOffset;
                for (uint i = 0; i < numFiles; i++)
                {
                    Files[i] = new MemoryStream(br.ReadBytes((int)(endOffsets[i] - startOffsets[i]), GMIFOffset + startOffsets[i] + 0x8));
                }
            }
        }
        public void Dispose()
        {
            if (Files != null)
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    Files[i].Dispose();
                }
            }
            Files = null;
        }

        public void SaveFilesToFolder(string path)
        {
            Directory.CreateDirectory(path);
            for (int i = 0; i < Files.Length; i++)
            {
                using (var fs = new FileStream(path + Path.DirectorySeparatorChar + i, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    Files[i].WriteTo(fs);
                }
            }
        }
    }
}
