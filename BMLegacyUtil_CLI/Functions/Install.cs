using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using BMLegacyUtil.Utilities;
using Microsoft.VisualBasic.FileIO;
using static BMLegacyUtil.Program;

namespace BMLegacyUtil.Functions
{
    public class Install
    {
        public static void InstallGame()
        {
            Con.Space();
            ConsoleColor foregroundColor = Console.ForegroundColor;
            Con.Log("This Feature has been removed from the program");
            Con.Log("This was removed due to steam always auto-updating the game and causing frustrations within the community");
            Con.Log("Once you download the game, you can then press", "option 5", ConsoleColor.Cyan,
                $"and the run the game from within the {BuildInfo.Name} Folder", foregroundColor);
            Con.Space();
            BeginInputOption();
        }
        
        public static void AskForPath()
        {
            Con.Space();
            Con.Log("Current game path is ", NotFoundHandler(), ConsoleColor.Yellow);
            Con.Log("Would you like to change this?", " [Y/N]", ConsoleColor.Yellow);
            Con.Input();
            string changeLocalation = Console.ReadLine();

            if (changeLocalation == "Y" || changeLocalation == "y" || changeLocalation == "YES" || changeLocalation == "yes" || changeLocalation == "Yes")
            {
                try
                {
                    FolderSelect.InitialFolder = NotFoundHandler();
                    var dialogResult = FolderSelect.ShowDialog();
                    _gamePath = FolderSelect.Folder;
                }
                catch { Con.Error("Select Folder Failed"); BeginInputOption(); }
            }
            else _gamePath = NotFoundHandler();
        }
        public static string NotFoundHandler() {
            bool found = false;
            while (found == false) {
                using (var folderDialog = new OpenFileDialog()) {
                    folderDialog.Title = "Select BeamNG.drive.exe";
                    folderDialog.FileName = "BeamNG.drive.exe";
                    folderDialog.Filter = "BeamNG Executable|BeamNG.drive.exe";
                    if (folderDialog.ShowDialog() == DialogResult.OK) {
                        string path = folderDialog.FileName;
                        if (path.Contains("BeamNG.drive.exe")) {
                            string pathedited = path.Replace(@"\BeamNG.drive.exe", "");
                            PathLogic.installPath = pathedited;
                            return pathedited;
                        } else {
                            MessageBox.Show("The directory you selected doesn't contain BeamNG.drive.exe! please try again!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } else {
                        return null;
                    }
                }
            }
            return string.Empty;
        }
    }
}
