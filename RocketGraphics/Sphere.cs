using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace RocketGraphics
{
  public class Sphere
  {
    private float _radius;
    private uint _sectorCount;
    private uint _stackCount;
    private float[] _vertices;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private Vector4 _colour;
    private Shader _shader { get; set; }
    int _modelUniformLocation;
    int _viewUniformLocation;
    int _projectionUniformLocation;
    int _colourUniformLocation;
    private Matrix4 _model;
    public Matrix4 Model {
      get => _model;
      set => _model = value;
    }

    public static float[] GenerateVertexArray(float radius, uint sectorCount, uint stackCount)
    {
      List<float> vertices = new List<float>();
      float sectorAngle = 2 * MathF.PI / sectorCount;
      float stackAngle = MathF.PI / stackCount;

      for (uint sector = 0; sector < sectorCount; sector++)
      {
        float thetaStart = sector * sectorAngle;
        float thetaEnd = (sector + 1) * sectorAngle;
        for (uint stack = 0; stack < stackCount; stack++)
        {
          float phiStart = (stack * stackAngle) - MathF.PI/2;
          float phiEnd = ((stack + 1) * stackAngle) - MathF.PI/2;

          // line going down
          vertices.Add(radius * MathF.Cos(phiStart) * MathF.Cos(thetaStart));
          vertices.Add(radius * MathF.Cos(phiStart) * MathF.Sin(thetaStart));
          vertices.Add(radius * MathF.Sin(phiStart));

          vertices.Add(radius * MathF.Cos(phiEnd) * MathF.Cos(thetaStart));
          vertices.Add(radius * MathF.Cos(phiEnd) * MathF.Sin(thetaStart));
          vertices.Add(radius * MathF.Sin(phiEnd));

          // line going across
          if (stack < stackCount - 1)
          {
            vertices.Add(radius * MathF.Cos(phiEnd) * MathF.Cos(thetaStart));
            vertices.Add(radius * MathF.Cos(phiEnd) * MathF.Sin(thetaStart));
            vertices.Add(radius * MathF.Sin(phiEnd));

            vertices.Add(radius * MathF.Cos(phiEnd) * MathF.Cos(thetaEnd));
            vertices.Add(radius * MathF.Cos(phiEnd) * MathF.Sin(thetaEnd));
            vertices.Add(radius * MathF.Sin(phiEnd));
          }
        }
      }

      return vertices.ToArray();
    }

    public float[] GenerateVertexArray()
    {
      return GenerateVertexArray(_radius, _sectorCount, _stackCount);
    }

    public Sphere(float radius, uint sectorCount, uint stackCount, Vector4 colour)
    {
      _radius = radius;
      _sectorCount = sectorCount;
      _stackCount = stackCount;
      _vertices = GenerateVertexArray();
      Model = Matrix4.Identity;
      _colour = colour;
    }

    public void Initialise()
    {
      _vertexBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
      GL.BufferData(
        BufferTarget.ArrayBuffer,
        _vertices.Length * sizeof(float),
        _vertices,
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

      _shader = new Shader(
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

          uniform vec4 colour;

          void main()
          {
            outColour = colour;
          }
        "
      );
      _shader.Use();
      _modelUniformLocation = GL.GetUniformLocation(_shader.Handle, "model");
      _viewUniformLocation = GL.GetUniformLocation(_shader.Handle, "view");
      _projectionUniformLocation = GL.GetUniformLocation(_shader.Handle, "projection");
      _colourUniformLocation = GL.GetUniformLocation(_shader.Handle, "colour");
    }

    public void Render(Matrix4 view, Matrix4 projection)
    {
      _shader.Use();
      GL.UniformMatrix4(_modelUniformLocation, true, ref _model);
      GL.UniformMatrix4(_viewUniformLocation, true, ref view);
      GL.UniformMatrix4(_projectionUniformLocation, true, ref projection);
      GL.Uniform4(_colourUniformLocation, ref _colour);

      GL.BindVertexArray(_vertexArrayObject);
      GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Length / 3);
    }
  }
}
