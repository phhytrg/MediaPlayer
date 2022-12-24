using MediaPlayer.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace MediaPlayer.data
{
    public interface ISubject
    {
        void Attach(IObserver observer);
        void Detach(IObserver observer);
        void NotifyOnMediaElementChanged();
        void NotifyOnPlayButtonChanged();
    }
    public interface IObserver
    {
        void OnDoubleClickedMediaListener();
        void OnPlayButtonUpdate();
    }

    public class MusicPlayerViewModel :INotifyPropertyChanged, ISubject
    {
        public int MediaIndex { get; set; }
        public TimeSpan Position { get; set; }
        public TimeSpan TimeLeft { get; set; } = new TimeSpan();

        private List<IObserver> _observers = new List<IObserver>();

        public event PropertyChangedEventHandler? PropertyChanged;

        [JsonIgnore]
        public MediaElement MediaElement { get; set; } = new MediaElement();
        public TimeSpan MediaMaximum { get; set; } = new TimeSpan(0, 0, 0, 0, 1);
        public bool IsShuffling { get; set; } = false;
        public int ReplayingFlag { get; set; } = Constants.NON_REPLAYING;
        public ObservableCollection<Media> RecentlyPlayed { get; set; } = new ObservableCollection<Media>();
        public List<Media> RecentlyPlayedOfShufflingMode { get; set; } = new List<Media>();
        public Playlist CurrentPlaylist { get; set; }

        private Media? _currentMedia;
        public double Progress { get; set; } = 0;

        [JsonIgnore]
        public Media CurrentMedia { 
            get 
            {
                return this._currentMedia!;
            }
            set
            {
                this._currentMedia = value;
                this.MediaElement.Source = new Uri(this._currentMedia.Fullpath);
            } 
        }

        public void Attach(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyOnMediaElementChanged()
        {
            _observers.ForEach(o =>
            {
                o.OnDoubleClickedMediaListener();
            });
        }

        public void NotifyOnPlayButtonChanged()
        {
            _observers.ForEach(o =>
            {
                o.OnPlayButtonUpdate();
            });
        }
    }
}
