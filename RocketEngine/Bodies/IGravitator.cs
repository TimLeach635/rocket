using System;
using System.Numerics;

namespace RocketEngine.Bodies
{
  public interface IGravitator
  {
    float Mass { get; }
    float StandardGravitationalParameter { get; }
    Vector3 LocationAt(DateTime dateTime);
  }
}
