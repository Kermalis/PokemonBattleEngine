using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System.IO;

namespace Kermalis.PokemonBattleEngineClient.Android
{
    [Activity(Label = "Pokémon Battle Engine", Icon = "@drawable/icon", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AvaloniaActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            if (Avalonia.Application.Current == null)
            {
                CopyDatabase();
                AppBuilder.Configure(new App())
                    .UseAndroid()
                    .SetupWithoutStarting();
                Content = new MainView();
            }
            base.OnCreate(savedInstanceState);
        }

        private void CopyDatabase()
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            const string dbName = "PokemonBattleEngine.db";
            using (var br = new BinaryReader(Utils.GetResourceStream(dbName)))
            {
                using (var bw = new BinaryWriter(new FileStream(Path.Combine(path, dbName), FileMode.Create)))
                {
                    byte[] buffer = new byte[0x800];
                    int length = 0;
                    while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, length);
                    }
                }
            }
            Utils.ForwardCreateDatabaseConnection(path);
        }
    }
}

