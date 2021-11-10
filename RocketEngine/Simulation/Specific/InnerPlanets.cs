using System;
using System.Collections.Generic;
using RocketEngine.Bodies;

namespace RocketEngine.Simulation.Specific
{
    public class InnerPlanets
    {
        public InnerPlanets(DateTime initialTime)
        {
            Sun = new OriginSun(initialTime);

            var mPerAu = 1.496e11f;

            var mercuryOrbit = new Orbit(
                0.20563069f,
                0.38709893f * mPerAu,
                7.00487f * MathF.PI / 180,
                48.33167f * MathF.PI / 180,
                77.45645f * MathF.PI / 180,
                252.25084f * MathF.PI / 180,
                new DateTime(2000, 1, 1)
            );
            Mercury = new Craft(initialTime, mercuryOrbit, Sun);

            var venusOrbit = new Orbit(
                0.00677323f,
                0.72333199f * mPerAu,
                3.39471f * MathF.PI / 180,
                76.68069f * MathF.PI / 180,
                131.53298f * MathF.PI / 180,
                181.97973f * MathF.PI / 180,
                new DateTime(2000, 1, 1)
            );
            Venus = new Craft(initialTime, venusOrbit, Sun);

            var earthOrbit = new Orbit(
                0.01671022f,
                1.00000011f * mPerAu,
                0.00005f * MathF.PI / 180,
                -11.26064f * MathF.PI / 180,
                102.94719f * MathF.PI / 180,
                100.46435f * MathF.PI / 180,
                new DateTime(2000, 1, 1)
            );
            Earth = new Craft(initialTime, earthOrbit, Sun);

            var marsOrbit = new Orbit(
                0.09341233f,
                1.52366231f * mPerAu,
                1.85061f * MathF.PI / 180,
                49.57854f * MathF.PI / 180,
                336.04084f * MathF.PI / 180,
                355.45332f * MathF.PI / 180,
                new DateTime(2000, 1, 1)
            );
            Mars = new Craft(initialTime, marsOrbit, Sun);

            Simulation = new Simulation(new List<IBody>
            {
                Sun, Mercury, Venus, Earth, Mars
            }, TimeSpan.FromDays(1));
        }

        public Simulation Simulation { get; }

        public OriginSun Sun { get; }

        public Craft Mercury { get; }

        public Craft Venus { get; }

        public Craft Earth { get; }

        public Craft Mars { get; }
    }
}