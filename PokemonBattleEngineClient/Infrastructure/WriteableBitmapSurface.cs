using Avalonia.Controls.Platform.Surfaces;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Kermalis.PokemonBattleEngineClient.Infrastructure
{
    class WriteableBitmapSurface : IFramebufferPlatformSurface
    {
        WriteableBitmap _bitmap;
        public WriteableBitmapSurface(WriteableBitmap bmp) => _bitmap = bmp;
        public ILockedFramebuffer Lock() => _bitmap.Lock();
    }
}
