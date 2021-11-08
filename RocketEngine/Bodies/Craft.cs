using RocketEngine.Positioning;
using RocketEngine.Simulation;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace RocketEngine.Bodies
{
  public class Craft : IGravitatee
  {
    private TimeSpan _minimumTimestep = TimeSpan.FromDays(1f);
    private DateTime _currentTime;
    private Position _position;
    private Vector3 _velocity;

    public ICollection<IGravitator> Gravitators { get; set; }

    public Position Position => _position;

    public DateTime CurrentTime => _currentTime;

    private void ExplicitEuclidStep(TimeSpan timestep)
      {
        // update location
        _position.ChangeBy((float)timestep.TotalSeconds * _velocity);

        // update velocity
        foreach (IGravitator gravitator in Gravitators)
        {
          Vector3 difference = gravitator.Position.ICRSVectorf - _position.ICRSVectorf;
          float distance = (float)Math.Sqrt((double)Vector3.Dot(difference, difference));
          Vector3 unitDirection = difference * (1 / distance);
          Vector3 acceleration = unitDirection * (gravitator.StandardGravitationalParameter / (distance * distance));
          _velocity += (float)timestep.TotalSeconds * acceleration;
        }
      }

    public void Update(TimeSpan timestep)
    {
      TimeSpan timeSimulated = new TimeSpan(0);
      while (timeSimulated < timestep)
      {
        TimeSpan nextTimestep = SimulationHelper.Min(_minimumTimestep, timestep - timeSimulated);
        ExplicitEuclidStep(nextTimestep);
        timeSimulated += nextTimestep;
      }

      _currentTime += timestep;
    }

    public Craft(DateTime initialTime, Position initialPosition, Vector3 initialVelocity)
    {
      _currentTime = initialTime;
      _position = initialPosition;
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
    ) { }
  }
}
