using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

public class WallpaperWindow : Form {
    private Image backgroundImage;

    public WallpaperWindow(string imagePath) {
        // Load the image from the specified path
        backgroundImage = Image.FromFile(imagePath);

        // Set the form properties
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        ShowInTaskbar = false;
        TopMost = true;
        TransparencyKey = BackColor = Color.Black;
        BackgroundImage = backgroundImage;
        BackgroundImageLayout = ImageLayout.Stretch;

        // Set the form size to cover the entire screen
        Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
        Size = screenBounds.Size;
        Location = screenBounds.Location;
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            backgroundImage.Dispose();
        }
        base.Dispose(disposing);
    }

    [STAThread]
    public static void Main() {
        // Replace "imagePath" with the path to your desired image
        string imagePath = "C:\\path\\to\\image.jpg";

        // Create and show the wallpaper window
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        WallpaperWindow wallpaperWindow = new WallpaperWindow(imagePath);
        Application.Run(wallpaperWindow);
    }
}