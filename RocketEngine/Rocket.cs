using RocketEngine.Bodies;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace RocketEngine
{
    public class Rocket
    {
        private ICollection<IGravitator> _gravitators;
        private Vector3 _position = new Vector3(6741000.0f, 0.0f, 0.0f);
        private Vector3 _velocity = new Vector3(0.0f, 7777.7777f, 0.0f);
        private float _mass = 1.0f;
        private float _minimumTimestep = 1f;

        public Vector3 Position => _position;
        public Vector3 Velocity => _velocity;
        public float Mass => _mass;

        public Rocket(
            ICollection<IGravitator> gravitators,
            Vector3 initialPosition,
            Vector3 initialVelocity
        )
        {
            _gravitators = gravitators;
            _position = initialPosition;
            _velocity = initialVelocity;
        }

        public Rocket(
            List<IGravitator> gravitators,
            Vector3 initialPosition,
            Vector3 initialVelocity,
            float minimumTimestep
        ) : this(gravitators, initialPosition, initialVelocity)
        {
            _minimumTimestep = minimumTimestep;
        }

        private void ExplicitEuclidStep(float timestep)
        {
            // update location
            _position += timestep * _velocity;

            // update velocity
            foreach (IGravitator gravitator in _gravitators)
            {
                Vector3 difference = gravitator.LocationAt(DateTime.Now) - _position;
                float distance = (float)Math.Sqrt((double)Vector3.Dot(difference, difference));
                Vector3 unitDirection = difference * (1 / distance);
                Vector3 acceleration = unitDirection * (gravitator.StandardGravitationalParameter / (distance * distance));
                _velocity += acceleration * timestep;
            }
        }

        public void Update(float timestep)
        {
            float timeSimulated = 0;
            while (timeSimulated < timestep)
            {
                float nextTimestep = MathF.Min(_minimumTimestep, timestep - timeSimulated);
                ExplicitEuclidStep(nextTimestep);
                timeSimulated += nextTimestep;
            }
        }
    }
}
