using System;
using System.Collections.Generic;
using System.Numerics;

namespace RocketGraphics
{
  public class Sphere
  {
    private float _radius;
    private Vector3 _origin;
    private uint _sectorCount;
    private float SectorAngle => 2 * MathF.PI / _sectorCount;
    private uint _stackCount;
    private float StackAngle => MathF.PI / _stackCount;

    public float[] VertexArray
    {
      get
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
            vertices.Add((_radius * MathF.Cos(phiStart) * MathF.Cos(thetaStart)) + _origin.X);
            vertices.Add((_radius * MathF.Cos(phiStart) * MathF.Sin(thetaStart)) + _origin.Y);
            vertices.Add((_radius * MathF.Sin(phiStart)) + _origin.Z);

            vertices.Add((_radius * MathF.Cos(phiEnd) * MathF.Cos(thetaStart)) + _origin.X);
            vertices.Add((_radius * MathF.Cos(phiEnd) * MathF.Sin(thetaStart)) + _origin.Y);
            vertices.Add((_radius * MathF.Sin(phiEnd)) + _origin.Z);

            // line going across
            if (stack < _stackCount - 1)
            {
              vertices.Add((_radius * MathF.Cos(phiEnd) * MathF.Cos(thetaStart)) + _origin.X);
              vertices.Add((_radius * MathF.Cos(phiEnd) * MathF.Sin(thetaStart)) + _origin.Y);
              vertices.Add((_radius * MathF.Sin(phiEnd)) + _origin.Z);

              vertices.Add((_radius * MathF.Cos(phiEnd) * MathF.Cos(thetaEnd)) + _origin.X);
              vertices.Add((_radius * MathF.Cos(phiEnd) * MathF.Sin(thetaEnd)) + _origin.Y);
              vertices.Add((_radius * MathF.Sin(phiEnd)) + _origin.Z);
            }
          }
        }

        return vertices.ToArray();
      }
    }

    public Sphere(float radius, Vector3 origin, uint sectorCount, uint stackCount)
    {
      _radius = radius;
      _origin = origin;
      _sectorCount = sectorCount;
      _stackCount = stackCount;
    }
  }
}
