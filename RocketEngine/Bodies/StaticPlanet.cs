using System;
using System.Numerics;

namespace RocketEngine.Bodies
{
  public class StaticPlanet : IGravitator
  {
    private DateTime _currentTime;

    public Vector3 Position { get; }
    public float Mass { get; }
    public float StandardGravitationalParameter => Mass * Constants.BIG_G;

    public DateTime CurrentTime => _currentTime;

    public void Update(TimeSpan timestep)
    {
      _currentTime += timestep;
    }

    public StaticPlanet(DateTime initialTime, Vector3 position, float mass)
    {
      _currentTime = initialTime;
      Position = position;
      Mass = mass;
    }
  }
}
