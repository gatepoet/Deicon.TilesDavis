using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TilesDavis.Core
{
    [XmlType(TypeName = "Application")]
    public class Manifest
    {
        [XmlIgnore]
        protected string Square150x150LogoFilename { get; set; } = ".tilesdavis_logo.png";

        [XmlIgnore]
        protected XmlWriterSettings XmlWriterSettings { get; set; } = new XmlWriterSettings { Indent = true, NewLineOnAttributes = true };

        [XmlIgnore]
        public string ManifestPath { get; protected set; }

        public VisualElements VisualElements { get; set; }

        /// <summary>
        /// Creates a new Manifest
        /// </summary>
        /// <param name = "targetPath">The full path of the target executable to create a Manifest for.</param>
        public Manifest(string targetPath) : this()
        {
            ManifestPath = GetManifestPath(targetPath);
        }

        protected Manifest()
        {
            VisualElements = new VisualElements();
        }

        private static string GetManifestPath(string targetPath)
        {
            var folder = Path.GetDirectoryName(targetPath);
            var executable = Path.GetFileNameWithoutExtension(targetPath);
            var manifestPath = $@"{folder}\{executable}.visualelementsmanifest.xml";
            return manifestPath;
        }

        public static Manifest Load(string targetPath)
        {
            var manifestPath = GetManifestPath(targetPath);
            if (!File.Exists(manifestPath))
                return null;
            try
            {
                using (var reader = XmlReader.Create(manifestPath))
                {
                    var ser = new XmlSerializer(typeof(Manifest));
                    var manifest = (Manifest)ser.Deserialize(reader);
                    manifest.ManifestPath = manifestPath;
                    manifest.VisualElements.Square150x150Logo = manifest.GetFullPath(manifest.VisualElements.Square150x150Logo);
                    return manifest;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading file '{manifestPath}'. Message: {ex.Message}");
                return null;
            }
        }

        public static void Delete(string targetPath)
        {
            var manifestPath = GetManifestPath(targetPath);
            if (File.Exists(manifestPath))
                File.Delete(manifestPath);
        }

        private string GetFullPath(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;
            var path = Path.IsPathRooted(imagePath) ? imagePath : Path.Combine(Path.GetDirectoryName(ManifestPath), imagePath);
            return path;
        }

        public bool HasSquare150x150Logo => !string.IsNullOrWhiteSpace(VisualElements.Square150x150Logo) && File.Exists(VisualElements.Square150x150Logo);
        public virtual void Save()
        {
            var tilesDir = Path.GetDirectoryName(ManifestPath);
            if (HasSquare150x150Logo)
            {
                CopyIfNew(VisualElements.Square150x150Logo, Path.Combine(tilesDir, Square150x150LogoFilename));
                VisualElements.Square150x150Logo = VisualElements.Square70x70Logo = Square150x150LogoFilename;
            }
            else
            {
                VisualElements.Square150x150Logo = VisualElements.Square70x70Logo = null;
            }

            using (var writer = XmlWriter.Create(ManifestPath, XmlWriterSettings))
            {
                new XmlSerializer(typeof(Manifest)).Serialize(writer, this);
            }
        }

        private static void CopyIfNew(string logoPath, string targetPath)
        {
            if (logoPath != targetPath)
                File.Copy(logoPath, targetPath, true);
        }
    }
}