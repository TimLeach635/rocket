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

        public Simulation(ICollection<IBody> bodies)
        {
            Bodies = bodies;

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
        public ICollection<IBody> Bodies { get; }

        public ICollection<IGravitator> Gravitators { get; private set; }

        public ICollection<IGravitatee> Gravitatees { get; private set; }

        private void UpdateGravitatorsAndGravitatees()
        {
            Gravitators = Bodies
                .OfType<IGravitator>()
                .ToList();
            Gravitatees = Bodies
                .OfType<IGravitatee>()
                .ToList();
            foreach (var gravitatee in Gravitatees) gravitatee.Gravitators = Gravitators;
        }

        private void SyncCheck()
        {
            if (Bodies.Count > 0 && Bodies.Select(b => b.CurrentTime).Any(t => !t.Equals(Bodies.First().CurrentTime)))
                throw new SimulationDesyncException("Simulation is out of sync", Bodies);
        }

        public void SimulateSeconds(double seconds)
        {
            // the idea is that each simulation should be able to be done in a fraction of a second.
            var numberOfSteps = 1000;
            var simSecondsPerStep = seconds / numberOfSteps;
            var timeSpanPerStep = new TimeSpan((long) (simSecondsPerStep * 1e7));
            for (var i = 0; i < numberOfSteps; i++)
                foreach (var body in Bodies)
                    body.Update(timeSpanPerStep);

            SyncCheck();
        }

        public void AddBody(IBody body)
        {
            Bodies.Add(body);

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