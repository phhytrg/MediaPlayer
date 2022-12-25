using MediaPlayer.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    public partial class UcRecentlyPlayed : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        MainWindow? main;
        public ObservableCollection<Media> List { get; set; }
        public UcRecentlyPlayed(ObservableCollection<Media> medias)
        {
            InitializeComponent();
            this.List = medias;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            main = (MainWindow)Application.Current.MainWindow;
            this.recentlyPlayedListView.ItemsSource = List;
        }
        private void navigateUpButton_Click(object sender, RoutedEventArgs e)
        {
            main!.NavigateUp();        
        }
    }
}
