using System;
using System.Collections.Generic;
using RocketEngine.Bodies;

namespace RocketEngine.Simulation.Specific
{
    public class EarthIss
    {
        public EarthIss(DateTime initialTime)
        {
            Earth = new OriginEarth(initialTime);
            var issOrbit = new Orbit(
                0.0003938f,
                Constants.EARTH_RADIUS + (417000 + 423000) / 2,
                51.6444f,
                38.4733f * MathF.PI / 180,
                153.2242f * MathF.PI / 180,
                27.0427f * MathF.PI / 180,
                new DateTime(2021, 10, 29, 12, 34, 51)
            );
            Iss = new Craft(initialTime, issOrbit, Earth);

            Simulation = new Simulation(new List<IBody> {Earth, Iss});
        }

        public Craft Iss { get; }

        public OriginEarth Earth { get; }

        public Simulation Simulation { get; }
    }
}