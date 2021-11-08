using RocketEngine.Bodies;
using RocketEngine.Positioning;
using System;
using System.Numerics;

namespace RocketEngine
{
  public class OriginEarth : StaticPlanet
  {
    public OriginEarth(DateTime initialTime) : base(initialTime, new Position(Vector3.Zero), Constants.EARTH_MASS) { }
  }
}
