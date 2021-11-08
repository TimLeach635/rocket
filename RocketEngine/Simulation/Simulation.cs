using RocketEngine.Bodies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketEngine.Simulation
{
  public enum SimulationTimeScale
  {
    RealTime = 1,
    DoubleSpeed = 2,
    MinutesPerSecond = 60,
    HoursPerSecond = 3600,
    DaysPerSecond = 86400,
  }

  public class Simulation
  {
    private TimeSpan _minimumTimestep = TimeSpan.FromSeconds(1f);
    private ICollection<IBody> _bodies;
    private ICollection<IGravitator> _gravitators;
    private ICollection<IGravitatee> _gravitatees;

    public SimulationTimeScale TimeScale { get; set; }
    public ICollection<IBody> Bodies => _bodies;
    public ICollection<IGravitator> Gravitators => _gravitators;
    public ICollection<IGravitatee> Gravitatees => _gravitatees;

    private void UpdateGravitatorsAndGravitatees()
    {
      _gravitators = _bodies
        .Where(b => b is IGravitator)
        .Select(b => b as IGravitator)
        .ToList();
      _gravitatees = _bodies
        .Where(b => b is IGravitatee)
        .Select(b => b as IGravitatee)
        .ToList();
      foreach (IGravitatee gravitatee in _gravitatees)
      {
        gravitatee.Gravitators = _gravitators;
      }
    }

    private void SyncCheck()
    {
      if (Bodies.Count > 0 && Bodies.Select(b => b.CurrentTime).Any(t => !t.Equals(Bodies.First().CurrentTime)))
      {
        throw new SimulationDesyncException("Simulation is out of sync", Bodies);
      }
    }

    public void SimulateSeconds(double seconds)
    {
      // the idea is that each simulation should be able to be done in a fraction of a second.
      int numberOfSteps = 1000;
      double simSecondsPerStep = seconds / (double)numberOfSteps;
      TimeSpan timeSpanPerStep = new TimeSpan((long)(simSecondsPerStep * 1e7));
      for (int i = 0; i < numberOfSteps; i++)
      {
        foreach (IBody body in Bodies)
        {
          body.Update(timeSpanPerStep);
        }
      }

      SyncCheck();
    }

    public void AddBody(IBody body)
    {
      _bodies.Append(body);

      try
      {
        SyncCheck();
      }
      catch(SimulationDesyncException e)
      {
        throw new ArgumentException(
          "Attempted to add a desynchronised body to a Simulation",
          e
        );
      }

      UpdateGravitatorsAndGravitatees();
    }

    public Simulation(ICollection<IBody> bodies)
    {
      _bodies = bodies;

      try
      {
        SyncCheck();
      }
      catch(SimulationDesyncException e)
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
  }
}
