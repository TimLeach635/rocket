using System;
using RocketEngine.Positioning;

namespace RocketEngine.Bodies
{
  public interface IBody
  {
    Position Position { get; }
    DateTime CurrentTime { get; }
    void Update(TimeSpan timestep);
  }
}
