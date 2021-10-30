using RocketEngine.Bodies;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RocketEngine.Simulation
{
  [Serializable]
  public class SimulationDesyncException : ApplicationException
  {
    public ICollection<IBody> Bodies { get; }
    public SimulationDesyncException() { }
    public SimulationDesyncException(string message) : base(message) { }
    public SimulationDesyncException(string message, ICollection<IBody> bodies) : this(message)
    {
      Bodies = bodies;
    }
    public SimulationDesyncException(string message, Exception inner) : base(message, inner) { }
    protected SimulationDesyncException(
      SerializationInfo info,
      StreamingContext context
    ) : base(info, context) { }
  }
}
