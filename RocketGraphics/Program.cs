using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace RocketGraphics
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings
            {
                Size = new Vector2i(800, 600),
                Title = "Rocket",
                Flags = ContextFlags.ForwardCompatible
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}