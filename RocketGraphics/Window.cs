using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using RocketEngine;
using RocketEngine.Simulation.Specific;
using System;
using System.Diagnostics;
using RocketGraphics.Camera;

namespace RocketGraphics
{
  public class Window: GameWindow
  {
    private float _worldUnitsPerMetre = 4e-11f;

    private Sphere _sunSphere;
    private Texture _sunTexture;

    private Sphere _mercurySphere;
    private Texture _mercuryTexture;

    private Sphere _venusSphere;
    private Texture _venusTexture;

    private Sphere _earthSphere;
    private Texture _earthTexture;

    private Sphere _marsSphere;
    private Texture _marsTexture;

    // common rendering
    private LockedCamera _camera;
    private float _cameraHorizontalSensitivity = 5e-3f;
    private float _cameraVerticalSensitivity = 5e-3f;
    private float _cameraZoomSensitivity = 1e-1f;

    // mouse input
    private bool _firstMove = true;
    private Vector2 _lastMousePosition;

    // simulation
    private Stopwatch _timer;
    private float _simDaysPerRealSecond = 50f;
    private InnerPlanets _simulation;

    public float AspectRatio => Size.X / (float)Size.Y;

    private void GrabCursor()
    {
      if (!CursorGrabbed) CursorGrabbed = true;
    }

    private void ReleaseCursor()
    {
      if (!CursorVisible) CursorVisible = true;
    }

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

      _sunTexture = Texture.LoadFromFile("resources/textures/8k_sun.jpg");
      _sunTexture.Use(TextureUnit.Texture0);
      _sunSphere = new Sphere(
        Constants.SUN_RADIUS * _worldUnitsPerMetre * sunScale,
        200,
        100,
        new Vector4(1f, 1f, 0f, 1f),
        _sunTexture,
        TextureUnit.Texture0
      );
      _sunSphere.Initialise();

      _mercuryTexture = Texture.LoadFromFile("resources/textures/8k_mercury.jpg");
      _mercuryTexture.Use(TextureUnit.Texture1);
      _mercurySphere = new Sphere(
        Constants.MERCURY_RADIUS * _worldUnitsPerMetre * planetScale,
        200,
        100,
        new Vector4(0.8f, 0.5f, 0.5f, 1f),
        _mercuryTexture,
        TextureUnit.Texture1
      );
      _mercurySphere.Initialise();

      _venusTexture = Texture.LoadFromFile("resources/textures/4k_venus_atmosphere.jpg");
      _venusTexture.Use(TextureUnit.Texture2);
      _venusSphere = new Sphere(
        Constants.VENUS_RADIUS * _worldUnitsPerMetre * planetScale,
        200,
        100,
        new Vector4(1f, 0.8f, 0.8f, 1f),
        _venusTexture,
        TextureUnit.Texture2
      );
      _venusSphere.Initialise();

      _earthTexture = Texture.LoadFromFile("resources/textures/8k_earth_daymap.jpg");
      _earthTexture.Use(TextureUnit.Texture3);
      _earthSphere = new Sphere(
        Constants.EARTH_RADIUS * _worldUnitsPerMetre * planetScale,
        200,
        100,
        new Vector4(0f, 1f, 0f, 1f),
        _earthTexture,
        TextureUnit.Texture3
      );
      _earthSphere.Initialise();

      _marsTexture = Texture.LoadFromFile("resources/textures/8k_mars.jpg");
      _marsTexture.Use(TextureUnit.Texture4);
      _marsSphere = new Sphere(
        Constants.MARS_RADIUS * _worldUnitsPerMetre * planetScale,
        200,
        100,
        new Vector4(1f, 0f, 0f, 1f),
        _marsTexture,
        TextureUnit.Texture4
      );
      _marsSphere.Initialise();

      _camera = new LockedCamera(Vector3.Zero, AspectRatio);

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

      // camera
      Matrix4 view = _camera.ViewMatrix;
      Matrix4 projection = _camera.ProjectionMatrix;

      // transform
      _mercurySphere.Model = Matrix4.CreateTranslation(
        (float)_simulation.Mercury.Position.X * _worldUnitsPerMetre,
        (float)_simulation.Mercury.Position.Y * _worldUnitsPerMetre,
        (float)_simulation.Mercury.Position.Z * _worldUnitsPerMetre
      );
      _venusSphere.Model = Matrix4.CreateTranslation(
        (float)_simulation.Venus.Position.X * _worldUnitsPerMetre,
        (float)_simulation.Venus.Position.Y * _worldUnitsPerMetre,
        (float)_simulation.Venus.Position.Z * _worldUnitsPerMetre
      );
      _earthSphere.Model = Matrix4.CreateTranslation(
        (float)_simulation.Earth.Position.X * _worldUnitsPerMetre,
        (float)_simulation.Earth.Position.Y * _worldUnitsPerMetre,
        (float)_simulation.Earth.Position.Z * _worldUnitsPerMetre
      );
      _marsSphere.Model = Matrix4.CreateTranslation(
        (float)_simulation.Mars.Position.X * _worldUnitsPerMetre,
        (float)_simulation.Mars.Position.Y * _worldUnitsPerMetre,
        (float)_simulation.Mars.Position.Z * _worldUnitsPerMetre
      );

      _sunSphere.Render(view, projection);
      _mercurySphere.Render(view, projection);
      _venusSphere.Render(view, projection);
      _earthSphere.Render(view, projection);
      _marsSphere.Render(view, projection);

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
      float mouseDeltaX = 0f;
      float mouseDeltaY = 0f;
      
      if (_firstMove)
      {
        _lastMousePosition = new Vector2(mouse.X, mouse.Y);
        _firstMove = false;
      }
      else
      {
        mouseDeltaX = mouse.X - _lastMousePosition.X;
        mouseDeltaY = mouse.Y - _lastMousePosition.Y;
        _lastMousePosition = new Vector2(mouse.X, mouse.Y);
      }

      if (mouse.IsButtonDown(MouseButton.Right))
      {
        GrabCursor();
        if (mouseDeltaX != 0) _camera.RotateXY(mouseDeltaX * _cameraHorizontalSensitivity);
        if (mouseDeltaY != 0) _camera.RotateVertical(mouseDeltaY * _cameraVerticalSensitivity);
      }
      else
      {
        ReleaseCursor();
      }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      base.OnMouseWheel(e);
      _camera.ZoomIn(e.OffsetY * _cameraZoomSensitivity);
    }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);
      GL.Viewport(0, 0, Size.X, Size.Y);
      _camera.AspectRatio = AspectRatio;
    }
  }
}
