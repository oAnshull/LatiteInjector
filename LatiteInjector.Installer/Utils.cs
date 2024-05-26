﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace LatiteInjector.Installer
{
    public class Utils
    {
        // carlton you were right fuck com what the fuck is this
        
        [ComImport]
        [Guid("00021401-0000-0000-C000-000000000046")]
        internal class ShellLink
        {
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F9-0000-0000-C000-000000000046")]
        internal interface IShellLink
        {
            void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
            void GetIDList(out IntPtr ppidl);
            void SetIDList(IntPtr pidl);
            void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
            void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
            void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
            void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
            void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
            void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
            void GetHotkey(out short pwHotkey);
            void SetHotkey(short wHotkey);
            void GetShowCmd(out int piShowCmd);
            void SetShowCmd(int iShowCmd);
            void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
            void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
            void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
            void Resolve(IntPtr hwnd, int fFlags);
            void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
        }

        public static void WriteColor(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static async Task DownloadFile(Uri uri, string fileName)
        {
            using HttpClient client = new();
            using Stream asyncStream = await client.GetStreamAsync(uri);
            using FileStream fs = new(fileName, FileMode.CreateNew);
            await asyncStream.CopyToAsync(fs);
        }

        public static bool IsNet8Installed()
        {
            Process dotnetVersionProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            dotnetVersionProcess?.WaitForExit();
            string output = dotnetVersionProcess?.StandardOutput.ReadToEnd();
            if (output != null && output.StartsWith("8."))
                return true;
            return false;
        }

        public static void CreateShortcut(string shortcutPath, string shortcutDescription, string shortcutIconPath, string targetPath)
        {
            IShellLink link = (IShellLink)new ShellLink();

            // setup shortcut information
            link.SetPath(shortcutPath);
            link.SetDescription(shortcutDescription);
            link.SetIconLocation($"{Program.LatiteInjectorFolder}\\Latite Injector.exe", 0);

            // save it
            IPersistFile file = link as IPersistFile;
            file.Save(targetPath, false);
        } // https://stackoverflow.com/a/14632782
    }
}