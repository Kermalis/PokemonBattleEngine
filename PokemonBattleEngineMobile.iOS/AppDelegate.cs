using Foundation;
using System;
using System.IO;
using UIKit;

namespace Kermalis.PokemonBattleEngineMobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            CopyDatabases();
            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }

        void CopyDatabases()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "Data");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            void CopyIfNotExists(string dbName)
            {
                string dbPath = Path.Combine(path, $"{dbName}.db");
                if (!File.Exists(dbPath))
                {
                    File.Copy(NSBundle.MainBundle.PathForResource(dbName, "db"), dbPath);
                }
            }
            CopyIfNotExists("PokemonData");
        }
    }
}
