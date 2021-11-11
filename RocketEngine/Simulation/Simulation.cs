using System;
using System.Collections.Generic;
using System.Linq;
using RocketEngine.Bodies;

namespace RocketEngine.Simulation
{
    public enum SimulationTimeScale
    {
        RealTime = 1,
        DoubleSpeed = 2,
        MinutesPerSecond = 60,
        HoursPerSecond = 3600,
        DaysPerSecond = 86400
    }

    public class Simulation
    {
        private TimeSpan _minimumTimestep = TimeSpan.FromSeconds(1f);
        private List<IBody> _bodies;
        private List<IGravitator> _gravitators;
        private List<IGravitatee> _gravitatees;

        public Simulation(ICollection<IBody> bodies)
        {
            _bodies = bodies.ToList();

            try
            {
                SyncCheck();
            }
            catch (SimulationDesyncException e)
            {
                throw new ArgumentException(
                    "Attempted to initialise Simulation class with bodies that were not synchronised",
                    e
                );
            }

            UpdateGravitatorsAndGravitatees();
        }

        public Simulation(ICollection<IBody> bodies, TimeSpan minimumTimestep) : this(bodies)
        {
            _minimumTimestep = minimumTimestep;
        }

        public SimulationTimeScale TimeScale { get; set; }
        public ICollection<IBody> Bodies => _bodies;

        public ICollection<IGravitator> Gravitators
        {
            get => _gravitators;
            private set => _gravitators = value.ToList();
        }

        public ICollection<IGravitatee> Gravitatees
        {
            get => _gravitatees;
            private set => _gravitatees = value.ToList();
        }

        private void UpdateGravitatorsAndGravitatees()
        {
            Gravitators = _bodies
                .OfType<IGravitator>()
                .ToList();
            Gravitatees = _bodies
                .OfType<IGravitatee>()
                .ToList();
            foreach (var gravitatee in _gravitatees) gravitatee.Gravitators = Gravitators;
        }

        private void SyncCheck()
        {
            if (_bodies.Count > 0 && _bodies.Select(b => b.CurrentTime).Any(t => !t.Equals(Bodies.First().CurrentTime)))
                throw new SimulationDesyncException("Simulation is out of sync", Bodies);
        }

        public void SimulateSeconds(double seconds)
        {
            // the idea is that each simulation should be able to be done in a fraction of a second.
            var numberOfSteps = 1000;
            var simSecondsPerStep = seconds / numberOfSteps;
            var timeSpanPerStep = new TimeSpan((long) (simSecondsPerStep * 1e7));
            for (var i = 0; i < numberOfSteps; i++)
                foreach (var body in _bodies)
                    body.Update(timeSpanPerStep);

            SyncCheck();
        }

        public void AddBody(IBody body)
        {
            _bodies.Add(body);

            try
            {
                SyncCheck();
            }
            catch (SimulationDesyncException e)
            {
                throw new ArgumentException(
                    "Attempted to add a desynchronised body to a Simulation",
                    e
                );
            }

            UpdateGravitatorsAndGravitatees();
        }
    }
}