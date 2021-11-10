using System;

namespace RocketEngine.Simulation
{
    public static class SimulationHelper
    {
        public static TimeSpan Min(TimeSpan t1, TimeSpan t2)
        {
            if (t1.CompareTo(t2) <= 0) return t1;
            return t2;
        }
    }
}