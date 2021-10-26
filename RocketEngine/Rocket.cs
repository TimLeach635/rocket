using System;
using System.Collections.Generic;
using System.Numerics;

namespace RocketEngine
{
    public class Rocket
    {
        public const float G = 6.674e-11f;

        private ICollection<IGravitator> _gravitators;
        private Vector3 _location = new Vector3(6741000.0f, 0.0f, 0.0f);
        private Vector3 _velocity = new Vector3(0.0f, 7777.7777f, 0.0f);
        private float _mass = 1.0f;
        private float _minimumTimestep = 1f;

        public Vector3 Location => _location;
        public Vector3 Velocity => _velocity;
        public float Mass => _mass;

        public Rocket(ICollection<IGravitator> gravitators)
        {
            _gravitators = gravitators;
        }

        public Rocket(List<IGravitator> gravitators, float minimumTimestep) : this(gravitators)
        {
            _minimumTimestep = minimumTimestep;
        }

        private void ExplicitEuclidStep(float timestep)
        {
            // update location
            _location += timestep * _velocity;

            // update velocity
            foreach (IGravitator gravitator in _gravitators)
            {
                Vector3 difference = gravitator.LocationAt(DateTime.Now) - _location;
                float distance = (float)Math.Sqrt((double)Vector3.Dot(difference, difference));
                Vector3 unitDirection = difference * (1 / distance);
                Vector3 acceleration = unitDirection * (G * gravitator.Mass / (distance * distance));
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
