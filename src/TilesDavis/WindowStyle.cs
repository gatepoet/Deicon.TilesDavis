namespace TilesDavis.Core
{
    public enum WindowStyle
    {
        Restore = 1, //Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position.
        Maximize = 3, //Activates the window and displays it as a maximized window.
        Minimize = 7, //Minimizes the window and activates the next top-level window.
    }
}