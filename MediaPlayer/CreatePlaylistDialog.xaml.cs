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
using System.Windows.Shapes;
using MediaPlayer.models;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for CreatePlaylistDialog.xaml
    /// </summary>
    public partial class CreatePlaylistDialog : Window
    {
        private bool IsClosed = false;
        public Playlist? newPlaylist { get; set; }
        public CreatePlaylistDialog(Point position)
        {
            InitializeComponent();
            Top = position.Y + 30 + Application.Current.MainWindow.Top + 40 ;
            
            Left = position.X + Application.Current.MainWindow.Left; ;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (!IsClosed)
            {
                DialogResult = false;
            }
        }

        private void createPlaylist_Click(object sender, RoutedEventArgs e)
        {
            if(playlistName.Text == "")
            {
                createPlaylist.IsEnabled = false;
                return;
            }
            newPlaylist = new Playlist(playlistName.Text,new ObservableCollection<Media>());
            IsClosed = true;
            DialogResult = true;
        }
    }
}
