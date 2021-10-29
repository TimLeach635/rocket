using System;
using System.Collections.Generic;
using System.Numerics;
using RocketEngine;

namespace RocketConsole
{
    class Program
    {
        static float DegreesToRadians(float degrees)
        {
            return degrees * MathF.PI / 180;
        }

        static void Main(string[] args)
        {
            float earthRadius = 6.371e6f;
            IGravitator earth = new OriginEarth();
            List<IGravitator> gravitators = new List<IGravitator> {earth};
            Orbit orbit = new Orbit(
                0.0003938f,
                earthRadius + (417000 + 423000) / 2,
                51.6444f,
                DegreesToRadians(38.4733f),
                DegreesToRadians(153.2242f),
                DegreesToRadians(27.0427f),
                new DateTime(2021, 10, 29, 12, 34, 51)
            );
            DateTime now = DateTime.UtcNow;
            Rocket rocket = new Rocket(
                gravitators,
                orbit.GetPositionFromGravitator(earth, now),
                orbit.GetVelocityFromGravitator(earth, now)
            );
            float timestep = 60f;

            for (int i = 0; i < 90; i++)
            {
                Console.Out.WriteLine($"Rocket at ({rocket.Position.X}, {rocket.Position.Y}, {rocket.Position.Z})");
                rocket.Update(timestep);
            }
        }
    }
}
