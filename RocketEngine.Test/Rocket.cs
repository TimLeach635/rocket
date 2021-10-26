using System.Collections.Generic;
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
            _rocket = new Rocket(new List<IGravitator>());
        }

        [Test]
        public void Rocket_HasDefaultMass()
        {
            Assert.That(_rocket.Mass, Is.EqualTo(1.0f));
        }
    }
}