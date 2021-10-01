using System.IO;
using System.Reflection;

namespace LudumDare49
{
    public static class Assets
    {
        public static Stream LoadAsset(string path)
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream(nameof(LudumDare49) + ".Assets." + path);
        }
    }
}