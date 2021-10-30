using RocketEngine.Bodies;
using System;
using System.Numerics;

namespace RocketEngine
{
  public abstract class Planet : IGravitator
  {
    protected float _mass;

    public float Mass => _mass;
    public float StandardGravitationalParameter => _mass * Constants.BIG_G;

    public abstract Vector3 LocationAt(DateTime dateTime);
  }
}
