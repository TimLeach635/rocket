using System;
using System.Collections.Generic;
using RocketEngine.Bodies;

namespace RocketEngine.Simulation.Specific
{
  public class InnerPlanets
  {
    private Simulation _simulation;
    private OriginSun _sun;
    private Craft _mercury;
    private Craft _venus;
    private Craft _earth;
    private Craft _mars;

    public Simulation Simulation => _simulation;
    public OriginSun Sun => _sun;
    public Craft Mercury => _mercury;
    public Craft Venus => _venus;
    public Craft Earth => _earth;
    public Craft Mars => _mars;

    public InnerPlanets(DateTime initialTime)
    {
      _sun = new OriginSun(initialTime);

      float mPerAu = 1.496e11f;

      Orbit mercuryOrbit = new Orbit(
        0.20563069f,
        0.38709893f * mPerAu,
        7.00487f * MathF.PI / 180,
        48.33167f * MathF.PI / 180,
        77.45645f * MathF.PI / 180,
        252.25084f * MathF.PI / 180,
        new DateTime(2000, 1, 1)
      );
      _mercury = new Craft(initialTime, mercuryOrbit, _sun);

      Orbit venusOrbit = new Orbit(
        0.00677323f,
        0.72333199f * mPerAu,
        3.39471f * MathF.PI / 180,
        76.68069f * MathF.PI / 180,
        131.53298f * MathF.PI / 180,
        181.97973f * MathF.PI / 180,
        new DateTime(2000, 1, 1)
      );
      _venus = new Craft(initialTime, venusOrbit, _sun);

      Orbit earthOrbit = new Orbit(
        0.01671022f,
        1.00000011f * mPerAu,
        0.00005f * MathF.PI / 180,
        -11.26064f * MathF.PI / 180,
        102.94719f * MathF.PI / 180,
        100.46435f * MathF.PI / 180,
        new DateTime(2000, 1, 1)
      );
      _earth = new Craft(initialTime, earthOrbit, _sun);

      Orbit marsOrbit = new Orbit(
        0.09341233f,
        1.52366231f * mPerAu,
        1.85061f * MathF.PI / 180,
        49.57854f * MathF.PI / 180,
        336.04084f * MathF.PI / 180,
        355.45332f * MathF.PI / 180,
        new DateTime(2000, 1, 1)
      );
      _mars = new Craft(initialTime, marsOrbit, _sun);

      _simulation = new Simulation(new List<IBody> {
        _sun, _mercury, _venus, _earth, _mars
      }, TimeSpan.FromDays(1));
    }
  }
}
