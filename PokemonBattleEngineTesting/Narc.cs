using System;
using System.IO;

namespace Kermalis.PokemonBattleEngineTesting
{
    class NARC : IDisposable
    {
        public MemoryStream[] Files;

        public NARC(string path)
        {
            using (var br = new BinaryReader(File.OpenRead(path)))
            {
                br.BaseStream.Position = 0x18;
                uint numFiles = br.ReadUInt32();
                Files = new MemoryStream[numFiles];
                var startOffsets = new uint[numFiles];
                var endOffsets = new uint[numFiles];
                for (int i = 0; i < numFiles; i++)
                {
                    startOffsets[i] = br.ReadUInt32();
                    endOffsets[i] = br.ReadUInt32();
                }
                long BTNFOffset = br.BaseStream.Position;
                br.BaseStream.Position += 0x4;
                long GMIFOffset = br.ReadUInt32() + BTNFOffset;
                for (int i = 0; i < numFiles; i++)
                {
                    br.BaseStream.Position = GMIFOffset + startOffsets[i] + 0x8;
                    Files[i] = new MemoryStream(br.ReadBytes((int)(endOffsets[i] - startOffsets[i])));
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
