using System;
using System.Collections.Generic;
using System.Numerics;

namespace RocketEngine.Bodies
{
  public interface IGravitatee : IBody
  {
    ICollection<IGravitator> Gravitators { get; set; }
  }
}
