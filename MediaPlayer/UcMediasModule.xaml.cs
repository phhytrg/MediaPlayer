using MediaPlayer.data;
using MediaPlayer.models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TagLib.Flac;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for UcMediasModule.xaml
    /// </summary>
    public partial class UcMediasModule : UserControl, INotifyPropertyChanged
    {
        private MainWindow mainWindow;
        public event PropertyChangedEventHandler? PropertyChanged;
        public MusicPlayerViewModel MusicPlayerViewModel { get; set; }
        public Playlist Playlist { get; set; }

        public UcMediasModule()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MusicPlayerViewModel = mainWindow.MusicPlayerViewModel;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            //playSound(Playlist.Medias[0].Fullpath);
            this.MusicPlayerViewModel.MediaIndex = 0;
            if (Playlist != this.MusicPlayerViewModel.CurrentPlaylist)
            {
                this.MusicPlayerViewModel.CurrentPlaylist = Playlist;
            }
            this.MusicPlayerViewModel.NotifyOnPlayButtonChanged();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new OpenFileDialog();
            screen.Filter = "All Supported File Types (*.mp3,*.wav,*.mpeg,*.wmv,*.avi)|*.mp3;*.wav;*.mpeg;*.wmv;*.avi|All files (*.*)|*.*";
            screen.Multiselect = true;

            if (screen.ShowDialog() == true)
            {
                screen.FileNames.ToList().ForEach(item =>
                {
                    var fullPath = item;
                    Playlist.Medias.Add(new Media(fullPath));

                    if (Playlist.Medias.Count <= 4)
                    {
                        Playlist.NotifyOnPlaylistChanged();
                    }
                });
            }
        }

        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            Point relativePoint = this.renameButton.TransformToAncestor(this)
              .Transform(new Point(0, 0));
            RenameDialog dialog = new RenameDialog(Playlist.Name ,relativePoint);
            dialog.NameChanged += screen_PlaylistNameChanged;
            dialog.Owner = mainWindow;
            var playlistName = Playlist.Name;

            if (dialog.ShowDialog() != true)
            {
                this.playlistName.Text = playlistName;
                return;
            }
            else
            {
                Playlist.Name = dialog.CurrentName;
            }
        }

        private void screen_PlaylistNameChanged(string playlistName)
        {
            this.playlistName.Text = playlistName;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            Point relativePoint = this.renameButton.TransformToAncestor(this)
              .Transform(new Point(0, 0));
            OKCancelDialog dialog = new OKCancelDialog(relativePoint);
            dialog.Owner = mainWindow;
            if(dialog.ShowDialog() == true)
            {
                mainWindow.NavigateUp();
                mainWindow.PlaylistViewModel.Playlists.Remove(Playlist);
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.GoBack()
            mainWindow.GbCurrentModule.Content = mainWindow.Previous;
            mainWindow.Previous = null;
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(this.MusicPlayerViewModel.CurrentPlaylist != Playlist)
            {
                this.MusicPlayerViewModel.CurrentPlaylist = Playlist;
            }

            int selectedIndex = this.tracksListView.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            this.MusicPlayerViewModel.MediaIndex = selectedIndex;
            //this.MusicPlayerViewModel.CurrentMedia = Playlist.Medias[selectedIndex];
            //this.MusicPlayerViewModel.MediaElement.MediaOpened += MediaElement_MediaOpened;
            //this.MusicPlayerViewModel.MediasQueue.Clear();
            //this.Playlist.Medias.Skip(selectedIndex).ToList().ForEach(item =>
            //{
            //    this.MusicPlayerViewModel.MediasQueue.Add(item);
            //});
            this.MusicPlayerViewModel.NotifyOnMediaElementChanged();
        }
        private void removeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            int indexSelected = this.tracksListView.SelectedIndex;

            if (indexSelected < 0) 
            { 
                return;
            }

            //Update media index
            if(indexSelected <= this.MusicPlayerViewModel.MediaIndex)
            {
                this.MusicPlayerViewModel.MediaIndex--;
            }

            this.Playlist.Medias.RemoveAt(indexSelected);
        }
    }
}
