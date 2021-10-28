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
    private Shader _earthShader;
    private int _earthVBO;
    private int _earthVAO;
    private Matrix4 _earthModel;

    private Sphere _rocketSphere;
    private float[] _rocketVertices;
    private Shader _rocketShader;
    private int _rocketVBO;
    private int _rocketVAO;
    private Matrix4 _rocketModel;

    private Stopwatch _timer;
    private Matrix4 _view;
    private Matrix4 _projection;

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

          uniform mat4 model;

          void main()
          {
            gl_Position = vec4(aPos, 1.0) * model;
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

          uniform mat4 model;

          void main()
          {
            gl_Position = vec4(aPos, 1.0) * model;
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
      _earthSphere = new Sphere(_earthRadius * _worldUnitsPerMetre, 20, 10, _earthShader);
      _earthVertices = _earthSphere.GenerateVertexArray();

      _earthVBO = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _earthVBO);
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        _earthVertices.Length * sizeof(float),
        _earthVertices,
        BufferUsageHint.StaticDraw
      );

      _earthVAO = GL.GenVertexArray();
      GL.BindVertexArray(_earthVAO);
      GL.VertexAttribPointer(
        0,
        3,
        VertexAttribPointerType.Float,
        false,
        3 * sizeof(float),
        0
      );

      GL.EnableVertexAttribArray(0);

      _rocketSphere = new Sphere(_earthRadius * _worldUnitsPerMetre / 20, 10, 5, _rocketShader);
      _rocketVertices = _rocketSphere.GenerateVertexArray();

      _rocketVBO = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _rocketVBO);
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        _rocketVertices.Length * sizeof(float),
        _rocketVertices,
        BufferUsageHint.StaticDraw
      );

      _rocketVAO = GL.GenVertexArray();
      GL.BindVertexArray(_rocketVAO);
      GL.VertexAttribPointer(
        0,
        3,
        VertexAttribPointerType.Float,
        false,
        3 * sizeof(float),
        0
      );

      GL.EnableVertexAttribArray(0);

      _timer = new Stopwatch();
      _timer.Start();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      // timing
      double elapsed = _timer.Elapsed.TotalSeconds;

      // transform
      _earthModel = Matrix4.Identity;
      _earthModel *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(45));

      _rocketModel = Matrix4.Identity;
      _rocketModel *= Matrix4.CreateTranslation(0.75f, 0f, 0f);

      _view = Matrix4.Identity;
      // _view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-80f));
      // _view *= Matrix4.CreateTranslation(0f, 0f, -2f);

      _projection = Matrix4.Identity;
      // _projection *= Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);

      _earthShader.Use();
      int earthModelUniformLocation = GL.GetUniformLocation(_earthShader.Handle, "model");
      GL.UniformMatrix4(earthModelUniformLocation, true, ref _earthModel);

      _rocketShader.Use();
      int rocketModelUniformLocation = GL.GetUniformLocation(_rocketShader.Handle, "model");
      GL.UniformMatrix4(rocketModelUniformLocation, true, ref _rocketModel);

      // render earth
      _earthShader.Use();
      GL.BindVertexArray(_earthVAO);
      GL.DrawArrays(PrimitiveType.Lines, 0, _earthVertices.Length / 3);

      // render rocket
      _rocketShader.Use();
      GL.BindVertexArray(_rocketVAO);
      GL.DrawArrays(PrimitiveType.Lines, 0, _rocketVertices.Length / 3);

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
