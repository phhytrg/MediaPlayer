using MediaPlayer.data;
using MediaPlayer.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MediaService.xaml
    /// </summary>
    
    public interface IMediaControl
    {
        void SkipNext3Secs();
        void SkipBack3Secs();
        void SkipNextSong();
        void SkipPreviousSong();
    }
    public partial class MediaService : UserControl, INotifyPropertyChanged, IMediaControl
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public const int SIZE_LIMIT = 200;
        public MainWindow mainWindow { get; private set; }

        DispatcherTimer timer;

        public delegate void timerTick();
        event timerTick? tick;
        public MusicPlayerViewModel MusicPlayerViewModel { get; set; }
        public MediaState CurrentState { get; set; }

        private string _saveMedia;
        public MediaService()
        {
            InitializeComponent();
            mainWindow = (MainWindow)Application.Current.MainWindow;
            MusicPlayerViewModel = mainWindow.MusicPlayerViewModel;
        }
        private class MediaElementObserver : IObserver
        {
            private MediaService _parent;
            public MediaElementObserver(MediaService parent)
            {
                this._parent = parent;
            }

            public void OnDoubleClickedMediaListener()
            {
                _parent.MusicPlayerViewModel.CurrentMedia = _parent.MusicPlayerViewModel.CurrentPlaylist.Medias[_parent.MusicPlayerViewModel.MediaIndex];
                _parent.PlaySound();
            }

            public void OnPlayButtonUpdate()
            {
                if (_parent.CurrentState == MediaState.Play)
                {
                    _parent.StopSound();
                }
                else
                {
                    if(_parent.MusicPlayerViewModel.CurrentMedia == null)
                    {
                        _parent.MusicPlayerViewModel.CurrentMedia = _parent.MusicPlayerViewModel.CurrentPlaylist.Medias[_parent.MusicPlayerViewModel.MediaIndex];
                    }
                    _parent.PlaySound();
                }
            }
        }

        private void MediaPlayerService_Loaded(object sender, RoutedEventArgs e)
        {
            MusicPlayerViewModel.MediaElement = this.MediaElement;
            this.MusicPlayerViewModel.MediaElement.LoadedBehavior = MediaState.Manual;
            MusicPlayerViewModel.MediaElement.MediaOpened += MediaElement_MediaOpened;

            if (this.MusicPlayerViewModel.IsShuffling)
            {
                this.skipNextButton.Click += new RoutedEventHandler(shufflingSkipNextButton_Click);
                this.skipPreviousButton.Click += new RoutedEventHandler(shufflingSkipPreviousButton_Click);
                MediaElement.MediaEnded += ShufflingMediaElement_MediaEnded;
            }
            else
            {
                this.skipNextButton.Click += new RoutedEventHandler(linearSkipNextButton_Click);
                this.skipPreviousButton.Click += new RoutedEventHandler(linearSkipPreviousButton_Click);
                MediaElement.MediaEnded += LinearMediaElement_MediaEnded;
            }

            CurrentState = GetMediaState(this.MusicPlayerViewModel.MediaElement);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick!);
            tick = new timerTick(changeStatus);


            MusicPlayerViewModel.Attach(new MediaElementObserver(this));
        }

        private void MediaElement_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "/" + "media_element.json"))
            {
                var json = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/" + "media_element.json");
                if (json != null || json != "")
                {
                    var save = JsonConvert.DeserializeObject<MediaElementSave>(json!)!;
                    MusicPlayerViewModel.MediaElement.Volume = save.Volume;
                    MusicPlayerViewModel.CurrentMedia = new Media(save.Path);
                    MusicPlayerViewModel.MediaElement.Source = new Uri(save.Path);
                    _saveMedia = save.Path;
                    MusicPlayerViewModel.Position = save.Position;
                    MediaElement.MediaOpened += mediaElement_SaveMediaOpened;
                }
            }
        }

        private void mediaElement_SaveMediaOpened(object sender, RoutedEventArgs e)
        {
            if (!_saveMedia.Equals(this.MusicPlayerViewModel.CurrentMedia.Fullpath))
            {
                MediaElement.MediaOpened -= mediaElement_SaveMediaOpened;
                return;
            }

            this.MusicPlayerViewModel.MediaMaximum = this.MusicPlayerViewModel.MediaElement.NaturalDuration.TimeSpan;
            MusicPlayerViewModel.MediaElement.Position += MusicPlayerViewModel.Position;
        }
        public void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            this.MusicPlayerViewModel.MediaMaximum = this.MusicPlayerViewModel.MediaElement.NaturalDuration.TimeSpan;
            addToRecentlyplayed(MusicPlayerViewModel.CurrentMedia);
        }
        public void LinearMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.MediaElement.Stop();
            this.MediaElement.Close();

            if (this.MusicPlayerViewModel.ReplayingFlag == Constants.NON_REPLAYING)
            {
                if (this.MusicPlayerViewModel.MediaIndex >= this.MusicPlayerViewModel.CurrentPlaylist.Medias.Count - 1)
                {
                    return;
                }
                this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[++this.MusicPlayerViewModel.MediaIndex];
                this.MusicPlayerViewModel.MediaElement.Play();
            }
            else if (this.MusicPlayerViewModel.ReplayingFlag == Constants.REPLAY_PLAYLIST)
            {
                if (this.MusicPlayerViewModel.MediaIndex >= this.MusicPlayerViewModel.CurrentPlaylist.Medias.Count - 1)
                {
                    this.MusicPlayerViewModel.MediaIndex = 0;
                    this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[this.MusicPlayerViewModel.MediaIndex];
                    this.MusicPlayerViewModel.MediaElement.Play();
                    return;
                }
                this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[++this.MusicPlayerViewModel.MediaIndex];
                this.MusicPlayerViewModel.MediaElement.Play();
            }
            else
            {
                this.MusicPlayerViewModel.MediaElement.Play();
            }
        }
        public void ShufflingMediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {

            this.MediaElement.Stop();
            this.MediaElement.Close();
            this.MusicPlayerViewModel.RecentlyPlayedOfShufflingMode.Add(this.MusicPlayerViewModel.CurrentMedia);

            if (this.MusicPlayerViewModel.ReplayingFlag == Constants.REPLAY_SINGLE)
            {
                this.MusicPlayerViewModel.MediaElement.Play();
            }
            else
            {
                Random rand = new Random();
                int index = rand.Next() % this.MusicPlayerViewModel.CurrentPlaylist.Medias.Count;
                this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[index];
                this.MusicPlayerViewModel.MediaIndex = index;
                this.MusicPlayerViewModel.MediaElement.Play();
            }

        }

        private void addToRecentlyplayed(Media item)
        {
            if(this.MusicPlayerViewModel.RecentlyPlayed.Count >= SIZE_LIMIT)
            {
                this.MusicPlayerViewModel.RecentlyPlayed.RemoveAt(SIZE_LIMIT - 1);
            }
            this.MusicPlayerViewModel.RecentlyPlayed.Insert(0, item);
        }

        public MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }


        private void replayButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayerViewModel.ReplayingFlag = (MusicPlayerViewModel.ReplayingFlag + 1) % 3;
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            MusicPlayerViewModel.IsShuffling = !MusicPlayerViewModel.IsShuffling;
            if (MusicPlayerViewModel.IsShuffling)
            {
                MediaElement.MediaEnded -= LinearMediaElement_MediaEnded;
                MediaElement.MediaEnded += ShufflingMediaElement_MediaEnded;

                this.skipNextButton.Click -= new RoutedEventHandler(linearSkipNextButton_Click);
                this.skipPreviousButton.Click -= new RoutedEventHandler(linearSkipPreviousButton_Click);

                this.skipNextButton.Click += new RoutedEventHandler(shufflingSkipNextButton_Click);
                this.skipPreviousButton.Click += new RoutedEventHandler(shufflingSkipPreviousButton_Click);
            }
            else 
            {
                this.MusicPlayerViewModel.RecentlyPlayedOfShufflingMode.Clear();
                MediaElement.MediaEnded += LinearMediaElement_MediaEnded;
                MediaElement.MediaEnded -= ShufflingMediaElement_MediaEnded;

                this.skipNextButton.Click += new RoutedEventHandler(linearSkipNextButton_Click);
                this.skipPreviousButton.Click += new RoutedEventHandler(linearSkipPreviousButton_Click);

                this.skipNextButton.Click -= new RoutedEventHandler(shufflingSkipNextButton_Click);
                this.skipPreviousButton.Click -= new RoutedEventHandler(shufflingSkipPreviousButton_Click);
            }
        }
        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentState == MediaState.Play)
            {
                StopSound();
            }
            else
            {
                PlaySound();
            }
        }

        public void StopSound()
        {
            timer.Stop();
            this.MusicPlayerViewModel.MediaElement.Pause();
            CurrentState = GetMediaState(this.MusicPlayerViewModel.MediaElement);
        }

        public void PlaySound()
        {
            timer.Start();
            this.MusicPlayerViewModel.MediaElement.Play();
            CurrentState = GetMediaState(this.MusicPlayerViewModel.MediaElement);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.Invoke(tick);
        }
        void changeStatus()
        {
            if (CurrentState == MediaState.Play)
            {
                MusicPlayerViewModel.TimeLeft = this.MusicPlayerViewModel.MediaMaximum - this.MusicPlayerViewModel.MediaElement.Position;
                MusicPlayerViewModel.Position = this.MusicPlayerViewModel.MediaElement.Position;
                this.MusicPlayerViewModel.Progress = this.MusicPlayerViewModel.MediaElement.Position.TotalMilliseconds;
            }

        }

        private void MediaSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var seekValue = (int)this.MediaSlider.Value;
            this.MusicPlayerViewModel.MediaElement.Position = new TimeSpan(0,0,0,0,seekValue);
            isDragging = false;
        }

        private bool isDragging = false;

        private void MediaSlider_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
        }

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isDragging)
            {
                var seekValue = (int)this.MediaSlider.Value;
                MusicPlayerViewModel.Position = TimeSpan.Zero;
                MusicPlayerViewModel.TimeLeft = new TimeSpan(0, 0, 0, 0, (int)(this.MusicPlayerViewModel.MediaMaximum.TotalMilliseconds - seekValue));
            }
        }
        private void linearSkipNextButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MusicPlayerViewModel.MediaIndex + 1 >= this.MusicPlayerViewModel.CurrentPlaylist.Medias.Count)
            {
                return;
            }

            this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[++this.MusicPlayerViewModel.MediaIndex];
        }
        private void shufflingSkipNextButton_Click(object sender, RoutedEventArgs e)
        {
            this.MusicPlayerViewModel.RecentlyPlayedOfShufflingMode.Add(this.MusicPlayerViewModel.CurrentMedia);
            Random rand = new Random();
            int index = rand.Next() % this.MusicPlayerViewModel.CurrentPlaylist.Medias.Count;
            this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[index];
            this.MusicPlayerViewModel.MediaIndex = index;
        }

        private void linearSkipPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MusicPlayerViewModel.MediaIndex <= 0)
            {
                return;
            }

            this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.CurrentPlaylist.Medias[--this.MusicPlayerViewModel.MediaIndex];
        }
        private void shufflingSkipPreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.MusicPlayerViewModel.RecentlyPlayedOfShufflingMode.Count <= 0)
            {
                return;
            }

            this.MusicPlayerViewModel.CurrentMedia = this.MusicPlayerViewModel.RecentlyPlayedOfShufflingMode.First();
            this.MusicPlayerViewModel.RecentlyPlayedOfShufflingMode.RemoveAt(0);
        }

        private void recentlyPlayButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.NavigateRecentlyPlayedList();
        }

        public void SkipNext3Secs()
        {
            if (CurrentState == MediaState.Close || CurrentState == MediaState.Stop)
            {
                return;
            }

            this.MusicPlayerViewModel.Position += new TimeSpan(0, 0, 3);
            this.MusicPlayerViewModel.MediaElement.Position += new TimeSpan(0, 0, 3);
        }

        public void SkipBack3Secs()
        {
            if (CurrentState == MediaState.Close || CurrentState == MediaState.Stop)
            {
                return;
            }

            this.MusicPlayerViewModel.Position -= new TimeSpan(0, 0, 3);
            this.MusicPlayerViewModel.MediaElement.Position -= new TimeSpan(0, 0, 3);
        }

        public void SkipNextSong()
        {
            //this.skipNextButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //ButtonAutomationPeer peer = new ButtonAutomationPeer(this.skipNextButton);
            //IInvokeProvider invokeProv = peer!.GetPattern(PatternInterface.Invoke)! as IInvokeProvider;
            //invokeProv!.Invoke();
            if (this.MusicPlayerViewModel.IsShuffling)
            {
                this.shufflingSkipNextButton_Click(new object(), new RoutedEventArgs());
            }
            else
            {
                this.linearSkipNextButton_Click(new object(), new RoutedEventArgs());
            }
        }

        public void SkipPreviousSong()
        {
            //this.skipPreviousButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //ButtonAutomationPeer peer = new ButtonAutomationPeer(this.skipPreviousButton);
            //IInvokeProvider invokeProv = (IInvokeProvider)peer!.GetPattern(PatternInterface.Invoke)!;
            //invokeProv!.Invoke();
            if (this.MusicPlayerViewModel.IsShuffling)
            {
                this.shufflingSkipPreviousButton_Click(new object(), new RoutedEventArgs());
            }
            else
            {
                this.linearSkipPreviousButton_Click(new object(), new RoutedEventArgs());
            }
        }
    }



    public class SliderSmoother
    {
        public static double GetSmoothValue(DependencyObject obj)
        {
            return (double)obj.GetValue(SmoothValueProperty);
        }

        public static void SetSmoothValue(DependencyObject obj, double value)
        {
            obj.SetValue(SmoothValueProperty, value);
        }

        public static readonly DependencyProperty SmoothValueProperty =
            DependencyProperty.RegisterAttached("SmoothValue", typeof(double), typeof(SliderSmoother), new PropertyMetadata(0.0, changing));

        private static void changing(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var anim = new DoubleAnimation((double)e.OldValue, (double)e.NewValue, new TimeSpan(0, 0, 0, 0, 300));
            (d as Slider).BeginAnimation(Slider.ValueProperty, anim, HandoffBehavior.Compose);
        }
    }
}
