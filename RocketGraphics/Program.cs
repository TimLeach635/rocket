using System;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using RocketEngine;
using System.Collections.Generic;

namespace RocketGraphics
{
    class Program
    {
        static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "Rocket",
                Flags = ContextFlags.ForwardCompatible,
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
            /*
            // constants
            uint windowSize = 800;
            float pixelsPerMetre = 3e-5f;
            float earthRadius = 6.371e6f;
            Vector2f windowCentre = new Vector2f(windowSize/2, windowSize/2);
            float simSecondsPerRealSecond = 360f;

            // simulation setup
            OriginEarth earth = new OriginEarth();
            List<IGravitator> gravitators = new List<IGravitator> { earth };
            Rocket rocket = new Rocket(gravitators);

            // window setup
            ContextSettings settings = new ContextSettings();
            settings.AntialiasingLevel = 8;
            RenderWindow window = new RenderWindow(
                new VideoMode(windowSize, windowSize),
                "Earth",
                Styles.Default,
                settings
            );
            window.SetActive();

            // opengl setup
            GL.Viewport(0, 0, (int)window.Size.X, (int)window.Size.Y);
            GL.ClearColor(0f, 0f, 0f, 1f);
            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            _vertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(_vertexArrayObject);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            // earth sprite setup
            CircleShape earthSprite = new CircleShape(earthRadius * pixelsPerMetre);
            earthSprite.Origin = new Vector2f(earthRadius * pixelsPerMetre, earthRadius * pixelsPerMetre);
            earthSprite.SetPointCount(100);
            earthSprite.FillColor = new Color(64, 224, 208);
            earthSprite.Position = windowCentre;

            // rocket sprite setup
            float rocketSpriteRadius = 5f;
            CircleShape rocketSprite = new CircleShape(rocketSpriteRadius);
            rocketSprite.Origin = new Vector2f(rocketSpriteRadius, rocketSpriteRadius);
            rocketSprite.FillColor = Color.White;
            Vector2f GetCurrentRocketScreenPosition()
            {
                return windowCentre
                    + new Vector2f(
                        rocket.Location.X * pixelsPerMetre,
                        rocket.Location.Y * pixelsPerMetre
                    );
            }
            void UpdateRocketSpritePosition()
            {
                rocketSprite.Position = GetCurrentRocketScreenPosition();
            }

            // orbital path drawing setup
            int pastLocationsCount = 50000;
            LinkedList<Vertex> pastLocations = new LinkedList<Vertex>();
            void UpdateOrbitalPathQueue()
            {
                Vertex newVertex = new Vertex();
                newVertex.Position = GetCurrentRocketScreenPosition();
                newVertex.Color = Color.Red;
                pastLocations.AddLast(newVertex);
                if (pastLocations.Count > pastLocationsCount)
                {
                    pastLocations.RemoveFirst();
                }
            }
            UpdateOrbitalPathQueue();

            // event handlers
            window.Closed += (sender, e) => {
                Console.Out.WriteLine("Window closed manually");
                ((Window)sender).Close();
            };
            window.KeyPressed += (sender, e) => {
                if (e.Code == Keyboard.Key.Escape) {
                    Console.Out.WriteLine("Escape key pressed");
                    ((Window)sender).Close();
                }
            };
            window.Resized +=
                (sender, e) =>
                {
                    ((RenderWindow)sender).SetView(new View(new FloatRect(0, 0, e.Width, e.Height)));
                    GL.Viewport(0, 0, (int)e.Width, (int)e.Height);
                };

            // time handler
            Clock clock = new Clock();

            // main loop
            while (window.IsOpen)
            {
                // clear window
                window.Clear();

                // handle input
                window.DispatchEvents();

                // update engine
                Time realElapsed = clock.Restart();
                float simElapsed = realElapsed.AsSeconds() * simSecondsPerRealSecond;
                rocket.Update(simElapsed);

                // update graphics
                window.Draw(earthSprite);
                UpdateOrbitalPathQueue();
                window.Draw(pastLocations.ToArray(), SFML.Graphics.PrimitiveType.LineStrip);
                UpdateRocketSpritePosition();
                window.Draw(rocketSprite);

                // update window
                window.Display();
            }
            */
        }
    }
}
