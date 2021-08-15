using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DirCastWebServer.Services
{
    public static class InitializationService
    {
        /// <summary>
        /// root browsing directory
        /// </summary>
        public static string Dir { get; private set; }
        /// <summary>
        /// Listening Url. ex: http://localhost:5000
        /// </summary>
        public static string Url { get; private set; }

        public static void Initialize()
        {
            var settings = GetSettings();

            if (Dir.IsNullOrWhiteSpace() && settings?.Dir?.IsNullOrWhiteSpace() == false && Ask($"Use {settings.Dir} for root browing directory?"))
                Dir = settings.Dir;

            while (Dir.IsNullOrWhiteSpace())
            {
                Console.WriteLine("Enter root browing directory path");
                var dir = Console.ReadLine();

                if (!Directory.Exists(dir))
                {
                    Console.WriteLine("Directory does not exist");
                }
                else
                {
                    Dir = dir;
                }
            }

            if (Url.IsNullOrWhiteSpace() && settings?.Url?.IsNullOrWhiteSpace() == false && Ask($"Use {settings.Url} as web server url?"))
                Url = settings.Url;

            while (Url.IsNullOrWhiteSpace())
            {
                Console.WriteLine("Enter web server url without scheme (ex: 192.168.1.2:5000)");

                var url = "http://" + Console.ReadLine();

                if (Uri.TryCreate(url, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp)
                {
                    Url = url;
                }
                else
                {
                    Console.WriteLine($"{url} is not valid http url");
                }
            }

            SetSettings(new InitializationSettings(Url, Dir));

            static bool Ask(string question)
            {
#if DEBUG
                return true;
#else
                Console.WriteLine(question);
                while (true)
                {
                    Console.Write("y/n: ");
                    var r = Console.ReadKey().KeyChar;
                    Console.WriteLine();
                    if (r == 'y')
                        return true;
                    if (r == 'n')
                        return false;
                }
#endif
            }
        }


        
        record InitializationSettings(string Url, string Dir);
        static string SettingsFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "initializationSettings.json");

        static InitializationSettings GetSettings()
        {
            InitializationSettings settings = null;

            if (File.Exists(SettingsFilePath))
            {
                var contentJson = File.ReadAllText(SettingsFilePath);
                try
                {
                    settings = System.Text.Json.JsonSerializer.Deserialize<InitializationSettings>(contentJson);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
            

            return settings;
        }

        static void SetSettings(InitializationSettings settings)
        {
            File.WriteAllText(SettingsFilePath, System.Text.Json.JsonSerializer.Serialize(settings));
        }
    }
}
