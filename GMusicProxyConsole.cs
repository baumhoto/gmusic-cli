using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

namespace gmusic_cli {

    public class GMusicProxyConsole {

        private string baseUrl;

        private string playListSaveDir;

        private IGoogleMusicProxyApi api;

        public GMusicProxyConsole(string baseUrl, string playListSaveDir)
        {
           this.baseUrl = baseUrl;
           this.playListSaveDir = playListSaveDir;
           this.api = RestEase.RestClient.For<IGoogleMusicProxyApi>(baseUrl);
        }
        public void MainSelection()
        {
            var options = new List<string>();
            options.Add("Get User Playlist");
            options.Add("Get User Station");
            options.Add("Search Artist");

            var selection = ShowSelection(options);

            switch(selection)
            {
                case "0" : GetUserPlaylist(); break;
                case "1" : GetUserStation(); break;
                case "2" : SearchArtist(); break;
            }
        }

        private void SearchArtist()
        {
            string artistId = string.Empty;
            string selection = string.Empty;
            string artistName = string.Empty;

            do
            {
                selection = ShowSelection(new List<string>() { "Please enter Artist Name:"});
                var task = api.GetIdForArtist(selection);

                if(string.IsNullOrEmpty(task.Result))
                {
                    Console.WriteLine($"No artist found for {selection}. Please try again.");
                }
                else
                {
                    artistId = task.Result;
                    artistName = selection;
                }
            }
            while(string.IsNullOrEmpty(artistId));

            var options = new List<string>();
            options.Add("Get Discography");
            options.Add("Get Top Songs");

            selection = ShowSelection(options);

            switch(selection)
            {
                case "0" : GetArtistDiscography(artistId); break;
                case "1" : GetTopSongs(artistName, artistId); break;
            }
        }

        private void GetTopSongs(string artistName, string artistId)
        {
            var topSongs = api.GetTopSongsForArtist(artistId);

            SaveAsM3u($"Top_Songs_{artistName}", topSongs.Result);
        }

        private void GetArtistDiscography(string artistId)
        {
            var discoGraphy = api.GetDiscoGraphyArtist(artistId);

            var entries = ConvertResponseToTuple(discoGraphy.Result, 1);

            var selection = ShowSelection(entries.Select(t => t[0]).ToList());

            var selectedItem = entries[int.Parse(selection)];

            DownloadAlbum(selectedItem[0], selectedItem[2].Split('=')[1]);
        }

        private void GetUserStation()
        {
            var stations = api.GetAllStations();
            var entries = ConvertResponseToTuple(stations.Result, 0);
            var selection = ShowSelection(entries.Select(t => t[0]).ToList());

            var selectedItem  = entries[int.Parse(selection)];

            DownloadStation(selectedItem[0], selectedItem[1].Split('=')[1]);
        }

        private void GetUserPlaylist()
        {
           var playLists = api.GetAllPlaylists(); 
           var entries = ConvertResponseToTuple(playLists.Result, 0);
           var selection = ShowSelection(entries.Select(t => t[0]).ToList());

           var selectedItem  = entries[int.Parse(selection)];

           DownloadPlaylist(selectedItem[0], selectedItem[1].Split('=')[1]);
        }

        private void DownloadPlaylist(string playlistName, string playlistId)
        {
            var playList = api.GetPlaylist(playlistId);
            SaveAsM3u(playlistName, playList.Result);
        }
        
        private void DownloadStation(string stationName, string stationId)
        {
            var station = api.GetStation(stationId);
            SaveAsM3u(stationName, station.Result);
        }

        private void DownloadAlbum(string albumName, string albumId)
        {
            var album = api.GetAlbum(albumId);
            SaveAsM3u(albumName, album.Result);
        }

        public static string ShowSelection(List<string> options)
        {
            if(options.Count > 1)
            {
                for(int i = 0; i < options.Count; i++)
                {
                    Console.WriteLine($"[{i}] - {options[i]}");
                }
            }
            else
            {
                Console.WriteLine(options[0]);
            }

            var input = Console.ReadLine();

            if(string.IsNullOrEmpty(input))
            {
                Environment.Exit(0);
            }

            return input;
        }

        public static List<string[]> ConvertResponseToTuple(string responseData, int orderByDescColumn)
        {
            var result = responseData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Split('|')).OrderByDescending(x => x[orderByDescColumn]).ToList();

            return result; 
        }

        public void SaveAsM3u(string playlistName, string data)
        {
            if(string.IsNullOrEmpty(data))
            {
                Console.WriteLine("Data is empty. Playlist not saved.");
                return;
            }

            playlistName = playlistName.Replace(" ", "_").Replace(".", string.Empty);
            var fileName = Path.Combine(playListSaveDir, playlistName + ".m3u");
            File.WriteAllLines(fileName,
                    data.Split(new string[] { Environment.NewLine }, 
                        StringSplitOptions.RemoveEmptyEntries));

            Console.WriteLine($"Written playlist {fileName}");
        }
    }
}