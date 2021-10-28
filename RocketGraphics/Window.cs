using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using OpenTK.Mathematics;
using System.Collections.Generic;
using RocketEngine;

namespace RocketGraphics
{
  public class Window: GameWindow
  {
    private float _worldUnitsPerMetre = 1e-7f;
    private float _earthRadius = 6.371e6f;

    // earth rendering
    private Sphere _earthSphere;

    // rocket rendering
    private Sphere _rocketSphere;

    // common rendering
    private Matrix4 _view;
    private Matrix4 _projection;

    // simulation
    private Stopwatch _timer;
    private float _simSecondsPerRealSecond = 360f;
    private OriginEarth _earth;
    private List<IGravitator> _gravitators;
    private Rocket _rocket;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings) {}

    protected override void OnLoad()
    {
      base.OnLoad();

      // simulation setup
      _earth = new OriginEarth();
      _gravitators = new List<IGravitator> { _earth };
      _rocket = new Rocket(_gravitators);

      GL.ClearColor(0f, 0f, 0f, 1f);
      GL.Enable(EnableCap.DepthTest);

      _earthSphere = new Sphere(
        _earthRadius * _worldUnitsPerMetre,
        20,
        10,
        new Vector4(0f, 1f, 0f, 1f)
      );
      _earthSphere.Initialise();

      _rocketSphere = new Sphere(
        _earthRadius * _worldUnitsPerMetre / 20,
        10,
        6,
        new Vector4(1f, 1f, 1f, 1f)
      );
      _rocketSphere.Initialise();

      _timer = new Stopwatch();
      _timer.Start();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      // timing
      double elapsed = _timer.Elapsed.TotalSeconds;
      _timer.Restart();
      float simElapsed = (float)elapsed * _simSecondsPerRealSecond;
      _rocket.Update(simElapsed);

      // transform
      _rocketSphere.Model = Matrix4.Identity;
      _rocketSphere.Model *= Matrix4.CreateTranslation(
        _rocket.Location.X * _worldUnitsPerMetre,
        _rocket.Location.Y * _worldUnitsPerMetre,
        _rocket.Location.Z * _worldUnitsPerMetre
      );

      _view = Matrix4.Identity;
      _view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-70f));
      _view *= Matrix4.CreateTranslation(0f, 0f, -2f);

      _projection = Matrix4.Identity;
      _projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);

      // render earth
      _earthSphere.Render(_view, _projection);

      // render rocket
      _rocketSphere.Render(_view, _projection);

      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      var input = KeyboardState;

      if (input.IsKeyDown(Keys.Escape))
      {
        Close();
      }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);
    }
  }
}
