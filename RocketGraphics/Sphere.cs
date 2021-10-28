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
    private float SectorAngle => 2 * MathF.PI / _sectorCount;
    private uint _stackCount;
    private float StackAngle => MathF.PI / _stackCount;

    private float[] _vertices;
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    public Shader Shader { get; set; }
    public Vector3 Position { get; set; }

    public float[] GenerateVertexArray()
    {
      List<float> vertices = new List<float>();

      for (uint sector = 0; sector < _sectorCount; sector++)
      {
        float thetaStart = sector * SectorAngle;
        float thetaEnd = (sector + 1) * SectorAngle;
        for (uint stack = 0; stack < _stackCount; stack++)
        {
          float phiStart = (stack * StackAngle) - MathF.PI/2;
          float phiEnd = ((stack + 1) * StackAngle) - MathF.PI/2;

          // line going down
          vertices.Add(_radius * MathF.Cos(phiStart) * MathF.Cos(thetaStart));
          vertices.Add(_radius * MathF.Cos(phiStart) * MathF.Sin(thetaStart));
          vertices.Add(_radius * MathF.Sin(phiStart));

          vertices.Add(_radius * MathF.Cos(phiEnd) * MathF.Cos(thetaStart));
          vertices.Add(_radius * MathF.Cos(phiEnd) * MathF.Sin(thetaStart));
          vertices.Add(_radius * MathF.Sin(phiEnd));

          // line going across
          if (stack < _stackCount - 1)
          {
            vertices.Add(_radius * MathF.Cos(phiEnd) * MathF.Cos(thetaStart));
            vertices.Add(_radius * MathF.Cos(phiEnd) * MathF.Sin(thetaStart));
            vertices.Add(_radius * MathF.Sin(phiEnd));

            vertices.Add(_radius * MathF.Cos(phiEnd) * MathF.Cos(thetaEnd));
            vertices.Add(_radius * MathF.Cos(phiEnd) * MathF.Sin(thetaEnd));
            vertices.Add(_radius * MathF.Sin(phiEnd));
          }
        }
      }

      return vertices.ToArray();
    }

    public Sphere(float radius, uint sectorCount, uint stackCount, Shader shader)
    {
      _radius = radius;
      _sectorCount = sectorCount;
      _stackCount = stackCount;
      _vertices = GenerateVertexArray();
      Position = new Vector3(0f, 0f, 0f);
      Shader = shader;
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
    }

    public void Render(Matrix4 model, Matrix4 view, Matrix4 projection)
    {
      Shader.Use();
      int modelLocation = GL.GetUniformLocation(Shader.Handle, "model");
      int viewLocation = GL.GetUniformLocation(Shader.Handle, "view");
      int projectionLocation = GL.GetUniformLocation(Shader.Handle, "projection");
      GL.UniformMatrix4(modelLocation, true, ref model);
      GL.UniformMatrix4(viewLocation, true, ref view);
      GL.UniformMatrix4(projectionLocation, true, ref projection);

      // Matrix4 model = Matrix4.CreateTranslation(Position);

      GL.BindVertexArray(_vertexArrayObject);
      GL.DrawArrays(PrimitiveType.Lines, 0, _vertices.Length / 3);
    }
  }
}
