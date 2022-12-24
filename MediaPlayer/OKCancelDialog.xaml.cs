using System;
using System.Collections.Generic;
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
    /// Interaction logic for OKCancelDialog.xaml
    /// </summary>
    public partial class OKCancelDialog : Window
    {
        private bool IsClosed = false;
        public OKCancelDialog(Point position)
        {
            InitializeComponent();
            Top = position.Y + 30 + Application.Current.MainWindow.Top + 40;

            Left = position.X + Application.Current.MainWindow.Left; ;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = true;
            DialogResult = true;
        }

        private void cancelRenamePlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            IsClosed = true;
            DialogResult= false;
        }
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            if (!IsClosed)
            {
                DialogResult = false;
            }
        }
    }
}
