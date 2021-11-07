using System;
using OpenTK.Mathematics;

namespace RocketGraphics.Camera
{
  public class LockedCamera
  {
    // camera position
    private Vector3 _origin;
    private float _distanceFromOrigin = 2f;
    private float _xyRotation = 0f; // 0 to 2*pi radians, clockwise from the top
    private float _verticalRotation = 0f; // -pi/2 to pi/2 radians, with positive angle being above the xy plane

    private Vector3 _cameraPosition => _origin + Vector3.Transform(
      -Vector3.UnitY * _distanceFromOrigin,
      new Quaternion(-_verticalRotation, 0, -_xyRotation)
    );

    // camera properties
    private float _fov = MathHelper.PiOver4;
    private float _nearClipping = 0.1f;
    private float _farClipping = 100f;
    public float Fov {
      get
      {
        return _fov;
      }
      set
      {
        _fov = value;
      }
    }
    public float AspectRatio { get; set; }

    public Matrix4 ViewMatrix => Matrix4.LookAt(_cameraPosition, _origin, Vector3.UnitZ);

    public Matrix4 ProjectionMatrix => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, _nearClipping, _farClipping);

    public void RotateXY(float rotation)
    {
      _xyRotation += rotation;
    }

    public LockedCamera(Vector3 origin, float aspectRatio)
    {
      _origin = origin;
      AspectRatio = aspectRatio;
    }
  }
}
