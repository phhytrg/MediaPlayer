using MediaPlayer.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for UcRecentlyPlayed.xaml
    /// </summary>
    public partial class UcRecentlyPlayed : UserControl
    {
        MainWindow? main;
        public ObservableCollection<Media> RecentlyPlayed { get; set; }
        public UcRecentlyPlayed()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            main = (MainWindow)Application.Current.MainWindow;
            RecentlyPlayed = main.MusicPlayerViewModel.RecentlyPlayed;
            this.recentlyPlayedListView.ItemsSource = RecentlyPlayed;
        }
        private void navigateUpButton_Click(object sender, RoutedEventArgs e)
        {
            main.NavigateUp();        
        }
    }
}
