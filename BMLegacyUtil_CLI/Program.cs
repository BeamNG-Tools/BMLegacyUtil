using BMLegacyUtil.Functions;
using BMLegacyUtil.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMLegacyUtil {
    public class BuildInfo {
        public const string Name = "BMLegacyUtil";
        public const string Version = "1.0.0";
        public const string Author = "ThaNightHawk (Original Code: MintLily)";
        public static bool IsWindows;
        public static Version TargetDotNetVer = new("6.0.0");
    }

    internal class Program
    {

        internal static string _steamUsername, _steamPassword;
        private static string _stepInput, _versionInput;
        public static string _gamePath = string.Empty;
        public static FolderDialog.Bll.FolderDialog.ISelect FolderSelect = new FolderDialog.Bll.FolderDialog.Select();

        [STAThread]
        static void Main(string[] args) {
            BuildInfo.IsWindows = Environment.OSVersion.ToString().ToLower().Contains("windows");
            
            Con.BSL_Logo();
            Con.Log("This tool will allow you to easily downgrade BeamNG.");
            Con.Log("Brought to you by", "ThaNightHawk", ConsoleColor.DarkCyan);
            Con.Space();
            
            Con.Space();
            Con.WriteLineCentered("Hiya, people!");
            Con.Space();

            var showError = false;
            if (BuildInfo.IsWindows)
            {
                if (!Directory.Exists(Environment.CurrentDirectory + "\\Depotdownloader"))
                    showError = true;
            }
            else
            {
                MessageBox.Show("Linux is not supported at this time.", "Unsupported OS", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            }
            if (showError)
            {
                Con.Error("Please be sure you have extracted the files before running this!");
                Con.Error("Program will not run until you have extracted all the contents out of the ZIP file.");
                Con.Exit();
            }
            else BeginInputOption();
        }

        public static void BeginInputOption() {
            Con.WriteSeperator();

            var sys = Environment.Version;
            var tar = BuildInfo.TargetDotNetVer;

            if (!(sys.Major == tar.Major && sys.Minor == tar.Minor && sys.Build >= tar.Build)) {
                if (BuildInfo.IsWindows) {
                    MessageBox.Show("Make sure you have the required packages installed on your machine\n" +
                                    ".NET Desktop Runtime v6.0.0+: https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-6.0.11-windows-x64-installer \n" +
                                    "This MUST be installed in order to use this app properly.\n\n" +
                                    "If you already have just installed these, Press \"OK\" and ignore this message.",
                        "Required Libraries Needed", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                }
                else {
                    MessageBox.Show("Linux is not supported at this time.", "Unsupported OS", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                }
            }
            
            Con.Log("Select a step to get started");
            Con.InputOption("1", "\tDownload a version of BeamNG.drive");
            Con.Space();
            Con.InputOption("2", "\tExit Program");
            Con.Space();
            Con.Input();
            _stepInput = Console.ReadLine();
            Con.ResetColors();
            Con.Space();

            switch (_stepInput) {
                case "1":
                    SelectGameVersion(true);
                    break;
                case "2":
                case "c":
                    Process.GetCurrentProcess().Kill();
                    break;
                default:
                    Con.Error("Invalid input, please select 1-2");
                    BeginInputOption();
                    break;
            }
        }

        public static void SelectGameVersion(bool dlGame) {
            Con.Log("Select which version you'd like to use", "- Type \'cancel\' to go back");

            Dictionary<ushort, StringBuilder> sb = new();
            string lastMajor = "MintLily is cool!~", lastMinor = "Thank you for your work, BSLegacy Group"; // I didn't want to remove this.

            foreach (var v in Download.Info.Versions) {
                var s = $"  {v.Version}";
                var tVer = v.Version.Split('.');

                if (tVer.Length != 3)
                    continue;

                if (!sb.ContainsKey(v.year)) {
                    lastMajor = tVer[0];
                    lastMinor = tVer[1];
                    sb.Add(v.year, new StringBuilder(">\n\t"));
                }

                if (tVer[0] != lastMajor || tVer[1] != lastMinor) {
                    lastMajor = tVer[0];
                    lastMinor = tVer[1];
                    sb[v.year].Append($"\n\t{s}"); // AppendLine was borked
                } else
                    sb[v.year].Append(s);
            }

            Con.Space();
            foreach (var stringBuilder in sb)
                Console.WriteLine(stringBuilder.ToString().Replace("[", "").Replace(",", "").Replace("]", ""));
            Con.Space();

            Con.Input();
            _versionInput = Console.ReadLine();
            Con.ResetColors();
            Con.Space();

            if (_versionInput.ToLower().Contains('c'))
                BeginInputOption();

            if (dlGame)
                Download.DlGameAsync(_versionInput);
            else
                //nothing
                Con.Log("Didn't download? - Contact Hawk");
        }

        public static void InputSteamLogin()
        {
            Con.Log("Steam Username", "(not display name)");
            Con.SteamUN();
            _steamUsername = Console.ReadLine();
            Con.Log("Steam Password");//, "(press enter once before you start typing)");
            Con.SteamPW();
            _steamPassword = Console.ReadLine();
        }
    }
}
