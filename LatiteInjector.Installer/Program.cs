﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LatiteInjector.Installer
{
    internal class Program
    {
        public static readonly string LatiteInjectorFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\LatiteInjector";

        private static async Task Main(string[] args)
        {
            Console.Title = "Latite Injector Installer";

            if (!Environment.Is64BitOperatingSystem)
            {
                Utils.WriteColor(
                    "It looks like you're running a 32 bit OS/Computer.\nSadly, you cannot use Latite Client with a 32 bit OS/Computer.\nPlease do not report this as a bug, make a ticket, or ask how to switch to 64 bit in the Discord, you cannot use Latite Client AT ALL!!!",
                    ConsoleColor.Red);
                Console.ReadLine();
                Environment.Exit(1);
            }

            Utils.WriteColor("Welcome to the Latite Injector Installer!", ConsoleColor.White);
            Utils.WriteColor("The installer will now start..", ConsoleColor.White);
            Thread.Sleep(4000);

            Console.Clear();
            Utils.WriteColor("[1/3] Installing .NET 8\n", ConsoleColor.White);

            if (Utils.IsNet8Installed())
            {
                Utils.WriteColor(".NET 8 is already installed. Skipping .NET 8 installation", ConsoleColor.Green);
                Thread.Sleep(4000);
            }
            else
            {
                Utils.WriteColor(
                    "It looks like .NET 8, which Latite Injector needs to run, isn't installed on your system. Installing .NET 8 now..",
                    ConsoleColor.Red);
                string downloadURL =
                    "https://download.visualstudio.microsoft.com/download/pr/0ff148e7-bbf6-48ed-bdb6-367f4c8ea14f/bd35d787171a1f0de7da6b57cc900ef5/windowsdesktop-runtime-8.0.5-win-x64.exe";
                string downloadPath = Path.Combine(Path.GetTempPath(), "dotnet8.exe");
                Utils.WriteColor("Downloading the .NET 8 installer..", ConsoleColor.Yellow);
                await Utils.DownloadFile(new Uri(downloadURL), downloadPath);
                Utils.WriteColor("Downloaded .NET 8 installer!", ConsoleColor.Green);

                Process dotnetInstaller = Process.Start(new ProcessStartInfo
                {
                    FileName = downloadPath,
                    Arguments = "/q" // run installer quietly
                });

                dotnetInstaller?.WaitForExit();
                File.Delete(downloadPath);
                if (Utils.IsNet8Installed())
                {
                    Utils.WriteColor(".NET 8 has been installed!", ConsoleColor.Green);
                    Thread.Sleep(4000);
                }
                else
                {
                    Utils.WriteColor(".NET 8 installation has failed in some way. Please try manually installing .NET 8", ConsoleColor.Red);
                    Console.ReadLine();
                }
            }

            Console.Clear();
            Utils.WriteColor("[2/3] Downloading Latite Injector\n", ConsoleColor.White);

            if (Directory.Exists(LatiteInjectorFolder))
                Utils.WriteColor("The Latite Injector directory already exists, no need to create a new one", ConsoleColor.DarkGray);
            else if (!Directory.Exists(LatiteInjectorFolder))
            {
                Utils.WriteColor("The Latite Injector directory does not exist. Creating it now..", ConsoleColor.Yellow);
                Directory.CreateDirectory(LatiteInjectorFolder);
                Utils.WriteColor("Created directory!", ConsoleColor.Green);
            }

            Uri latiteInjectorLink = new("https://plextora.is-from.space/r/Latite_Injector.exe"); // tixte link is temporary
            Utils.WriteColor("Downloading Latite Injector..", ConsoleColor.Yellow);
            await Utils.DownloadFile(latiteInjectorLink, $"{LatiteInjectorFolder}\\Latite Injector.exe");
            Utils.WriteColor($"Downloaded Latite Injector to directory {LatiteInjectorFolder}!", ConsoleColor.Green);

            Thread.Sleep(4000);

            Console.Clear();
            Utils.WriteColor("[3/3] Extra", ConsoleColor.White);

            Utils.WriteColor("Do you want to create a shortcut to Latite Injector on your Desktop?", ConsoleColor.White);
            Utils.WriteColor("For yes, type and enter \"yes\" for no, type and enter \"no\" ", ConsoleColor.White);
            Console.Write("> ");

            if (Console.ReadLine() == "yes")
            {
                Utils.CreateShortcut($"{LatiteInjectorFolder}\\Latite Injector.exe",
                    "Latite Client's new and improved injector!",
                    "$\"{LatiteInjectorFolder}\\\\Latite Injector.exe",
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                        "Latite Injector.lnk"));

                Utils.WriteColor("Added shortcut to Desktop!", ConsoleColor.Green);
            }

            Utils.WriteColor(
                $"Latite Injector's installation has completed!\nLatite Injector's exe is now located in {LatiteInjectorFolder}.",
                ConsoleColor.Green);
            Utils.WriteColor("Press any key to close this window.", ConsoleColor.White);

            Console.ReadLine();
        }
    }
}