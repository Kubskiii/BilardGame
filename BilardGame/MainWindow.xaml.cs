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
                    new Triangle(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 1, 0)) { color = Colors.Red },
                    new Triangle(new Point3D(0, 0, 0), new Point3D(1, 0, 0), new Point3D(0, 0, 1)) { color = Colors.Blue },
                    new Triangle(new Point3D(0, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1)) { color = Colors.Green },
                    new Triangle(new Point3D(1, 0, 0), new Point3D(0, 1, 0), new Point3D(0, 0, 1)) { color = Colors.Pink }
                }),
            new Model(new Triangle[]
                {
                    new Triangle(new Point3D(0, 0, 0), new Point3D(2, 0, 0), new Point3D(0, 2, 0)) { color = Colors.Wheat }, //z
                    new Triangle(new Point3D(0, 0, 0), new Point3D(2, 0, 0), new Point3D(0, 0, 2)) { color = Colors.White }, //y
                    new Triangle(new Point3D(0, 0, 0), new Point3D(0, 2, 0), new Point3D(0, 0, 2)) { color = Colors.Gray } //x
                }),
            };

        Viewer viewer;
        UInt32[,] colors;
        Resolution res = new Resolution(1200, 800);
        BackgroundWorker worker = new BackgroundWorker();
        public MainWindow()
        {
            InitializeComponent();
            viewer = new Viewer(res);
            img.Source = BitmapEx.CreateBitmap(res);
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            colors = new UInt32[res.Width, res.Height];
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (img.Source as WriteableBitmap).FillBitmap(colors);
            colors = new UInt32[res.Width, res.Height];
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //models[0].Move(0.5f, 0, 0);
            models[0].RotateZ((float)Math.PI * 2 * 0.02f);
            //models[0].Move(-0.5f, 0, 0);
            viewer.Draw(models, colors);
        }
    }
}
