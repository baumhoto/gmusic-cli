namespace gmusic_cli {

    using System.Threading.Tasks;
    using RestEase;

    public interface IGoogleMusicProxyApi {

        [Get("get_discography_artist?format=text")]
        Task<string> GetDiscoGraphyArtist(string id);

        [Get("search_id?type=artist")]
        Task<string> GetIdForArtist(string artist);

        [Get("get_album")]
        Task<string> GetAlbum(string id);

        [Get("get_collection")]
        Task<string> GetCollection();

        [Get("get_all_stations?format=text")]
        Task<string> GetAllStations();

        [Get("get_all_playlists?format=text")]
        Task<string> GetAllPlaylists();

        [Get("get_playlist")]
        Task<string> GetPlaylist(string id);

        [Get("get_station")]
        Task<string> GetStation(string id);

        [Get("get_by_search?type=matches")]
        Task<string> SearchForArtist(string artist);
    }
}