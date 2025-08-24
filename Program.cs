using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Hackground;

public static class Program
{
    // --------------------------------------------------------------------------------------------------------------------------------
    // CLI

    private static void ExitWithError(string error)
    {
        MessageBox.Show(error);
        Environment.Exit(1);
    }

    private static void RunCLI(string[] args)
    {
        try
        {
            var config = new HackgroundConfig();
            var stop = false;

            foreach (var arg in args)
            {
                if (arg.StartsWith("-"))
                {
                    var option = arg.TrimStart('-');
                    switch (option)
                    {
                        case "fill":
                            config.Mode = HackgroundMode.FILL;
                            break;

                        case "fit":
                            config.Mode = HackgroundMode.FIT;
                            break;

                        case "stretch":
                            config.Mode = HackgroundMode.STRETCH;
                            break;

                        case "stop":
                            stop = true;
                            break;

                        default:
                            ExitWithError($"Unknown option: {arg}");
                            break;
                    }
                }
                else if (arg.StartsWith("#"))
                {
                    var colorString = arg.Substring(1);
                    if (colorString.Length != 6)
                    {
                        ExitWithError($"Invalid hex color: {arg}");
                    }

                    if (!int.TryParse(colorString, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var colorNumber))
                    {
                        ExitWithError($"Invalid hex color: {arg}");
                    }

                    config.Color = Color.FromArgb((colorNumber >> 16) & 0xff, (colorNumber >> 8) & 0xff, (colorNumber >> 0) & 0xff);
                }
                else
                {
                    config.Image = arg;
                }
            }

            Hackground.SetBackground(stop ? null : config);
        }
        catch (Exception e)
        {
            ExitWithError(e.ToString());
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------
    // GUI

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        ShowError(e.Exception);
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        ShowError(e.ExceptionObject);
    }

    private static void ShowError(object obj)
    {
        MessageBox.Show(obj.ToString(), "Hackground", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private static void RunGUI()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.ThreadException += Application_ThreadException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        Application.Run(new MainForm());
    }

    // --------------------------------------------------------------------------------------------------------------------------------

    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            RunCLI(args);
        }
        else
        {
            RunGUI();
        }
    }
}
