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

        Model[] models = new Model[]
            {
            new Model(new Triangle[]
                {
                    new Triangle(new List<Vector4>() { new Vector4(0, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1) }) { color = Colors.Red },
                    new Triangle(new List<Vector4>() { new Vector4(0, 0, 0, 1), new Vector4(1, 0, 0, 1), new Vector4(0, 0, 1, 1) }) { color = Colors.Blue },
                    new Triangle(new List<Vector4>() { new Vector4(0, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1) }) { color = Colors.Green },
                    new Triangle(new List<Vector4>() { new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), new Vector4(0, 0, 1, 1) }) { color = Colors.Pink }
                }),
            };
        CPUEngine engine;
        uint[,] colors;
        Resolution res = new Resolution(1200, 800);
        BackgroundWorker worker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            engine = new CPUEngine(res);
            img.Source = BitmapExtensions.CreateBitmap(res);
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (img.Source as WriteableBitmap).FillBitmap(colors);
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            models[0].RotateZ((float)Math.PI * 2 * 0.02f);
            colors = engine.Render(models);
        }
    }
}
