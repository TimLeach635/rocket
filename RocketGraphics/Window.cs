using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using OpenTK.Mathematics;
using System.Collections.Generic;
using RocketEngine;
using System;
using RocketGraphics.Camera;

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
    private LockedCamera _camera;
    private float _horizontalCameraSensitivity = 1f;
    private float _verticalCameraSensitivity = 1f;

    // mouse input
    private bool _firstMove = true;
    private Vector2 _lastMousePosition;

    // simulation
    private Stopwatch _timer;
    private float _simSecondsPerRealSecond = 360f;
    private OriginEarth _earth;
    private List<IGravitator> _gravitators;
    private Rocket _rocket;

    public float AspectRatio => Size.X / (float)Size.Y;

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

      _camera = new LockedCamera(Vector3.Zero, AspectRatio);

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

      // camera
      Matrix4 view = _camera.ViewMatrix;
      Matrix4 projection = _camera.ProjectionMatrix;

      // render earth
      _earthSphere.Render(view, projection);

      // render rocket
      _issModel.Render(view, projection);

      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      if (!IsFocused) return;

      var keyboard = KeyboardState;

      if (keyboard.IsKeyDown(Keys.Escape))
      {
        Close();
      }

      var mouse = MouseState;

      if (_firstMove)
      {
        _lastMousePosition = new Vector2(mouse.X, mouse.Y);
        _firstMove = false;
      }
      else
      {
        var mouseDeltaX = mouse.X - _lastMousePosition.X;
        var mouseDeltaY = mouse.Y - _lastMousePosition.Y;
        _lastMousePosition = new Vector2(mouse.X, mouse.Y);

        _camera.RotateXY(-mouseDeltaX * _horizontalCameraSensitivity);
      }
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);
      _camera.AspectRatio = AspectRatio;
    }
  }
}
