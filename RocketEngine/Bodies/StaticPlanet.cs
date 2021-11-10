using System;
using RocketEngine.Positioning;

namespace RocketEngine.Bodies
{
    public class StaticPlanet : IGravitator
    {
        public StaticPlanet(DateTime initialTime, Position position, float mass)
        {
            CurrentTime = initialTime;
            Position = position;
            Mass = mass;
        }

        public Position Position { get; }
        public float Mass { get; }
        public float StandardGravitationalParameter => Mass * Constants.BIG_G;

        public DateTime CurrentTime { get; private set; }

        public void Update(TimeSpan timestep)
        {
            CurrentTime += timestep;
        }
    }
}