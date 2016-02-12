using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace TilesDavis.Core
{
    public class TilesDavis
    {
        private const string LinkExtension = "*.lnk";
        public static IEnumerable<Shortcut> GetAllShortcuts(IEnumerable<string> shortcutsToIgnore)
        {
            var ignoreList = shortcutsToIgnore.Select(s => s = "^" + Regex.Escape(s).Replace(@"\*", ".*") + "$").ToList();

            var files = new List<string>();
            files.AddRange(Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), LinkExtension, SearchOption.AllDirectories));
            files.AddRange(Directory.EnumerateFiles(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), LinkExtension, SearchOption.AllDirectories));
            files.RemoveAll(filename => ignoreList.Any(i => Regex.IsMatch(filename, i)));
            return files.Select(Shortcut.Load)
                .Where(IsValidShortCut);
        }
        private static HashSet<string> ValidFilenames = new HashSet<string> { ".exe", ".dll", ".ico" };
        private static bool IsValidShortCut(Shortcut s)
        {
            if (!File.Exists(s.TargetPath)) return false;

            //if (!ValidFilenames.Contains(Path.GetExtension(s.TargetPath))) return false;

            return true;
        }
    }
}