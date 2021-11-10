using System;
using System.Numerics;
using RocketEngine.Bodies;
using RocketEngine.Positioning;

namespace RocketEngine
{
    public class Orbit
    {
        // we define an orbit internally by the six Keplerian elements:
        // https://en.wikipedia.org/wiki/Orbital_elements

        // the above are taken relative to a reference epoch

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
            Eccentricity = eccentricity;
            SemimajorAxisInMetres = semimajorAxis;
            Inclination = inclination;
            LongitudeOfAscendingNode = longitudeOfAscendingNode;
            ArgumentOfPeriapsis = argumentOfPeriapsis;
            MeanAnomalyAtEpoch = meanAnomalyAtEpoch;
            ReferenceEpoch = referenceEpoch;
        }

        public Orbit(
            Vector3 position,
            Vector3 velocity,
            IGravitator centralBody
        )
        {
        }

        public float Eccentricity { get; }

        public float SemimajorAxisInMetres { get; }

        public float Inclination { get; }

        public float LongitudeOfAscendingNode { get; }

        public float ArgumentOfPeriapsis { get; }

        public float MeanAnomalyAtEpoch { get; }

        public DateTime ReferenceEpoch { get; }

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
            var eccentricAnomaly = meanAnomaly;

            for (var i = 0; i < iterations; i++) eccentricAnomaly -= F(eccentricAnomaly) / FPrime(eccentricAnomaly);

            return eccentricAnomaly;
        }

        public Position GetPositionFromGravitator(IGravitator centralBody, DateTime time)
        {
            // adapted from https://farside.ph.utexas.edu/teaching/celestial/Celestial/node34.html
            // for now, we use the x-y plane as our reference plane, and the positive x-axis as our vernal point.
            var timeFromEpoch = time.Subtract(ReferenceEpoch);
            var meanAnomaly =
                MeanAnomalyAtEpoch
                + (float) (
                    timeFromEpoch.TotalSeconds
                    * Math.Sqrt(
                        centralBody.StandardGravitationalParameter
                        / Math.Pow(SemimajorAxisInMetres, 3)
                    )
                );

            var eccentricAnomaly = SolveKepler(meanAnomaly, Eccentricity, 20);
            var trueAnomaly = 2 * MathF.Atan2(
                MathF.Sqrt(1 + Eccentricity) * MathF.Sin(eccentricAnomaly / 2),
                MathF.Sqrt(1 - Eccentricity) * MathF.Cos(eccentricAnomaly / 2)
            );

            var distanceToCentralBody = SemimajorAxisInMetres * (1 - Eccentricity * MathF.Cos(eccentricAnomaly));

            // z-axis perpendicular to orbital plane,
            // x-axis pointing to periapsis
            var orbitalFramePosition = new Vector3(
                distanceToCentralBody * MathF.Cos(trueAnomaly),
                distanceToCentralBody * MathF.Sin(trueAnomaly),
                0
            );

            var rotation = Matrix4x4.Identity
                           * Matrix4x4.CreateRotationZ(-ArgumentOfPeriapsis)
                           * Matrix4x4.CreateRotationX(-Inclination)
                           * Matrix4x4.CreateRotationZ(-LongitudeOfAscendingNode);

            var inertialFramePosition = Vector3.Transform(orbitalFramePosition, rotation);

            return new Position(inertialFramePosition);
        }

        public Vector3 GetVelocityFromGravitator(IGravitator centralBody, DateTime time)
        {
            // adapted from https://farside.ph.utexas.edu/teaching/celestial/Celestial/node34.html
            // for now, we use the x-y plane as our reference plane, and the positive x-axis as our vernal point.
            var timeFromEpoch = time.Subtract(ReferenceEpoch);
            var meanAnomaly =
                MeanAnomalyAtEpoch
                + (float) (
                    timeFromEpoch.TotalSeconds
                    * Math.Sqrt(
                        centralBody.StandardGravitationalParameter
                        / Math.Pow(SemimajorAxisInMetres, 3)
                    )
                );

            var eccentricAnomaly = SolveKepler(meanAnomaly, Eccentricity, 20);
            var trueAnomaly = 2 * MathF.Atan2(
                MathF.Sqrt(1 + Eccentricity) * MathF.Sin(eccentricAnomaly / 2),
                MathF.Sqrt(1 - Eccentricity) * MathF.Cos(eccentricAnomaly / 2)
            );

            var distanceToCentralBody = SemimajorAxisInMetres * (1 - Eccentricity * MathF.Cos(eccentricAnomaly));

            // z-axis perpendicular to orbital plane,
            // x-axis pointing to periapsis
            var orbitalFrameVelocity = new Vector3(
                                           -MathF.Sin(eccentricAnomaly),
                                           MathF.Sqrt(1 - Eccentricity * Eccentricity) * MathF.Cos(eccentricAnomaly),
                                           0
                                       )
                                       * MathF.Sqrt(centralBody.StandardGravitationalParameter * SemimajorAxisInMetres)
                                       / distanceToCentralBody;

            var rotation = Matrix4x4.Identity
                           * Matrix4x4.CreateRotationZ(-ArgumentOfPeriapsis)
                           * Matrix4x4.CreateRotationX(-Inclination)
                           * Matrix4x4.CreateRotationZ(-LongitudeOfAscendingNode);

            var inertialFrameVelocity = Vector3.Transform(orbitalFrameVelocity, rotation);

            return inertialFrameVelocity;
        }
    }
}