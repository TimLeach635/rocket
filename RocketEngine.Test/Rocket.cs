using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using RocketEngine;

namespace RocketEngine.Test
{
    public class RocketTests
    {
        private Rocket _rocket;

        [SetUp]
        public void Setup()
        {
            _rocket = new Rocket(new List<IGravitator>(), new Vector3(), new Vector3());
        }

        [Test]
        public void Rocket_HasDefaultMass()
        {
            Assert.That(_rocket.Mass, Is.EqualTo(1.0f));
        }
    }
}