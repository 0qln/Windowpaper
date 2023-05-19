using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawBehindDesktopIcons {
    class Program {
        static void Main(string[] args) {

            //PrintVisibleWindowHandles(2);
            Console.WriteLine("Start");

            IntPtr progman = W32.FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;
            W32.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);

            Console.WriteLine(result);
            //PrintVisibleWindowHandles(2);

            IntPtr workerw = IntPtr.Zero;

            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) => {
                IntPtr p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero) {
                    workerw = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);


            //DEMO
            Form testForm = new Form();
            testForm.Text = "Test Window";
            testForm.StartPosition = FormStartPosition.CenterScreen;

            testForm.Load += new EventHandler((s, e) => {
                testForm.Width = 500;
                testForm.Height = 500;
                testForm.Left = 500;
                testForm.Top = 0;

                Button button = new Button() { Text = "Catch Me" };
                testForm.Controls.Add(button);
                Random rnd = new Random();
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 100;
                timer.Tick += new EventHandler((sender, eventArgs) => {
                    button.Left = rnd.Next(0, testForm.Width - button.Width);
                    button.Top = rnd.Next(0, testForm.Height - button.Height);
                });
                timer.Start();

                W32.SetParent(testForm.Handle, workerw);
            });
            Application.Run(testForm);
        }

        static void PrintVisibleWindowHandles(IntPtr hwnd, int maxLevel = -1, int level = 0) {
            bool isVisible = W32.IsWindowVisible(hwnd);

            if (isVisible && (maxLevel == -1 || level <= maxLevel)) {
                StringBuilder className = new StringBuilder(256);
                W32.GetClassName(hwnd, className, className.Capacity);

                StringBuilder windowTitle = new StringBuilder(256);
                W32.GetWindowText(hwnd, windowTitle, className.Capacity);

                Console.WriteLine("".PadLeft(level * 2) + "0x{0:X8} \"{1}\" {2}", hwnd.ToInt64(), windowTitle, className);

                level++;

                // Enumerates all child windows of the current window
                W32.EnumChildWindows(hwnd, new W32.EnumWindowsProc((childhandle, childparamhandle) => {
                    PrintVisibleWindowHandles(childhandle, maxLevel, level);
                    return true;
                }), IntPtr.Zero);
            }
        }
        static void PrintVisibleWindowHandles(int maxLevel = -1) {
            // Enumerates all existing top window handles. This includes open and visible windows, as well as invisible windows.
            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) => {
                PrintVisibleWindowHandles(tophandle, maxLevel);
                return true;
            }), IntPtr.Zero);
        }

    }
}
