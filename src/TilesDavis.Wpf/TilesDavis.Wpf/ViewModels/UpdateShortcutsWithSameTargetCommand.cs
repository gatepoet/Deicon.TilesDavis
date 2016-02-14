namespace TilesDavis.Wpf.ViewModels
{
    public class UpdateShortcutsWithSameTargetCommand
    {
        public UpdateShortcutsWithSameTargetCommand(ShortcutViewModel shortcut)
        {
            Shortcut = shortcut;
        }

        public ShortcutViewModel Shortcut
        {
            get;
            private set;
        }
    }
}