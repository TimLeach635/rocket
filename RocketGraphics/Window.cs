using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using OpenTK.Mathematics;
using System.Collections.Generic;
using RocketEngine;
using System;

namespace RocketGraphics
{
  public class Window: GameWindow
  {
    private float _worldUnitsPerMetre = 1e-7f;
    private float _earthRadius = 6.371e6f;

    // earth rendering
    private Sphere _earthSphere;
    private Texture _earthTexture;

    // rocket rendering
    private Iss _issModel;
    private Texture _issTexture;

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
      Orbit orbit = new Orbit(
        0.0003938f,
        _earthRadius + (417000 + 423000) / 2,
        51.6444f,
        MathHelper.DegreesToRadians(38.4733f),
        MathHelper.DegreesToRadians(153.2242f),
        MathHelper.DegreesToRadians(27.0427f),
        new DateTime(2021, 10, 29, 12, 34, 51)
      );
      DateTime now = DateTime.UtcNow;
      _rocket = new Rocket(
        _gravitators,
        orbit.GetPositionFromGravitator(_earth, now),
        orbit.GetVelocityFromGravitator(_earth, now)
      );

      GL.ClearColor(0f, 0f, 0f, 1f);
      GL.Enable(EnableCap.DepthTest);

      _earthTexture = Texture.LoadFromFile("resources/textures/8k_earth_daymap.jpg");
      _earthTexture.Use(TextureUnit.Texture0);
      _earthSphere = new Sphere(
        _earthRadius * _worldUnitsPerMetre,
        200,
        100,
        new Vector4(0f, 1f, 0f, 1f),
        _earthTexture,
        TextureUnit.Texture0
      );
      _earthSphere.Initialise();

      _issTexture = Texture.LoadFromFile("resources/textures/iss.png");
      _issTexture.Use(TextureUnit.Texture1);
      _issModel = new Iss(
        5e-4f,
        _issTexture,
        TextureUnit.Texture1
      );
      _issModel.Initialise();

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
      _issModel.Model = Matrix4.Identity;
      _issModel.Model *= Matrix4.CreateTranslation(
        _rocket.Position.X * _worldUnitsPerMetre,
        _rocket.Position.Y * _worldUnitsPerMetre,
        _rocket.Position.Z * _worldUnitsPerMetre
      );
      _earthSphere.Model *= Matrix4.CreateRotationZ((float)elapsed / 5);

      _view = Matrix4.Identity;
      _view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-70f));
      _view *= Matrix4.CreateTranslation(0f, 0f, -2f);

      _projection = Matrix4.Identity;
      _projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);

      // render earth
      _earthSphere.Render(_view, _projection);

      // render rocket
      _issModel.Render(_view, _projection);

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
