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
    private Sphere _rocketSphere;
    private float[] _earthVertices;
    private float[] _rocketVertices;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private Shader _earthShader;
    private Shader _rocketShader;
    private Stopwatch _timer;
    private Matrix4 _earthModel;
    private Matrix4 _rocketModel;
    private Matrix4 _view;
    private Matrix4 _projection;

    public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings) { }

    protected override void OnLoad()
    {
      base.OnLoad();

      _earthSphere = new Sphere(_earthRadius * _worldUnitsPerMetre, System.Numerics.Vector3.Zero, 20, 10);
      _earthVertices = _earthSphere.VertexArray;

      _rocketSphere = new Sphere(_earthRadius * _worldUnitsPerMetre / 30, System.Numerics.Vector3.Zero, 10, 5);
      _rocketVertices = _rocketSphere.VertexArray;

      List<float> allVertices = _earthVertices.ToList();
      allVertices.AddRange(_rocketVertices);

      GL.ClearColor(0f, 0f, 0f, 1f);
      GL.Enable(EnableCap.DepthTest);

      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(BufferTarget.ArrayBuffer, (allVertices.Count) * sizeof(float), allVertices.ToArray(), BufferUsageHint.StaticDraw);
      
      _vertexArrayObject = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);

      _earthShader = new Shader(
        @"
          #version 330 core
          layout (location = 0) in vec3 aPos;
          
          uniform mat4 model;
          uniform mat4 view;
          uniform mat4 projection;

          void main()
          {
            gl_Position = vec4(aPos, 1.0) * model * view * projection;
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
          uniform mat4 view;
          uniform mat4 projection;

          void main()
          {
            gl_Position = vec4(aPos, 1.0) * model * view * projection;
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

      _timer = new Stopwatch();
      _timer.Start();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      GL.BindVertexArray(_vertexArrayObject);

      // timing
      double elapsed = _timer.Elapsed.TotalSeconds;

      // transform
      _earthModel = Matrix4.Identity;
      _rocketModel = Matrix4.Identity;
      _rocketModel *= Matrix4.CreateTranslation(1f, 1f, 0f);

      _view = Matrix4.Identity;
      _view *= Matrix4.CreateRotationX(MathHelper.DegreesToRadians(-80f));
      _view *= Matrix4.CreateTranslation(0f, 0f, -2f);

      _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Size.X / (float)Size.Y, 0.1f, 100.0f);

      int earthModelLocation = GL.GetUniformLocation(_earthShader.Handle, "model");
      int earthViewLocation = GL.GetUniformLocation(_earthShader.Handle, "view");
      int earthProjectionLocation = GL.GetUniformLocation(_earthShader.Handle, "projection");

      int rocketModelLocation = GL.GetUniformLocation(_rocketShader.Handle, "model");
      int rocketViewLocation = GL.GetUniformLocation(_rocketShader.Handle, "view");
      int rocketProjectionLocation = GL.GetUniformLocation(_rocketShader.Handle, "projection");

      GL.UniformMatrix4(earthModelLocation, true, ref _earthModel);
      GL.UniformMatrix4(earthViewLocation, true, ref _view);
      GL.UniformMatrix4(earthProjectionLocation, true, ref _projection);

      GL.UniformMatrix4(rocketModelLocation, true, ref _rocketModel);
      GL.UniformMatrix4(rocketViewLocation, true, ref _view);
      GL.UniformMatrix4(rocketProjectionLocation, true, ref _projection);

      // draw earth
      _earthShader.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, _earthVertices.Length / 3);

      // draw rocket
      _rocketShader.Use();
      GL.DrawArrays(PrimitiveType.Lines, _earthVertices.Length / 3, _rocketVertices.Length / 3);

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
