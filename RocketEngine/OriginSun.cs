using RocketEngine.Bodies;
using RocketEngine.Positioning;
using System;
using System.Numerics;

namespace RocketEngine
{
  public class OriginSun : StaticPlanet
  {
    public OriginSun(DateTime initialTime) : base(initialTime, new Position(Vector3.Zero), Constants.SUN_MASS) { }
  }
}
