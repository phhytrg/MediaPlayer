using MediaPlayer.data;
using MediaPlayer.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for UcCurrentPlaylist.xaml
    /// </summary>
    public partial class UcCurrentPlaylist : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        MainWindow? main;
        private MusicPlayerViewModel _musicPlayerViewModel;
        public Playlist Playlist { get; set; }
        public UcCurrentPlaylist(Playlist playlist)
        {
            InitializeComponent();
            this.Playlist = playlist;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            main = (MainWindow)Application.Current.MainWindow;
            _musicPlayerViewModel = main.MusicPlayerViewModel;
            //this.currentPlaylistListView.ItemsSource = this.Playlist.Medias;
        }
        private void navigateUpButton_Click(object sender, RoutedEventArgs e)
        {
            main!.NavigateUp();
        }
        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this._musicPlayerViewModel.CurrentPlaylist != Playlist)
            {
                this._musicPlayerViewModel.CurrentPlaylist = Playlist;
            }

            int selectedIndex = this.currentPlaylistListView.SelectedIndex;

            if (selectedIndex < 0)
            {
                return;
            }

            this._musicPlayerViewModel.MediaIndex = selectedIndex;
            this._musicPlayerViewModel.NotifyOnMediaElementChanged();
        }
    }
}
