using Microsoft.Win32;
using System;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TilesDavis.Core;
using System.IO;
using ReactiveUI;
using System.Linq.Expressions;
using System.ComponentModel;
using TilesDavis.Wpf.Util;

namespace TilesDavis.Wpf.ViewModels
{
    public static class NotificationExtenisons
    {
        public static void Bubble<T>(Expression<Func<T>> property, Expression<Func<T>>[] target)where T : INotifyPropertyChanged
        {
        }
    }

    public class ShortcutViewModel : ViewModel
    {
        private Shortcut shortcut;
        private Manifest manifest;
        public ShortcutViewModel(Shortcut shortcut)
        {
            this.shortcut = shortcut;
            this.manifest = shortcut.Manifest ?? shortcut.CreateManifest();
            SelectTileCommand = new ActionCommand(SelectNewTile);
            RemoveTileCommand = new ActionCommand(RemoveTile);
            RemoveAllModificationsCommand = new ActionCommand(RemoveAllModifications);
            ToggleColorPickerCommand = new ActionCommand(() => ColorPickerVisible = !ColorPickerVisible);
            SaveCommand = new ActionCommand(Save);
            ShortcutPath = shortcut.ShortcutPath;

            this.icon = new Lazy<BitmapSource>(() =>
            {
                using (var i = shortcut.LoadIcon())
                {
                    return i.ToBitmapSource();
                }
            });
        }

        private void RemoveAllModifications()
        {
            shortcut.Manifest = null;
        }

        public ICommand SelectTileCommand { get; set; }

        public ICommand ToggleColorPickerCommand { get; set; }

        public ICommand RemoveTileCommand { get; set; }

        public ICommand RemoveAllModificationsCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        private void SelectNewTile()
        {
            var dialog = CreateOpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName != manifest.VisualElements.Square150x150Logo)
                {
                    manifest.VisualElements.Square150x150Logo = dialog.FileName;
                    Tile = GetImage();
                }
            }
        }

        private BitmapSource GetImage()
        {
            if (manifest.HasSquare150x150Logo)
            {
                return LoadImage(manifest.VisualElements.Square150x150Logo);
            }
            else
                return Icon;
        }

        private BitmapSource GetIcon()
        {
            using (var icon = shortcut.LoadIcon())
            {
                if (icon == null)
                    return null;
                var ms = new MemoryStream();
                icon.Save(ms);
                var ico = new IconBitmapDecoder(ms, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                return ico.Frames[0];
            }
        }

        private void RemoveTile()
        {
            manifest.VisualElements.Square150x150Logo = null;
            Tile = null;
        }

        private void Save()
        {
            shortcut.Manifest = manifest;
            shortcut.Save();
        }

        private static FileDialog CreateOpenFileDialog()
        {
            return new OpenFileDialog{DefaultExt = ".png", Filter = "PNG images|*.png;", Title = "Please select a tile image.", Multiselect = false};
        }

        private Lazy<Color> accentColor = new Lazy<Color>(() => ImmersiveColors.ImmersiveStartSystemTilesBackground.Get());
        public Color AccentColor => accentColor.Value;
        public bool UseWindowsAccent
        {
            get
            {
                return manifest.VisualElements.BackgroundColor == IconBackgroundColor.Transparent;
            }

            set
            {
                manifest.VisualElements.BackgroundColor = value ? IconBackgroundColor.Transparent : AccentColor.ToHexString();
                this.RaisePropertyChanged(nameof(UseWindowsAccent));
                this.RaisePropertyChanged(nameof(BackgroundColor));
                this.RaisePropertyChanged(nameof(ColorPickerVisible));
            }
        }

        public string ShortcutPath
        {
            get;
            private set;
        }

        public bool ShowNameOnSquare150x150Logo
        {
            get
            {
                return manifest.VisualElements.ShowNameOnSquare150x150Logo == ShowName.On;
            }

            set
            {
                manifest.VisualElements.ShowNameOnSquare150x150Logo = value ? ShowName.On : ShowName.Off;
                this.RaisePropertyChanged(nameof(ShowNameOnSquare150x150Logo));
            }
        }

        private bool colorPickerVisible;
        public bool ColorPickerVisible
        {
            get
            {
                return !UseWindowsAccent && colorPickerVisible;
            }

            set
            {
                if (value == colorPickerVisible)
                    return;
                colorPickerVisible = value;
                this.RaisePropertyChanged(nameof(ColorPickerVisible));
            }
        }

        public Brush BackgroundColor
        {
            get
            {
                return UseWindowsAccent ? new SolidColorBrush(AccentColor) : (Brush)new BrushConverter().ConvertFromString(manifest.VisualElements.BackgroundColor);
            }

            set
            {
                var brush = value as SolidColorBrush;
                if (brush == null)
                    return;
                manifest.VisualElements.BackgroundColor = brush.Color.ToHexString();
                this.RaisePropertyChanged(nameof(UseWindowsAccent));
                this.RaisePropertyChanged(nameof(BackgroundColor));
            }
        }

        public string Name => shortcut.DisplayName;
        private BitmapSource tile;
        public BitmapSource Tile
        {
            get
            {
                return tile ?? (tile = GetImage() ?? Icon);
            }

            set
            {
                this.RaiseAndSetIfChanged(ref tile, value);
                this.RaisePropertyChanged(nameof(HasTile));
            }
        }

        private Lazy<BitmapSource> icon;
        public BitmapSource Icon
        {
            get
            {
                return icon.Value;
            }
        }

        private void TileUpdated()
        {
            this.RaisePropertyChanged(nameof(Tile));
            this.RaisePropertyChanged(nameof(HasTile));
        }

        private BitmapImage LoadImage(string uri)
        {
            if (uri == null)
                return null;
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.UriSource = new Uri(uri);
            image.EndInit();
            return image;
        }

        public bool HasTile => manifest.HasSquare150x150Logo;
        public string Target => $"{shortcut.TargetPath} {shortcut.Arguments}";
    }
}