using System;
using System.IO;

namespace gmusic_cli
{
    class Program
    {
        static string playListSavePath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Music/playlists");
        static string baseUrl = "http://localhost:9999/";

        static void Main(string[] args)
        {
            var gmusicConsole = new GMusicProxyConsole(baseUrl, playListSavePath);
            gmusicConsole.MainSelection();
        }
    }
}
