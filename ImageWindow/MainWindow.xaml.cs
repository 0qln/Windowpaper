using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace ImageWindow {
    
    public partial class MainWallpaperWindow : System.Windows.Window {
        public MainWallpaperWindow() {
            InitializeComponent();

            FullScreen(1);
        }


        public void FullScreen(int monitor) {
            if (monitor >= Screen.AllScreens.Length) {
                // Invalid monitor index provided
                return;
            }

            Screen targetScreen = Screen.AllScreens[monitor];
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            ResizeMode = ResizeMode.NoResize;
            Left = targetScreen.Bounds.Left;
            Top = targetScreen.Bounds.Top;
            Width = targetScreen.Bounds.Width;
            Height = targetScreen.Bounds.Height;
        }
    }
}
