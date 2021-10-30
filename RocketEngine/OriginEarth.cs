using RocketEngine.Bodies;
using System;
using System.Numerics;

namespace RocketEngine
{
  public class OriginEarth : StaticPlanet
  {
    public OriginEarth(DateTime initialTime) : base(initialTime, Vector3.Zero, Constants.EARTH_MASS) { }
  }
}
