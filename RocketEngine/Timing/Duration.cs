using System;
using RocketEngine.Simulation;

namespace RocketEngine.Timing
{
  public class Duration
  {
    // internally represented in seconds
    private double _seconds;

    public double Days => _seconds / 86400d;
    public double Hours => _seconds / 3600d;
    public double Minutes => _seconds / 60d;
    public double Seconds => _seconds;
    public double Milliseconds => _seconds * 1e3;
    public double Microseconds => _seconds * 1e6;
    // a "tick" in the context of C# is 100 nanoseconds (0.1 microseconds)
    public double Ticks => _seconds * 1e7;
    public double Nanoseconds => _seconds * 1e9;

    public Duration(double seconds)
    {
      _seconds = seconds;
    }
  }
}
