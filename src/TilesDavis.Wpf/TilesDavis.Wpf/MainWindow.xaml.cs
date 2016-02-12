using MahApps.Metro.Controls;
using System.Windows;
using TilesDavis.Wpf.ViewModels;

namespace TilesDavis.Wpf
{
    public partial class MainWindow : MetroWindow
    {
        public AppBootstrapper AppBootstrapper { get; protected set; }

        public MainWindow()
        {
            InitializeComponent();

            AppBootstrapper = new AppBootstrapper();
            DataContext = AppBootstrapper;
        }
    }
}
