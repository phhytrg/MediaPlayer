using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using MediaPlayer.models;

namespace MediaPlayer.data
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Playlist> Playlists { get; set; }

        public PlaylistViewModel()
        {
            Playlists = new ObservableCollection<Playlist>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
