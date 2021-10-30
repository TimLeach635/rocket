using System;
using System.Collections.Generic;
using RocketEngine.Bodies;

namespace RocketEngine.Simulation.Specific
{
  public class EarthIss
  {
    private Craft _iss;
    private OriginEarth _earth;
    private Simulation _simulation;

    public Craft Iss => _iss;
    public OriginEarth Earth => _earth;
    public Simulation Simulation => _simulation;

    public EarthIss(DateTime initialTime)
    {
      _earth = new OriginEarth(initialTime);
      Orbit issOrbit = new Orbit(
        0.0003938f,
        Constants.EARTH_RADIUS + (417000 + 423000) / 2,
        51.6444f,
        38.4733f * MathF.PI / 180,
        153.2242f * MathF.PI / 180,
        27.0427f * MathF.PI / 180,
        new DateTime(2021, 10, 29, 12, 34, 51)
      );
      _iss = new Craft(initialTime, issOrbit, _earth);

      _simulation = new Simulation(new List<IBody> {_earth, _iss});
    }
  }
}
