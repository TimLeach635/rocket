using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using RocketEngine.Bodies;

namespace RocketEngine.Simulation
{
    [Serializable]
    public class SimulationDesyncException : ApplicationException
    {
        public SimulationDesyncException()
        {
        }

        public SimulationDesyncException(string message) : base(message)
        {
        }

        public SimulationDesyncException(string message, ICollection<IBody> bodies) : this(message)
        {
            Bodies = bodies;
        }

        public SimulationDesyncException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SimulationDesyncException(
            SerializationInfo info,
            StreamingContext context
        ) : base(info, context)
        {
        }

        public ICollection<IBody> Bodies { get; }
    }
}