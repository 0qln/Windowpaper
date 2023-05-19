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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Microsoft.Win32;
using Microsoft;
using System.Runtime.InteropServices;
using System.Reflection.Metadata;

namespace WallpaperWindow {

    public partial class MainWindow : Window {
        IntPtr workerW = IntPtr.Zero;
        public MainWindow() {
            Debugger.Console.ClearAll();
            InitializeComponent();


            Button createWorkerWButton = new();
            createWorkerWButton.Content = "Create WorkerW";
            createWorkerWButton.Click += (s, e) => {
                workerW =  CreateWorkerW();
            };
            Button disposeOfWorkerWButton = new();
            disposeOfWorkerWButton.Content = "Dispose WorkerW";
            disposeOfWorkerWButton.Click += (s, e) => {
                if (DisposeWorkerW()) {
                    Debugger.Console.Log("Disposed WorkerW");
                }
                else {
                    Debugger.Console.Log("Could not dispose WorkerW");
                }

            };
            Button moveWindowButton = new();
            moveWindowButton.Content = "Move this window";
            moveWindowButton.Click += (s, e) => {
                W32.SetParent(new WindowInteropHelper(this).Handle, workerW);
                Debugger.Console.Log("Moved window");
            };

            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(createWorkerWButton);
            stackPanel.Children.Add(disposeOfWorkerWButton);
            stackPanel.Children.Add(moveWindowButton);
            mCanvas.Children.Add(stackPanel);

            Closed += (s, e) => {
                Debugger.Console.Log("Start closing the applicaiton...");
                if (DisposeWorkerW()) Debugger.Console.Log("Disposed WorkerW");
                else Debugger.Console.Log("Could not disopose WorkerW");
            };
        }

        private bool DisposeWorkerW() {
            if (workerW != IntPtr.Zero) {
                return false;
            }

            W32.SendMessage(workerW, W32.WM_SYSCOMMAND, W32.SC_CLOSE, 0);
            workerW = IntPtr.Zero;
            return true;
        }

        /// <summary>
        /// credits to https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows-plus
        /// </summary>
        /// <returns></returns>
        private IntPtr CreateWorkerW() {
            IntPtr progman = W32.FindWindow("Progman", null!);
            if (progman != IntPtr.Zero) {
                Debugger.Console.Log($"progman Handle: {progman}");
            }

            
            IntPtr result = IntPtr.Zero;
            // Send 0x052C to Progman. This message directs Progman to spawn a WorkerW behind the desktop icons. If it is already there, nothing happens.
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
                IntPtr p = W32.FindWindowEx(tophandle,IntPtr.Zero,"SHELLDLL_DefView",IntPtr.Zero);
                if (p != IntPtr.Zero) {
                    workerw = W32.FindWindowEx(IntPtr.Zero, tophandle, "WorkerW", IntPtr.Zero);
                }
                return true;
            }), IntPtr.Zero);

            if (workerw != IntPtr.Zero) Debugger.Console.Log($"workerW Handle: {workerw}");
            else Debugger.Console.Log("No WorkerW was created");

            return workerw;
        }
    }
}
