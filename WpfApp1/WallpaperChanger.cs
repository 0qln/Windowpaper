using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO;

namespace WpfApp1 {
    public class WallpaperChanger {
        // Constants for monitor-related Windows API functions
        private const int MONITOR_DEFAULTTOPRIMARY = 1;
        private const int MONITOR_DEFAULTTONEAREST = 2;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        // Windows API functions for changing the wallpaper
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, int dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        // Windows API structures
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFO {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        // Set wallpaper for a specific monitor
        public static void SetWallpaperForMonitor(string imagePath, int monitorIndex) {
            IntPtr hMonitor = IntPtr.Zero;
            if (monitorIndex >= 0) {
                // Get the handle to the monitor based on monitor index
                IntPtr hWnd = IntPtr.Zero;
                hWnd = GetDesktopWindow();
                hMonitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTOPRIMARY);

                for (int i = 0; i < monitorIndex; i++) {
                    // Get the next monitor handle
                    hMonitor = MonitorFromWindow(hWnd, MONITOR_DEFAULTTONEAREST);
                }
            }

            // Set the wallpaper
            SystemParametersInfo(0x0014, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
    }

    public static class DesktopBackgroundChanger {
        public static void SetDesktopBackground(string imagePath) {
            // Set the image path in the registry
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "Wallpaper", imagePath);

            // Notify the system of the change
            SendMessageTimeout(HWND_BROADCAST, WM_SETTINGCHANGE, 0, "Wallpaper", SMTO_NORMAL, 1000, out _);

            // Force an update of the desktop
            UpdatePerUserSystemParameters();
        }

        // Constants used for the SendMessageTimeout function
        private const int HWND_BROADCAST = 0xffff;
        private const int WM_SETTINGCHANGE = 0x001A;
        private const int SMTO_NORMAL = 0x0000;

        // External functions from user32.dll
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessageTimeout(int hWnd, int Msg, int wParam, string lParam, int fuFlags, int uTimeout, out int lpdwResult);

        // Function to update the desktop settings
        private static void UpdatePerUserSystemParameters() {
            SystemParametersInfo(0x0094, 0, null, 0x01 | 0x02);
        }
    }

    public class Wallpaper {
        Wallpaper() { }

        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public enum Style : int {
            Tiled,
            Centered,
            Stretched
        }

        public static void Set(Uri uri, Style style) {
            System.IO.Stream s = new System.Net.WebClient().OpenRead(uri.ToString());

            System.Drawing.Image img = System.Drawing.Image.FromStream(s);
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched) {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Centered) {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }

            if (style == Style.Tiled) {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}