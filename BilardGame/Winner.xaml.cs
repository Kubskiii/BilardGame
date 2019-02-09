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

namespace BilardGame
{
    /// <summary>
    /// Interaction logic for Winner.xaml
    /// </summary>
    public partial class Winner : Window
    {
        public Winner(string playerName)
        {
            InitializeComponent();
            PlayerName.Text = playerName;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
