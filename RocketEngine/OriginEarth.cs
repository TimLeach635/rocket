using System;
using System.Numerics;

namespace RocketEngine
{
  public class OriginEarth : Planet
  {
    public OriginEarth()
    {
      _mass = 5.972e24f;
    }

    public override Vector3 LocationAt(DateTime _)
    {
      return new Vector3(0.0f, 0.0f, 0.0f);
    }
  }
}
