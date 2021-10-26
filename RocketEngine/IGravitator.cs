using System;
using System.Numerics;

namespace RocketEngine
{
  public interface IGravitator
  {
    float Mass { get; }
    Vector3 LocationAt(DateTime dateTime);
  }
}
