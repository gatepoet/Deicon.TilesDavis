using IWshRuntimeLibrary;
using System;
using System.Drawing;
using System.IO;
using TAFactory.IconPack;

namespace TilesDavis.Core
{
    public class Shortcut
    {
        protected IWshShortcut link;
        public Manifest Manifest { get; set; }
        public string ShortcutPath { get; protected set; }
        private Shortcut(IWshShortcut link, string shortcutPath)
        {
            ShortcutPath = shortcutPath;
            Initialize(link);
        }

        private void Initialize(IWshShortcut link)
        {
            this.link = link;
            if (link.IconLocation.StartsWith(",")) link.IconLocation = link.TargetPath + link.IconLocation;
            Manifest = GetManifest();
        }

        public Manifest CreateManifest()
        {
            return new Manifest(TargetPath);
        }
        public void Reload()
        {
            var link = new WshShell().CreateShortcut(ShortcutPath) as IWshShortcut;
            Initialize(link);
        }
        public static Shortcut Load(string shortcutPath)
        {
            var link = new WshShell().CreateShortcut(shortcutPath) as IWshShortcut;
            return new Shortcut(link, shortcutPath);
        }

        public string DisplayName => Path.GetFileNameWithoutExtension(FullName);

        public string FullName => link.FullName;

        public string TargetPath {
            get { return link.TargetPath; }
            set { link.TargetPath = value; }
        }

        public string WorkingDirectory
        {
            get { return link.WorkingDirectory; }
            set { link.WorkingDirectory = value; }
        }

        public string Arguments
        {
            get { return link.Arguments; }
            set { link.Arguments = value; }
        }

        public string Description
        {
            get { return link.Description; }
            set { link.Description = value; }
        }

        public string Hotkey
        {
            get { return link.Hotkey; }
            set { link.Hotkey = value; }
        }

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

        public IconLocation IconLocation
        {
            get { return IconLocation.ParseComString(link.IconLocation); }
            set { link.IconLocation = value.ToComString(); }
        }

        public WindowStyle WindowStyle
        {
            get { return (WindowStyle) link.WindowStyle; }
            set { link.WindowStyle = (int) value; }
        }

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

        public void ReloadManifest()
        {
            Manifest = GetManifest();
        }
    }
}