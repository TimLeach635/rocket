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
    private bool _textured = false;
    private string _texturePath;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;
    private Vector4 _colour;
    private Shader _shader;
    private Texture _texture;
    int _modelUniformLocation;
    int _viewUniformLocation;
    int _projectionUniformLocation;
    int _colourUniformLocation;
    int _textureUniformLocation;
    private Matrix4 _model;
    public Matrix4 Model {
      get => _model;
      set => _model = value;
    }

    public static float[] GenerateVertexArray(float radius, uint sectorCount, uint stackCount, bool withTexture = false)
    {
      List<float> vertices = new List<float>();
      float sectorTextureAmount = 1f / sectorCount;
      float sectorAngle = 2 * MathF.PI / sectorCount;
      float stackTextureAmount = 1f / stackCount;
      float stackAngle = MathF.PI / stackCount;

      // vertices at the poles
      vertices.Add(0);
      vertices.Add(0);
      vertices.Add(radius);
      if (withTexture)
      {
        vertices.Add(0.5f);
        vertices.Add(1.0f);
      }

      vertices.Add(0);
      vertices.Add(0);
      vertices.Add(-radius);
      if (withTexture)
      {
        vertices.Add(0.5f);
        vertices.Add(0.0f);
      }

      // sector count +1 to double-count the seam, for texturing purposes
      for (uint sector = 0; sector < sectorCount + 1; sector++)
      {
        float theta = sector * sectorAngle;
        for (uint stack = 1; stack < stackCount; stack++)
        {
          float phi = (stack * stackAngle) - MathF.PI/2;

          vertices.Add(radius * MathF.Cos(phi) * MathF.Cos(theta));
          vertices.Add(radius * MathF.Cos(phi) * MathF.Sin(theta));
          vertices.Add(radius * MathF.Sin(phi));
          if (withTexture)
          {
            vertices.Add(sector * sectorTextureAmount);
            vertices.Add(stack * stackTextureAmount);
          }
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
          indices.Add(stackFirstVertexIndex + stack);
          indices.Add(stackFirstVertexIndex + stack + verticesPerStack);
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

        for (uint stack = 0; stack < verticesPerStack - 1; stack++)
        {
          // top left triangle
          indices.Add(stackFirstVertexIndex + stack);
          indices.Add(stackFirstVertexIndex + stack + 1);
          indices.Add(stackFirstVertexIndex + stack + 1 + verticesPerStack);

          // bottom right triangle
          indices.Add(stackFirstVertexIndex + stack + verticesPerStack);
          indices.Add(stackFirstVertexIndex + stack + verticesPerStack + 1);
          indices.Add(stackFirstVertexIndex + stack);
        }

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
      return GenerateVertexArray(_radius, _sectorCount, _stackCount, _textured);
    }

    public uint[] GenerateLinesIndexArray()
    {
      return GenerateLinesIndexArray(_sectorCount, _stackCount);
    }

    public uint[] GenerateTrianglesIndexArray()
    {
      return GenerateTrianglesIndexArray(_sectorCount, _stackCount);
    }

    public Sphere(float radius, uint sectorCount, uint stackCount, Vector4 colour, string texturePath)
    {
      _radius = radius;
      _sectorCount = sectorCount;
      _stackCount = stackCount;
      _textured = true;
      _texturePath = texturePath;
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

      _elementBufferObject = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
      GL.BufferData(
        BufferTarget.ElementArrayBuffer,
        _indices.Length * sizeof(uint),
        _indices,
        BufferUsageHint.StaticDraw
      );

      // _shader = new Shader(
      //   @"
      //     #version 330 core
      //     layout (location = 0) in vec3 aPos;

      //     uniform mat4 model;
      //     uniform mat4 view;
      //     uniform mat4 projection;

      //     void main()
      //     {
      //       gl_Position = vec4(aPos, 1.0) * model * view * projection;
      //     }
      //   ",
      //   @"
      //     #version 330 core
      //     out vec4 outColour;

      //     uniform vec4 colour;

      //     void main()
      //     {
      //       outColour = colour;
      //     }
      //   "
      // );
      _shader = new Shader(
        @"
          #version 330 core
          layout (location = 0) in vec3 aPos;
          layout (location = 1) in vec2 aTex;

          uniform mat4 model;
          uniform mat4 view;
          uniform mat4 projection;

          out vec2 tex;

          void main()
          {
            tex = aTex;
            gl_Position = vec4(aPos, 1.0) * model * view * projection;
          }
        ",
        @"
          #version 330 core
          out vec4 outColour;

          in vec2 tex;

          uniform sampler2D texture0;

          void main()
          {
            outColour = texture(texture0, tex);
          }
        "
      );
      _shader.Use();
      _modelUniformLocation = GL.GetUniformLocation(_shader.Handle, "model");
      _viewUniformLocation = GL.GetUniformLocation(_shader.Handle, "view");
      _projectionUniformLocation = GL.GetUniformLocation(_shader.Handle, "projection");
      // _colourUniformLocation = GL.GetUniformLocation(_shader.Handle, "colour");
      _textureUniformLocation = GL.GetUniformLocation(_shader.Handle, "texture0");

      var vertexLocation = GL.GetAttribLocation(_shader.Handle, "aPos");
      GL.EnableVertexAttribArray(vertexLocation);
      GL.VertexAttribPointer(
        vertexLocation,
        3,
        VertexAttribPointerType.Float,
        false,
        5 * sizeof(float),
        0
      );

      var texCoordLocation = GL.GetAttribLocation(_shader.Handle, "aTex");
      GL.EnableVertexAttribArray(texCoordLocation);
      GL.VertexAttribPointer(
        texCoordLocation,
        2,
        VertexAttribPointerType.Float,
        false,
        5 * sizeof(float),
        3 * sizeof(float)
      );

      _texture = Texture.LoadFromFile(_texturePath);
      _texture.Use(TextureUnit.Texture0);
    }

    public void Render(Matrix4 view, Matrix4 projection)
    {
      _shader.Use();
      GL.UniformMatrix4(_modelUniformLocation, true, ref _model);
      GL.UniformMatrix4(_viewUniformLocation, true, ref view);
      GL.UniformMatrix4(_projectionUniformLocation, true, ref projection);
      // GL.Uniform4(_colourUniformLocation, ref _colour);

      GL.BindVertexArray(_vertexArrayObject);
      GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
    }
  }
}
