using System.Diagnostics;
using WallpaperWindow;


namespace WinFormsApp1 {
    public partial class Form1 : Form {
        static IntPtr workerW = IntPtr.Zero;
        static Process wallpaperProcess = new();
        static IntPtr wallpaperHandle = IntPtr.Zero;
        const string wallpaperWindowPath = @"D:\Programmmieren\Projects\oqueueWallpaper\Release\ImageWindow.exe";

        public Form1() {
            InitializeComponent();
            Debugger.Console.ClearAll();

            Button printWorkerWButton = new();
            printWorkerWButton.AutoSize = true;
            printWorkerWButton.Text = "Print WorkerW";
            printWorkerWButton.Click += (s, e) => {
                PrintWorkerW();
            };

            Button createWorkerWButton = new();
            createWorkerWButton.AutoSize = true;
            createWorkerWButton.Text = "Create WorkerW";
            createWorkerWButton.Click += (s, e) => {
                workerW = GetWorkerW();
            };

            Button disposeOfWorkerWButton = new();
            disposeOfWorkerWButton.AutoSize = true;
            disposeOfWorkerWButton.Text = "Dispose WorkerW";
            disposeOfWorkerWButton.Click += (s, e) => {
                DisposeWorkerW();
            };

            Button createWallpaperWindowButton = new();
            createWallpaperWindowButton.AutoSize = true;
            createWallpaperWindowButton.Text = "Create Wallpaper Process as chid of WorkerW";
            createWallpaperWindowButton.Click += (s, e) => {
                wallpaperProcess.StartInfo.FileName = wallpaperWindowPath;
                wallpaperProcess.Start();

                // Wait for Handle to get generated
                while (Process.GetProcessById(wallpaperProcess.Id).MainWindowHandle == IntPtr.Zero) { }

                wallpaperHandle = Process.GetProcessById(wallpaperProcess.Id).MainWindowHandle;

                // Set Parent to WorkerW
                W32.SetParent(wallpaperHandle, workerW);

                // Change Styles
                int currentStyles = W32.GetWindowLong(wallpaperHandle, W32.GWL_STYLE);
                int newStyles = currentStyles
                                & ~(int)W32.WindowStyles.WS_SYSMENU
                                & ~(int)W32.WindowStyles.WS_CLIPCHILDREN;
                W32.SetWindowLong(wallpaperHandle, W32.GWL_STYLE, newStyles);

                // Change Extended Styles
                IntPtr currentExStyles = W32.GetWindowLongPtr(wallpaperHandle, W32.GWL_EXSTYLE);
                IntPtr newExStyles = currentExStyles
                                        & ~(IntPtr)W32.WindowStylesEx.WS_EX_APPWINDOW
                                        |  (IntPtr)W32.WindowStylesEx.WS_EX_TOOLWINDOW;
                W32.SetWindowLongPtr(wallpaperHandle, W32.GWL_EXSTYLE, newExStyles);

                // Change Properties of the Window
                /// Remove all Properties
                Debugger.Console.Log(W32.GetWindowLongPtr(wallpaperHandle, W32.GWL_EXSTYLE).ToString("X"));


                // Change Position And Update Window
                W32.SetWindowPos(wallpaperHandle, IntPtr.Zero, 1920, 0, 1080, 1920,
                    W32.SetWindowPosFlags.FrameChanged 
                    | W32.SetWindowPosFlags.NoSize 
                    | W32.SetWindowPosFlags.NoZOrder
                );

                // Show Message
                Debugger.Console.Log($"Created Wallpaper Process (Handle: {wallpaperHandle.ToString("X")}) as child of WorkerW (Handle: {workerW.ToString("X")})");
            };
            
            Button closeWallpaperWindowButton = new();
            closeWallpaperWindowButton.AutoSize = true;
            closeWallpaperWindowButton.Text = "Close Wallpaper process";
            closeWallpaperWindowButton.Click += (s, e) => {

                wallpaperProcess.Close();
                wallpaperProcess.Dispose();
            };

            flowLayoutPanel1.Controls.Add(printWorkerWButton);
            flowLayoutPanel1.Controls.Add(createWorkerWButton);
            flowLayoutPanel1.Controls.Add(disposeOfWorkerWButton);
            flowLayoutPanel1.Controls.Add(createWallpaperWindowButton);
            flowLayoutPanel1.Controls.Add(closeWallpaperWindowButton);
        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }


        private static void PrintWorkerW() {
            Debugger.Console.Log($"workerW Handle: {workerW.ToString("X")}");
        }


        private static bool DisposeWorkerW() {
            if (workerW == IntPtr.Zero) {
                Debugger.Console.Log("Could not dispose WorkerW");
                return false;
            }

            W32.SendMessage(workerW, W32.WM_SYSCOMMAND, W32.SC_CLOSE, IntPtr.Zero);
            workerW = IntPtr.Zero;
            Debugger.Console.Log("Disposed WorkerW");
            return true;
        }

        /// <summary>
        /// credits to https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows-plus
        /// </summary>
        /// <returns></returns>
        private static IntPtr GetWorkerW() {
            // Get Progman
            IntPtr progman = W32.FindWindow("Progman", null!);
            if (progman != IntPtr.Zero) {
                Debugger.Console.Log($"progman Handle: {progman.ToString("X")}");
            }


            // Search for a WorkerW
            // Send 0x052C to Progman. This message directs Progman to spawn a WorkerW behind the desktop icons. If it is already there, nothing happens.
            nint result; // not needed for the program to continue
            W32.SendMessageTimeout(
                progman,
                0x052C,
                new IntPtr(0),
                IntPtr.Zero,
                W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                1000,
                out result);


            IntPtr workerw = IntPtr.Zero;
            // We enumerate all Windows, until we find one, that has the SHELLDLL_DefView as a child 
            // If we found that window, we take its next sibling and assign it to workerw
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) => {
                IntPtr p = W32.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", IntPtr.Zero);
                if (p != IntPtr.Zero) {
                    workerw = W32.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", IntPtr.Zero);
                }
                return true;
            }), IntPtr.Zero);

            Debugger.Console.Log($"W32.SendMessageTimeout Result: {result.ToString("X")}");
            Debugger.Console.Log($"workerW Handle: {workerw.ToString("X")}");
            if (workerw == IntPtr.Zero) Debugger.Console.Log("No instance of WorkerW was created");

            return workerw;
        }
    }
}