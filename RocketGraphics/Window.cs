using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System.Numerics;
using System.Diagnostics;
using System;
using System.Linq;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace RocketGraphics
{
  public class Window: GameWindow
  {
    private float _worldUnitsPerMetre = 1e-7f;
    private float _earthRadius = 6.371e6f;
    private Sphere _earthSphere;
    private float[] _earthVertices;
    private float[] _triVertices =
    {
      -0.5f, -0.5f, 0.0f, // Bottom-left vertex
       0.5f, -0.5f, 0.0f, // Bottom-right vertex
       0.0f,  0.5f, 0.0f  // Top vertex
    };
    private Shader _earthShader;
    private Shader _rocketShader;
    private Stopwatch _timer;
    private Matrix4 _view;
    private Matrix4 _projection;
    private int _vertexBufferObject;
    private int _vertexArrayObject;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings) {}

    protected override void OnLoad()
    {
      base.OnLoad();

      GL.ClearColor(0f, 0f, 0f, 1f);
      GL.Enable(EnableCap.DepthTest);

      _earthShader = new Shader(
        @"
          #version 330 core
          layout (location = 0) in vec3 aPos;

          void main()
          {
            gl_Position = vec4(aPos, 1.0);
          }
        ",
        @"
          #version 330 core
          out vec4 outColour;

          void main()
          {
            outColour = vec4(0.0f, 1.0f, 0.0f, 1.0f);
          }
        "
      );
      _rocketShader = new Shader(
        @"
          #version 330 core
          layout (location = 0) in vec3 aPos;

          void main()
          {
            gl_Position = vec4(aPos, 1.0);
          }
        ",
        @"
          #version 330 core
          out vec4 outColour;

          void main()
          {
            outColour = vec4(1.0f, 1.0f, 1.0f, 1.0f);
          }
        "
      );
      // _earthSphere = new Sphere(_earthRadius * _worldUnitsPerMetre, 20, 10, _earthShader);
      _earthSphere = new Sphere(0.5f, 20, 10, _earthShader);
      _earthVertices = _earthSphere.GenerateVertexArray();

      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        _earthVertices.Length * sizeof(float),
        _earthVertices,
        BufferUsageHint.StaticDraw
      );

      _vertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject);
      GL.VertexAttribPointer(
        0,
        3,
        VertexAttribPointerType.Float,
        false,
        3 * sizeof(float),
        0
      );

      GL.EnableVertexAttribArray(0);

      _earthShader.Use();

      _timer = new Stopwatch();
      _timer.Start();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      _earthShader.Use();
      GL.BindVertexArray(_vertexArrayObject);
      GL.DrawArrays(PrimitiveType.Lines, 0, _earthVertices.Length / 3);

      // timing
      double elapsed = _timer.Elapsed.TotalSeconds;

      // transform
      _view = Matrix4.Identity;
      // _view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-80f));
      // _view *= Matrix4.CreateTranslation(0f, 0f, -2f);

      _projection = Matrix4.Identity;
      // _projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);

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
