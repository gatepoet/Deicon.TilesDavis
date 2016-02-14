using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;

namespace TilesDavis.Core
{
    public class ShortcutTargetComparer : IEqualityComparer<Shortcut>
    {
        public bool Equals(Shortcut x, Shortcut y)
        {
            return x.TargetPath == y.TargetPath;
        }

        public int GetHashCode(Shortcut obj)
        {
            return obj.TargetPath.GetHashCode();
        }
    }
    public class TilesDavis
    {
        public string IgnoreFile { get; set; } = "ignore.txt";

        private const string LinkExtension = "*.lnk";

        public void AddToIgnoreList(params IgnoreEntry[] entries)
        {
            File.AppendAllLines(IgnoreFile, entries.Select(entry => entry.ToJson()));
        }

        public IEnumerable<Shortcut> LoadShortcuts()
        {
            string[] shortcutsToIgnore = File.Exists(IgnoreFile)
    ? File.ReadAllLines(IgnoreFile)
    : new string[0];
            var ignoreList = shortcutsToIgnore.Select(json => IgnoreEntry.ParseJson(json)).ToList();
            var files = new List<string>();
            files.AddRange(Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), LinkExtension, SearchOption.AllDirectories));
            files.AddRange(Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), LinkExtension, SearchOption.AllDirectories));
            //files.AddRange(Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), LinkExtension, SearchOption.AllDirectories));
            //files.AddRange(Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory), LinkExtension, SearchOption.AllDirectories));
            files.RemoveAll(filename => ignoreList.Any(entry => entry.IsMatch(filename)));
            return files.Select(Shortcut.Load)
                .Where(IsValidShortCut);
                //.Distinct(new ShortcutTargetComparer());
        }

        private static HashSet<string> ValidFilenames = new HashSet<string>{".exe", ".dll"};
        private static bool IsValidShortCut(Shortcut s)
        {
            if (!File.Exists(s.TargetPath))
                return false;
            if (!ValidFilenames.Contains(Path.GetExtension(s.TargetPath))) return false;
            return true;
        }
    }
}