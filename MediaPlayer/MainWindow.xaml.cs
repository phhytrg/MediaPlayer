using MediaPlayer.data;
using MediaPlayer.Keys;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public PlaylistViewModel PlaylistViewModel { get; set; }
        public IDictionary<string, UserControl> userControls { get; } = new Dictionary<string, UserControl>();
        public UserControl? Previous { get; set; }
        public MusicPlayerViewModel MusicPlayerViewModel { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            HotkeysManager.SetupSystemHook();
            GlobalHotkey playStopHotkey = new GlobalHotkey(ModifierKeys.None, Key.Space,
                () => this.MusicPlayerViewModel!.NotifyOnPlayButtonChanged(),
                true
                );
            HotkeysManager.AddHotkey(playStopHotkey);

            GlobalHotkey skipNext3Second = new GlobalHotkey(ModifierKeys.None, Key.Right,
                () =>
                {
                    IMediaControl mediaControl = (IMediaControl)userControls["MediaService"];
                    mediaControl.SkipNext3Secs();
                }, true);

            HotkeysManager.AddHotkey(skipNext3Second);

            GlobalHotkey skipBack3Second = new GlobalHotkey(ModifierKeys.None, Key.Left,
                () =>
                {
                    IMediaControl mediaControl = (IMediaControl)userControls["MediaService"];
                    mediaControl.SkipBack3Secs();
                }, true);

            HotkeysManager.AddHotkey(skipBack3Second);

            GlobalHotkey skipNextSong = new GlobalHotkey(ModifierKeys.Control, Key.Right,
                () =>
                {
                    IMediaControl mediaControl = (IMediaControl)userControls["MediaService"];
                    mediaControl.SkipNextSong();
                });

            HotkeysManager.AddHotkey(skipNextSong);

            GlobalHotkey skipPreviousSong = new GlobalHotkey(ModifierKeys.Control, Key.Left,
                () =>
                {
                    IMediaControl mediaControl = (IMediaControl)userControls["MediaService"];
                    mediaControl.SkipPreviousSong();
                });

            HotkeysManager.AddHotkey(skipPreviousSong);

            Closing += MainWindow_Closing1;
        }

        private void MainWindow_Closing1(object? sender, CancelEventArgs e)
        {
            // Avoid memory leak
            HotkeysManager.ShutdownSystemHook();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //playlistsListView.ItemsSource = _playlists;
            userControls.Add("UcPlaylistModule", new UcPlaylistModule());
            userControls.Add("UcMediasModule", new UcMediasModule());
            userControls.Add("MediaService", new MediaService());
            userControls.Add("UcRecentlyPlayed", new UcRecentlyPlayed());
            GbCurrentModule.Content = userControls["UcPlaylistModule"];
            MediaService.Content = new MediaService();
        }
        public void NavigateRecentlyPlayedList()
        {
            Previous = (UserControl?)GbCurrentModule.Content;
            GbCurrentModule.Content = userControls["UcRecentlyPlayed"];
        }
        public void NavigateUp()
        {
            GbCurrentModule.Content = Previous;
        }

    }
}
