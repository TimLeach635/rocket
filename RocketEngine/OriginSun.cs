using RocketEngine.Bodies;
using System;
using System.Numerics;

namespace RocketEngine
{
  public class OriginSun : StaticPlanet
  {
    public OriginSun(DateTime initialTime) : base(initialTime, Vector3.Zero, Constants.SUN_MASS) { }
  }
}
