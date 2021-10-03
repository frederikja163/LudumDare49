using System.Globalization;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace LudumDare49
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.Title = "Uns-table Haloween";
            using Window window = new Window(GameWindowSettings.Default, settings);
            
            window.Run();
        }
    }
}