using Avalonia.Controls;
using MonitoringOfStudentProgress.ViewModels;

namespace MonitoringOfStudentProgress.Views {
    public partial class MainWindow: Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
