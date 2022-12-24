using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for RenameDialog.xaml
    /// </summary>
    public partial class RenameDialog : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public delegate void NameChangedHandler(string newPlaylistName);
        public event NameChangedHandler NameChanged;
        public string CurrentName { get; set; } = "";

        private bool IsClosed = false;
        public RenameDialog(string playlistName, Point position)
        {
            InitializeComponent();
            CurrentName = playlistName;
            Top = position.Y + 30 + Application.Current.MainWindow.Top + 40;

            Left = position.X + Application.Current.MainWindow.Left; ;
        }

        private void renamePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = true;
            DialogResult = true;
        }

        private void cancelRenamePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = true;
            DialogResult = false;
        }
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (!IsClosed)
            {
                DialogResult = false;
            }
        }

        private void playlistName_TextChanged(object sender, TextChangedEventArgs e)
        {
            NameChanged?.Invoke(playlistName.Text);
        }
    }
}
