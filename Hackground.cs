using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Hackground;

public static class User32
{
    [DllImport("user32.dll")]
    public static extern nint FindWindowEx(nint parentHandle, nint childAfter, string? className, string? windowTitle);

    [DllImport("user32.dll")]
    public static extern nint SendMessageTimeout(nint windowHandle, uint Msg, nint wParam, nint lParam, int flags, uint timeout, out nint result);
    public const int SMTO_NORMAL = 0x000;
    public const uint WM_CLOSE = 0x0010;

    [DllImport("user32.dll")]
    public static extern nint SetParent(nint hWndChild, nint hWndNewParent);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern bool SetProcessDPIAware();
}

public enum HackgroundMode
{
    FILL,
    FIT,
    STRETCH,
}

public class HackgroundConfig
{
    public Color Color = Color.Black;
    public string Image = "";
    public HackgroundMode Mode;
}

public static class Hackground
{
    public static void SetBackground(HackgroundConfig? config)
    {
        // --------------------------------------------------------------------------------------------------------------------------------
        // Find WorkerW vinduet

        // Gerald Degeneve skriver her hvordan man laver grafik mellem desktop baggrund og desktop ikoner:
        // https://www.codeproject.com/Articles/856020/Draw-Behind-Desktop-Icons-in-Windows-plus

        var hProgman = User32.FindWindowEx(0, 0, "Progman", null);
        if (hProgman == 0)
        {
            throw new Exception("Could not find Progman");
        }

        // Får Progman til at indsætte WorkerW vinduet, som skal være hackground vinduets parent
        User32.SendMessageTimeout(hProgman, 0x052C, 0x0000_000d, 0x0000_0001, User32.SMTO_NORMAL, 1000, out _);

        // Det er mange WorkerW vinduer, så loop indtil vi finder det korrekte
        var hWorkerW = (nint)0;
        while (true)
        {
            hWorkerW = User32.FindWindowEx(0, hWorkerW, "WorkerW", null);
            if (hWorkerW == 0)
            {
                break;
            }

            // Window "" WorkerW                       <--- Kan genkendes ved sit SHELLDLL_DefView child
            //     Window "" SHELLDLL_DefView           
            //         Window "FolderView" SysListView32
            //             Window "" SysHeader32
            // Window "" WorkerW                       <---- Den vi leder efter
            //     Window "Hackground" WindowsForm     <---- Her skal vi indsætte vores vindue
            // Window "Program Manager" Progman

            if (User32.FindWindowEx(hWorkerW, 0, "SHELLDLL_DefView", null) != 0)
            {
                hWorkerW = User32.FindWindowEx(0, hWorkerW, "WorkerW", null);
                break;
            }
        }

        if (hWorkerW == 0)
        {
            throw new Exception("Could not find WorkerW");
        }

        // --------------------------------------------------------------------------------------------------------------------------------
        // Luk gamle hackground vinduer fra sidste gang Hackground.exe blev kørt

        const string TITLE = "Hackground";

        var closeList = new List<nint>();
        var hOld = (nint)0;
        while (true)
        {
            hOld = User32.FindWindowEx(hWorkerW, hOld, null, TITLE);
            if (hOld == 0) break;
            closeList.Add(hOld);
        }

        foreach (var hClose in closeList)
        {
            User32.SendMessageTimeout(hClose, User32.WM_CLOSE, 0, 0, User32.SMTO_NORMAL, 1000, out _);
        }

        // --------------------------------------------------------------------------------------------------------------------------------
        // Åbn nye hackground vinduer

        if (config == null) return; // Hvis vi skal tilbage til normal desktop baggrund

        User32.SetProcessDPIAware(); // Sørger for at hele skærmen stadig dækkes hvis dpi >100%

        var screens = Screen.AllScreens;
        Console.WriteLine($"Found {screens.Length} screens");

        for (var i = 0; i < screens.Length; i += 1)
        {
            var screen = screens[i];
            var image = config.Image != "" ? Image.FromFile(config.Image) : null;
            var thread = new Thread(() =>
            {
                var form = new Form();
                form.Text = TITLE;
                form.FormBorderStyle = FormBorderStyle.None;

                form.Paint += (s, e) =>
                {
                    var g = e.Graphics;

                    g.Clear(config.Color);

                    if (image == null) return;

                    var screenW = screen.Bounds.Width;
                    var screenH = screen.Bounds.Height;
                    var imageW = image.Width;
                    var imageH = image.Height;

                    var screenR = (float)screenW / screenH;
                    var imageR  = (float)imageW / imageH;

                    var drawW = 0;
                    var drawH = 0;

                    switch (config.Mode)
                    {
                        case HackgroundMode.FIT:
                            if (imageR < screenR) // Skærm er bredere end billede
                            {
                                drawH = screenH;
                                drawW = (int)(screenH * imageR);
                            }
                            else
                            {
                                drawW = screenW;
                                drawH = (int)(screenW / imageR);
                            }
                            break;

                        case HackgroundMode.FILL:
                            if (imageR > screenR) // Billede er bredere end skærm
                            {
                                drawH = screenH;
                                drawW = (int)(screenH * imageR);
                            }
                            else
                            {
                                drawW = screenW;
                                drawH = (int)(screenW / imageR);
                            }
                            break;

                        case HackgroundMode.STRETCH:
                            drawH = screenH;
                            drawW = screenW;
                            break;
                    }

                    int drawX = (screenW - drawW) / 2;
                    int drawY = (screenH - drawH) / 2;

                    g.DrawImage(image, new Rectangle(drawX, drawY, drawW, drawH));
                };

                // Koordinater fra screen.Bounds er relativ til Primary Screen, men efter SetParent() vil
                // child vinduets position (form.Top og form.Left) være relativ til parent vinduets position,
                // så screen.Bounds skal justers after parentRect, for at oversætte til parent-relative koordinater.

                User32.SetParent(form.Handle, hWorkerW);
                User32.GetWindowRect(hWorkerW, out var parentRect);

                form.Top = screen.Bounds.Y - parentRect.Top;
                form.Left = screen.Bounds.X - parentRect.Left;
                form.Height = screen.Bounds.Height;
                form.Width  = screen.Bounds.Width;

                Application.Run(form); // Blocker indtil næste Hackground.exe process sender WM_CLOSE til os
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }
}
