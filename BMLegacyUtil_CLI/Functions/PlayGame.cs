using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using BMLegacyUtil.Utilities;
using static BMLegacyUtil.Program;
using Newtonsoft.Json;
using System.Linq;

namespace BMLegacyUtil.Functions
{

    //Came from https://github.com/RiskiVR/BSLegacyLauncher/blob/master/Assets/Scripts/LaunchBS.cs
    public class PlayGame
    {
        //static Values get = JSONSetup.get();
        
        public static void LaunchGame(bool oculusMode) {
            var p = new Process();

            var temp = BuildInfo.IsWindows ? $"{Environment.CurrentDirectory}\\" : $"{AppDomain.CurrentDomain.BaseDirectory}";
            p.StartInfo = new ProcessStartInfo($"{temp}Beat Saber\\Beat Saber.exe", oculusMode ? "-vrmode oculus " : "") {
                UseShellExecute = false,
                WorkingDirectory = $"{temp}Beat Saber",
            };

            try {
                p.StartInfo.Environment["SteamAppId"] = "620980";
                p.Start();
            }
            catch (Exception e) {
                Con.ErrorException(e.StackTrace, e.ToString());
            }
        }
    }
}
