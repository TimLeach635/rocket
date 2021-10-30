using RocketEngine.Bodies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RocketEngine.Simulation
{
  public class Simulation
  {
    private TimeSpan _minimumTimestep = TimeSpan.FromSeconds(1f);
    private ICollection<IBody> _bodies;
    private ICollection<IGravitator> _gravitators;
    private ICollection<IGravitatee> _gravitatees;

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

    public void Update(TimeSpan timestep)
    {
      TimeSpan timeSimulated = new TimeSpan(0);
      while (timeSimulated < timestep)
      {
        TimeSpan nextTimestep = SimulationHelper.Min(_minimumTimestep, timestep - timeSimulated);
        foreach (IBody body in Bodies)
        {
          body.Update(nextTimestep);
        }
        timeSimulated += nextTimestep;
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
