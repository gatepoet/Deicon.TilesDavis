using Shell32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TilesDavis.Core;

namespace TilesDavis.AddShortcut
{
    [RunInstaller(true)]
    public partial class ShortcutInstaller : System.Configuration.Install.Installer
    {
        public ShortcutInstaller()
        {
            InitializeComponent();
        }
        private const string ShortcutFilename = "TilesDavis.lnk";
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            try
            {
                string targetDir = Path.GetDirectoryName(Context.Parameters["assemblypath"]);
                var targetPath = Path.Combine(targetDir, "TilesDavis.exe");

                var shortcutPath = Path.Combine(targetDir, ShortcutFilename);
                CreateShortcut(targetDir, targetPath, shortcutPath);

                var folder = GetProgramsFolder();
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                var newShortcutPath = Path.Combine(folder, ShortcutFilename);
                if (File.Exists(newShortcutPath))
                    File.Delete(newShortcutPath);

                File.Move(shortcutPath, newShortcutPath);

                PinToStartMenu(ShortcutFilename, folder);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("DeiCon.TilesDavis", ex.ToString(), EventLogEntryType.Error);
                this.Rollback(stateSaver);
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            var shortcutPath = Path.Combine(GetProgramsFolder(), ShortcutFilename);
            if (File.Exists(shortcutPath)) {
                File.Delete(shortcutPath);
            }
        }

        private string GetProgramsFolder()
        {
            var programsFolder =  (Context.Parameters["allusers"] == "1")
                ? Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms)
                : Environment.GetFolderPath(Environment.SpecialFolder.Programs);

            return Path.Combine(programsFolder, "DeiCon");
        }

        private static void CreateShortcut(string targetDir, string targetPath, string shortcutPath)
        {
            var shortcut = Shortcut.Load(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = targetDir;
            shortcut.Description = "TilesDavis";
            shortcut.IconLocation = new IconLocation(targetPath);
            shortcut.Manifest = shortcut.CreateManifest();
            shortcut.Manifest.TilePath = Path.Combine(targetDir, ".tilesdavis_logo.png");
            shortcut.Manifest.VisualElements.BackgroundColor = "#000000";

            shortcut.Save();
        }

        private static void PinToStartMenu(string shortcutFilename, string folder)
        {
            var shell = new Shell32.Shell();
            var oFolder = shell.NameSpace(folder);
            var oItem = oFolder.ParseName(shortcutFilename);
            foreach (FolderItemVerb verb in oItem.Verbs())
            {
                if (verb.Name.Replace("&", "") == "Pin to Start Menu")
                {
                    verb.DoIt();
                }
            }
        }
    }
}
