using System.Globalization;
using OpenTK.Windowing.Desktop;

namespace LudumDare49
{
    internal static class Program
    {
        internal static void Main(string[] args)
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            using Window window = new Window(GameWindowSettings.Default, NativeWindowSettings.Default);
            
            window.Run();
        }
    }
}