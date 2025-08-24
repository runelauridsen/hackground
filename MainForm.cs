using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Hackground
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.Icon;
        }

        private string SelectedImage = "";
        private Color SelectedColor = Color.Black;

        private void HackgroundGui_Load(object sender, EventArgs e)
        {
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            var dialog = new ColorDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            SelectedColor = dialog.Color;
            SelectedImage = "";
            RefreshBackground();
        }

        private void ImageButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Hackground");
            var srcFilePath = dialog.FileName;
            var dstFilePath = Path.Combine(appDataPath, "image." + Path.GetExtension(srcFilePath));
            Directory.CreateDirectory(appDataPath); 
            File.Copy(srcFilePath, dstFilePath, true); // Lav en kopi i AppData/Local så bruger kan slette sin egen fil uden problemer

            SelectedImage = dstFilePath;
            RefreshBackground();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            SelectedColor = Color.Empty;
            SelectedImage = "";
            RefreshBackground();
        }

        private void RefreshBackground()
        {
            var exe = Application.ExecutablePath;
            var args = "";

            using RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
            if (SelectedColor != Color.Empty || SelectedImage != "")
            {
                key.SetValue("Hackground", $"{exe} {args}");
                args = $"#{SelectedColor.R:X2}{SelectedColor.G:X2}{SelectedColor.B:X2} \"{SelectedImage}\"";
                
            }
            else
            {
                key.DeleteValue("Hackground", throwOnMissingValue: false);
                args = "--stop";
            }

            Process.Start(new ProcessStartInfo { FileName = exe, Arguments = args });
        }
    }
}
