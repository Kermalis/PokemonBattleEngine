using Avalonia;
using Avalonia.iOS;
using Avalonia.Media;
using Foundation;
using Kermalis.PokemonBattleEngineClient.Infrastructure;
using System;
using System.IO;
using UIKit;

namespace Kermalis.PokemonBattleEngineClient.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        public override UIWindow Window { get; set; }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication uiapp, NSDictionary options)
        {
            CopyDatabase();
            AppBuilder.Configure<App>()
                .UseiOS()
                //.UseSkia()
                .SetupWithoutStarting();
            Window = new AvaloniaWindow() { Content = new MainView(), StatusBarColor = Colors.LightSteelBlue };
            Window.MakeKeyAndVisible();
            return true;
        }

        private void CopyDatabase()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
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