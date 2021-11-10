using System.Collections.Generic;

namespace RocketEngine.Bodies
{
    public interface IGravitatee : IBody
    {
        ICollection<IGravitator> Gravitators { get; set; }
    }
}