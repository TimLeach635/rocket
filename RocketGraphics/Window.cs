using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RocketEngine;
using RocketEngine.Simulation.Specific;
using System;
using System.Diagnostics;

namespace RocketGraphics
{
  public class Window: GameWindow
  {
    private float _worldUnitsPerMetre = 4e-11f;

    private Sphere _sunSphere;
    private Sphere _mercurySphere;
    private Sphere _venusSphere;
    private Sphere _earthSphere;
    private Sphere _marsSphere;

    // common rendering
    private Matrix4 _view;
    private Matrix4 _projection;

    // simulation
    private Stopwatch _timer;
    private float _simDaysPerRealSecond = 50f;
    private InnerPlanets _simulation;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings) {}

    protected override void OnLoad()
    {
      base.OnLoad();

      // simulation setup
      _simulation = new InnerPlanets(DateTime.UtcNow);

      GL.ClearColor(0f, 0f, 0f, 1f);
      GL.Enable(EnableCap.DepthTest);

      float sunScale = 20;
      float planetScale = 500;

      _sunSphere = new Sphere(
        Constants.SUN_RADIUS * _worldUnitsPerMetre * sunScale,
        10,
        6,
        new Vector4(1f, 1f, 0f, 1f)
      );
      _sunSphere.Initialise();

      _mercurySphere = new Sphere(
        Constants.MERCURY_RADIUS * _worldUnitsPerMetre * planetScale,
        10,
        6,
        new Vector4(0.8f, 0.5f, 0.5f, 1f)
      );
      _mercurySphere.Initialise();

      _venusSphere = new Sphere(
        Constants.VENUS_RADIUS * _worldUnitsPerMetre * planetScale,
        10,
        6,
        new Vector4(1f, 0.8f, 0.8f, 1f)
      );
      _venusSphere.Initialise();

      _earthSphere = new Sphere(
        Constants.EARTH_RADIUS * _worldUnitsPerMetre * planetScale,
        10,
        6,
        new Vector4(0f, 1f, 0f, 1f)
      );
      _earthSphere.Initialise();

      _marsSphere = new Sphere(
        Constants.MARS_RADIUS * _worldUnitsPerMetre * planetScale,
        10,
        6,
        new Vector4(1f, 0f, 0f, 1f)
      );
      _marsSphere.Initialise();

      _timer = new Stopwatch();
      _timer.Start();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      // timing
      TimeSpan elapsed = _timer.Elapsed;
      _timer.Restart();
      TimeSpan simElapsed = elapsed * _simDaysPerRealSecond * 86400;
      _simulation.Simulation.Update(simElapsed);

      // transform
      _mercurySphere.Model = Matrix4.CreateTranslation(
        _simulation.Mercury.Position.X * _worldUnitsPerMetre,
        _simulation.Mercury.Position.Y * _worldUnitsPerMetre,
        _simulation.Mercury.Position.Z * _worldUnitsPerMetre
      );
      _venusSphere.Model = Matrix4.CreateTranslation(
        _simulation.Venus.Position.X * _worldUnitsPerMetre,
        _simulation.Venus.Position.Y * _worldUnitsPerMetre,
        _simulation.Venus.Position.Z * _worldUnitsPerMetre
      );
      _earthSphere.Model = Matrix4.CreateTranslation(
        _simulation.Earth.Position.X * _worldUnitsPerMetre,
        _simulation.Earth.Position.Y * _worldUnitsPerMetre,
        _simulation.Earth.Position.Z * _worldUnitsPerMetre
      );
      _marsSphere.Model = Matrix4.CreateTranslation(
        _simulation.Mars.Position.X * _worldUnitsPerMetre,
        _simulation.Mars.Position.Y * _worldUnitsPerMetre,
        _simulation.Mars.Position.Z * _worldUnitsPerMetre
      );

      _view = Matrix4.Identity;
      _view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-70f));
      _view *= Matrix4.CreateTranslation(0f, 0f, -17f);

      _projection = Matrix4.Identity;
      _projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);

      _sunSphere.Render(_view, _projection);
      _mercurySphere.Render(_view, _projection);
      _venusSphere.Render(_view, _projection);
      _earthSphere.Render(_view, _projection);
      _marsSphere.Render(_view, _projection);

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
