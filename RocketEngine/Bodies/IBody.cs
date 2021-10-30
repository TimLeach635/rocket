using System;
using System.Numerics;

namespace RocketEngine.Bodies
{
  public interface IBody
  {
    Vector3 Position { get; }
    DateTime CurrentTime { get; }
    void Update(TimeSpan timestep);
  }
}
