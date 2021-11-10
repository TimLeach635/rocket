using System;
using System.Numerics;
using RocketEngine.Bodies;
using RocketEngine.Positioning;

namespace RocketEngine
{
    public class OriginSun : StaticPlanet
    {
        public OriginSun(DateTime initialTime) : base(initialTime, new Position(Vector3.Zero), Constants.SUN_MASS)
        {
        }
    }
}