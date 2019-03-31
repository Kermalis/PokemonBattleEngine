using Android.App;
using Android.Content.PM;
using Android.OS;
using System.IO;

namespace Kermalis.PokemonBattleEngineMobile.Droid
{
    [Activity(Label = "Pokémon Battle Engine", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(true);
            CopyDatabases();
            LoadApplication(new App());
        }

        void CopyDatabases()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            void CopyDatabaseIfNotExists(string dbName)
            {
                string dbPath = Path.Combine(path, dbName);
                if (!File.Exists(dbPath))
                {
                    using (var br = new BinaryReader(Application.Context.Assets.Open(dbName)))
                    {
                        using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
                        {
                            byte[] buffer = new byte[2048];
                            int length = 0;
                            while ((length = br.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                bw.Write(buffer, 0, length);
                            }
                        }
                    }
                }
            }
            CopyDatabaseIfNotExists("PokemonData.db");
        }
    }
}