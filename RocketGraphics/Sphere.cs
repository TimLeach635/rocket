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
    private uint[] _indices;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;
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

      // vertices at the poles
      vertices.Add(0);
      vertices.Add(0);
      vertices.Add(radius);

      vertices.Add(0);
      vertices.Add(0);
      vertices.Add(-radius);

      for (uint sector = 0; sector < sectorCount; sector++)
      {
        float theta = sector * sectorAngle;
        for (uint stack = 1; stack < stackCount; stack++)
        {
          float phi = (stack * stackAngle) - MathF.PI/2;

          vertices.Add(radius * MathF.Cos(phi) * MathF.Cos(theta));
          vertices.Add(radius * MathF.Cos(phi) * MathF.Sin(theta));
          vertices.Add(radius * MathF.Sin(phi));
        }
      }

      return vertices.ToArray();
    }

    public static uint[] GenerateLinesIndexArray(uint sectorCount, uint stackCount)
    {
      List<uint> indices = new List<uint>();
      const uint NORTH_POLE = 0;
      const uint SOUTH_POLE = 1;
      uint verticesPerStack = stackCount - 1;

      for (uint sector = 0; sector < sectorCount; sector++)
      {
        uint stackFirstVertexIndex = SOUTH_POLE + 1 + sector * verticesPerStack;

        // line to south pole
        // (we start drawing at the bottom)
        indices.Add(SOUTH_POLE);
        indices.Add(stackFirstVertexIndex);

        for (uint stack = 0; stack < verticesPerStack; stack++)
        {
          // line down
          if (stack < verticesPerStack - 1){
            indices.Add(stackFirstVertexIndex + stack);
            indices.Add(stackFirstVertexIndex + stack + 1);
          }

          // line across
          if (sector < sectorCount - 1)
          {
            indices.Add(stackFirstVertexIndex + stack);
            indices.Add(stackFirstVertexIndex + stack + verticesPerStack);
          }
          else
          {
            indices.Add(stackFirstVertexIndex + stack);
            indices.Add(SOUTH_POLE + 1 + stack);
          }
        }

        // line to north pole
        indices.Add(NORTH_POLE);
        indices.Add(stackFirstVertexIndex + verticesPerStack - 1);
      }

      return indices.ToArray();
    }

    public static uint[] GenerateTrianglesIndexArray(uint sectorCount, uint stackCount)
    {
      List<uint> indices = new List<uint>();
      const uint NORTH_POLE = 0;
      const uint SOUTH_POLE = 1;
      uint verticesPerStack = stackCount - 1;

      for (uint sector = 0; sector < sectorCount; sector++)
      {
        uint stackFirstVertexIndex = SOUTH_POLE + 1 + sector * verticesPerStack;

        // triangle to south pole
        // (we start drawing at the bottom)
        indices.Add(SOUTH_POLE);
        indices.Add(stackFirstVertexIndex);
        if (sector < sectorCount - 1)
        {
          indices.Add(stackFirstVertexIndex + verticesPerStack);
        }
        else
        {
          indices.Add(SOUTH_POLE + 1);
        }

        // for (uint stack = 0; stack < verticesPerStack; stack++)
        // {
        //   // line down
        //   if (stack < verticesPerStack - 1){
        //     indices.Add(stackFirstVertexIndex + stack);
        //     indices.Add(stackFirstVertexIndex + stack + 1);
        //   }

        //   // line across
        //   if (sector < sectorCount - 1)
        //   {
        //     indices.Add(stackFirstVertexIndex + stack);
        //     indices.Add(stackFirstVertexIndex + stack + verticesPerStack);
        //   }
        //   else
        //   {
        //     indices.Add(stackFirstVertexIndex + stack);
        //     indices.Add(SOUTH_POLE + 1 + stack);
        //   }
        // }

        // triangle to north pole
        indices.Add(NORTH_POLE);
        indices.Add(stackFirstVertexIndex + verticesPerStack - 1);
        if (sector < sectorCount - 1)
        {
          indices.Add(stackFirstVertexIndex + 2 * verticesPerStack - 1);
        }
        else
        {
          indices.Add(SOUTH_POLE + verticesPerStack);
        }
      }

      return indices.ToArray();
    }

    public float[] GenerateVertexArray()
    {
      return GenerateVertexArray(_radius, _sectorCount, _stackCount);
    }

    public uint[] GenerateLinesIndexArray()
    {
      return GenerateLinesIndexArray(_sectorCount, _stackCount);
    }

    public uint[] GenerateTrianglesIndexArray()
    {
      return GenerateTrianglesIndexArray(_sectorCount, _stackCount);
    }

    public Sphere(float radius, uint sectorCount, uint stackCount, Vector4 colour)
    {
      _radius = radius;
      _sectorCount = sectorCount;
      _stackCount = stackCount;
      _vertices = GenerateVertexArray();
      _indices = GenerateTrianglesIndexArray();
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

      _elementBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
      GL.BufferData(
        BufferTarget.ElementArrayBuffer,
        _indices.Length * sizeof(uint),
        _indices,
        BufferUsageHint.StaticDraw
      );

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
      GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
  }
}
