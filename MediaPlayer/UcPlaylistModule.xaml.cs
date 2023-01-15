using MediaPlayer.data;
using MediaPlayer.models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Id3v2;
using Path = System.IO.Path;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for UcPlaylistModule.xaml
    /// </summary>
    public partial class UcPlaylistModule : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        MainWindow mainWindow;
        public PlaylistViewModel playlistViewModel { get; set; }
        MusicPlayerViewModel musicPlayerViewModel;

        public UcPlaylistModule()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            playlistViewModel = (PlaylistViewModel)mainWindow.PlaylistViewModel;
            musicPlayerViewModel = (MusicPlayerViewModel)mainWindow.MusicPlayerViewModel;
        }



        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void addMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = playlistsListView.SelectedIndex;

            var screen = new OpenFileDialog();
            screen.Filter = "All Supported File Types (*.mp3,*.wav,*.mpeg,*.wmv,*.avi,*.m4a,*.flac)" +
                "|*.mp3;*.wav;*.mpeg;*.wmv;*.avi;.m4a,*.flac|All files (*.*)|*.*";
            screen.Multiselect = true;

            if (screen.ShowDialog() == true)
            {
                screen.FileNames.ToList().ForEach(item =>
                {
                    var fullPath = item;
                    playlistViewModel.Playlists[selectedIndex].Medias.Add(new Media(fullPath));
                    if (playlistViewModel.Playlists[selectedIndex].Medias.Count <= 4)
                    {
                        playlistViewModel.Playlists[selectedIndex].NotifyOnPlaylistChanged();
                    }
                });
            }
        }

        private void addPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            Point relativePoint = this.addPlaylistButton.TransformToAncestor(this)
                          .Transform(new Point(0, 0));
            CreatePlaylistDialog dialog = new CreatePlaylistDialog(relativePoint);
            dialog.Owner = mainWindow;

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            playlistViewModel.Playlists.Add(dialog.newPlaylist!);
        }

        private void deleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = playlistsListView.SelectedIndex;
            if (selectedIndex < 0)
            {
                return;
            }
            Point relativePoint = Mouse.GetPosition(Application.Current.MainWindow);
            OKCancelDialog dialog = new OKCancelDialog(relativePoint);
            dialog.Owner = mainWindow;
            if (dialog.ShowDialog() == true)
            {
                mainWindow.PlaylistViewModel.Playlists.Remove(playlistViewModel.Playlists[selectedIndex]);
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if(item != null && item.IsSelected)
            {
                int selectedIndex = playlistsListView.SelectedIndex;
                if (selectedIndex < 0)
                {
                    return;
                }
                mainWindow.Previous = (UserControl)mainWindow.GbCurrentModule.Content;
                UcMediasModule ucMediasModule = (UcMediasModule)mainWindow.userControls["UcMediasModule"];
                ucMediasModule.Playlist = playlistViewModel.Playlists[selectedIndex];
                mainWindow.GbCurrentModule.Content = ucMediasModule;
            }
        }

        #region Drag and drop items
        private void ListViewItem_Drop(object sender, DragEventArgs e)
        {
            var item = ((ListViewItem)sender);

            var playlist = (Playlist)item.DataContext;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length <= 0)
                {
                    return;
                }

                List<string> supportTypes = new List<string>() { ".mp3",".wav", ".mpeg", ".wmv", ".avi", ".m4a", ".flac" };

                foreach (string file in files)
                {
                    if (supportTypes.Contains( Path.GetExtension(file)))
                    {
                        playlist.Medias.Add(new Media(file));
                        if (playlist.Medias.Count <= 4)
                        {
                            playlist.NotifyOnPlaylistChanged();
                        }
                    }
                }
            }
        }
        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }
        #endregion

    }
}
