using System;
using System.Numerics;

namespace RocketEngine
{
  public abstract class Planet : IGravitator
  {
    protected float _mass;

    public float Mass => _mass;

    public abstract Vector3 LocationAt(DateTime dateTime);
  }
}
