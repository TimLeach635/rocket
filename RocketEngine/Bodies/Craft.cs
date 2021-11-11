using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using RocketEngine.Positioning;
using RocketEngine.Simulation;

namespace RocketEngine.Bodies
{
    public class Craft : IGravitatee
    {
        private readonly TimeSpan _minimumTimestep = TimeSpan.FromDays(1f);
        private Vector3 _velocity;
        private List<IGravitator> _gravitators;

        public Craft(DateTime initialTime, Position initialPosition, Vector3 initialVelocity)
        {
            CurrentTime = initialTime;
            Position = initialPosition;
            _velocity = initialVelocity;
        }

        public Craft(
            DateTime initialTime,
            Orbit initialOrbit,
            IGravitator initialCentralBody
        ) : this(
            initialTime,
            initialOrbit.GetPositionFromGravitator(initialCentralBody, initialTime),
            initialOrbit.GetVelocityFromGravitator(initialCentralBody, initialTime)
        )
        {
        }

        public ICollection<IGravitator> Gravitators
        {
            get => _gravitators;
            set => _gravitators = value.ToList();
        }

        public Position Position { get; }

        public DateTime CurrentTime { get; private set; }

        public void Update(TimeSpan timestep)
        {
            var timeSimulated = new TimeSpan(0);
            while (timeSimulated < timestep)
            {
                var nextTimestep = SimulationHelper.Min(_minimumTimestep, timestep - timeSimulated);
                ExplicitEuclidStep(nextTimestep);
                timeSimulated += nextTimestep;
            }

            CurrentTime += timestep;
        }

        private void ExplicitEuclidStep(TimeSpan timestep)
        {
            // update location
            Position.ChangeBy((float) timestep.TotalSeconds * _velocity);

            // update velocity
            foreach (var gravitator in _gravitators)
            {
                var difference = gravitator.Position.ICRSVectorf - Position.ICRSVectorf;
                var distance = (float) Math.Sqrt(Vector3.Dot(difference, difference));
                var unitDirection = difference * (1 / distance);
                var acceleration = unitDirection * (gravitator.StandardGravitationalParameter / (distance * distance));
                _velocity += (float) timestep.TotalSeconds * acceleration;
            }
        }
    }
}