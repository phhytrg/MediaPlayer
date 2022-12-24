using MediaPlayer.data;
using Newtonsoft.Json;
using System;
using System.Windows;
using File = System.IO.File;

namespace MediaPlayer
{
    public class Constants
    {
        public const int NON_REPLAYING = 0;
        public const int REPLAY_PLAYLIST = 1;
        public const int REPLAY_SINGLE = 2;
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private PlaylistViewModel _playlistViewModel = new PlaylistViewModel();
        private MusicPlayerViewModel _musicPlayerViewModel = new MusicPlayerViewModel();
        private string _baseDomain = AppDomain.CurrentDomain.BaseDirectory;
        private string _playlistDataFile = "playlists.json";
        private string _musicPlayerDataFile = "music_player.json";
        private string _slash = "/";
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();

            if (File.Exists(_baseDomain + _slash + _playlistDataFile))
            {
                var json = File.ReadAllText(_baseDomain + _slash + _playlistDataFile);
                if (json != null || json != "")
                {
                    _playlistViewModel = JsonConvert.DeserializeObject<PlaylistViewModel>(json!)!;
                }
            }
            window.PlaylistViewModel = _playlistViewModel;

            if (File.Exists(_baseDomain + _slash + _musicPlayerDataFile))
            {
                var json = File.ReadAllText(_baseDomain + _slash + _musicPlayerDataFile);
                if (json != null || json != "")
                {
                    _musicPlayerViewModel = JsonConvert.DeserializeObject<MusicPlayerViewModel>(json!)!;
                }
            }
            window.MusicPlayerViewModel = _musicPlayerViewModel;


            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _musicPlayerViewModel.MediaElement.Pause();
            base.OnExit(e);
            var json = JsonConvert.SerializeObject(_playlistViewModel).ToString();
            File.WriteAllText(_baseDomain + _slash + _playlistDataFile, json);
            json = JsonConvert.SerializeObject(_musicPlayerViewModel).ToString();
            File.WriteAllText(_baseDomain + _slash + _musicPlayerDataFile, json);
            var save = new MediaElementSave();
            if(_musicPlayerViewModel.CurrentMedia == null)
            {
                return;
            }
            save.Path = _musicPlayerViewModel.CurrentMedia.Fullpath;
            save.Volume = _musicPlayerViewModel.MediaElement.Volume;
            save.Position = _musicPlayerViewModel.MediaElement.Position;
            json = JsonConvert.SerializeObject(save).ToString();
            File.WriteAllText(_baseDomain + _slash + "media_element.json", json);
        }

    }

    public class MediaElementSave
    {
        public string Path { get; set; }
        public double Volume { get; set; }
        public TimeSpan Position { get; set; }

    }
}
