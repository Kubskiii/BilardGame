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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;
using GraphicsEngine;
using TheGame;

namespace BilardGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion
        Resolution res = new Resolution(1200, 800);
        uint[,] colors;
        Game game = new Game(new Resolution(1200, 800));
        BackgroundWorker worker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            img.Source = BitmapExtensions.CreateBitmap(res);
            game = new Game(res);
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (img.Source as WriteableBitmap).FillBitmap(colors);
            game.Update();
            worker.RunWorkerAsync();
        }
        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            colors = game.Display();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    game.RotateStickLeft();
                    break;
                case Key.Right:
                    game.RotateStickRigth();
                    break;
                case Key.H:
                    game.HoldStick();
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.H:
                    game.ReleaseStick();
                    break;
            }
        }

        #region Button handlers
        private void StaticCamera_Checked(object sender, RoutedEventArgs e)
        {
            game.StaticCamera();
        }

        private void ActiveCamera_Checked(object sender, RoutedEventArgs e)
        {
            game.ActiveCamera();
        }

        private void TrackingCamera_Checked(object sender, RoutedEventArgs e)
        {
            game.TrackingCamera();
        }

        private void PointLight_Checked(object sender, RoutedEventArgs e)
        {
            game.SwitchPointLight();
        }

        private void TrackingLight_Checked(object sender, RoutedEventArgs e)
        {
            game.SwitchTrackingLight();
        }

        private void ConstantShading_Checked(object sender, RoutedEventArgs e)
        {
            game.ConstantShading();
        }

        private void GouraudShading_Checked(object sender, RoutedEventArgs e)
        {
            game.GouraudShading();
        }

        private void PhongShading_Checked(object sender, RoutedEventArgs e)
        {
            game.PhongShading();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainMenu.Visibility = Visibility.Collapsed;
            SettingsMenu.Visibility = Visibility.Visible;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            SettingsMenu.Visibility = Visibility.Collapsed;
            MainMenu.Visibility = Visibility.Visible;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            game = new Game(res);
        }
        #endregion

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var about = new About();
            about.Owner = this;
            about.ShowDialog();
        }

        private void Instruction_Click(object sender, RoutedEventArgs e)
        {
            var instruction = new Instruction();
            instruction.Owner = this;
            instruction.ShowDialog();
        }
    }
}
