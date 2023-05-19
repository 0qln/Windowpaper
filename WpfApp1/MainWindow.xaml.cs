using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp1 {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

            string verticalPath = @"C:\Users\User\OneDrive\Bilder\Wallpapers\vertical2\a6ad291a0c65682a1818477f3b4f338e.jpg";
            string widePath = @"C:\Users\User\OneDrive\Bilder\Wallpapers\wide\3840x1080-hd-dual-monitor-forest-hv0reqwqlcwu58gk.jpg";


            Button setWide = new();
            setWide.Content = "Wide";
            setWide.Click += (s, e) => {
                WallpaperWindow window = new WallpaperWindow(widePath);
            };

            Button setVertical = new();
            setVertical.Content = "Vertical";
            setVertical.Click += (s, e) => {
                WallpaperWindow window = new WallpaperWindow(verticalPath);
            };

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(setVertical);
            stackPanel.Children.Add(setWide);
            MainCanvas.Children.Add(stackPanel);

        }
    }
}
