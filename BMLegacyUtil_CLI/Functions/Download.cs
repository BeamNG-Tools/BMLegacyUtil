using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using static BMLegacyUtil.Program;

namespace BMLegacyUtil.Functions {
    public class BmLegacyVersions {
        public List<Versions> Versions { get; set; }
    }

    public class Versions {
        public string Version { get; set; }
        public string manifestID { get; set; }
        public ushort year { get; set; }
    }


    public class Download
    {
        private static readonly string Jsonurl = "https://raw.githubusercontent.com/BeamNG-Tools/BMLegacyUtil/main/BMLegacyUtil_CLI/Resources/Versions.json";
        private static string _manifestId = "";
        private static string _gameVersion = string.Empty;
        private static Process _download;

        private static BmLegacyVersions LoadJson(string file) {
            try {
                var client = new HttpClient();
                var f = client.GetStringAsync(file).GetAwaiter().GetResult();
                var d = JsonConvert.DeserializeObject<BmLegacyVersions>(f);
                client.Dispose();
                if (d == null) throw new Exception();
                return d;
            }
            catch (Exception e) {
                Con.ErrorException(e.StackTrace, e.ToString());
                return null;
            }
        }

        public static BmLegacyVersions Info { get; set; } = LoadJson(Jsonurl);

        private static string GetManifestFromVersion(string input) {
            Info ??= LoadJson(Jsonurl);
            var _ = Info.Versions.FirstOrDefault(v => v.Version == input);
            return _ == null ? "" : _.manifestID;
        }

        private static ushort GetYearFromVersion(string input) {
            Info ??= LoadJson(Jsonurl);
            var _ = Info.Versions.FirstOrDefault(v => v.Version == input);
            return _?.year ?? (ushort)1970;
        }

        public static void DlGameAsync(string gameVersionInput)
        {
            _gameVersion = gameVersionInput;
            //bool faulted = false;
            if (BuildInfo.IsWindows) {
                if (!Directory.Exists(Environment.CurrentDirectory + "/BeamNG.drive"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "/BeamNG.drive");
            } else {
                if (!Directory.Exists($"{AppDomain.CurrentDomain.BaseDirectory}BeamNG.drive"))
                    Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}BeamNG.drive");
            }

            try {

                _manifestId = GetManifestFromVersion(gameVersionInput);

            } catch (Exception @switch) {
                //faulted = true;
                //Con.Error(@switch.ToString());
                Con.ErrorException(@switch.StackTrace, @switch.ToString());
                Con.Error("Invalid input. Please input a valid version number.");
                Con.Space();
                SelectGameVersion(true);
            }

            Con.Log($"Game Version: {gameVersionInput} => [{_manifestId}] from year {GetYearFromVersion(gameVersionInput)}");

            if (string.IsNullOrWhiteSpace(_manifestId)) return;
            InputSteamLogin();
            try { // Program from https://github.com/SteamRE/DepotDownloader
                Con.Space();
                if (BuildInfo.IsWindows) {
                    _download = Process.Start("dotnet",
                        $" \"{Environment.CurrentDirectory}{Path.DirectorySeparatorChar}Depotdownloader{Path.DirectorySeparatorChar}DepotDownloader.dll\" -app 284160 -depot 284161 -manifest {_manifestId}" +
                        $" -username {_steamUsername} -password {_steamPassword} -dir \"BeamNG.drive\" -validate");
                }
                else {
                    _download = Process.Start("dotnet",
                        $" \"{AppDomain.CurrentDomain.BaseDirectory}/Depotdownloader/DepotDownloader.dll\" -app 284160 -depot 284161 -manifest {_manifestId}" +
                        $" -username {_steamUsername} -password {_steamPassword} -dir \"BeamNG.drive\" -validate");
                }

                if (_download == null) return;
                _download.WaitForExit();
                Con.Space();
                Con.LogSuccess(string.IsNullOrWhiteSpace(_gameVersion)
                    ? "Finished downloading BeamNG.drive"
                    : $"Finished downloading BeamNG.drive {_gameVersion}");

                Con.Space();
                Con.Log("Would you like to continue? [Y/N]");
                Con.Input();
                var @continue = Console.ReadLine();

                if (@continue.ToLower().Contains("yes") || @continue.ToLower().Contains("y"))
                    BeginInputOption();
                else
                    Utilities.Utilities.Kill();
            }
            catch (Exception downgrade) {
                Con.ErrorException(downgrade.StackTrace, downgrade.ToString());
            }
        }
    }
}
