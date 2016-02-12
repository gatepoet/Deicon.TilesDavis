using IWshRuntimeLibrary;
using System;
using System.Drawing;
using System.IO;
using TAFactory.IconPack;

namespace TilesDavis.Core
{
    public class Shortcut
    {
        protected readonly IWshShortcut link;
        public Manifest Manifest { get; set; }
        public string ShortcutPath { get; protected set; }
        private Shortcut(IWshShortcut link, string shortcutPath)
        {

            this.link = link;
            ShortcutPath = shortcutPath;
            if (link.IconLocation.StartsWith(",")) link.IconLocation = link.TargetPath + link.IconLocation;
            Manifest = GetManifest();
        }

        public Manifest CreateManifest()
        {
            return new Manifest(TargetPath);
        }
        public static Shortcut Load(string shortcutPath)
        {
            var link = new WshShell().CreateShortcut(shortcutPath) as IWshShortcut;
            return new Shortcut(link, shortcutPath);
        }

        public string DisplayName => Path.GetFileNameWithoutExtension(FullName);

        public string FullName => link.FullName;

        public string TargetPath => link.TargetPath;

        public string WorkingDirectory => link.WorkingDirectory;

        public string Arguments => link.Arguments;

        public string Description => link.Description;

        public string Hotkey => link.Hotkey;
        
        public Icon LoadIcon()
        {
            var iconPath = IconLocation.Filename;
            var iconIndex = IconLocation.Index;
            if (iconIndex < 0)
                iconIndex = 0;
            try
            {
                return IconHelper.ExtractIcon(iconPath, iconIndex) ?? IconHelper.ExtractIcon(FullName, 0);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message + Environment.NewLine + ex.InnerException?.Message);
                return null;
            }
        }

        public IconLocation IconLocation => IconLocation.ParseComString(link.IconLocation);

        public WindowStyle WindowStyle => (WindowStyle) link.WindowStyle;

        public bool HasManifest => Manifest != null;

        private Manifest GetManifest()
        {
            return System.IO.File.Exists(TargetPath)
                ? Manifest.Load(TargetPath)
                : null;
        }

        public void Save()
        {
            if (HasManifest) Manifest.Save();
            else if (GetManifest() != null) Manifest.Delete(TargetPath);
            link.Save();
        }
    }
}