using System.Numerics;

namespace RocketEngine.Positioning
{
  /// A Position is intended to represent a specific, unambiguous position in
  /// three-dimensional space.
  /// Internally, it uses double-precision floats to define positions in metres relative
  /// to the barycenter of the solar system. For the timescales used in this
  /// program, this can be considered an inertial reference frame. The orientation
  /// of the axes is that of the ICRS: the xy plane is (approximately) that of the
  /// earth's equator, with the positive x direction pointing towards the
  /// vernal equinox, and the positive z direction the earth's north pole
  /// (forming a right-handed basis).
  public class Position
  {
    // I'm considering biting the bullet and using the OpenTK Vector3d for this.
    // I don't know if it uses hardware acceleration like C#'s native Vector3,
    // but until they introduce generic vectors into native C# it may be the
    // best we can do!
    private double _x;
    private double _y;
    private double _z;

    public double X => _x;
    public double Y => _y;
    public double Z => _z;

    public Vector3 ICRSVectorf => new Vector3((float)_x, (float)_y, (float)_z);

    public Vector3 OffsetFrom(Position origin)
    {
      return ICRSVectorf - origin.ICRSVectorf;
    }

    public void ChangeBy(Vector3 positionChange)
    {
      _x += positionChange.X;
      _y += positionChange.Y;
      _z += positionChange.Z;
    }

    public Position(Vector3 position)
    {
      _x = position.X;
      _y = position.Y;
      _z = position.Z;
    }
  }
}
