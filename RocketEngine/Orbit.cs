using RocketEngine.Bodies;
using RocketEngine.Positioning;
using System;
using System.Numerics;

namespace RocketEngine
{
  public class Orbit
  {
    // we define an orbit internally by the six Keplerian elements:
    // https://en.wikipedia.org/wiki/Orbital_elements
    private float _eccentricity;
    private float _semimajorAxis;
    private float _inclination;
    private float _longitudeOfAscendingNode;
    private float _argumentOfPeriapsis;
    private float _meanAnomalyAtEpoch;

    // the above are taken relative to a reference epoch
    private DateTime _referenceEpoch;

    public float Eccentricity => _eccentricity;
    public float SemimajorAxisInMetres => _semimajorAxis;
    public float Inclination => _inclination;
    public float LongitudeOfAscendingNode => _longitudeOfAscendingNode;
    public float ArgumentOfPeriapsis => _argumentOfPeriapsis;
    public float MeanAnomalyAtEpoch => _meanAnomalyAtEpoch;
    public DateTime ReferenceEpoch => _referenceEpoch;

    public Orbit(
      float eccentricity,
      float semimajorAxis,
      float inclination,
      float longitudeOfAscendingNode,
      float argumentOfPeriapsis,
      float meanAnomalyAtEpoch,
      DateTime referenceEpoch
    )
    {
      _eccentricity = eccentricity;
      _semimajorAxis = semimajorAxis;
      _inclination = inclination;
      _longitudeOfAscendingNode = longitudeOfAscendingNode;
      _argumentOfPeriapsis = argumentOfPeriapsis;
      _meanAnomalyAtEpoch = meanAnomalyAtEpoch;
      _referenceEpoch = referenceEpoch;
    }

    public Orbit(
      Vector3 position,
      Vector3 velocity,
      IGravitator centralBody
    )
    {

    }

    public static float SolveKepler(float meanAnomaly, float eccentricity, uint iterations)
    {
      float F(float eccentricAnomaly)
      {
        return eccentricAnomaly
          - eccentricity * MathF.Sin(eccentricAnomaly)
          - meanAnomaly;
      }

      float FPrime(float eccentricAnomaly)
      {
        return 1 - eccentricity * MathF.Cos(eccentricAnomaly);
      }

      // Newton-Raphson
      float eccentricAnomaly = meanAnomaly;

      for (int i = 0; i < iterations; i++)
      {
        eccentricAnomaly -= F(eccentricAnomaly)/FPrime(eccentricAnomaly);
      }

      return eccentricAnomaly;
    }

    public Position GetPositionFromGravitator(IGravitator centralBody, DateTime time)
    {
      // adapted from https://farside.ph.utexas.edu/teaching/celestial/Celestial/node34.html
      // for now, we use the x-y plane as our reference plane, and the positive x-axis as our vernal point.
      TimeSpan timeFromEpoch = time.Subtract(_referenceEpoch);
      float meanAnomaly =
        _meanAnomalyAtEpoch
        + (float)(
          timeFromEpoch.TotalSeconds
          * Math.Sqrt(
            centralBody.StandardGravitationalParameter
            / (Math.Pow(_semimajorAxis, 3))
          )
        );

      float eccentricAnomaly = SolveKepler(meanAnomaly, _eccentricity, 20);
      float trueAnomaly = 2 * MathF.Atan2(
        MathF.Sqrt(1 + _eccentricity) * MathF.Sin(eccentricAnomaly / 2),
        MathF.Sqrt(1 - _eccentricity) * MathF.Cos(eccentricAnomaly / 2)
      );

      float distanceToCentralBody = _semimajorAxis * (1 - _eccentricity * MathF.Cos(eccentricAnomaly));

      // z-axis perpendicular to orbital plane,
      // x-axis pointing to periapsis
      Vector3 orbitalFramePosition = new Vector3(
        distanceToCentralBody * MathF.Cos(trueAnomaly),
        distanceToCentralBody * MathF.Sin(trueAnomaly),
        0
      );

      Matrix4x4 rotation = Matrix4x4.Identity
        * Matrix4x4.CreateRotationZ(-_argumentOfPeriapsis)
        * Matrix4x4.CreateRotationX(-_inclination)
        * Matrix4x4.CreateRotationZ(-_longitudeOfAscendingNode);

      Vector3 inertialFramePosition = Vector3.Transform(orbitalFramePosition, rotation);

      return new Position(inertialFramePosition);
    }

    public Vector3 GetVelocityFromGravitator(IGravitator centralBody, DateTime time)
    {
      // adapted from https://farside.ph.utexas.edu/teaching/celestial/Celestial/node34.html
      // for now, we use the x-y plane as our reference plane, and the positive x-axis as our vernal point.
      TimeSpan timeFromEpoch = time.Subtract(_referenceEpoch);
      float meanAnomaly =
        _meanAnomalyAtEpoch
        + (float)(
          timeFromEpoch.TotalSeconds
          * Math.Sqrt(
            centralBody.StandardGravitationalParameter
            / (Math.Pow(_semimajorAxis, 3))
          )
        );

      float eccentricAnomaly = SolveKepler(meanAnomaly, _eccentricity, 20);
      float trueAnomaly = 2 * MathF.Atan2(
        MathF.Sqrt(1 + _eccentricity) * MathF.Sin(eccentricAnomaly / 2),
        MathF.Sqrt(1 - _eccentricity) * MathF.Cos(eccentricAnomaly / 2)
      );

      float distanceToCentralBody = _semimajorAxis * (1 - _eccentricity * MathF.Cos(eccentricAnomaly));

      // z-axis perpendicular to orbital plane,
      // x-axis pointing to periapsis
      Vector3 orbitalFrameVelocity = new Vector3(
        -MathF.Sin(eccentricAnomaly),
        MathF.Sqrt(1 - _eccentricity * _eccentricity) * MathF.Cos(eccentricAnomaly),
        0
      )
        * MathF.Sqrt(centralBody.StandardGravitationalParameter * _semimajorAxis)
        / distanceToCentralBody;

      Matrix4x4 rotation = Matrix4x4.Identity
        * Matrix4x4.CreateRotationZ(-_argumentOfPeriapsis)
        * Matrix4x4.CreateRotationX(-_inclination)
        * Matrix4x4.CreateRotationZ(-_longitudeOfAscendingNode);

      Vector3 inertialFrameVelocity = Vector3.Transform(orbitalFrameVelocity, rotation);

      return inertialFrameVelocity;
    }
  }
}
