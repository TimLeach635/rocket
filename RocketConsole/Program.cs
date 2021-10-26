using System;
using System.Collections.Generic;
using RocketEngine;

namespace RocketConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            IGravitator earth = new OriginEarth();
            List<IGravitator> gravitators = new List<IGravitator> {earth};
            Rocket rocket = new Rocket(gravitators);

            DateTime now = DateTime.UtcNow;
            float timestep = 60f;

            for (int i = 0; i < 90; i++)
            {
                Console.Out.WriteLine($"Rocket at ({rocket.Location.X}, {rocket.Location.Y}, {rocket.Location.Z})");
                rocket.Update(timestep);
            }
        }
    }
}
